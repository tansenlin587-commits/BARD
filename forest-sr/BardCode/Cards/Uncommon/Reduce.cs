using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Forest_Sr.BardCode.Cards.KeyWord;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 缩小术｜Reduce
/// 效果：使一个敌人获得缩小效果（造成的强化攻击伤害减少30%，持续2/3回合）
/// </summary>
public sealed class Reduce : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new RepeatVar(2)   // 缩小持续回合数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    };

    public Reduce() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [BardKeyword.Magic];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature target = cardPlay.Target;
        if (target == null) return;

        // 播放绿色特效
        NCombatRoom.Instance?.PlaySplashVfx(target, new Color("65cf81"));
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 施加缩小能力（2/3层，每层持续1回合）
        await PowerCmd.Apply<ShrinkPower>(
            target,
            base.DynamicVars.Repeat.BaseValue,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：持续时间 2 → 3 回合
        base.DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}