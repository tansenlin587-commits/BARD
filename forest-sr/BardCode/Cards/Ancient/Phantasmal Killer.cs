using BaseLib.Abstracts;
using BaseLib.Utils;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.Basic;
using Forest_Sr.BardCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Ancient
{
    [Pool(typeof(BardCardPool))]
    internal class PhantasmalKiller : BardCard , ITranscendenceCard
    {
        public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<ViciousMockery>();   //替换恶言相加
        public PhantasmalKiller() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy) { }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]{HoverTipFactory.FromPower<WeakPower>()};
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DamageVar(22m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move),
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

        public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
        {
            if (player == base.Owner && CombatManager.Instance.History.CardPlaysFinished.Any((CardPlayFinishedEntry e) => e.RoundNumber == base.CombatState.RoundNumber - 1 && e.CardPlay.Card == this))
            {
                CardPile? pile = base.Pile;
                if (pile == null || pile.Type != PileType.Hand)
                {
                    await CardPileCmd.Add(this, PileType.Hand);
                }
            }
        }

        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(6m);
            //base.DynamicVars.Weak.UpgradeValueBy(1m);  //敲了加虚弱层数
            //base.EnergyCost.UpgradeBy(-1);
        }
    }
}
