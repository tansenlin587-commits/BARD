using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 不谐低语｜Dissonant Whispers
/// 效果：对单个敌人造成12/15点伤害，施加2/3层虚弱。魔法。
/// </summary>
public sealed class DissonantWhispers : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(14m, ValueProp.Move),      // 伤害
        new PowerVar<WeakPower>(2m)              // 虚弱层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<WeakPower>()
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic
    };

    public DissonantWhispers() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 播放不谐低语特效
        NCombatRoom.Instance?.PlaySplashVfx(cardPlay.Target, new Color("#8E44AD"));

        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.AttackAnimDelay);

        // 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 施加虚弱
        await PowerCmd.Apply<WeakPower>(
            cardPlay.Target,
            base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +3（12 → 15）
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        // 升级：虚弱 +1（2 → 3）
        base.DynamicVars.Weak.UpgradeValueBy(1m);
    }
}