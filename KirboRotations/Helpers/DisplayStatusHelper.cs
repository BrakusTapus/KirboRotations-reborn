using FFXIVClientStructs.FFXIV.Client.Game;
using KirboRotations;
namespace KirboRotations.Helpers;

internal static class DisplayStatusHelper
{
    static int _idScope;

    /// <summary>
    /// gets a unique id that can be used with ImGui.PushId() to avoid conflicts with type inspectors
    /// </summary>
    /// <returns></returns>
    public static int GetScopeId() => _idScope++;

    public static void SmallVerticalSpace() => ImGui.Dummy(new Vector2(0, 5));

    public static void MediumVerticalSpace() => ImGui.Dummy(new Vector2(0, 10));

    /// <summary>
    /// adds a DrawList command to draw a border around the group
    /// </summary>
    public static void BeginBorderedGroup()
    {
        ImGui.BeginGroup();
    }

    public static void EndBorderedGroup() => EndBorderedGroup(new Vector2(3, 2), new Vector2(0, 3));

    public static void EndBorderedGroup(Vector2 minPadding, Vector2 maxPadding = default(Vector2))
    {
        ImGui.EndGroup();

        // attempt to size the border around the content to frame it
        var color = ImGui.GetStyle().Colors[(int) ImGuiCol.Border];

        var min = ImGui.GetItemRectMin();
        var max = ImGui.GetItemRectMax();
        max.X = min.X + ImGui.GetContentRegionAvail().X;
        ImGui.GetWindowDrawList().AddRect(min - minPadding, max + maxPadding, ImGui.ColorConvertFloat4ToU32(color));

        // this fits just the content, not the full width
        //ImGui.GetWindowDrawList().AddRect( ImGui.GetItemRectMin() - padding, ImGui.GetItemRectMax() + padding, packedColor );
    }

    public static void DisplayGCDStatus()
    {
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, Dalamud.Interface.Colors.ImGuiColors.ParsedGold);
        BeginBorderedGroup();
        float boxXStart = ImGui.GetCursorPosX();
        ImGui.Text("GCD Total: " + CustomRotationEx.DefaultGCDTotal.ToString("F2") + "s" + "\\" + CustomRotationEx.DefaultGCDRemain.ToString("F2") + "s");
        ImGui.SameLine();
        float textsize = ImGui.CalcTextSize("Animation Lock Delay: " + ActionManagerHelper.GetCurrentAnimationLock().ToString("F2")).X;
        ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMax().X - textsize - ImGui.GetStyle().ItemSpacing.X);
        ImGui.Text("Animation Lock Delay: " + ActionManagerHelper.GetCurrentAnimationLock().ToString("F2"));
        ImGui.Spacing();

        float padding = ImGui.GetStyle().WindowPadding.X;
        float windowSize = ImGui.GetContentRegionAvail().X;
        float progressBarWidth = windowSize - (2 * padding);
        Vector2 progressBarSize = new Vector2(progressBarWidth, 20);

        // NextAbilityToNextGCD

        ImGui.Text("GCD Remain: " + CustomRotationEx.DefaultGCDRemain.ToString("F2"));
        ImGui.SameLine();
        float textsize2 = ImGui.CalcTextSize("NextAbilityToNextGCD: " + CustomRotation.NextAbilityToNextGCD.ToString("F2")).X;
        ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMax().X - textsize2 - ImGui.GetStyle().ItemSpacing.X);
        ImGui.Text("NextAbilityToNextGCD: " + CustomRotation.NextAbilityToNextGCD.ToString("F2"));
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padding);
        ImGui.ProgressBar(CustomRotationEx.DefaultGCDRemain / CustomRotationEx.DefaultGCDTotal, progressBarSize, "");

        // Add some padding between the progress bars
        ImGui.Dummy(new Vector2(0, padding));

        //ImGui.Text("GCD Elapsed: " + DataBased.DefaultGCDElapsed.ToString("F2"));
        //ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padding);
        //ImGui.ProgressBar(DataBased.DefaultGCDElapsed / DataBased.DefaultGCDTotal, progressBarSize, "");

        // End the bordered group
        EndBorderedGroup();
        ImGui.PopStyleColor();
    }

    public static bool BeginPaddedChild(string str_id, bool border = false, ImGuiWindowFlags flags = 0)
    {
        float padding = ImGui.GetStyle().WindowPadding.X;
        // Set cursor position with padding
        float cursorPosX = ImGui.GetCursorPosX() + padding;
        ImGui.SetCursorPosX(cursorPosX);

        // Adjust the size to account for padding
        // Get the available size and adjust it to account for padding
        Vector2 size = ImGui.GetContentRegionAvail();
        size.X -= 2 * padding;
        size.Y -= 2 * padding;

        // Begin the child window
        return ImGui.BeginChild(str_id, size, border, flags);
    }

    public static void EndPaddedChild()
    {
        ImGui.EndChild();
    }

    public static void SetCursorMiddle()
    {
        float cursorPosX = ImGui.GetContentRegionAvail().X / 2;
        ImGui.SetCursorPosX(cursorPosX);
    }

}
