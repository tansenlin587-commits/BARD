using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 绝不认输能力
/// </summary>
public sealed class NeverGiveUpPower : BardPower
{
    // 效果类型：增益效果
    public override PowerType Type => PowerType.Buff;

    // 堆叠类型：无层数（单次触发）
    public override PowerStackType StackType => PowerStackType.Single;

    // 不允许负层数
    public override bool AllowNegative => false;

    private bool _wasTriggered;


    public NeverGiveUpPower()
    {
        _wasTriggered = false;
    }

    /// <summary>
    /// 判断是否应该死亡（参考 LizardTail）
    /// </summary>
    public override bool ShouldDieLate(Creature creature)
    {
        // 不是自己，正常死亡
        if (creature != Owner) return true;
        // 已经触发过，正常死亡
        if (_wasTriggered) return true;
        // 否则阻止死亡
        return false;
    }

    /// <summary>
    /// 阻止死亡后执行（参考 LizardTail）
    /// </summary>
    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (creature != Owner) return;
        if (_wasTriggered) return;

        Flash();  // 能力闪烁效果
        _wasTriggered = true;

        // 获取回复量和抽牌数
        int healAmount = 1;
        int drawAmount = 3;

        // 回复生命（参考 LizardTail 的 Healing）
        await CreatureCmd.Heal(creature, healAmount);

        // 抽牌
        await CardPileCmd.Draw(null, drawAmount, Owner.Player);

        // 消耗此能力
        await PowerCmd.Remove(this);
    }
}