using BaseLib.Abstracts;
using BaseLib.Extensions;
using Forest_Sr.BardCode.Extensions;
using Godot;
using Forest_Sr.Bard;

namespace Forest_Sr.BardCode.Powers;

public abstract class BardPower : CustomPowerModel
{
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}
