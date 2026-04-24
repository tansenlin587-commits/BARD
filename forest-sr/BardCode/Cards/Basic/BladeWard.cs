using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Basic
{
    public class BladeWard() : BardCard(1,
        CardType.Skill, CardRarity.Basic,
        TargetType.Self)
    {
        private const string _powerVarName = "BladeWard";
        public override bool GainsBlock => true;
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
            new BlockVar(1m, ValueProp.Move),  // 格挡变量，基础值1
            new DynamicVar("BladeWard", 1m)      // 自定义变量，用于BladeWardPower
        };

        public override IEnumerable<CardKeyword> CanonicalKeywords => [Forest_Sr.BardCode.Cards.KeyWord.BardKeyword.Magic];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            // 1. 播放施法动画
            await CreatureCmd.TriggerAnim(
                base.Owner.Creature,           // 施法者生物
                "Cast",                         // 动画名称
                base.Owner.Character.CastAnimDelay  // 动画延迟
            );

            // 2. 获得格挡
            await CreatureCmd.GainBlock(
                base.Owner.Creature,           // 目标：自己
                base.DynamicVars.Block,        // 格挡值：1（升级后3）
                cardPlay                        // 关联的打牌信息
            );

            // 3. 施加剑刃防护（自定义能力）
            await PowerCmd.Apply<BladeWardPower>(
                base.Owner.Creature,                      // 目标：自己
                base.DynamicVars["BladeWard"].BaseValue,   // 层数：1
                base.Owner.Creature,                      // 来源：自己
                this                                      // 关联卡牌
            );
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);  //添加保留
        }
    }
}