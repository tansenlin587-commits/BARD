using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Common;
/// <summary>
/// 法师之手
/// </summary>
public sealed class MadeHand() : BardCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("RetainAmount", 1m),  // 基础保留数量：1张
        new CardsVar(1)                    // 抽牌数量
    };



    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    };
    public override IEnumerable<CardKeyword> CanonicalKeywords => [Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int retainCount = (int)base.DynamicVars["RetainAmount"].BaseValue;

        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, Owner);

        CardModel[] selectedCards = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, retainCount),  // 使用变量控制数量
            context: choiceContext,
            player: base.Owner,
            filter: (CardModel c) => !c.Keywords.Contains(CardKeyword.Retain),
            source: this)).ToArray();

        

        foreach (var card in selectedCards)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }

        
    }

    protected override void OnUpgrade()
    {
        //base.DynamicVars["RetainAmount"].UpgradeValueBy(1m);  // 升级后可以保留2张牌
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}