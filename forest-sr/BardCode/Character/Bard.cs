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

    public override string PlaceholderID => "defect";  //指向死灵的美术资源

    public static readonly Color Color = new Color("#FFD1DC");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 78;

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

    public override string CustomVisualPath => "res://Bard/Scenes/BardVisual.tscn";                   //战斗中的3D/2D模型场景
    public override string CustomIconPath => "res://Bard/Scenes/bard_icon.tscn";                      //左上角头像
    public override string CustomCharacterSelectBg => "res://Bard/Scenes/char_select_bg_bard.tscn";   //选人背景
    //public override string CustomEnergyCounterPath => "res://Bard/Scenes/Bard_energy_counter.tscn";   //能量表盘
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();  //局外显示头像
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();  //选人界面大图标 (静态图片)	
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath(); //选人界面锁定状态图标
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();  //地图上的角色标记图标
    public override string CustomMerchantAnimPath => "res://Bard/Scenes/bard_merchant.tscn";  //商店
    //public override string CustomRestSiteAnimPath => "res://Bard/Scenes/bard_rest_site.tscn";  //火堆

}
