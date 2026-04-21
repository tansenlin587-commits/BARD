using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 道听途说｜Hearsay
/// 效果：随机获得一张诗人乐曲或法术牌，其本回合0费。
/// 升级：移除消耗
/// </summary>
public sealed class Hearsay : BardCard
{
    private CardModel? _mockSelectedCard;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust  // 消耗（升级后移除）
    };

    public Hearsay()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? selectedCard;

        // 测试用：如果预设了模拟卡牌
        if (_mockSelectedCard != null)
        {
            selectedCard = _mockSelectedCard;
        }
        else
        {
            // 获取所有已解锁的诗人卡牌
            var allUnlockedCards = base.Owner.Character.CardPool
                .GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint);

            // 过滤出乐曲或法术牌
            var eligibleCards = allUnlockedCards
                .Where(card => card.Keywords.Contains(BardKeyword.SONG)
                            || card.Keywords.Contains(BardKeyword.Magic))
                .ToList();

            // 如果没有符合条件的卡牌，返回
            if (eligibleCards.Count == 0)
            {
                return;
            }

            // 随机选择3张供玩家选择（参考 Discovery）
            List<CardModel> cards = CardFactory.GetDistinctForCombat(
                base.Owner,
                eligibleCards,
                3,
                base.Owner.RunState.Rng.CombatCardGeneration)
                .ToList();

            selectedCard = await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                cards,
                base.Owner,
                canSkip: true);
        }

        if (selectedCard != null)
        {
            // 设置本回合0费
            selectedCard.EnergyCost.SetThisTurnOrUntilPlayed(0);
            // 添加到手牌
            await CardPileCmd.AddGeneratedCardToCombat(selectedCard, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：移除消耗关键字
        RemoveKeyword(CardKeyword.Exhaust);
    }

}