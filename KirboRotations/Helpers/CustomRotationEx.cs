using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace KirboRotations.Helpers;

internal unsafe class CustomRotationEx
{
    internal static Action? action = new();
    internal IBaseAction? BaseAction { get; set; }

    private static readonly TargetSystem* targetSystem = FFXIVClientStructs.FFXIV.Client.Game.Control.TargetSystem.Instance();
    internal TargetSystem* TargetSystem = targetSystem;

    internal GameObject* GetGameObject = targetSystem->GetTargetObject();
    internal ObjectKind ObjectKind = targetSystem->GetTargetObject()->GetObjectKind();

    private static ActionManager* instanceActionManager;
    internal static ActionManager* InstanceActionManager
    {
        get
        {
            if (instanceActionManager == null)
            {
                instanceActionManager = GetActionManagerInstance();
                if (instanceActionManager == null)
                {
                    throw new Exception("Could not create an instanceActionManager of the ActionManager.");
                }
            }

            return instanceActionManager;
        }
    }
    private static unsafe ActionManager* GetActionManagerInstance()
    {
        var pInstance = ActionManager.Instance();

        if (pInstance == null)
        {
            Serilog.Log.Logger.Warning($"ThrowHelper.ThrowNullAddress(\"ActionManager.InstanceActionManager\", \"48 8D 0D ?? ?? ?? ?? F3 0F 10 13\")"); ;
            return null;
        }

        return pInstance;
    }

    internal GameMain* gameMain = GameMain.Instance();

    internal ActionManager.TargetCategory TargetCategory;

    internal ActionManager.Delegates.GetActionInRangeOrLoS? GetActionInRangeOrLoS;

    #region CoolDown
    internal byte CoolDownGroup { get; }

    private unsafe RecastDetail* CoolDownDetail => ActionIdHelper.GetCoolDownDetail(CoolDownGroup);

    private unsafe float RecastTime
    {
        get
        {
            if (CoolDownDetail != null)
            {
                return CoolDownDetail->Total;
            }

            return 0f;
        }
    }

    public float RecastTimeElapsed => RecastTimeElapsedRaw - DefaultGCDRemain;

    internal unsafe float RecastTimeElapsedRaw
    {
        get
        {
            if (CoolDownDetail != null)
            {
                return CoolDownDetail->Elapsed;
            }

            return 0f;
        }
    }

    public bool IsCoolingDown => ActionIdHelper.IsCoolingDown(CoolDownGroup);

    private float RecastTimeRemain => RecastTime - RecastTimeElapsedRaw;

    public bool HasOneCharge
    {
        get
        {
            if (IsCoolingDown)
            {
                return RecastTimeElapsedRaw >= RecastTimeOneChargeRaw;
            }

            return true;
        }
    }

    public float RecastTimeRemainOneCharge => RecastTimeRemainOneChargeRaw - DefaultGCDRemain;

    private float RecastTimeRemainOneChargeRaw => RecastTimeRemain % RecastTimeOneChargeRaw;

    public float RecastTimeElapsedOneCharge => RecastTimeElapsedOneChargeRaw - DefaultGCDElapsed;

    private float RecastTimeElapsedOneChargeRaw => RecastTimeElapsedRaw % RecastTimeOneChargeRaw;

    public unsafe ushort CurrentCharges => (ushort)ActionManager.Instance()->GetCurrentCharges(BaseAction.Info.AdjustedID);

    internal float RecastTimeOneChargeRaw => ActionManager.GetAdjustedRecastTime(ActionType.Action, BaseAction.Info.AdjustedID) / 1000f;



    public static unsafe float GetRecastTime(ActionType type, uint id)
    {
        ActionManager* actionManager = ActionManager.Instance();
        if (actionManager == null)
        {
            return 0;
        }

        return actionManager->GetRecastTime(type, id);
    }

    public static unsafe float GetDefaultRecastTime()
    {
        return GetRecastTime(ActionType.Action, 11);
    }

