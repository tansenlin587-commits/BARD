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
/// 英勇打击｜ValiantStrike
/// 效果：造成8点伤害，获得2点活力。
/// 升级：伤害 8→10，活力 2→3
/// </summary>
public sealed class ValiantStrike : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),      // 伤害值
        new PowerVar<VigorPower>(2m)            // 活力层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<VigorPower>()  // 活力提示
    };

    public ValiantStrike()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 2. 获得活力
        await PowerCmd.Apply<VigorPower>(
                base.Owner.Creature,
                base.DynamicVars["VigorPower"].IntValue,
                base.Owner.Creature,
                this
         );
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 8→10
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        // 升级：活力 2→3
        base.DynamicVars["VigorPower"].UpgradeValueBy(1m);
    }
}