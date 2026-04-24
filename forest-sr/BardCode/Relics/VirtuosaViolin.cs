using BaseLib.Extensions;
using Forest_Sr.BardCode.Cards.KeyWord;
using Forest_Sr.BardCode.Powers;
using Forest_Sr.BardCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Relics;
public class VirtuosaViolin : BardRelics
{
    public override RelicRarity Rarity => RelicRarity.Uncommon; //稀有度

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<DoomPower>(2m) };  //基础数值

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromPower<DoomPower>() };   //显示
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay) //打出卡牌后
    {
        if (cardPlay.Card.Keywords.Contains(BardKeyword.SONG) && cardPlay.Card.Owner == base.Owner)
        {
            foreach (Creature hittableEnemy2 in base.Owner.Creature.CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<DoomPower>(hittableEnemy2, base.DynamicVars["PoisonPower"].IntValue, base.Owner.Creature, null);
            }
        }
    }
}