using Forest_Sr.BardCode.Cards.Common;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Forest_Sr.BardCode.Powers;

public class SlideStepPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Card<SlideStep>();
}