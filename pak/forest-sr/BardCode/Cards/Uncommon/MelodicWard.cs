using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 旋律护身｜Melodic Ward
/// 效果：每当你打出法术牌时，获得2/3点格挡。固有。
/// </summary>
public sealed class MelodicWard : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(2m, ValueProp.Unpowered)  
    };


    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.Static(StaticHoverTip.Block)
    };

    public MelodicWard() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施放旋律护身能力
        await PowerCmd.Apply<MelodicWardPower>(
            base.Owner.Creature,
            2m,  
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);  
    }
}