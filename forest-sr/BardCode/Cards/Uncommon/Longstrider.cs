using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;
/// <summary>
/// 大步奔行
/// </summary>
public sealed class Longstrider : BardCard
{
    public Longstrider() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer) { }

    protected override List<DynamicVar> CanonicalVars => new List<DynamicVar>
    {
        new PowerVar<DexterityPower>(1m),  // 1层敏捷
        new CardsVar(1)                   // 抽1张牌
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic
    };



    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 统一处理目标
        Creature target = cardPlay.Target ?? base.Owner.Creature;
        Player targetPlayer = target.Player ?? base.Owner;

        // 施加敏捷
        await PowerCmd.Apply<DexterityPower>(
            target,
            base.DynamicVars.Dexterity.BaseValue,
            base.Owner.Creature,
            this
        );

        // 抽牌 - 使用统一的 targetPlayer
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, targetPlayer);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);

        //base.DynamicVars.Dexterity.UpgradeValueBy(1m);
    }






}