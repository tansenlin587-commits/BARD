using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Ancient;

/// <summary>
/// 韵律风暴｜RhythmStorm
/// 效果：能力。每当你获得活力时，对所有敌人造成2点法术伤害。
///       每当你使用法术牌或乐曲牌时，获得1层活力。
/// 升级：活力伤害+1（2→3），获得活力+1（1→2）
/// </summary>
public sealed class RhythmStorm : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("DamageAmount", 3m),   // AOE伤害
        new IntVar("VigorAmount", 2m)     // 获得活力层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<VigorPower>()
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        BardKeyword.Magic,
        BardKeyword.SONG
    };

    public RhythmStorm()
        : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 施加能力
        await PowerCmd.Apply<RhythmStormPower>(
            base.Owner.Creature,
            1m,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}