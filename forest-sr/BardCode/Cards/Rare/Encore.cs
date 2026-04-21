using BaseLib.Utils;
using Forest_Sr.BardCode.Cards;
using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Cards.Rare;

/// <summary>
/// 安可｜Encore
/// 效果：造成6点伤害。
/// 本场战斗每打出过一张乐曲牌，伤害增加3点。
/// 升级：基础伤害+2（6→8），每张乐曲牌加成+1（3→4）
/// </summary>
public sealed class Encore : BardCard
{
    public Encore() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, ValueProp.Move),  // 基础伤害值
        new IntVar("Increase", 3m)          // 每张乐曲牌加成值
    };

    // 获取本场战斗打出的乐曲牌数量
    private int GetUsedSongCount()
    {
        var history = CombatManager.Instance.History.CardPlaysStarted;
        int usedSongs = 0;

        // 过滤出自己打出的牌
        var filtered = history.Where(entry => entry.CardPlay.Card.Owner == base.Owner);

        foreach (var entry in filtered)
        {
            // 检查是否包含乐曲牌关键词
            if (entry.CardPlay.Card.Keywords.Contains(BardKeyword.SONG))
            {
                usedSongs++;
            }
        }

        return usedSongs;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 获取乐曲牌数量
        int songCount = GetUsedSongCount();

        // 获取基础伤害和每张加成
        decimal baseDamage = DynamicVars.Damage.BaseValue;
        decimal bonusPerSong = DynamicVars["Increase"].BaseValue;

        // 计算总伤害 = 基础伤害 + (乐曲牌数量 × 每张加成)
        decimal totalDamage = baseDamage + (songCount * bonusPerSong);

        // 造成伤害
        await DamageCmd.Attack(totalDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 基础伤害 +2（6 → 8）
        DynamicVars.Damage.UpgradeValueBy(2m);
        // 每张乐曲牌加成 +1（3 → 4）
        DynamicVars["Increase"].UpgradeValueBy(1m);
    }
}