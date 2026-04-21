using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

public sealed class HastePower : BardPower
{
    // 效果类型：增益效果
    public override PowerType Type => PowerType.Buff;

    // 堆叠类型：有层数（可叠加）
    public override PowerStackType StackType => PowerStackType.Counter;

    // 叠加行为：不新建独立实例
    public override bool IsInstanced => false;

    // 不允许负层数
    public override bool AllowNegative => false;

    // 效果的附加提示
    protected override List<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Energy),
        HoverTipFactory.Static(StaticHoverTip.CardReward)
    ];

    // 修改最大能量（每回合+2费）
    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != base.Owner?.Player) return amount;
        return amount + 2m;  // +2最大能量
    }

    // 修改每回合抽牌数（每回合+2抽牌）
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != base.Owner?.Player) return count;
        return count + 2m;  // +2抽牌
    }

    // 回合结束时触发，减少持续时间
    
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
            await PowerCmd.Apply<VulnerablePower>(base.Owner, 2m, base.Owner, null);
            await PowerCmd.Apply<WeakPower>(base.Owner, 2m, base.Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}