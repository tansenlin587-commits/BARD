using Forest_Sr.BardCode.Cards.KeyWord;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Other
{
    public sealed class FoxCunning : BardCard, KnowledgeDemon.IChoosable
    {
        public override bool CanBeGeneratedInCombat => false;

        public override int MaxUpgradeLevel => 0;

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("Cards", 2m)
        };

        public FoxCunning()
            : base(-1, CardType.Status, CardRarity.Status, TargetType.None)
        {
        }

        public async Task OnChosen()
        {
            int drawCount = base.DynamicVars["Cards"].IntValue;

            var choiceContext = new BlockingPlayerChoiceContext();
            await CardPileCmd.Draw(choiceContext, drawCount, base.Owner);
            // 本回合下一张法术牌费用-1
            await PowerCmd.Apply<NextSpellCostReductionPower>(
                base.Owner.Creature,
                1m,
                base.Owner.Creature,
                this
            );
        }
    }
}