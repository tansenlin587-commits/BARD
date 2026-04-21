using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Forest_Sr.BardCode.Cards.KeyWord;
using Forest_Sr.BardCode.Powers;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 英勇气势｜Valiant Presence
/// 效果：能力牌。每当你失去活力时，获得等量的格挡。
/// </summary>
public sealed class ValiantPresence : BardCard
{
    private const string _blockForVigorKey = "BlockForVigor";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("BlockForVigor", 1m)   // 每层活力转化为1点格挡
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.Static(StaticHoverTip.Block)
    };

    public ValiantPresence() : base(1, CardType.Power, CardRarity.Rare , TargetType.Self)
    {
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // 施加英勇气势能力
        await PowerCmd.Apply<ValiantPresencePower>(
            Owner.Creature,
            DynamicVars["BlockForVigor"].BaseValue,  // 转换比例（1:1）
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：获得固有
        AddKeyword(CardKeyword.Innate);
    }
}