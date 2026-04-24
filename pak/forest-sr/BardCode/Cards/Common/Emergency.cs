using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 应急｜Emergency
/// 效果：造成7点伤害。下一张法术或乐曲卡减少1费。
/// 升级：伤害 7→9
/// </summary>
public sealed class Emergency : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move)  // 伤害值
    };

    public Emergency()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", null, "sword_attack.mp3")
            .Execute(choiceContext);

        // 2. 施加 NextSpellOrSongCostReductionPower（下一张法术或乐曲卡费用-1）
        await PowerCmd.Apply<NextSpellOrSongCostReductionPower>(
            base.Owner.Creature,
            1m,  // 减少1费
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 7 → 9
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}