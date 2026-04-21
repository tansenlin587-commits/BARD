using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 法力充沛｜ManaSurge
/// 效果：能力。每回合第一次释放法术时，抽1张牌。
/// 升级：抽牌 1→2
/// </summary>
public sealed class ManaSurge : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(1)  // 抽牌数量
    };


    public ManaSurge()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加法力充沛能力Power
        await PowerCmd.Apply<ManaSurgePower>(
            base.Owner.Creature,
            base.DynamicVars.Cards.BaseValue,  // 抽牌数量（1或2）
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：抽牌 1 → 2
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}