using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 轻快节拍能力：每回合开始时抽1张牌，持续若干回合
/// </summary>
public sealed class BriskBeatPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;  // 层数表示剩余回合数

    // 是否实例化：false（只存在一个实例）
    public override bool IsInstanced => false;

    // 允许负层数：false（层数为0时自动移除）
    public override bool AllowNegative => false;

    /// <summary>
    /// 每回合开始时触发
    /// </summary>
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != base.Owner?.Player) return count;
        return count + 1m;  // +1抽牌
    }

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == base.Owner.Player)
        {
            await PowerCmd.Decrement(this);  // 减少一层能力
        }
        int currentAmount = Amount;
        if (currentAmount <= 0)
        {
            Flash();
            await PowerCmd.Remove(this);
        }
    }
}