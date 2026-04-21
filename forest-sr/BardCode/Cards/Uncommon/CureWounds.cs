using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Uncommon;

public sealed class CureWounds : BardCard
{
	protected override List<DynamicVar> CanonicalVars => new List<DynamicVar>
	{
		new HealVar(8m)
	};

	public override List<CardKeyword> CanonicalKeywords => new List<CardKeyword>
	{
		CardKeyword.Exhaust,
		Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic
	};

	public CureWounds() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		// 确定治疗目标
		Creature target = cardPlay.Target ?? base.Owner.Creature;

		// 播放治疗特效
		NCombatRoom.Instance?.PlaySplashVfx(target, new Color("#FFD1DC"));

		// 回复生命
		await CreatureCmd.Heal(target, base.DynamicVars.Heal.IntValue);
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.Heal.UpgradeValueBy(3m);
	}
}
