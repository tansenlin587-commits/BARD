using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 战歌能力：使用乐曲卡时获得活力
/// </summary>
public sealed class WarSongPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 卡牌打出后触发
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 检查是否是当前玩家打出的牌
        if (cardPlay.Card.Owner.Creature != base.Owner) return;

        // 检查是否是乐曲卡
        if (!cardPlay.Card.Keywords.Contains(BardKeyword.SONG)) return;

        // 获得活力
        await PowerCmd.Apply<VigorPower>(
            base.Owner,
            base.Amount,  // 活力层数（3）
            base.Owner,
            cardPlay.Card
        );

        // 闪烁提示
        Flash();
    }
}