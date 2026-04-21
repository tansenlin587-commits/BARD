using Forest_Sr.BardCode.Cards.KeyWord;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 绝不认输｜NeverGiveUp
/// 效果：乐曲。能力。每当你即将死亡时，改为回复到1点生命，消耗此能力并抽3张牌。
/// 升级：回复到2点生命，抽4张牌。
/// </summary>
public sealed class NeverGiveUp : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("HealAmount", 1m),    // 回复生命值
        new IntVar("DrawAmount", 3m)     // 抽牌数量
    };

    public NeverGiveUp()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        BardKeyword.SONG,  // 乐曲牌
        CardKeyword.Ethereal
    };




    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 施加能力Power
        await PowerCmd.Apply<NeverGiveUpPower>(
            base.Owner.Creature,
            1m,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}