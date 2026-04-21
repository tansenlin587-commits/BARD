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
/// 和谐之盾｜HarmonyShield
/// 效果：获得6点格挡，获得2点临时敏捷（回合结束消失）。
/// 升级：临时敏捷 2→4
/// </summary>
public sealed class HarmonyShield : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, ValueProp.Move),           // 基础格挡值
        new PowerVar<DexterityPower>(2m)            // 临时敏捷层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<DexterityPower>()  // 敏捷提示
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => [Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.SONG];
    public HarmonyShield()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        

        // 2. 施加 HarmonyShieldPower（自动处理临时敏捷，回合结束消失）
        await PowerCmd.Apply<HarmonyShieldPower>(
            base.Owner.Creature,
            base.DynamicVars.Dexterity.BaseValue,
            base.Owner.Creature,
            this
        );

        // 1. 获得格挡
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        // 升级：临时敏捷 2 → 4
        DynamicVars.Dexterity.UpgradeValueBy(2m);
    }
}