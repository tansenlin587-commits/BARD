using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 剑舞｜Sword Dance
/// 效果：造成8点伤害。
/// 如果上一张打出的牌是攻击牌，改为攻击2次。
/// 如果上一张打出的牌是技能牌，获得4点格挡。
/// 升级：伤害+2（8→10），格挡+2（4→6）
/// </summary>
public sealed class SwordDance : BardCard
{
    // 发金光条件：上一张是攻击牌或技能牌（有额外效果）
    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedAttack || WasLastCardPlayedSkill;

    // 动态变量：基础伤害8，格挡4
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),  // 基础伤害
        new BlockVar(4m, ValueProp.Move)    // 格挡值
    };

    // 构造函数：1费攻击牌，普通稀有度，目标任意敌人
    public SwordDance() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    // 判断上一张打出的牌是否是攻击牌
    private bool WasLastCardPlayedAttack
    {
        get
        {
            CardPlayStartedEntry entry = CombatManager.Instance.History
                .CardPlaysStarted
                .LastOrDefault(e => e.CardPlay.Card.Owner == base.Owner
                && e.HappenedThisTurn(base.CombatState)
                && e.CardPlay.Card != this);
            return entry?.CardPlay.Card.Type == CardType.Attack;
        }
    }

    // 判断上一张打出的牌是否是技能牌
    private bool WasLastCardPlayedSkill
    {
        get
        {
            CardPlayStartedEntry entry = CombatManager.Instance.History
                .CardPlaysStarted
                .LastOrDefault(e => e.CardPlay.Card.Owner == base.Owner
                && e.HappenedThisTurn(base.CombatState)
                && e.CardPlay.Card != this);
            return entry?.CardPlay.Card.Type == CardType.Skill;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 判断上一张牌的类型
        bool isAttack = WasLastCardPlayedAttack;
        bool isSkill = WasLastCardPlayedSkill;

        // 执行攻击
        if (isAttack)
        {
            // 上一张是攻击牌：攻击2次
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(2)                         // 攻击2次
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        else
        {
            // 上一张不是攻击牌：攻击1次
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        // 如果上一张是技能牌，获得格挡
        if (isSkill)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +2（8 → 10）
        DynamicVars.Damage.UpgradeValueBy(2m);
        // 升级：格挡 +2（4 → 6）
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}