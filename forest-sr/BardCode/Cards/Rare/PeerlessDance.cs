using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 无双华舞｜PeerlessDance
/// 效果：选择一张手牌中的攻击牌，使其获得重放1。消耗。
/// 升级：移除消耗
/// </summary>
public sealed class PeerlessDance : BardCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust  
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1)  // 选择数量
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.Static(StaticHoverTip.ReplayStatic)  // 额外打出提示
    };

    public PeerlessDance()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 从手牌中选择一张攻击牌（参考 Transfigure）
        foreach (CardModel selectedCard in await CardSelectCmd.FromHand(
            choiceContext,
            base.Owner,
            new CardSelectorPrefs(base.SelectionScreenPrompt, 1),  
            (CardModel c) => c.Type == CardType.Attack,  // 只选攻击牌
            this))
        {
            // 增加额外打出次数（参考 Transfigure 的 BaseReplayCount++）
            selectedCard.BaseReplayCount++;
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}