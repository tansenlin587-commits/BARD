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

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// See You Again
/// 效果：从弃牌堆回收2张牌，然后丢弃1张手牌。
/// 升级：回收 2→3 张牌
/// </summary>
public sealed class SeeYouAgain : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(3)  // 回收数量
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(BardKeyword.SONG)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        BardKeyword.SONG
    };

    public SeeYouAgain()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int recycleCount = base.DynamicVars.Cards.IntValue;

        // 1. 从弃牌堆中选择 recycleCount 张牌回收
        CardPile discardPile = PileType.Discard.GetPile(base.Owner);
        if (discardPile.Cards.Count > 0)
        {
            int actualCount = System.Math.Min(recycleCount, discardPile.Cards.Count);
            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, actualCount);

            List<CardModel> selectedCards = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                discardPile.Cards,
                base.Owner,
                prefs)).ToList();

            foreach (CardModel card in selectedCards)
            {
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }

        // 2. 丢弃1张手牌（参考 Prepared）
        await CardCmd.Discard(
            choiceContext,
            await CardSelectCmd.FromHandForDiscard(
                choiceContext,
                base.Owner,
                new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
                null,
                this
            )
        );

    }

    protected override void OnUpgrade()
    {
        // 升级：回收 2 → 3 张牌
        base.DynamicVars.Cards.UpgradeValueBy(+1);
    }
}