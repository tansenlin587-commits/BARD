using Forest_Sr.BardCode.Cards.Common;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 勇气之歌能力：提供临时力量，回合结束自动消失
/// </summary>
public sealed class CourageSongPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<CourageSong>();
}