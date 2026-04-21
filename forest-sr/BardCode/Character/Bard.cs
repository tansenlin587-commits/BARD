using BaseLib.Abstracts;
using Forest_Sr.BardCode.Character;
using Forest_Sr.BardCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.Basic;
using Forest_Sr.BardCode.Relics;
using System;

namespace Forest_Sr.BardCode.Character;

public class Bard : PlaceholderCharacterModel
{
	public const string CharacterId = "Bard";

	//public override string CustomCharacterSelectBg => "res://Selphina/Scenes/Char_Select/char_select_bg_selphina.tscn";   //背景场景

	public override string PlaceholderID => "necrobinder";  //指向死灵的美术资源

	public static readonly Color Color = new Color("#FFD1DC");

	public override Color NameColor => Color;
	public override CharacterGender Gender => CharacterGender.Feminine;
	public override int StartingHp => 77;

	public override IEnumerable<CardModel> StartingDeck => [
		ModelDb.Card<BardAttack>(),
		ModelDb.Card<BardAttack>(),
		ModelDb.Card<BardAttack>(),
		ModelDb.Card<BardAttack>(),
		ModelDb.Card<BardBlock>(),
		ModelDb.Card<BardBlock>(),
		ModelDb.Card<BardBlock>(),
		ModelDb.Card<BardBlock>(),
		ModelDb.Card<ViciousMockery>(),
		ModelDb.Card<BladeWard>()
	];

	public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<Bardic_Inspiration>()];

	public override CardPoolModel CardPool => ModelDb.CardPool<BardCardPool>();
	public override RelicPoolModel RelicPool => ModelDb.RelicPool<BardRelicPool>();
	public override PotionPoolModel PotionPool => ModelDb.PotionPool<SharedPotionPool>(); 

	/*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
		override all the other methods that define those assets.
		These are just some of the simplest assets, given some placeholders to differentiate your character with.
		You don't have to, but you're suggested to rename these images. */
		
	public override string CustomVisualPath => "res://Bard/Scenes/BardVisual.tscn";
	public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
	public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
	public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
	public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
