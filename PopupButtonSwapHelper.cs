using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace St2ChangeYesAndNo;

internal static class PopupButtonSwapHelper
{
    private static readonly MethodInfo? YesButtonSetter = AccessTools.PropertySetter(typeof(NVerticalPopup), nameof(NVerticalPopup.YesButton));
    private static readonly MethodInfo? NoButtonSetter = AccessTools.PropertySetter(typeof(NVerticalPopup), nameof(NVerticalPopup.NoButton));

    private static readonly FieldInfo? HsvField = AccessTools.Field(typeof(NPopupYesNoButton), "_hsv");
    private static readonly FieldInfo? BaseSField = AccessTools.Field(typeof(NPopupYesNoButton), "_baseS");
    private static readonly FieldInfo? BaseVField = AccessTools.Field(typeof(NPopupYesNoButton), "_baseV");

    private static readonly ButtonVisualStyle LeftYesStyle = new(
        0.25f,
        1.2f,
        1.1f,
        new Color(0.121333f, 0.28f, 0f, 1f));

    private static readonly ButtonVisualStyle RightNoStyle = new(
        0.45f,
        2.0f,
        1.3f,
        new Color(0.35f, 0.07f, 0f, 1f));

    internal static void EnsureYesButtonOnLeft(object popup, LocString yesText, LocString noText)
    {
        if (popup is not Node node)
        {
            return;
        }

        NVerticalPopup? verticalPopup = node.GetNodeOrNull<NVerticalPopup>("VerticalPopup");
        if (verticalPopup?.YesButton == null || verticalPopup.NoButton == null)
        {
            return;
        }

        if (!TryGetPhysicalButtons(verticalPopup, out NPopupYesNoButton leftButton, out NPopupYesNoButton rightButton))
        {
            return;
        }

        verticalPopup.DisconnectSignals();
        verticalPopup.DisconnectHotkeys();

        YesButtonSetter?.Invoke(verticalPopup, new object[] { leftButton });
        NoButtonSetter?.Invoke(verticalPopup, new object[] { rightButton });

        verticalPopup.InitYesButton(yesText, button => InvokePopupHandler(popup, "OnYesButtonPressed", button));
        verticalPopup.InitNoButton(noText, button => InvokePopupHandler(popup, "OnNoButtonPressed", button));

        ApplyStyle(leftButton, LeftYesStyle);
        ApplyStyle(rightButton, RightNoStyle);
    }

    private static bool TryGetPhysicalButtons(NVerticalPopup verticalPopup, out NPopupYesNoButton leftButton, out NPopupYesNoButton rightButton)
    {
        NPopupYesNoButton? firstButton = verticalPopup.GetNodeOrNull<NPopupYesNoButton>("YesButton");
        NPopupYesNoButton? secondButton = verticalPopup.GetNodeOrNull<NPopupYesNoButton>("NoButton");
        if (firstButton == null || secondButton == null)
        {
            leftButton = null!;
            rightButton = null!;
            return false;
        }

        if (firstButton.Position.X <= secondButton.Position.X)
        {
            leftButton = firstButton;
            rightButton = secondButton;
        }
        else
        {
            leftButton = secondButton;
            rightButton = firstButton;
        }

        return true;
    }

    private static void ApplyStyle(NPopupYesNoButton button, ButtonVisualStyle style)
    {
        TextureRect? image = button.GetNodeOrNull<TextureRect>("Visuals/Image");
        Control? label = button.GetNodeOrNull<Control>("Visuals/Label");
        if (image?.Material is not ShaderMaterial currentMaterial || label == null)
        {
            return;
        }

        ShaderMaterial runtimeMaterial = (ShaderMaterial)currentMaterial.Duplicate();
        image.Material = runtimeMaterial;
        runtimeMaterial.SetShaderParameter("h", style.Hue);
        runtimeMaterial.SetShaderParameter("s", style.Saturation);
        runtimeMaterial.SetShaderParameter("v", style.Value);

        HsvField?.SetValue(button, runtimeMaterial);
        BaseSField?.SetValue(button, style.Saturation);
        BaseVField?.SetValue(button, style.Value);

        label.AddThemeColorOverride("font_outline_color", style.FontOutlineColor);
    }

    private static void InvokePopupHandler(object popup, string methodName, NButton button)
    {
        MethodInfo? method = AccessTools.Method(popup.GetType(), methodName);
        method?.Invoke(popup, new object[] { button });
    }

    internal static bool IsMainMenuQuitPopup(LocString header)
    {
        return header.LocTable == "main_menu_ui" && header.LocEntryKey == "QUIT_CONFIRM_POPUP.header";
    }

    private readonly record struct ButtonVisualStyle(float Hue, float Saturation, float Value, Color FontOutlineColor);
}
