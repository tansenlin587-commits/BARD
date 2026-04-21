using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 街头卖艺｜StreetPerformance
/// </summary>
public sealed class StreetPerformance : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("BonusGold", 15m)      // 额外金币
    };


    public StreetPerformance()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
   {
        CardKeyword.Exhaust  // 消耗
    };



    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int bonusGold = base.DynamicVars["BonusGold"].IntValue;


        // 检查本回合是否打出过乐曲牌
        bool hasPlayedSong = HasPlayedSongThisTurn();
        if (hasPlayedSong)
        {
            await PlayerCmd.GainGold(bonusGold, base.Owner);
        }

    }
    protected override bool ShouldGlowGoldInternal => HasPlayedSongThisTurn();

    /// <summary>
    /// 检查本回合是否打出过乐曲牌
    /// </summary>
    private bool HasPlayedSongThisTurn()
    {
        return CombatManager.Instance.History.CardPlaysStarted
            .Any(e => e.CardPlay.Card.Owner == base.Owner
                && e.HappenedThisTurn(base.CombatState)
                && e.CardPlay.Card.Keywords.Contains(BardKeyword.SONG));
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BonusGold"].UpgradeValueBy(5m);
    }
}