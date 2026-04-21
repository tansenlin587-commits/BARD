using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 收场｜CurtainCall
/// 效果：造成6点伤害。本场战斗每使用过一种卡牌类型（攻击/技能/能力/状态）重复一次。
/// 升级：基础伤害+2（6→8）
/// </summary>
public sealed class CurtainCall : BardCard
{
    private const string _calculatedHitsKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move)
    
    };
    private int GetUsedCardTypeCount()
    {
        var history = CombatManager.Instance.History.CardPlaysStarted;
        var usedTypes = new HashSet<CardType>();

        // 过滤：只统计自己打出的牌（整场战斗）
        var filtered = history.Where(entry => entry.CardPlay.Card.Owner == base.Owner);

        foreach (var entry in filtered)
        {
            var cardType = entry.CardPlay.Card.Type;  // 注意：entry.CardPlay.Card
            usedTypes.Add(cardType);
        }

        return usedTypes.Count;
    }

    public CurtainCall()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = GetUsedCardTypeCount();
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(count)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 基础伤害 +2（6 → 8）
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}