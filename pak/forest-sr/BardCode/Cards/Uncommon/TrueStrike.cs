using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 克敌机先
/// </summary>
public sealed class TrueStrike() : BardCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{

    //protected override IEnumerable<DynamicVar> CanonicalVars => new global::_003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("Power", 2m));
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<VulnerablePower>(2m) };  

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]{CardKeyword.Exhaust,KeyWord.BardKeyword.Magic}; //消耗,法术

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
    HoverTipFactory.FromPower<VulnerablePower>()
    };



    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        if (cardPlay.Target.HasPower<ArtifactPower>())
        {
            await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
        }

        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, base.DynamicVars.Vulnerable.IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1m);
    }
}