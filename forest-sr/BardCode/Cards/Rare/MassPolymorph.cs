using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 群体变形术｜MassPolymorph
/// 效果：消耗。选择手牌中任意张牌，将其变为随机法术牌。
/// 升级：变形得到的卡牌自动升级
/// </summary>
public sealed class MassPolymorph : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        // 无动态变量
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.Static(StaticHoverTip.Transform)  // 变形提示
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust,      // 消耗
        BardKeyword.Magic         // 法术标签
    };

    public MassPolymorph()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放施法动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        // 从手牌中选择任意张牌进行变形（参考 Guards）
        List<CardModel> selectedCards = (await CardSelectCmd.FromHand(
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this,
            prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, 999)
        )).ToList();

        if (selectedCards.Count == 0) return;

        // 对每张选中的牌进行变形
        foreach (CardModel card in selectedCards)
        {
            // 生成随机法术牌
            CardModel randomSpell = await CreateRandomSpellCard();

            if (randomSpell != null)
            {
                // 变形
                await CardCmd.Transform(card, randomSpell);

            }
        }
    }

    /// <summary>
    /// 创建随机法术牌
    /// </summary>
    private async Task<CardModel> CreateRandomSpellCard()
    {
        // 获取所有已解锁的法术牌
        var eligibleCards = base.Owner.Character.CardPool
            .GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
            .Where(card => card.Keywords.Contains(BardKeyword.Magic))
            .ToList();

        if (eligibleCards.Count == 0)
        {
            // 如果没有法术牌，使用默认法术牌
            return ModelDb.Card<DefendIronclad>().ToMutable();
        }

        // 随机选择一个模板
        CardModel template = base.Owner.RunState.Rng.CombatCardGeneration.NextItem(eligibleCards);

        // 生成卡牌
        var cards = CardFactory.GetDistinctForCombat(
            base.Owner,
            new List<CardModel> { template },
            1,
            base.Owner.RunState.Rng.CombatCardGeneration
        );

        CardModel result = cards.FirstOrDefault();

        // ★ 升级效果：变形得到的卡牌自动升级 ★
        if (result != null && base.IsUpgraded)
        {
            CardCmd.Upgrade(result);
        }

        await Cmd.Wait(0.05f);
        return result;
    }

    protected override void OnUpgrade()
    {
        // 升级：变形得到的卡牌自动升级（在 CreateRandomSpellCard 中处理）
        // 费用保持不变（仍是2费）
    }
}