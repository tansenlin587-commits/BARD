using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

public sealed class Polymorph : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, ValueProp.Move)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.Static(StaticHoverTip.Transform)
    };

    public Polymorph() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [BardKeyword.Magic];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue, ValueProp.Move, null);

        // 从手牌中选择一张牌进行变形
        CardModel selectedCard = (await CardSelectCmd.FromHand(
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this,
            prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1)
        )).FirstOrDefault();

        if (selectedCard != null)
        {
            // 生成变形后的卡牌
            CardModel transformedCard = await CreateRandomNonMagicBardAttackCard();

            if (transformedCard != null)
            {
                // 变形
                await CardCmd.Transform(selectedCard, transformedCard);

                // 本回合0费
                transformedCard.EnergyCost.SetThisTurnOrUntilPlayed(0);
            }
        }
    }

    private async Task<CardModel> CreateRandomNonMagicBardAttackCard()
    {
        // 获取符合条件的卡牌模板
        var eligibleCards = base.Owner.Character.CardPool
            .GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
            .Where(card =>
                card.Type == CardType.Attack &&
                !card.Keywords.Contains(BardKeyword.Magic))
            .ToList();

        if (eligibleCards.Count == 0)
            return null;

        // 随机选择一个模板
        CardModel template = base.Owner.RunState.Rng.CombatCardGeneration.NextItem(eligibleCards);

        // 生成卡牌
        var cards = CardFactory.GetDistinctForCombat(
            base.Owner,
            new List<CardModel> { template },
            1,
            base.Owner.RunState.Rng.CombatCardGeneration
        );

        CardModel result = cards.FirstOrDefault();

        // ★ 关键：如果变形术已升级，自动升级生成的卡牌 ★
        if (result != null && base.IsUpgraded)
        {
            CardCmd.Upgrade(result);
        }
        await Cmd.Wait(0.05f);

        return result;
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡 6 → 9
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}