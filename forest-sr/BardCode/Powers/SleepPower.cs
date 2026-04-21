using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 睡眠能力：使敌人跳过回合
/// </summary>
public sealed class SleepPower : PowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 回合开始时跳过行动
    /// </summary>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只对敌人生效
        if (base.Owner.IsEnemy)
        {
            // 跳过敌人的回合
            await CreatureCmd.Stun(base.Owner);
        }
    }

    /// <summary>
    /// 受到伤害后减少层数（被打醒）
    /// </summary>
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner) return;
        if (base.Amount <= 0) return;

        // 只要收到非0未阻挡攻击，就减少1层
        if (target == base.Owner && result.UnblockedDamage != 0) await PowerCmd.Decrement(this);

        if (base.Amount == 0) await PowerCmd.Remove(this);
    }

    /// <summary>
    /// 回合结束时减少层数
    /// </summary>
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Decrement(this);
        }
    }
}