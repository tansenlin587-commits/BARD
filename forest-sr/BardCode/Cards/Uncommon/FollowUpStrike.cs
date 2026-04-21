using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 顺势戳击｜Follow-Up Strike
/// 效果：造成4/6点伤害。如果上一张打出的牌是技能牌，此卡回到手牌。
/// </summary>
public sealed class FollowUpStrike : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move)
    };

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedSkill;

    public FollowUpStrike() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    // 判断上一张打出的牌是否是技能牌
    private bool WasLastCardPlayedSkill
    {
        get
        {
            CardPlayStartedEntry entry = CombatManager.Instance.History
                .CardPlaysStarted
                .LastOrDefault(e => e.CardPlay.Card.Owner == base.Owner && e.HappenedThisTurn(base.CombatState));
            return entry?.CardPlay.Card.Type == CardType.Skill;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 播放戳击特效
        NCombatRoom.Instance?.PlaySplashVfx(cardPlay.Target, new Color("#2ECC71"));

        // 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card, bool isAutoPlay, ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        // 如果不是当前卡牌，使用默认行为
        if (card != this) return (pileType, position);

        // 如果上一张是技能牌，回到手牌顶部
        if (WasLastCardPlayedSkill)
        {
            return (PileType.Hand, CardPilePosition.Top);
        }

        // 否则正常进入弃牌堆
        return (pileType, position);
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +2（4 → 6）
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}