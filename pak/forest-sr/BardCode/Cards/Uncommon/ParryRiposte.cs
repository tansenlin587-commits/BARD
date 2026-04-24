using BaseLib.Utils;
using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;
/// <summary>
/// 格挡反击
/// </summary>
public sealed class ParryRiposte : BardCard
{
    // 高亮条件：上一张是技能牌 或 敌人意图攻击
    protected override bool ShouldGlowGoldInternal
    {
        get
        {
            if (base.CombatState == null)
                return false;

            return WasLastCardSkill ;
        }
    }

    private bool WasLastCardSkill
    {
        get
        {
            var entry = CombatManager.Instance.History
                .CardPlaysStarted
                .LastOrDefault(e => e.CardPlay.Card.Owner == base.Owner
                                    && e.HappenedThisTurn(base.CombatState)
                                    && e.CardPlay.Card != this);
            return entry?.CardPlay.Card.Type == CardType.Skill;
        }
    }


    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, ValueProp.Move),
        new DamageVar(6m, ValueProp.Move),
        new EnergyVar(1)  // 用于显示回费效果
    };

    public ParryRiposte()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 获得格挡
        await CommonActions.CardBlock(this, cardPlay);

        // 如果敌人意图攻击，造成伤害（参考 GoForTheEyes）
        if (cardPlay.Target.Monster.IntendsToAttack)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash", null, "sword_riposte.mp3")
                .Execute(choiceContext);
        }

        // 如果上一张是技能牌，获得1点能量
        if (WasLastCardSkill)
        {
            await PlayerCmd.GainEnergy(1, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2m);   // 6→8
        base.DynamicVars.Damage.UpgradeValueBy(2m);  // 6→8
    }
}