using FFXIVClientStructs.FFXIV.Client.Game;

namespace KirboRotations.Helpers;
internal class ActionManagerHelper
{
    /// <summary>
    /// Retrieves the current animation lock duration from the ActionManager.
    /// </summary>
    /// <returns>
    /// The current animation lock duration in seconds, or a default value of 0.6 seconds if the ActionManager instanceActionManager is null.
    /// </returns>
    public unsafe static float GetCurrentAnimationLock()
    {
        ActionManager* ptr = ActionManager.Instance();
        if (ptr == null)
        {
            return 0.6f;
        }

        return *(float*)((byte*)ptr + 8);
    }

    /// <summary>
    /// Retrieves the remaining recast time for a specific action type and ID.
    /// </summary>
    /// <param name="type">The type of action.</param>
    /// <param name="id">The ID of the action.</param>
    /// <returns>
    /// The remaining recast time in seconds, or 0 if the ActionManager instanceActionManager is null.
    /// </returns>
    public unsafe static float GetRecastTime(ActionType type, uint id)
    {
        ActionManager* ptr = ActionManager.Instance();
        if (ptr == null)
        {
            return 0f;
        }

        return ptr->GetRecastTime(type, id);
    }

    /// <summary>
    /// Retrieves the default recast time for the action with ID 11 of type Action.
    /// </summary>
    /// <returns>The default recast time in seconds.</returns>
    public static float GetDefaultRecastTime()
    {
        return GetRecastTime(ActionType.Action, 11u); // Surely we can get a dynamic value for this somewhere
    }

    /// <summary>
    /// Retrieves the elapsed recast time for a specific action type and ID.
    /// </summary>
    /// <param name="type">The type of action.</param>
    /// <param name="id">The ID of the action.</param>
    /// <returns>
    /// The elapsed recast time in seconds, or 0 if the ActionManager instanceActionManager is null.
    /// </returns>
    public unsafe static float GetRecastTimeElapsed(ActionType type, uint id)
    {
        ActionManager* ptr = ActionManager.Instance();
        if (ptr == null)
        {
            return 0f;
        }

        return ptr->GetRecastTimeElapsed(type, id);
    }

    /// <summary>
    /// Retrieves the default elapsed recast time for the action with ID 11 of type Action.
    /// </summary>
    /// <returns>The default elapsed recast time in seconds.</returns>
    public static float GetDefaultRecastTimeElapsed()
    {
        return GetRecastTimeElapsed(ActionType.Action, 11u);
    }

    internal static ActionManagerHelper Instance { get; } = new ActionManagerHelper();
    internal unsafe ActionManager* InstanceActionManager { get; } = GetActionManagerInstance();

    private unsafe static ActionManager* GetActionManagerInstance()
    {
        return ActionManager.Instance();
    }
}