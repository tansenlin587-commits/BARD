using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 守卫刻文能力：失去格挡时对全体敌人造成等量伤害
/// </summary>
public sealed class GuardianRunePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 格挡被清除时触发（参考 Reflect 的 AfterDamageReceived）
    /// </summary>
    public override async Task AfterBlockCleared(Creature creature)
    {
        // 只对自己生效
        if (creature != base.Owner) return;

        // 获取清除的格挡量（需要记录）
        // 注意：这里需要通过其他方式获取清除的格挡值
        // 简化方案：记录每回合失去的格挡
    }

    /// <summary>
    /// 受到伤害后触发（参考 Reflect）
    /// 当格挡被消耗时，对全体敌人造成等量伤害
    /// </summary>
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只对自己生效
        if (target != base.Owner) return;

        // 如果实际损失了格挡（格挡被消耗）
        if (result.BlockedDamage > 0)
        {
            // 对所有敌人造成等量伤害（参考 NecroMasteryPower）
            await CreatureCmd.Damage(
                choiceContext,
                base.Owner.CombatState.HittableEnemies,  // 全体敌人
                result.BlockedDamage,                     // 伤害 = 消耗的格挡值
                ValueProp.Unblockable | ValueProp.Unpowered,
                base.Owner,
                null
            );
        }
    }

    /// <summary>
    /// 回合结束时减少层数（参考 Reflect）
    /// </summary>
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Decrement(this);
        }
    }
}