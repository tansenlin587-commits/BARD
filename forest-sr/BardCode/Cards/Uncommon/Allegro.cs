using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 快板｜Allegro
/// 效果：全体友方获得“快板”效果，接下来2回合开始时获得1点能量。
/// 升级：持续时间 2→3 回合
/// </summary>
public sealed class Allegro : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Duration", 2m)  // 持续时间（回合数）
    };

    public Allegro()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.SONG];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int duration = base.DynamicVars["Duration"].IntValue;

        // 获取全体友方（参考 EnergySurge）
        IEnumerable<Creature> allies = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
                                       where c != null && c.IsAlive && c.IsPlayer
                                       select c;

        // 对每个友方施加快板Power
        foreach (Creature ally in allies)
        {
            await PowerCmd.Apply<AllegroPower>(
                ally,
                duration,  // 层数 = 持续回合数
                base.Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：持续时间 2 → 3
        base.DynamicVars["Duration"].UpgradeValueBy(1m);
    }
}