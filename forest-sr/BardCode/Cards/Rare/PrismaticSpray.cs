using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 虹光喷射｜Prismatic Spray
/// 效果：对单个敌人攻击8次，每次造成2/3点伤害。
/// </summary>
public sealed class PrismaticSpray : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(2m, ValueProp.Move),      // 每次伤害
        new RepeatVar(8)          // 攻击次数
    };


    public PrismaticSpray() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 播放七彩特效
        NCombatRoom.Instance?.PlaySplashVfx(cardPlay.Target, new Color("#9B59B6"));

        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.AttackAnimDelay);



        // 对目标执行多次攻击
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);


        // 短暂延迟，让动画更清晰
        await Cmd.Wait(0.05f);


    }

    protected override void OnUpgrade()
    {
        // 升级：每次伤害 +1（2 → 3）
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        // 攻击次数不变（8 → 8）
    }
}
