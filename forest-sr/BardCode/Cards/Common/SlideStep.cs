using BaseLib.Utils;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 滑步｜SlideStep
/// 效果：获得4点格挡，获得1点临时敏捷。
/// 升级：格挡+2（4→6），临时敏捷+1（1→2）
/// </summary>
public sealed class SlideStep : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1),                   // 抽1张牌 
        new BlockVar(4m, ValueProp.Move),           // 基础格挡值
        new PowerVar<DexterityPower>(1m)            // 临时敏捷层数
    };



    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<DexterityPower>()  // 敏捷提示
    };

    public SlideStep()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        await PowerCmd.Apply<SlideStepPower>(
            base.Owner.Creature,
            base.DynamicVars.Dexterity.BaseValue,
            base.Owner.Creature,
            this
         );

        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue,base.Owner);
    }

    protected override void OnUpgrade()
    {
        //.Block.UpgradeValueBy(2m);
        // 升级：临时敏捷 +1（1→2）
        DynamicVars.Dexterity.UpgradeValueBy(1m);
    }
}