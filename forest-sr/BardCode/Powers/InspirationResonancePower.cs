using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 灵感共鸣能力：打出乐曲牌时回复能量
/// </summary>
public sealed class InspirationResonancePower : PowerModel
{
    private bool _hasTriggeredThisTurn = false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 卡牌打出后触发
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 只对自己打出的牌生效
        if (cardPlay.Card.Owner.Creature != base.Owner) return;

        // 检查是否是乐曲牌
        if (!cardPlay.Card.Keywords.Contains(BardKeyword.SONG)) return;

        // 回复能量
        int energyToGain = (int)base.Amount;
        if (energyToGain > 0)
        {
            Flash();
            await PlayerCmd.GainEnergy(energyToGain, base.Owner.Player);
        }
    }

    /// <summary>
    /// 回合结束时移除自身
    /// </summary>
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}