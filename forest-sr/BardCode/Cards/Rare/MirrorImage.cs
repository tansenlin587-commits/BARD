using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 镜影术｜MirrorImage
/// 效果：获得2层镜像。每次被攻击时，减少4×层数的伤害，然后减少1层。
/// 升级：镜像层数 2→3
/// </summary>
public sealed class MirrorImage : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<MirrorImagePower>(2m),      // 镜像层数
        new IntVar("ReductionPerLayer", 4m)      // 每层减少的伤害
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<MirrorImagePower>()  // 镜像提示
    };

    public MirrorImage()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        BardKeyword.Magic
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加镜像Power
        await PowerCmd.Apply<MirrorImagePower>(
            base.Owner.Creature,
            base.DynamicVars["MirrorImagePower"].BaseValue,      // 镜像层数
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：镜像层数 2 → 3
        base.DynamicVars["MirrorImagePower"].UpgradeValueBy(1m);
    }
}