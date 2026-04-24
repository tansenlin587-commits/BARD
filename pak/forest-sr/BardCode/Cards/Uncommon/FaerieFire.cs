using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//妖火
namespace Forest_Sr.BardCode.Cards.Uncommon
{
    public sealed class FaerieFire : BardCard
    {
        private const string _powerKey = "Power";

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new PowerVar<VulnerablePower>(3m)  // 易伤层数
        };

        public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
        {
            CardKeyword.Exhaust,  // 消耗
            KeyWord.BardKeyword.Magic
        };

        protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
        {
        HoverTipFactory.FromPower<VulnerablePower>()
        };

        public FaerieFire() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

            foreach (Creature enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<VulnerablePower>(enemy, base.DynamicVars.Vulnerable.IntValue, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Vulnerable.UpgradeValueBy(2m);  // 3 → 5 层易伤
        }
    }
}
