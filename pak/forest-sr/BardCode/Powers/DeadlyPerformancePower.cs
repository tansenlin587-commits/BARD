using Forest_Sr.BardCode.Cards.KeyWord;
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
/// 夺命演奏能力
/// 效果：每当你打出带有Magic的牌时，对随机敌人造成伤害
/// </summary>
public sealed class DeadlyPerformancePower : PowerModel
{


    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;



    // 卡牌打出前记录伤害值
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 检查是否是法术牌
        if (!cardPlay.Card.Keywords.Contains(BardKeyword.Magic)) return;
        if (base.Owner == null) return;

        // 获取当前层数
        int stacks = base.Amount;

        if (stacks <= 0) return;

        // 等待一小段时间，让卡牌动画播放
        await Cmd.CustomScaledWait(0.1f, 0.2f);

        // 随机选择一个敌人
        Creature target = base.Owner.Player.RunState.Rng.CombatTargets.NextItem(base.Owner.CombatState.HittableEnemies);

        if (target != null)
        {
            // 播放攻击特效
            VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_attack_slash");
            // 造成1点伤害
            await CreatureCmd.Damage(context, target, stacks, ValueProp.Unpowered, base.Owner);
        }
        else { return; }

    }
    
}