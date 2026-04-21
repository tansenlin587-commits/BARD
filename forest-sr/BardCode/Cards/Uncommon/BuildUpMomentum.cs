using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 蓄势待发｜BuildUpMomentum
/// 效果：获得8点活力。在本回合中保留你的手牌。
/// 升级：费用 2→1
/// </summary>
public sealed class BuildUpMomentum : BardCard
{
    private const string RETAIN_KEY = "Retain";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<VigorPower>(8m),           // 活力层数
        new DynamicVar(RETAIN_KEY, 1m)          // 保留手牌标记
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<VigorPower>(),  // 活力提示
        HoverTipFactory.FromKeyword(CardKeyword.Retain)  // 保留提示
    };

    public BuildUpMomentum()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 获得活力（参考 Equilibrium 的格挡获取方式）
        await PowerCmd.Apply<VigorPower>(
                base.Owner.Creature,
                base.DynamicVars["VigorPower"].IntValue,
                base.Owner.Creature,
                this
         );

        // 2. 本回合保留手牌（参考 Equilibrium 的 RetainHandPower）
        await PowerCmd.Apply<RetainHandPower>(
            base.Owner.Creature,
            base.DynamicVars[RETAIN_KEY].BaseValue,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：费用 2 → 1
        base.EnergyCost.UpgradeBy(-1);
    }
}