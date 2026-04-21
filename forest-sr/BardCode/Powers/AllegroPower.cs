using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 快板能力：每回合开始时获得1点能量，持续若干回合
/// </summary>
public sealed class AllegroPower : PowerModel
{
    // 效果类型：增益效果
    public override PowerType Type => PowerType.Buff;

    // 堆叠类型：有层数（可叠加），层数表示剩余回合数
    public override PowerStackType StackType => PowerStackType.Counter;

    // 是否实例化：false（只存在一个实例）
    public override bool IsInstanced => false;

    // 允许负层数：false（层数为0时自动移除）
    public override bool AllowNegative => false;

    // 效果的附加提示
    protected override List<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.Static(StaticHoverTip.Energy)
        ];

    /// <summary>
    /// 每回合开始时触发
    /// </summary>
    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != base.Owner?.Player) return amount;
        return amount + 2m;  // +2最大能量
    }

    /// <summary>
    /// 回合开始时触发：减少一层能力
    /// </summary>
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