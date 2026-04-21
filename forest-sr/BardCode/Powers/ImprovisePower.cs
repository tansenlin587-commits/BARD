using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 即兴能力：每回合开始时随机获得一首乐曲
/// </summary>
public sealed class ImprovisePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromKeyword(BardKeyword.SONG)  // 乐曲提示
    };

    /// <summary>
    /// 抽牌前触发（每回合开始时）
    /// </summary>
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        // 只对自己生效
        if (player != base.Owner.Player) return;

        // 获取所有已解锁的乐曲牌
        var allUnlockedCards = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint);

        // 过滤出乐曲牌
        var songCards = allUnlockedCards
            .Where(card => card.Keywords.Contains(BardKeyword.SONG))
            .ToList();

        if (songCards.Count == 0) return;

        // 随机选择 Amount 张乐曲牌（Amount 默认为1）
        CardModel[] cardsToAdd = new CardModel[base.Amount];
        Rng combatCardGeneration = player.RunState.Rng.CombatCardGeneration;

        for (int i = 0; i < base.Amount; i++)
        {
            // 从乐曲池中随机获取1张
            var randomCard = CardFactory.GetDistinctForCombat(player, songCards, 1, combatCardGeneration).First();
            cardsToAdd[i] = randomCard;
            CardCmd.ApplyKeyword(randomCard, CardKeyword.Exhaust);
        }

        // 闪烁提示
        Flash();

        // 将随机乐曲牌加入手牌
        await CardPileCmd.AddGeneratedCardsToCombat(cardsToAdd, PileType.Hand, addedByPlayer: true);
    }
}