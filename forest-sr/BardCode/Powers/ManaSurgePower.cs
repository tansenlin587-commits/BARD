using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 法力充沛能力：每回合第一次释放法术时抽牌
/// </summary>
public sealed class ManaSurgePower : PowerModel
{
    private bool _usedThisTurn = false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 卡牌打出后触发
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 只对自己打出的牌生效
        if (cardPlay.Card.Owner.Creature != base.Owner) return;

        // 检查是否是法术牌
        if (!cardPlay.Card.Keywords.Contains(BardKeyword.Magic)) return;

        // 本回合已使用过，不再触发
        if (_usedThisTurn) return;

        // 标记已使用
        _usedThisTurn = true;

        // 抽牌
        int drawCount = (int)base.Amount;
        if (drawCount > 0)
        {
            Flash();
            await CardPileCmd.Draw(context, drawCount, base.Owner.Player);
        }
    }

    /// <summary>
    /// 回合结束时重置标记
    /// </summary>
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            _usedThisTurn = false;
        }
        await Task.CompletedTask;
    }
}