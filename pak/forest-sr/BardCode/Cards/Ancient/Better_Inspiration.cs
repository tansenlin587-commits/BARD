using BaseLib.Extensions;
using Forest_Sr.BardCode.Powers;
using Forest_Sr.BardCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Relics;

public class Better_Inspiration : BardRelics
{
    public override RelicRarity Rarity => RelicRarity.Ancient; //稀有度

    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<Bardic_Inspiration>();

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<VigorPower>(6m) };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromPower<VigorPower>() };
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) //回合开始时
    {
        Flash(); //闪一下

        await PowerCmd.Apply<VigorPower>(
                player.Creature.CombatState.Allies,
                base.DynamicVars["VigorPower"].IntValue,
                player.Creature,
                null
            );
    }
}