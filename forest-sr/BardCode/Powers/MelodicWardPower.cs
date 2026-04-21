using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards.KeyWord;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 旋律护身能力
/// 效果：每当你打出法术牌时，获得格挡
/// </summary>
public sealed class MelodicWardPower : PowerModel
{
    private decimal _blockAmount = 2m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.Static(StaticHoverTip.Block)
    };

    public void SetBlockAmount(decimal amount)
    {
        _blockAmount = amount;
    }

    // 当打出卡牌时触发
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {

        if (!cardPlay.Card.Keywords.Contains(BardKeyword.Magic)) return;
        if (base.Owner == null) return;

        decimal stacks = base.Amount;  // 使用Stacks属性获取当前层数
        
        if (stacks <= 0) return;
        // 获得格挡
        await CreatureCmd.GainBlock(base.Owner, stacks, ValueProp.Move, null);
    }
}