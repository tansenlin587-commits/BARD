using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 尖锐刺耳｜EarPiercingShriek
/// 效果：对所有敌人造成12点伤害，给予2层虚弱。
/// 升级：伤害 12→14，虚弱 2→3
/// </summary>
public sealed class EarPiercingShriek : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(12m, ValueProp.Move),      // 伤害值
        new PowerVar<WeakPower>(2m)              // 虚弱层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<WeakPower>()   // 虚弱提示
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.SONG  // 乐曲标签
    };

    public EarPiercingShriek()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);

        await PowerCmd.Apply<WeakPower>(base.CombatState.HittableEnemies, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 12 → 14
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        // 升级：虚弱 2 → 3
        base.DynamicVars.Weak.UpgradeValueBy(1m);
    }
}