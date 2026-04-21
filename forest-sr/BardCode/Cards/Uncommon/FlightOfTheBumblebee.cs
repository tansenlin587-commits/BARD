using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 野蜂飞翔｜FlightOfTheBumblebee
/// 效果：随机造成2点伤害4次。
/// 升级：随机造成2点伤害5次
/// </summary>
public sealed class FlightOfTheBumblebee : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(2m, ValueProp.Move),   // 单次伤害
        new RepeatVar(4)                     // 攻击次数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(BardKeyword.SONG)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        BardKeyword.SONG
    };

    public FlightOfTheBumblebee()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(base.DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .TargetingRandomOpponents(base.CombatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级：攻击次数 4 → 5
        base.DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}