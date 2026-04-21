using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 安定心神｜CalmEmotions
/// 效果：清除所有负面效果（Debuff）。
/// 升级：费用 1→0
/// </summary>
public sealed class CalmEmotions : BardCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust, // 消耗
        Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();


    public CalmEmotions()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyPlayer)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 统一处理目标（参考 Longstrider）
        Creature target = cardPlay.Target ?? base.Owner.Creature;
        Player targetPlayer = target.Player ?? base.Owner;

        // 清除目标所有负面效果
        await ClearDebuffs(target);

    }

    /// <summary>
    /// 清除生物身上的所有负面效果
    /// </summary>
    private async Task ClearDebuffs(Creature target)
    {
        // 获取目标身上的所有Power
        var powers = target.Powers.ToList();

        // 清除所有负面效果（Debuff）
        foreach (var power in powers)
        {
            if (power.Type == PowerType.Debuff)
            {
                await PowerCmd.Remove(power);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：费用 1 → 0
        base.EnergyCost.UpgradeBy(-1);
    }
}