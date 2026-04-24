using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 超频施法能力：法术牌获得消耗和再次打出
/// </summary>
public sealed class OverclockCastingPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 修改卡牌打出次数（实现再次打出）
    /// </summary>
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        // 只对法术牌生效
        if (!card.Keywords.Contains(BardKeyword.Magic)) return playCount;

        // 只对自己打出的牌生效
        if (card.Owner.Creature != base.Owner) return playCount;

        // 再次打出（+1次）
        return playCount + 1;
    }

    /// <summary>
    /// 修改卡牌打出后的去向（添加消耗）- 修正版
    /// </summary>
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        PileType pileType,
        CardPilePosition position)
    {
        // 只对自己打出的牌生效
        if (card.Owner.Creature != base.Owner)
        {
            return (pileType, position);
        }

        // 检查法术牌
        if (!card.Keywords.Contains(BardKeyword.Magic))
        {
            return (pileType, position);
        }

        // 改为消耗牌堆
        return (PileType.Exhaust, position);
    }

    /// <summary>
    /// 闪烁提示
    /// </summary>
    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }
}