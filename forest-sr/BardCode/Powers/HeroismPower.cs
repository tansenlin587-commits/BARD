using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 英雄气概能力
/// 效果：每回合开始时获得固定格挡
/// </summary>
public sealed class HeroismPower : BardPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;  // 独立实例，支持不同目标

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(0m, ValueProp.Unpowered)  // 格挡值，由 SetBlockAmount 设置
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.Static(StaticHoverTip.Block)
    };

    /// <summary>
    /// 设置每回合获得的格挡值
    /// </summary>
    public void SetBlockAmount(decimal amount)
    {
        AssertMutable();
        base.DynamicVars.Block.BaseValue = amount;
    }

    /// <summary>
    /// 每回合开始时获得格挡
    /// </summary>
    public override async Task AfterBlockCleared(Creature creature)
    {
        if (creature == base.Owner)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner, base.DynamicVars.Block, null);
            await PowerCmd.Decrement(this);
        }
    }
}