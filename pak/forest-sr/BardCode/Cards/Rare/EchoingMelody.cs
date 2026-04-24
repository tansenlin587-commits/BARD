using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 余音绕梁｜EchoingMelody
/// 效果：给予自己2层虚弱。每回合开始时，打出上回合最后一张乐曲卡的带消耗复制。
/// 升级：虚弱 2→1 层
/// </summary>
public sealed class EchoingMelody : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<WeakPower>(2m)  // 虚弱层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<WeakPower>()
    };

    public EchoingMelody()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 给予自己虚弱
        await PowerCmd.Apply<WeakPower>(
            base.Owner.Creature,
            base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature,
            this
        );

        // 2. 施加能力Power
        await PowerCmd.Apply<EchoingMelodyPower>(
            base.Owner.Creature,
            1m,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：虚弱 2 → 1 层
        base.DynamicVars.Weak.UpgradeValueBy(-1m);
    }
}