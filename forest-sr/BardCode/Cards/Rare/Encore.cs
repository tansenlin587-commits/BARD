using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 安可｜Encore
/// 效果：造成6点伤害。本场战斗每打出过一张乐曲牌，就造成一次伤害。
/// 升级：基础伤害+2（6→8）
/// </summary>
public sealed class Encore : BardCard
{
    private const string _calculatedHitsKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(_calculatedHitsKey).WithMultiplier((card, _) =>
            CombatManager.Instance.History.CardPlaysStarted
                .Count(e => e.CardPlay.Card.Owner == card.Owner
                            && e.CardPlay.Card.Keywords.Contains(BardKeyword.SONG))
        )
    };

    public Encore()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        int hitCount = (int)((CalculatedVar)DynamicVars[_calculatedHitsKey]).Calculate(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitVfxNode(t => NStabVfx.Create(t, facingEnemies: true))
            .WithHitFx("vfx/vfx_attack_slash", null, "encore_sword.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);  // 6 → 8
    }
}