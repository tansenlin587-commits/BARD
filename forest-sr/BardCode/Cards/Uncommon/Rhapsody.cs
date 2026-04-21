using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

/// <summary>
/// 狂想曲
/// </summary>
public sealed class Rhapsody : BardCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(2),
        new EnergyVar(1)
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        BardKeyword.SONG
    };

    public Rhapsody() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int drawCount = base.DynamicVars.Cards.IntValue;
        int energyPerSpell = base.DynamicVars.Energy.IntValue;

        // 获取手牌列表（通过 PlayerCombatState.Hand.Cards）
        var handCards = base.Owner.PlayerCombatState?.Hand?.Cards;
        if (handCards == null) return;

        // 记录抽牌前的手牌（使用 Cards 属性）
        var beforeHand = new HashSet<CardModel>(handCards);

        // 抽牌
        await CardPileCmd.Draw(choiceContext, drawCount, base.Owner);

        // 重新获取手牌
        handCards = base.Owner.PlayerCombatState?.Hand?.Cards;
        if (handCards == null) return;

        // 计算新抽到的法术牌数量
        int spellCount = handCards
            .Where(c => !beforeHand.Contains(c) && c.Keywords.Contains(BardKeyword.Magic))
            .Count();

        if (spellCount > 0)
        {
            await PlayerCmd.GainEnergy(spellCount * energyPerSpell, base.Owner);
        }

    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}