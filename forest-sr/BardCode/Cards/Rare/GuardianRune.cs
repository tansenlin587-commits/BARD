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
/// 守卫刻文｜GuardianRune
/// 效果：获得18点格挡。每当你失去格挡时，对所有敌人造成等量伤害。
/// 升级：格挡 18→22
/// </summary>
public sealed class GuardianRune : BardCard
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(18m, ValueProp.Move)  // 格挡值
    };


    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic  // 法术标签
    };

    public GuardianRune()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 获得格挡
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 施加守卫刻文能力Power
        await PowerCmd.Apply<GuardianRunePower>(
            base.Owner.Creature,
            1m,  // 标记层数
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡 18 → 22
        base.DynamicVars.Block.UpgradeValueBy(4m);
    }
}