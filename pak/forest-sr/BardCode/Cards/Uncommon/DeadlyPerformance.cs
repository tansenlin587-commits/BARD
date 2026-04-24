using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 夺命演奏｜Deadly Performance
/// 效果：每当你打出带有法术标签的牌时，对随机敌人造成3/4点伤害。
/// </summary>
public sealed class DeadlyPerformance : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Unpowered)  // 伤害值
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    };

    public DeadlyPerformance() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal stacks = base.DynamicVars.Damage.BaseValue;

        var power = await PowerCmd.Apply<DeadlyPerformancePower>(
            base.Owner.Creature,
            stacks,
            base.Owner.Creature,
            this
        );

        //power?.SetDamageAmount(base.DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +1（3 → 4）
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}