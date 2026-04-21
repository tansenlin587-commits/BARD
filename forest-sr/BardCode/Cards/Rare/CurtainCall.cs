using Forest_Sr.BardCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;


namespace Forest_Sr.BardCode.Cards.Rare;
/// <summary>
/// 收场｜CurtainCall
/// 效果：造成6点伤害。本场战斗每使用过一种卡牌类型，就攻击一次。
/// 升级：基础伤害+2（6→8）
/// </summary>
public sealed class CurtainCall : BardCard
{
    private const string _calculatedKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),           // 基础伤害
        new CalculationBaseVar(0m),                  // 计算基数
        new CalculationExtraVar(1m),                 // 额外乘数
        new CalculatedVar(_calculatedKey).WithMultiplier((card, _) =>
            // 计算使用过的卡牌类型数量
            CombatManager.Instance.History.CardPlaysStarted
                .Where(e => e.CardPlay.Card.Owner == card.Owner)
                .Select(e => e.CardPlay.Card.Type)
                .Distinct()
                .Count()
        )
    };

    public CurtainCall()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        int hitCount = (int)((CalculatedVar)DynamicVars[_calculatedKey]).Calculate(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);  // 6 → 8
    }
}