    public static unsafe float GetRecastTimeElapsed(ActionType type, uint id)
    {
        ActionManager* actionManager = ActionManager.Instance();
        if (actionManager == null)
        {
            return 0;
        }

        return actionManager->GetRecastTimeElapsed(type, id);
    }

    public static unsafe float GetDefaultRecastTimeElapsed()
    {
        return GetRecastTimeElapsed(ActionType.Action, 11);
    }
    #endregion

    #region GCD
    internal static float AnimationLockTime => GetCurrentAnimationLock();

    internal static unsafe float GetCurrentAnimationLock()
    {
        ActionManager* actionManager = ActionManager.Instance();
        if (actionManager == null)
        {
            return 0.6f;
        }

        nint animationLockRaw = (nint)actionManager + 8;
        return *(float*)animationLockRaw;
    }

    public static float DefaultGCDTotal =>
        GetDefaultRecastTime();

    public static float DefaultGCDRemain =>
        GetDefaultRecastTime() - GetDefaultRecastTimeElapsed();

    public static float DefaultGCDElapsed =>
        GetDefaultRecastTimeElapsed();

    public static float CalculatedActionAhead =>
        DefaultGCDTotal * 0.25f;

    public static float GCDTime(uint gcdCount = 0, float offset = 0) =>
        GetDefaultRecastTime() * gcdCount + offset;
    #endregion

    #region Limit break
    [System.ComponentModel.Description("Bar Count")]
    internal unsafe static byte CurrentBarCount
    {
        get
        {
            FFXIVClientStructs.FFXIV.Client.Game.UI.LimitBreakController limitBreakController = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->LimitBreakController;
            byte barCount = *&limitBreakController.BarCount;

            return barCount;
        }
    }

    [System.ComponentModel.Description("Limit Break Level")]
    internal unsafe static byte CurrentLimitBreakLevel
    {
        get
        {
            FFXIVClientStructs.FFXIV.Client.Game.UI.LimitBreakController limitBreakController = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->LimitBreakController;
            ushort currentUnits = *&limitBreakController.CurrentUnits;

            if (currentUnits >= 9000)
            {
                return 3;
            }
            else if (currentUnits >= 6000)
            {
                return 2;
            }
            else if (currentUnits >= 3000)
            {
                return 1;
            }
            else
            {
                return 0; // Assuming 0 is the default or undefined state.
            }
        }
    }

    [System.ComponentModel.Description("Current Units")]
    internal unsafe static ushort CurrentCurrentUnits
    {
        get
        {
            FFXIVClientStructs.FFXIV.Client.Game.UI.LimitBreakController limitBreakController = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->LimitBreakController;
            ushort currentUnits = *&limitBreakController.CurrentUnits;

            return currentUnits;
        }
    }

    [System.ComponentModel.Description("Is PvP")]
    internal unsafe static bool IsCurrentPvP
    {
        get
        {
            FFXIVClientStructs.FFXIV.Client.Game.UI.LimitBreakController limitBreakController = FFXIVClientStructs.FFXIV.Client.Game.UI.UIState.Instance()->LimitBreakController;
            bool isPvP = *&limitBreakController.IsPvP;

            return isPvP;
        }
    }
    #endregion

    #region PvP limit breaks
    /// <summary>
    /// PLD
    /// </summary>
    internal static IBaseAction PhalanxPvP { get; } = new BaseAction((ActionID)29069);

    /// <summary>
    /// WAR
    /// </summary>
    internal static IBaseAction PrimalScreamPvP { get; } = new BaseAction((ActionID)29083);

    /// <summary>
    /// DRK
    /// </summary>
    internal static IBaseAction EvenTidePvP { get; } = new BaseAction((ActionID)29097);

    /// <summary>
    /// GNB
    /// </summary>
    internal static IBaseAction TerminalTriggerPvP { get; } = new BaseAction((ActionID)29469);

    /// <summary>
    /// WHM
    /// </summary>
    internal static IBaseAction AfflatusPurgationPvP { get; } = new BaseAction((ActionID)29230);

    /// <summary>
    /// AST
    /// </summary>
    internal static IBaseAction CelestialRiverPvP { get; } = new BaseAction((ActionID)29255);

