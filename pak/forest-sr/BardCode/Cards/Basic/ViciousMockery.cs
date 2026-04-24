using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Basic
{
    internal class ViciousMockery() : BardCard(0,
    CardType.Skill, CardRarity.Basic,
    TargetType.AnyEnemy)
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
        {
            HoverTipFactory.FromPower<WeakPower>()
        };

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
        new DamageVar(4m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
        new PowerVar<WeakPower>(1m)
        };
        public override IEnumerable<CardKeyword> CanonicalKeywords => [Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic];



        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            float num = base.Owner.Character.AttackAnimDelay;

           
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, base.DynamicVars.Damage, this);

            await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(4m);
            //base.DynamicVars.Weak.UpgradeValueBy(1m);  //敲了加虚弱层数
            //base.EnergyCost.UpgradeBy(-1);
        }
    }
}
