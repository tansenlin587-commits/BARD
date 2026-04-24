using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 灵感共鸣｜InspirationResonance
/// 效果：消耗。本回合内，每打出一张乐曲牌，回复1点能量。
/// 升级：回复能量 1→2
/// </summary>
public sealed class InspirationResonance : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(1)  // 每张乐曲回复能量
    };


    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust  // 消耗
    };

    public InspirationResonance()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int energyPerSong = base.DynamicVars.Energy.IntValue;

        // 施加灵感共鸣能力Power
        await PowerCmd.Apply<InspirationResonancePower>(
            base.Owner.Creature,
            energyPerSong,  // 每张乐曲回复能量
            base.Owner.Creature,
            this
        );

    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}