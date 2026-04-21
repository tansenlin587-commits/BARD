using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 通宵谱曲｜All-Night Composition
/// 效果：获得2点能量，往抽牌堆加入1张晕眩。消耗。
/// 升级：能量2→3
/// </summary>
public sealed class AllNightComposition : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(2),           // 获得能量
    };

    public AllNightComposition() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromCard<Dazed>() };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放创作特效（紫色灵感光芒）
        NCombatRoom.Instance?.PlaySplashVfx(base.Owner.Creature, new Color("#9B59B6"));

        // 获得能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);

        //塞眩晕
        CardModel card = base.CombatState.CreateCard<Dazed>(base.Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: true));

        // 短暂延迟，让特效和动画有足够时间播放
        await Cmd.Wait(0.25f);
    }


    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}