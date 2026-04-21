using Forest_Sr.BardCode.Cards.Common;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Forest_Sr.BardCode.Powers;

/// <summary>
/// 和谐之盾能力：提供临时敏捷，回合结束自动消失
/// </summary>
public sealed class HarmonyShieldPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Card<HarmonyShield>();
}