﻿using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace Xperimental.Common;
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
        return GetRecastTime(ActionType.Action, 11u);
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
    //========================================================================================================

    internal static ActionManagerHelper Instance { get; } = new ActionManagerHelper();
    internal unsafe ActionManager* InstanceActionManager { get; } = GetActionManagerInstance();

    private unsafe static ActionManager* GetActionManagerInstance()
    {
        return ActionManager.Instance();
    }


    //private static float GCD_DURATION = GetDefaultRecastTime();
    //private static float ANIMATION_LOCK_DURATION = GetCurrentAnimationLock();
    //private const float BUFFER_TIME = 0.25f;

    ///// <summary>
    ///// Determines if there is enough time left in the current GCD window to use an ability.
    ///// </summary>
    ///// <returns>True if an ability can be used in the first slot, false otherwise.</returns>
    //public static bool CanUseFirstAbility()
    //{
    //    float timeRemaining = DataBase.DefaultGCDRemain;
    //    return timeRemaining >= (BUFFER_TIME + ANIMATION_LOCK_DURATION);
    //}

    ///// <summary>
    ///// Determines if there is enough time left in the current GCD window to use a second ability.
    ///// </summary>
    ///// <returns>True if an ability can be used in the second slot, false otherwise.</returns>
    //public static bool CanUseSecondAbility()
    //{
    //    float timeRemaining = DataBase.DefaultGCDRemain;
    //    return timeRemaining >= (BUFFER_TIME + 2 * ANIMATION_LOCK_DURATION + BUFFER_TIME);
    //}

    //public unsafe static float UseAction(ActionType type, uint id)
    //{
    //    ActionManager* ptr = ActionManager.Instance();
    //    if (ptr == null)
    //    {
    //        return 0f;
    //    }

    //    return ptr->UseAction(type, id);
    //}

    //public static float GetLastActionDefaultRecastTime()
    //{
    //    //id = lasta;
    //    return GetRecastTime(ActionType.Action, 11u); // Surely we can get a dynamic value for this somewhere
    //}




    //static Lumina.Excel.GeneratedSheets.Action? Action = new();
    //static TargetType? TargetType = null;

    //public static unsafe bool UseAction(ActionID actionID)
    //{
    //    return ActionManager.Instance()->UseAction(ActionType.Action, (uint)actionID);
    //}

    //public static unsafe bool Use(ActionID actionID)
    //{
    //    ActionManager* actionManager = ActionManager.Instance();
    //    uint adjustedID = actionManager->GetAdjustedActionId((uint)actionID);

    //    var loc = new FFXIVClientStructs.FFXIV.Common.Math.Vector3() { X = Target.Position.X, Y = Target.Position.Y, Z = Target.Position.Z };

    //    if (Action.TargetArea)
    //    {
    //        Serilog.Log.Logger.Debug($"Using action: {actionID.ToString()}");
    //        return ActionManager.Instance()->UseActionLocation(ActionType.Action, (uint)actionID, Player.TargetObjectId, (Vector3*)&loc);
    //    }
    //    else if (Target == null)
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        Serilog.Log.Logger.Debug("Using action: " + actionID.ToString());
    //        return ActionManager.Instance()->UseAction(ActionType.Action, (uint)actionID, Player.TargetObjectId);
    //    }
    //}

    /*

    if (!IsRealGCD)
    {
        if (option.HasFlag(CanUseOption.OnLastAbility))
        {
            if (DataCenter.NextAbilityToNextGCD > AnimationLockTime + DataCenter.Ping + DataCenter.MinAnimationLock) return false;
        }
        else if (!option.HasFlag(CanUseOption.IgnoreClippingCheck))
        {
            if (DataCenter.NextAbilityToNextGCD < AnimationLockTime) return false;
        }
    }

    */
}