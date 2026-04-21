using BaseLib.Extensions;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Extensions;

namespace MegaCrit.Sts2.Core.Models.Powers; //这里有问题，之后在改

public sealed class BladeWardPower : BardPower
{
    private const string _damageDecreaseKey = "DamageDecrease";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new DynamicVar("DamageDecrease", 0.5m)  // 伤害减免系数：0.5
        };

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return 1m;
        }
        if (props.HasFlag(ValueProp.Unpowered))
        {
            return 1m;  // 如果是 Unpowered，不减免
        }
        if (dealer == null)
        {
            return 1m;
        }
        if (!dealer.HasPower<WeakPower>())
        {
            return 1m;
        }
        return base.DynamicVars["DamageDecrease"].BaseValue;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }

}
