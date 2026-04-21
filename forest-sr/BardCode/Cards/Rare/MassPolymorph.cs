using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
/// 群体变形术｜MassPolymorph
/// 效果：消耗。选择手牌中任意张牌，将其变为随机法术牌。
/// 升级：变形得到的卡牌自动升级
/// </summary>
public sealed class MassPolymorph : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        // 无动态变量
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.Static(StaticHoverTip.Transform)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,
        BardKeyword.Magic
    };

    public MassPolymorph()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 从手牌中选择任意张牌进行变形（参考 EntropyPower）
        CardSelectorPrefs selectPrefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, 999);
        List<CardModel> selectedCards = (await CardSelectCmd.FromHand(
            choiceContext,
            base.Owner,
            selectPrefs,
            null,
            this)).ToList();

        if (selectedCards.Count == 0) return;

        // 对每张选中的牌进行变形
        foreach (CardModel card in selectedCards)
        {
            // 使用 TransformToRandom 变形为随机牌（参考 Begone）
            CardPileAddResult result = await CardCmd.TransformToRandom(
                card,
                base.Owner.RunState.Rng.CombatCardGeneration
            );

            CardModel transformedCard = result.cardAdded;

            // 如果升级，自动升级变形后的卡牌
            if (transformedCard != null && IsUpgraded)
            {
                CardCmd.Upgrade(transformedCard);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级效果：变形得到的卡牌自动升级（在 OnPlay 中处理）
    }
}