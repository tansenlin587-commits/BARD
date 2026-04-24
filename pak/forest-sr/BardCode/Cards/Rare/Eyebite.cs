using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 摄心目光｜Eyebite
/// 效果：使一个敌人眩晕1回合，给予2层虚弱，再减少2点力量。
/// 升级：虚弱 2→3，减力量 2→3
/// </summary>
public sealed class Eyebite : BardCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        StunIntent.GetStaticHoverTip(),           // 眩晕提示
        HoverTipFactory.FromPower<WeakPower>(),   // 虚弱提示
        HoverTipFactory.FromPower<StrengthPower>() // 力量提示
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<WeakPower>(2m),      // 虚弱层数
        new PowerVar<StrengthPower>(2m)   // 力量减少层数（负数）
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust  // 消耗
    };

    public Eyebite()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 1. 眩晕敌人（参考 Whistle）
        await CreatureCmd.Stun(cardPlay.Target);

        // 2. 给予虚弱
        await PowerCmd.Apply<WeakPower>(
            cardPlay.Target,
            base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature,
            this
        );

        // 3. 减少力量（给予负数的力量Power）
        await PowerCmd.Apply<StrengthPower>(
            cardPlay.Target,
            -base.DynamicVars.Strength.BaseValue,  // 负数表示减少力量
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：虚弱 2→3，减力量 2→3
        base.DynamicVars.Weak.UpgradeValueBy(1m);
        base.DynamicVars.Strength.UpgradeValueBy(1m);
    }
}