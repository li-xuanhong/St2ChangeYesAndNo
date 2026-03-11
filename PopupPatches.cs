using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;

namespace St2ChangeYesAndNo;

[HarmonyPatch(typeof(NAbandonRunConfirmPopup), nameof(NAbandonRunConfirmPopup._Ready))]
internal static class NAbandonRunConfirmPopupPatch
{
    private static void Postfix(NAbandonRunConfirmPopup __instance)
    {
        PopupButtonSwapHelper.EnsureYesButtonOnLeft(
            __instance,
            new LocString("main_menu_ui", "GENERIC_POPUP.confirm"),
            new LocString("main_menu_ui", "GENERIC_POPUP.cancel"));
    }
}

[HarmonyPatch(typeof(NDisconnectConfirmPopup), nameof(NDisconnectConfirmPopup._Ready))]
internal static class NDisconnectConfirmPopupPatch
{
    private static void Postfix(NDisconnectConfirmPopup __instance)
    {
        PopupButtonSwapHelper.EnsureYesButtonOnLeft(
            __instance,
            new LocString("main_menu_ui", "GENERIC_POPUP.confirm"),
            new LocString("main_menu_ui", "GENERIC_POPUP.cancel"));
    }
}

[HarmonyPatch(typeof(NGenericPopup), nameof(NGenericPopup.WaitForConfirmation))]
internal static class NGenericPopupPatch
{
    private static void Postfix(NGenericPopup __instance, LocString body, LocString header, LocString? noButton, LocString yesButton)
    {
        if (noButton == null)
        {
            return;
        }

        if (!PopupButtonSwapHelper.IsMainMenuQuitPopup(header))
        {
            return;
        }

        PopupButtonSwapHelper.EnsureYesButtonOnLeft(__instance, yesButton, noButton);
    }
}
