using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Other
{
   public sealed class BullStrength : BardCard, KnowledgeDemon.IChoosable
    {
        public override bool CanBeGeneratedInCombat => false;

        public override int MaxUpgradeLevel => 0;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
             new PowerVar<StrengthPower>(2m)
        };

        public BullStrength()
            : base(-1, CardType.Status, CardRarity.Status, TargetType.None)
        {
        }

        public async Task OnChosen()
        {
            await PowerCmd.Apply<StrengthPower>(
                base.Owner.Creature,
                base.DynamicVars["StrengthPower"].IntValue,
                base.Owner.Creature,
                this
            );
        }
    }
}
