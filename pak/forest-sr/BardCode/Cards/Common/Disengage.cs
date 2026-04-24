using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 脱离｜Disengage
/// 效果：获得6点格挡。
/// 如果上一张打出的牌是攻击牌，抽2张牌。
/// 如果上一张打出的牌是技能牌，再获得6点格挡。
/// 升级：格挡+2（6→8），连击格挡+2（6→8）
/// </summary>
public sealed class Disengage : BardCard
{
    public Disengage() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, ValueProp.Move)  // 基础格挡值
    };

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

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedAttack || WasLastCardPlayedSkill;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取基础格挡值
        decimal blockAmount = base.DynamicVars.Block.BaseValue;

        // 判断上一张牌的类型
        bool isAttack = WasLastCardPlayedAttack;
        bool isSkill = WasLastCardPlayedSkill;

        // 获得基础格挡
        await CommonActions.CardBlock(this, cardPlay);

        // 根据上一张牌类型触发额外效果
        if (isAttack)
        {
            // 上一张是攻击牌：抽2张牌
            await CardPileCmd.Draw(choiceContext, 2, Owner);
        }
        else if (isSkill)
        {
            // 上一张是技能牌：再获得6点格挡
            await CommonActions.CardBlock(this, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡 +2（6 → 8）
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}