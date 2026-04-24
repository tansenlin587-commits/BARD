using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 英勇冲刺｜ValiantDash
/// 效果：消耗所有的活力，每消耗2层抽1张牌。消耗。
/// 升级：移除消耗，获得保留。
/// </summary>
public sealed class ValiantDash : BardCard
{
    public override List<CardKeyword> CanonicalKeywords => new List<CardKeyword>
    {
        CardKeyword.Exhaust
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Threshold", 2m)  // 每2层抽1张牌
    };

    public ValiantDash()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取当前的活力层数
        var vigorPower = base.Owner.Creature.GetPower<VigorPower>();
        int vigorAmount = vigorPower?.Amount ?? 0;

        if (vigorAmount > 0)
        {
            // 计算抽牌数量：每2层抽1张（向下取整）
            int threshold = base.DynamicVars["Threshold"].IntValue;
            int drawCount = vigorAmount / threshold;

            if (drawCount > 0)
            {
                // 抽牌
                await CardPileCmd.Draw(choiceContext, drawCount, base.Owner);
            }

            // 消耗所有活力
            await PowerCmd.Remove(vigorPower);
        }
    }

    
    protected override void OnUpgrade()
    {
        
         AddKeyword(CardKeyword.Retain);
        
    }
}