using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 超频施法｜OverclockCasting
/// 效果：能力。你的所有法术牌获得消耗和再次打出。
/// 升级：移除虚无
/// </summary>
public sealed class OverclockCasting : BardCard
{
    private const string POWER_KEY = "OverclockCasting";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar(POWER_KEY, 1m)  // 能力标记
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Ethereal  // 虚无（升级后移除）
    };

    public OverclockCasting()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 施加超频施法能力Power
        await PowerCmd.Apply<OverclockCastingPower>(
            base.Owner.Creature,
            base.DynamicVars[POWER_KEY].BaseValue,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：移除虚无关键字
        RemoveKeyword(CardKeyword.Ethereal);
    }
}