using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 韵律风暴能力：获得活力时造成AOE伤害，使用法术/乐曲牌时获得活力
/// </summary>
public sealed class RhythmStormPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _damageAmount => 3;
    private int _vigorAmount => 2;

    /// <summary>
    /// 卡牌打出后触发：使用法术牌或乐曲牌时获得活力
    /// </summary>
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 只对自己打出的牌生效
        if (cardPlay.Card.Owner.Creature != base.Owner) return;

        // 检查是否是法术牌或乐曲牌
        bool isMagic = cardPlay.Card.Keywords.Contains(BardKeyword.Magic);
        bool isSong = cardPlay.Card.Keywords.Contains(BardKeyword.SONG);


        if (!isMagic && !isSong) return;
        
        Flash();

        await PowerCmd.Apply<VigorPower>(
             base.Owner,
             _vigorAmount,  
             base.Owner,
             cardPlay.Card
        );
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 只关注活力 Power 的变化
        if (!(power is VigorPower)) return;

        // 只关注增加（amount 为正数表示增加）
        if (amount <= 0) return;

        // 检查所有者
        if (power.Owner != base.Owner) return;

        Flash();

        await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), base.CombatState.HittableEnemies, _damageAmount, ValueProp.Unpowered, base.Owner, null);
    }
}