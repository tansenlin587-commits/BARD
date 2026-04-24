using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 舞动青春能力：每回合开始时，若上一回合打出了至少2种类型的牌，抽1张牌
/// </summary>
public sealed class DancingYouthPower : PowerModel
{
    private const string TYPE_THRESHOLD_KEY = "TypeThreshold";
    private const int TYPE_THRESHOLD_VALUE = 2;  // 至少2种类型

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new DynamicVar(TYPE_THRESHOLD_KEY, TYPE_THRESHOLD_VALUE)
    };

    /// <summary>
    /// 修改摸牌数量（在抽牌阶段触发）
    /// </summary>
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        // 只对自己生效
        if (player != base.Owner.Player)
        {
            return count;
        }

        // 获取上一回合打出的卡牌类型数量
        int typeCount = GetLastTurnCardTypeCount();

        // 如果类型数量达到阈值，增加摸牌数量
        if (typeCount >= base.DynamicVars[TYPE_THRESHOLD_KEY].IntValue)
        {
            Flash();  // 闪烁提示
            return count + (decimal)base.Amount;  // base.Amount = 抽牌数量
        }

        return count;
    }

    /// <summary>
    /// 获取上一回合打出的卡牌类型数量
    /// </summary>
    private int GetLastTurnCardTypeCount()
    {
        var usedTypes = new HashSet<CardType>();
        int lastRound = base.CombatState.RoundNumber - 1;

        // 遍历上一回合的卡牌打出记录（参考 PaleBlueDotPower）
        var lastTurnPlays = CombatManager.Instance.History.CardPlaysFinished
            .Where(entry => entry.RoundNumber == lastRound
                         && entry.CardPlay.Card.Owner == base.Owner.Player);

        foreach (var entry in lastTurnPlays)
        {
            usedTypes.Add(entry.CardPlay.Card.Type);
        }

        return usedTypes.Count;
    }

    public override Task AfterModifyingHandDraw()
    {
        // 可选：播放额外特效
        return Task.CompletedTask;
    }
}