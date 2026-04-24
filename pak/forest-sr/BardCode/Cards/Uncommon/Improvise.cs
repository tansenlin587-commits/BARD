using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 即兴｜Improvise
/// 效果：能力。每回合开始时，随机获得一首乐曲。
/// 升级：获得固有（战斗开始时在手牌中）
/// </summary>
public sealed class Improvise : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1)  // 每回合获得1张牌
    };

    public Improvise()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加 ImprovisePower，每回合随机获得一首乐曲
        await PowerCmd.Apply<ImprovisePower>(
            base.Owner.Creature,
            base.DynamicVars.Cards.BaseValue,  // 获得数量（默认1）
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：获得固有关键字（战斗开始时自动在手牌）
        AddKeyword(CardKeyword.Innate);
    }
}