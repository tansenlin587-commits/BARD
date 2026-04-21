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
/// Never Gonna Give You Up
/// 效果：从消耗牌堆回收1张牌。
/// 升级：回收 1→2 张牌
/// </summary>
public sealed class NeverGonnaGiveYouUp : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1)  // 回收数量
    };


    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        BardKeyword.SONG,
        CardKeyword.Exhaust
    };

    public NeverGonnaGiveYouUp()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int recycleCount = base.DynamicVars.Cards.IntValue;

        // 从消耗牌堆中选择 recycleCount 张牌回收
        CardPile exhaustPile = PileType.Exhaust.GetPile(base.Owner);
        if (exhaustPile.Cards.Count > 0)
        {
            int actualCount = System.Math.Min(recycleCount, exhaustPile.Cards.Count);
            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, actualCount);

            List<CardModel> selectedCards = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                exhaustPile.Cards,
                base.Owner,
                prefs)).ToList();

            foreach (CardModel card in selectedCards)
            {
                // 将选中的牌从消耗堆移除，加入手牌
                await CardPileCmd.RemoveFromCombat(card);
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }

    }

    protected override void OnUpgrade()
    {
        // 升级：回收 1 → 2 张牌
        base.DynamicVars.Cards.UpgradeValueBy(+1);
    }
}