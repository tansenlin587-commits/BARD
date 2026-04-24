using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Common;
/// <summary>
/// 鸣雷波
/// </summary>
public class Thunderwave() : BardCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("StrengthLoss", 1m)
    };
    public override IEnumerable<CardKeyword> CanonicalKeywords => [Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IReadOnlyList<Creature> enemies = base.CombatState.HittableEnemies;
        foreach (Creature item in enemies)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSpikeSplashVfx.Create(item));
        }


        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitCount(2)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);

        await PowerCmd.Apply<CrushUnderPower>(enemies, base.DynamicVars["StrengthLoss"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["StrengthLoss"].UpgradeValueBy(1m);  // 1 → 2
    }
}
