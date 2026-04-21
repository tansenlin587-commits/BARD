using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 巧舌如簧｜Honeyed Tongue HONEYED_TONGUE
/// 效果：获得4点格挡，给予所有敌人1层易伤。
/// 如果上一张打出的牌是技能牌，改为给予所有敌人1层虚弱。
/// 升级：格挡+3（4→7），易伤/虚弱层数+1（1→2）
/// </summary>
public sealed class HoneyedTongue : BardCard
{
    public HoneyedTongue() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(4m, ValueProp.Move),        // 基础格挡值
        new PowerVar<VulnerablePower>(1m),             // 易伤层数       
        new PowerVar<WeakPower>(1m)              // 虚软弱层
    };

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

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedSkill;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得格挡
        await CommonActions.CardBlock(this, cardPlay);

        // 判断上一张是否是技能牌
        if (WasLastCardPlayedSkill)
        {
            // 改为给予所有敌人虚弱
            await PowerCmd.Apply<WeakPower>(
                base.CombatState.HittableEnemies,
                base.DynamicVars.Weak.BaseValue,
                base.Owner.Creature,
                this
            );
        }
        else
        {
            // 基础效果：给予所有敌人易伤
            await PowerCmd.Apply<VulnerablePower>(
                base.CombatState.HittableEnemies,
                base.DynamicVars.Vulnerable.BaseValue,
                base.Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        // 格挡 +3（4 → 7）
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}