    /// <summary>
    /// SCH
    /// </summary>
    internal static IBaseAction SummonSeraphPvP { get; } = new BaseAction((ActionID)29237);

    /// <summary>
    /// SCH
    /// </summary>
    internal static IBaseAction SeraphFlightPvP { get; } = new BaseAction((ActionID)29239);

    /// <summary>
    /// SGE
    /// </summary>
    internal static IBaseAction MesotesPvP { get; } = new BaseAction((ActionID)29266);

    /// <summary>
    /// NIN
    /// </summary>
    internal static IBaseAction SeitonTenchuPvP { get; } = new BaseAction((ActionID)29515);

    /// <summary>
    /// MNK
    /// </summary>
    internal static IBaseAction MeteoDivePvP { get; } = new BaseAction((ActionID)29485);

    /// <summary>
    /// DRG
    /// </summary>
    internal static IBaseAction SkyHighPvP { get; } = new BaseAction((ActionID)29497);

    /// <summary>
    /// DRG
    /// </summary>
    internal static IBaseAction SkyShatterPvP { get; } = new BaseAction((ActionID)29499);

    /// <summary>
    /// SAM
    /// </summary>
    internal static IBaseAction ZantetsukenPvP { get; } = new BaseAction((ActionID)29537);

    /// <summary>
    /// RPR
    /// </summary>
    internal static IBaseAction TenebraelemuruPvP { get; } = new BaseAction((ActionID)29553);

    /// <summary>
    /// BRD
    /// </summary>
    internal static IBaseAction FinalFantasiaPvP { get; } = new BaseAction((ActionID)29401);

    /// <summary>
    /// MCH
    /// </summary>
    internal static IBaseAction MarksmansSpitePvP { get; } = new BaseAction((ActionID)29415);

    /// <summary>
    /// DNC
    /// </summary>
    internal static IBaseAction ContraDancePvP { get; } = new BaseAction((ActionID)29432);

    /// <summary>
    /// BLM
    /// </summary>
    internal static IBaseAction SoulResonancePvP { get; } = new BaseAction((ActionID)29662);


    /// <summary>
    /// SMN
    /// </summary>
    internal static IBaseAction SummonBahamutPvP { get; } = new BaseAction((ActionID)29673);

    /// <summary>
    /// SMN
    /// </summary>
    internal static IBaseAction SummonPhoenixPvP { get; } = new BaseAction((ActionID)29678);

    /// <summary>
    /// SMN
    /// </summary>
    internal static IBaseAction MegaflarePvP { get; } = new BaseAction((ActionID)29675);

    /// <summary>
    /// SMN
    /// </summary>
    internal static IBaseAction EverlastingPvP { get; } = new BaseAction((ActionID)29680);

    /// <summary>
    /// RDM
    /// </summary>
    internal static IBaseAction SouthernCrossPvP { get; } = new BaseAction((ActionID)29704);
    #endregion

    internal unsafe bool Use(ActionID actionID)
    {
        GameObject* targetObject = targetSystem->GetTargetObject();
        GameObjectId targetObjectId = targetSystem->GetTargetObjectId();
        Vector3 tarObjectPosition = targetObject->Position;

        float locX = tarObjectPosition.X;
        float locY = tarObjectPosition.Y;
        float locZ = tarObjectPosition.Z;
        Vector3 loc = new FFXIVClientStructs.FFXIV.Common.Math.Vector3(locX, locY, locZ);
        uint adjustedID = InstanceActionManager->GetAdjustedActionId((uint)actionID);

        if (adjustedID == 0)
        {
            return false;
        }

        if (action == null)
        {
            return false;
        }

        if (action.TargetArea)
        {
            return ActionManager.Instance()->UseActionLocation(ActionType.Action, (uint)actionID, targetObjectId, &loc);
        }
        else if (targetObject == null)
        {
            return false;
        }
        else
        {
            return ActionManager.Instance()->UseAction(ActionType.Action, (uint)actionID, targetObjectId);
        }
    }

}