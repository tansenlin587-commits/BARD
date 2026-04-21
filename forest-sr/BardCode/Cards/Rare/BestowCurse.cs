using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards.KeyWord;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 降咒｜Bestow Curse
/// 效果：消耗X点能量，造成8/10点伤害X次，施加X层虚弱，减少X层临时力量。消耗。
/// </summary>
public sealed class BestowCurse : BardCard
{
    public override TargetType TargetType => TargetType.AnyEnemy;

    protected override bool HasEnergyCostX => true;  // X费卡牌

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),      // 基础伤害（每X）
        new PowerVar<WeakPower>(1m),             // 基础虚弱层数（每X）
        new DynamicVar("StrengthLoss", 1m)       // 基础减力量层数（每X）
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        BardKeyword.Magic
    };

    public BestowCurse() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 获取 X 值（消耗的能量）
        int xValue = ResolveEnergyXValue();

        if (xValue <= 0) return;

        // 计算伤害和层数
        int weakAmount = (int)base.DynamicVars.Weak.BaseValue * xValue;
        int strengthLoss = (int)base.DynamicVars["StrengthLoss"].BaseValue * xValue;

        // 播放诅咒特效
        NCombatRoom.Instance?.PlaySplashVfx(cardPlay.Target, new Color("#8E44AD"));

        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.AttackAnimDelay);

        // 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(xValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 施加虚弱
        await PowerCmd.Apply<WeakPower>(
            cardPlay.Target,
            weakAmount,
            base.Owner.Creature,
            this
        );

        // 减少临时力量（使用 StrengthLoss 变量，通过 CrushUnderPower 或类似能力）
        await PowerCmd.Apply<CrushUnderPower>(
            cardPlay.Target,
            strengthLoss,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：基础伤害 +2（8 → 10）
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        // 虚弱层数不变（1 → 1）
        // StrengthLoss 不变（1 → 1）
    }
}