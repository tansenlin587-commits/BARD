using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 魔法武器｜MagicWeapon
/// 效果：获得1点力量。你的所有非魔法攻击牌获得魔法词条。
/// 升级：力量 1→2
/// </summary>
public sealed class MagicWeapon : BardCard
{
    private const string POWER_KEY = "Power";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar(POWER_KEY, 1m),           // 能力标记
        new PowerVar<StrengthPower>(1m)          // 力量层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<StrengthPower>(),  // 力量提示
        HoverTipFactory.FromKeyword(Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic)  // 魔法提示
    };

    public MagicWeapon()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放特效（参考 Corruption）
        NPowerUpVfx.CreateNormal(base.Owner.Creature);
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 1. 获得力量
        await PowerCmd.Apply<StrengthPower>(
            base.Owner.Creature,
            base.DynamicVars.Strength.BaseValue,
            base.Owner.Creature,
            this
        );

        // 2. 施加魔法武器能力Power
        await PowerCmd.Apply<MagicWeaponPower>(
            base.Owner.Creature,
            base.DynamicVars[POWER_KEY].BaseValue,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：力量 1 → 2
        base.DynamicVars[POWER_KEY].UpgradeValueBy(1);
    }
}