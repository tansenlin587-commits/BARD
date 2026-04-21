using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 变形术｜Polymorph
/// 效果：获得{Block:diff()}点格挡。选择手牌中最多{Amount:diff()}张牌，将其变形为随机的诗人牌。本回合变出的牌费用为0。
/// 升级：格挡+2，可选择牌数+1
/// </summary>
public sealed class Polymorph : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, ValueProp.Move),
        new CardsVar(1)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.Static(StaticHoverTip.Transform)
    };

    public Polymorph()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, null);

        int transformCount = DynamicVars.Cards.IntValue;

        // 从手牌中选择卡牌进行变形（参考 EntropyPower）
        CardSelectorPrefs selectPrefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, transformCount);
        List<CardModel> selectedCards = (await CardSelectCmd.FromHand(
            choiceContext,
            base.Owner,
            selectPrefs,
            null,
            this)) .ToList();

        // 对每张选中的牌进行变形（参考 Begone）
        foreach (CardModel selectedCard in selectedCards)
        {
            // 变形为随机诗人牌
            CardPileAddResult result = await CardCmd.TransformToRandom(
                selectedCard,
                base.Owner.RunState.Rng.CombatCardGeneration
            );

            // 获取变形后的卡牌
            CardModel transformedCard = result.cardAdded;

            if (transformedCard != null)
            {
                // 本回合0费
                transformedCard.EnergyCost.SetThisTurnOrUntilPlayed(0);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);  // 6→8
    }
}