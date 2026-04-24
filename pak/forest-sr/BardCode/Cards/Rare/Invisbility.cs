using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

public sealed class Invisbility : BardCard
{
    protected override List<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<IntangiblePower>(1m)
    };

    // 卡牌关键词
    public override List<CardKeyword> CanonicalKeywords => new List<CardKeyword>
    {
        CardKeyword.Exhaust,
        KeyWord.BardKeyword.Magic
    };

    // 悬停提示
    protected override List<IHoverTip> ExtraHoverTips => new List<IHoverTip>
    {
        HoverTipFactory.FromPower<IntangiblePower>()
    };

    // 构造函数 - 使用 AnyPlayer 即可选择自己或队友
    public Invisbility() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    // 打出效果
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature target = cardPlay.Target ?? Owner.Creature;

        await PowerCmd.Apply<IntangiblePower>(target, DynamicVars["IntangiblePower"].BaseValue, Owner.Creature, this);

    }

    // 升级效果
    protected override void OnUpgrade()
    {
        DynamicVars["IntangiblePower"].UpgradeValueBy(1m);  // 1 → 2
    }
}