using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 提振士气｜BoostMorale
/// 效果：全体友方获得5点格挡。消耗所有活力，每有一点活力额外获得1点格挡。
/// 升级：基础格挡 5→8
/// </summary>
public sealed class BoostMorale : BardCard
{

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(5m, ValueProp.Move)  // 基础格挡值
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<VigorPower>()  // 活力提示
    };

    public BoostMorale()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取当前活力层数（参考 EnergySurge 的获取方式）
        var vigorPower = base.Owner.Creature.GetPower<VigorPower>();
        int vigorAmount = vigorPower?.Amount ?? 0;

        // 计算总格挡 = 基础格挡 + 活力层数
        int totalBlock = base.DynamicVars.Block.IntValue + vigorAmount;
        BlockVar blockVar = new BlockVar(totalBlock, ValueProp.Move);

        // 获取全体友方（参考 EnergySurge）
        IEnumerable<Creature> allies = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
                                       where c != null && c.IsAlive && c.IsPlayer
                                       select c;

        // 对每个友方给予格挡
        foreach (Creature ally in allies)
        {
            await CreatureCmd.GainBlock(ally, blockVar, cardPlay);
        }

        // 消耗所有活力
        if (vigorPower != null)
        {
            await PowerCmd.Remove(vigorPower);
        }

    }

    protected override void OnUpgrade()
    {
        // 升级：基础格挡 5 → 8
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}