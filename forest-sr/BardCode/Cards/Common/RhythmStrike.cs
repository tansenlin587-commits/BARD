using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//韵律打击
namespace Forest_Sr.BardCode.Cards.Common
{
    public class RhythmStrike() : BardCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
    {
        protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, ValueProp.Move),  // 基础伤害
        new EnergyVar(1),                  // 获得1能量
        new CardsVar(1)                   // 抽1张牌

    };

        // 判断上一张打出的牌是否是攻击牌
        private bool WasLastCardPlayedAttack
        {
            get
            {
                CardPlayStartedEntry entry = CombatManager.Instance.History
                    .CardPlaysStarted
                    .LastOrDefault(e => e.CardPlay.Card.Owner == base.Owner
                    && e.HappenedThisTurn(base.CombatState)
                    && e.CardPlay.Card != this);
                return entry?.CardPlay.Card.Type == CardType.Attack;
            }
        }

        // 判断上一张打出的牌是否是技能牌
        private bool WasLastCardPlayedSkill
        {
            get
            {
                CardPlayStartedEntry entry = CombatManager.Instance.History
                    .CardPlaysStarted
                    .LastOrDefault(e => e.CardPlay.Card.Owner == base.Owner
                    && e.HappenedThisTurn(base.CombatState)
                    && e.CardPlay.Card != this);
                return entry?.CardPlay.Card.Type == CardType.Skill;
            }
        }

        protected override bool ShouldGlowGoldInternal => WasLastCardPlayedAttack || WasLastCardPlayedSkill;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

            // 判断上一张牌的类型
            bool isAttack = WasLastCardPlayedAttack;
            bool isSkill = WasLastCardPlayedSkill;

            // 执行攻击
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            // 根据上一张牌类型触发效果
            if (isAttack)
            {
                // 上一张是攻击牌：获得1点能量
                await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);
            }
            else if (isSkill)
            {
                // 上一张是技能牌：抽1张牌
                await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, base.Owner);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1m);
            DynamicVars.Energy.UpgradeValueBy(1m);
            DynamicVars.Cards.UpgradeValueBy(1m);
        }
    }


}
