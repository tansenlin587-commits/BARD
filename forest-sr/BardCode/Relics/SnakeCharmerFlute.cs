using BaseLib.Extensions;
using Forest_Sr.BardCode.Cards.KeyWord;
using Forest_Sr.BardCode.Powers;
using Forest_Sr.BardCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Relics;
public class SnakeCharmerFlute : BardRelics
{
    public override RelicRarity Rarity => RelicRarity.Common; //稀有度

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {   
        new CardsVar(1)  // 添加的卡牌数量
    };
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[]
    {
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromCard<Snakebite>()      // 显示 Snakebite 卡牌预览
    };
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        // 只对遗物拥有者生效，且只在第一回合
        if (player == base.Owner && combatState.RoundNumber == 1)
        {
            List<CardModel> list = new List<CardModel>();

            // 根据配置的数量创建 Luminesce 卡牌
            for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
            {
                CardModel Snakebite = base.Owner.Creature.CombatState.CreateCard<Snakebite>(base.Owner);
                CardCmd.Upgrade(Snakebite);
                CardCmd.ApplyKeyword(Snakebite, BardKeyword.SONG);
                list.Add(base.Owner.Creature.CombatState.CreateCard<Snakebite>(base.Owner));
            }
            

            // 将生成的卡牌添加到手牌
            await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer: true);
        }
    }
}