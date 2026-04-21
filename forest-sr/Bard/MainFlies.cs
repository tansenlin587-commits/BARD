using BaseLib.Utils;
using Forest_Sr.BardCode.Cards.Other;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.Timeline;

namespace Forest_Sr.Bard;

/**
 * Ideas
 * 
 * Self Bind
 * 
 * Bind effect - square texture based on model size, lines random generated (amount equal to bind amount)
 * shader of transparency of line based on average of point spread of the model
 * colored
 * 
 * Bind... rename? Necrobinder kinda overlaps.
 * */

[ModInitializer(nameof(Initialize))]
public class MainFile
{
	public const string ModId = "Bard"; //At the moment, this is used only for the Logger and harmony names.

	public static Logger Logger { get; } =
		new(ModId, LogType.Generic);

	public static void Initialize()
	{
		Harmony harmony = new(ModId);

		harmony.PatchAll();

        harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        Logger.Info("Harmony patches applied!");

    }
    
}
