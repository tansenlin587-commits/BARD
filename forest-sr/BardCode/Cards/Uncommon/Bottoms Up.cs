using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 把酒迎欢｜Bottoms Up
/// 效果：获得1层力量，4层活力，往弃牌堆加入2/1张眩晕。
/// </summary>
public sealed class BottomsUp : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(1m),      // 力量层数
        new PowerVar<VigorPower>(4m),         // 活力层数
        new DynamicVar("DizzyCount", 2m)      // 眩晕加入数量
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromCard<Dazed>()
    };

    public BottomsUp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放饮酒特效（红色/酒色）
        NCombatRoom.Instance?.PlaySplashVfx(base.Owner.Creature, new Color("#E74C3C"));

        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 获得力量
        await PowerCmd.Apply<StrengthPower>(
            base.Owner.Creature,
            base.DynamicVars.Strength.BaseValue,
            base.Owner.Creature,
            this
        );

        // 获得活力
        await PowerCmd.Apply<VigorPower>(
                base.Owner.Creature,
                base.DynamicVars["VigorPower"].IntValue,
                base.Owner.Creature,
                this
         );

        // 往弃牌堆加入眩晕
        int dizzyCount = base.DynamicVars["DizzyCount"].IntValue;
        for (int i = 0; i < dizzyCount; i++)
        {
            CardModel dizzy = base.CombatState.CreateCard<Dazed>(base.Owner);
            await CardPileCmd.AddGeneratedCardToCombat(dizzy, PileType.Discard, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：眩晕加入数量 -1（2 → 1）
        base.DynamicVars["DizzyCount"].UpgradeValueBy(-1m);
        // 力量不变（1 → 1）
        // 活力不变（4 → 4）
    }
}