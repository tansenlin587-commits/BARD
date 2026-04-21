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
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Relics;
public class Bardic_Inspiration : BardRelics
{
    public override RelicRarity Rarity => RelicRarity.Starter; //稀有度

    // 规范变量：定义一个名为 VigorPower 的动态变量，初始值为 3
    // 这意味着这张遗物会给予 3 层活力
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<VigorPower>(3m) };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>new IHoverTip[] { HoverTipFactory.FromPower<VigorPower>() };
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) //回合开始时
    {
        Flash(); //闪一下

        var combatState = player.Creature.CombatState;

        var allAllies = combatState.Players.Select(p => p.Creature);

        
        foreach (var ally in combatState.Players.Select(p => p.Creature))
        {
            await PowerCmd.Apply<VigorPower>(
                ally,
                base.DynamicVars["VigorPower"].IntValue,
                player.Creature,
                null
            );
        }
    } 
}