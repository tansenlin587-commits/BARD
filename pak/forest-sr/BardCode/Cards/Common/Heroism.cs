using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 英雄气概｜Heroism
/// 效果：目标每回合开始时获得临时生命值（格挡）
/// </summary>
public sealed class Heroism : BardCard
{
    private const string _durationKey = "Duration";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("Duration", 3m),      // 持续3回合
        new BlockVar(5m, ValueProp.Move)      // 每回合格挡值
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.Static(StaticHoverTip.Block)
    };

    public Heroism() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature target = cardPlay.Target ?? base.Owner.Creature;

        // 施加英雄气概效果，传递格挡值
        var power = await PowerCmd.Apply<HeroismPower>(
            target,
            base.DynamicVars["Duration"].BaseValue,
            base.Owner.Creature,
            this
        );

        // 设置每回合获得的格挡值
        power?.SetBlockAmount(base.DynamicVars.Block.BaseValue);
    }

    protected override void OnUpgrade()
    {
        // 升级：持续时间 +1（3 → 4）
        base.DynamicVars["Duration"].UpgradeValueBy(1m);
        // 可选：格挡值 +2（5 → 7）
        // base.DynamicVars.Block.UpgradeValueBy(2m);
    }
}