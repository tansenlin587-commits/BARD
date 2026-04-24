using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 舞动青春｜DancingYouth
/// 效果：能力。每回合开始时，若上一回合打出了至少2种类型的牌，抽1张牌。
/// 升级：费用 1→0
/// </summary>
public sealed class DancingYouth : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

    public DancingYouth()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加能力Power，在每回合开始时触发
        await PowerCmd.Apply<DancingYouthPower>(
            base.Owner.Creature,
            1m,  // 层数（用于标记）
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：费用 1 → 0
        base.EnergyCost.UpgradeBy(-1);
    }
}