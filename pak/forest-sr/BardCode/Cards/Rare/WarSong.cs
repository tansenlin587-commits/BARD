using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 战歌｜WarSong
/// 效果：能力。每当使用乐曲卡时，获得3点活力。
/// 升级：费用 1→0
/// </summary>
public sealed class WarSong : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<VigorPower>(3m)  // 活力层数
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<VigorPower>()  // 活力提示
    };

    public WarSong()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加战歌能力Power
        await PowerCmd.Apply<WarSongPower>(
            base.Owner.Creature,
            3m,  
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // 升级：费用 1 → 0
        base.EnergyCost.UpgradeBy(-1);
    }
}