using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 魔法武器能力：所有非魔法攻击牌获得魔法词条
/// </summary>
public sealed class MagicWeaponPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;  // 改为 Counter，支持叠加

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(BardKeyword.Magic)
    };

    /// <summary>
    /// 为攻击牌添加魔法词条
    /// </summary>
    private void ApplyMagicKeywordToAttackCards()
    {
        var allCards = base.Owner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>();

        foreach (var card in allCards)
        {
            if (card.Type == CardType.Attack && !card.Keywords.Contains(BardKeyword.Magic))
            {
                CardCmd.ApplyKeyword(card, BardKeyword.Magic);
            }
        }
    }

    /// <summary>
    /// 当 Power 层数变化时触发（参考 SwordSagePower）
    /// </summary>
    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!(power is MagicWeaponPower)) return Task.CompletedTask;
        if (power.Owner != base.Owner) return Task.CompletedTask;

        // 层数变化时，重新为所有攻击牌添加魔法词条
        ApplyMagicKeywordToAttackCards();

        return Task.CompletedTask;
    }

    /// <summary>
    /// 卡牌进入战斗时触发（参考 SwordSagePower）
    /// </summary>
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.Owner != base.Owner.Player) return Task.CompletedTask;
        if (card.Type != CardType.Attack) return Task.CompletedTask;
        if (card.Keywords.Contains(BardKeyword.Magic)) return Task.CompletedTask;

        CardCmd.ApplyKeyword(card, BardKeyword.Magic);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 能力被移除时触发（参考 SwordSagePower）
    /// </summary>
    public override Task AfterRemoved(Creature oldOwner)
    {
        // 注意：SwordSagePower 在移除时会恢复卡牌状态
        // 这里根据设计需求决定是否移除魔法词条
        // 通常魔法词条应该保留，因为已经获得的魔法属性不应消失

        // 如果需要移除，可以取消下面的注释
        /*
        var allCards = oldOwner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>();
        foreach (var card in allCards)
        {
            if (card.Type == CardType.Attack && card.Keywords.Contains(BardKeyword.Magic))
            {
                // 注意：RemoveKeyword 可能需要实现
                // CardCmd.RemoveKeyword(card, BardKeyword.Magic);
            }
        }
        */

        return Task.CompletedTask;
    }

    /// <summary>
    /// 修改卡牌费用（参考 SwordSagePower）
    /// 这里不需要修改费用，但保留结构
    /// </summary>
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card.Owner.Creature != base.Owner) return false;
        if (card.Type != CardType.Attack) return false;

        // 不修改费用，只确保有魔法词条
        if (!card.Keywords.Contains(BardKeyword.Magic))
        {
            CardCmd.ApplyKeyword(card, BardKeyword.Magic);
        }

        return false;  // 不修改费用
    }
}