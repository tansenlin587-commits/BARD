using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 睡眠术｜Sleep
/// 效果：使一个血量低于40的敌人陷入睡眠1回合。
/// 升级：血量阈值 40→50
/// </summary>
public sealed class Sleep : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Threshold", 40m)  // 血量阈值
    };


    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust  // 消耗
    };

    public Sleep()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        int threshold = base.DynamicVars["Threshold"].IntValue;

        // 检查目标血量是否低于阈值
        if (cardPlay.Target.CurrentHp > threshold)
        {
            return;  // 血量不符合条件，无效
        }

        // 施加1层睡眠（参考乐嘉的 AsleepPower）
        await PowerCmd.Apply<SleepPower>(
            cardPlay.Target,
            1m,  // 睡眠1回合
            base.Owner.Creature,
            this
        );

    }

    protected override void OnUpgrade()
    {
        // 升级：血量阈值 40 → 50
        base.DynamicVars["Threshold"].UpgradeValueBy(10m);
    }
}