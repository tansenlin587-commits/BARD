using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 连续突刺｜FlurryThrust
/// 效果：造成6点伤害。
/// 如果本回合打出过攻击牌，额外造成6点伤害。
/// 如果本回合打出过技能牌，额外造成6点伤害。
/// 升级：伤害+1（6→7）
/// </summary>
public sealed class FlurryThrust : BardCard
{
    public FlurryThrust() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move)  // 基础伤害值
    };

    // 判断本回合是否打出过攻击牌（不包括自己）
    private bool HasPlayedAttackThisTurn
    {
        get
        {
            return CombatManager.Instance.History
                .CardPlaysStarted
                .Any(e => e.CardPlay.Card.Owner == base.Owner
                    && e.HappenedThisTurn(base.CombatState)
                    && e.CardPlay.Card != this
                    && e.CardPlay.Card.Type == CardType.Attack);
        }
    }

    // 判断本回合是否打出过技能牌
    private bool HasPlayedSkillThisTurn
    {
        get
        {
            return CombatManager.Instance.History
                .CardPlaysStarted
                .Any(e => e.CardPlay.Card.Owner == base.Owner
                    && e.HappenedThisTurn(base.CombatState)
                    && e.CardPlay.Card != this
                    && e.CardPlay.Card.Type == CardType.Skill);
        }
    }

    protected override bool ShouldGlowGoldInternal => HasPlayedAttackThisTurn || HasPlayedSkillThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        decimal baseDamage = base.DynamicVars.Damage.BaseValue;
        int hitcount = 1;
        // 如果本回合打出过攻击牌，额外伤害
        if (HasPlayedAttackThisTurn)
        {
            hitcount += 1;
        }

        // 如果本回合打出过技能牌，额外伤害
        if (HasPlayedSkillThisTurn)
        {
            hitcount += 1;
        }

        // 基础伤害
        await DamageCmd.Attack(baseDamage)
            .WithHitCount(hitcount)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", null, "sword_thrust.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +1（6→2）
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}