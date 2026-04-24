using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards.KeyWord;
using Forest_Sr.BardCode.Cards.Other;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 强化属性｜Enhance Ability
/// 效果：选择并获得以下一种动物之力：
/// 公牛之力：获得2/3层力量
/// 猫之优雅：获得2/3层敏捷
/// 狐之狡黠：抽1/2张牌，本回合下一张法术牌费用-1
/// </summary>
public sealed class EnhanceAbility : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(3m),      // 力量层数
        new PowerVar<DexterityPower>(3m),     // 敏捷层数
        new DynamicVar("Cards", 2m)           // 抽牌数量
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.Static(StaticHoverTip.CardReward)
    };

    public EnhanceAbility() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [BardKeyword.Magic];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法特效
        NCombatRoom.Instance?.PlaySplashVfx(base.Owner.Creature, new Color("#9B59B6"));
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 获取升级后的数值
        decimal strengthAmount = base.DynamicVars.Strength.BaseValue;
        decimal dexterityAmount = base.DynamicVars.Dexterity.BaseValue;
        int drawCount = (int)base.DynamicVars["Cards"].BaseValue;

        // 关键修改：使用 CombatState.CreateCard 创建卡片
        var bullStrength = base.CombatState.CreateCard<BullStrength>(base.Owner);
        bullStrength.DynamicVars["StrengthPower"].BaseValue = strengthAmount;

        var catGrace = base.CombatState.CreateCard<CatGrace>(base.Owner);
        catGrace.DynamicVars["DexterityPower"].BaseValue = dexterityAmount;

        var foxCunning = base.CombatState.CreateCard<FoxCunning>(base.Owner);
        foxCunning.DynamicVars["Cards"].BaseValue = drawCount;

        var optionCards = new List<CardModel> { bullStrength, catGrace, foxCunning };

        // 弹出选择界面
        CardModel selected = await CardSelectCmd.FromChooseACardScreen(
            choiceContext,
            optionCards,
            base.Owner,
            canSkip: true 
        );

        if (selected != null && selected is KnowledgeDemon.IChoosable choosable)
        {
            await choosable.OnChosen();
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：力量 2→3，敏捷 2→3，抽牌 1→2
        base.DynamicVars.Strength.UpgradeValueBy(1m);
        base.DynamicVars.Dexterity.UpgradeValueBy(1m);
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}