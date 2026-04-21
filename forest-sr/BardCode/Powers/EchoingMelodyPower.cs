using Forest_Sr.BardCode.Cards.KeyWord;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;      
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Forest_Sr.BardCode.Powers;

public sealed class EchoingMelodyPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        
        if (player != base.Owner.Player) return;

        var creature = base.Owner;  
        if (creature.CombatState.RoundNumber == 1) return;

        int lastRound = creature.CombatState.RoundNumber - 1;

        // 获取上一回合最后一张乐曲卡
        var entry = CombatManager.Instance.History.CardPlaysFinished
            .LastOrDefault(e => e.CardPlay.Card.Owner == player  
                && e.RoundNumber == lastRound
                && e.CardPlay.Card.Keywords.Contains(BardKeyword.SONG)
                && !e.CardPlay.Card.IsDupe);

        if (entry != null)
        {
            Flash();
            CardModel dupe = entry.CardPlay.Card.CreateDupe();
            await CardCmd.AutoPlay(choiceContext, dupe, null);
        }
    }
}