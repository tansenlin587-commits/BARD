using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Other
{
    public sealed class CatGrace : BardCard, KnowledgeDemon.IChoosable
    {
        public override bool CanBeGeneratedInCombat => false;

        public override int MaxUpgradeLevel => 0;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new PowerVar<DexterityPower>(2m)
        };

        public CatGrace()
            : base(-1, CardType.Status, CardRarity.Status, TargetType.None)
        {
        }

        public async Task OnChosen()
        {
            await PowerCmd.Apply<DexterityPower>(
                base.Owner.Creature,
                base.DynamicVars["DexterityPower"].IntValue,
                base.Owner.Creature,
                this
            );
        }
    }
}