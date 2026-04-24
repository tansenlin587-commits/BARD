using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 英勇气势能力
/// 效果：每当你失去活力时，获得等量的格挡
/// </summary>
public sealed class ValiantPresencePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 当任何 Power 层数变化时触发
    /// </summary>
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 只关注活力 Power 的变化
        if (!(power is VigorPower)) return;

        // 只关注减少（amount 为负数表示减少）
        if (amount >= 0) return;

        // 检查所有者
        if (power.Owner != base.Owner) return;

        decimal reducedAmount = -amount;  // 减少的层数
        if (reducedAmount <= 0) return;

        Flash();

        // 获得格挡 = 消耗层数 × 当前 Power 层数（或固定倍数）
        int blockAmount = (int)(base.Amount * reducedAmount);
        await CreatureCmd.GainBlock(base.Owner, blockAmount, ValueProp.Unpowered, null);
    }
}