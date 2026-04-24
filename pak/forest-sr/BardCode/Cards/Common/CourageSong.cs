using System.Collections.Generic;
using System.Threading.Tasks;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Forest_Sr.BardCode.Cards.Common;

/// <summary>
/// 勇气之歌｜CourageSong
/// 效果：3-5活力。
/// 
/// </summary>
public sealed class CourageSong : BardCard
{
	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
	{
		new PowerVar<VigorPower>(3m)            // 活力层数
	};

	public CourageSong()
		: base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
	{
	}

	public override IEnumerable<CardKeyword> CanonicalKeywords => [Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.SONG];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		decimal strengthAmount = base.DynamicVars.Strength.BaseValue;


		await PowerCmd.Apply<VigorPower>(
				base.Owner.Creature,
				base.DynamicVars["VigorPower"].IntValue,
				base.Owner.Creature,
				this
		);
	}

	protected override void OnUpgrade()
	{
		// 升级：临时力量 2 → 4
		base.DynamicVars["VigorPower"].UpgradeValueBy(2m);
	}
}
