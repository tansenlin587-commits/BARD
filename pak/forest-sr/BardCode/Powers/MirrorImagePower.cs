using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

public sealed class MirrorImagePower : BardPower
{
    private int _reductionPerLayer = 4;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public void SetReductionPerLayer(int reduction)
    {
        _reductionPerLayer = reduction;
    }

    /// <summary>
    /// 修改受到的伤害（减法）
    /// </summary>
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner) return 0m;
        if (base.Amount <= 0) return 0m;

        decimal reduction = base.Amount * _reductionPerLayer;

        Flash();

        return -reduction;  // 减少伤害
    }

    /// <summary>
    /// 受到攻击后减少层数
    /// </summary>
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner) return;
        if (base.Amount <= 0) return;

        // 只要被攻击，就减少1层
        await PowerCmd.Decrement(this);

        if (base.Amount == 0) await PowerCmd.Remove(this);
    }
}