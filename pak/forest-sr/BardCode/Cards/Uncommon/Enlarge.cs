using Forest_Sr.BardCode.Cards.KeyWord;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 变巨术｜Enlarge
/// 效果：使一个友方获得巨化效果（下一次强化攻击造成3倍伤害）
/// </summary>
public sealed class Enlarge : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<GigantificationPower>(1m)   // 巨化层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<GigantificationPower>(),
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    };

    public Enlarge() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
    {
    }

    public override List<CardKeyword> CanonicalKeywords => new List<CardKeyword>
    {
        CardKeyword.Exhaust,
        BardKeyword.Magic
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature target = cardPlay.Target ?? base.Owner.Creature;
        if (target == null) return;

        // 播放红色特效
        NCombatRoom.Instance?.PlaySplashVfx(target, new Color(Colors.Red));
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 施加巨化能力（1层）
        await PowerCmd.Apply<GigantificationPower>(
            target,
            base.DynamicVars["GigantificationPower"].BaseValue,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}