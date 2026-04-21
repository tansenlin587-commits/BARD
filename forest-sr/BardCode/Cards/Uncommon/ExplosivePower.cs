using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Cards;

/// <summary>
/// 爆发力｜ExplosivePower
/// 效果：消耗3点活力，从抽牌堆抽攻击牌、技能牌、能力牌各1张加入手牌。消耗。
/// 升级：保留。
/// </summary>
public sealed class ExplosivePower : CardModel
{
    public override List<CardKeyword> CanonicalKeywords => new List<CardKeyword>
    {
        CardKeyword.Exhaust
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("VigorCost", 3)
    };

    public ExplosivePower() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取当前的活力层数
        var vigorPower = base.Owner.Creature.GetPower<VigorPower>();
        int vigorAmount = vigorPower?.Amount ?? 0;
        int vigorCost = base.DynamicVars["VigorCost"].IntValue;

        // 检查是否有足够的活力
        if (vigorAmount < vigorCost || vigorPower == null)
        {
            return;
        }

        // 消耗指定层数的活力
        await PowerCmd.ModifyAmount(vigorPower, -vigorCost, base.Owner.Creature, this);

        // 需要抽的牌类型
        CardType[] targetTypes = { CardType.Attack, CardType.Skill, CardType.Power };

        foreach (var targetType in targetTypes)
        {
            // 确保抽牌堆有牌
            await CardPileCmd.ShuffleIfNecessary(choiceContext, base.Owner);

            // 获取抽牌堆
            var drawPile = PileType.Draw.GetPile(base.Owner);

            // 查找指定类型的第一张牌
            var card = drawPile.Cards.FirstOrDefault(c => c.Type == targetType && !c.Keywords.Contains(CardKeyword.Unplayable));

            // 如果找到了，抽这张牌
            if (card != null)
            {
                await CardPileCmd.RemoveFromCombat(card);
                await CardPileCmd.Add(card, PileType.Hand);
            }
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}