using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace St2ChangeYesAndNo;

[ModInitializer(nameof(Initialize))]
public partial class ModEntry : Node
{
    private const string ModId = "St2ChangeYesAndNo";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        Logger.Info("Initialized: force confirm button to the left for abandon/disconnect popups.");
    }
}
