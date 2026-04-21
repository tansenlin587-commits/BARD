using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 洪雷｜ThunderclapPower
/// 效果：本回合获得Debuff时，每有一层洪雷造成1点伤害
/// </summary>
public sealed class ThunderclapPower : PowerModel
{
    private class Data
    {
        // 记录每张牌是否已触发（避免重复触发同一张牌）
        public readonly HashSet<CardModel> triggeredCards = new HashSet<CardModel>();
    }

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data();
    }

    /// <summary>
    /// 当其他Power层数变化时触发（Debuff施加时）
    /// </summary>
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 只处理Debuff的增加
        if (amount <= 0) return;
        if (power.GetTypeForAmount(amount) != PowerType.Debuff) return;

        // 检查施加者是否是当前玩家
        if (applier != base.Owner?.Player?.Creature) return;

        // 检查目标是自己（洪雷所在的敌人）
        if (power.Owner != base.Owner) return;

        // 排除临时Power和洪雷自身
        if (power is ITemporaryPower) return;
        if (power is ThunderclapPower) return;

        // 检查是否已经为这张牌触发过
        var data = GetInternalData<Data>();
        if (cardSource != null && data.triggeredCards.Contains(cardSource))
        {
            return;
        }

        // 记录已触发
        if (cardSource != null)
        {
            data.triggeredCards.Add(cardSource);
        }

        Flash();

        // 造成伤害 = 洪雷层数
        int damage = (int)base.Amount;
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            base.Owner,
            damage,
            ValueProp.Unpowered,
            base.Owner,
            cardSource
        );
    }

    /// <summary>
    /// 回合结束时移除洪雷
    /// </summary>
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}