using System;
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
/// 轰雷剑｜ThunderclapBlade
/// 效果：造成6点伤害。使敌人获得1层洪雷。
/// 升级：伤害 6→8，洪雷层数 1→2
/// </summary>
public sealed class ThunderclapBlade : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),           // 伤害值
        new PowerVar<ThunderclapPower>(5m)           // 洪雷层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<ThunderclapPower>()  // 洪雷提示
    };

    public ThunderclapBlade()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic  // 魔法标签
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 1. 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 2. 给予洪雷效果（参考 Strangle）
        await PowerCmd.Apply<ThunderclapPower>(
            cardPlay.Target,
            base.DynamicVars["ThunderclapPower"].BaseValue,  // 洪雷层数
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 6 → 8
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        // 升级：洪雷层数 1 → 2
        base.DynamicVars["ThunderclapPower"].UpgradeValueBy(1m);
    }
}