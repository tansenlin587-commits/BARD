using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;

namespace Forest_Sr.BardCode.Patch;

internal class TheAncient
{
    static bool Prefix(TheAncient __instance)
    {
        RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
        return false; // 不调用原方法


    }
}
