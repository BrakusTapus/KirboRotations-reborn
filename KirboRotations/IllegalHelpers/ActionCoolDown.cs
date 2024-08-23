
using Action = Lumina.Excel.GeneratedSheets.Action;

using FFXIVClientStructs.FFXIV.Client.Game;

namespace KirboRotations.Common;

internal unsafe class ActionCoolDown
{
    private readonly IBaseAction _action;

    private unsafe static ActionManager* instanceActionManager;


    internal ActionManager.TargetCategory* targetCategory;

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

    public static unsafe float TimeTillNextGCD => DefaultGCDRemain;
    public static float DefaultGCDRemain => GetDefaultRecastTime() - GetDefaultRecastTimeElapsed();

    public unsafe ushort CurrentCharges => (ushort)ActionManager.Instance()->GetCurrentCharges(_action.Info.AdjustedID);

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
        return GetRecastTime(ActionType.Action, 5);

    }
    public static unsafe float GetDefaultRecastTimeElapsed()
    {
        return GetRecastTimeElapsed(ActionType.Action, 11);
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

    public float RecastTimeElapsed => RecastTimeElapsedRaw; // - DefaultGCDRemain;
    internal byte CoolDownGroup { get; }
    private unsafe RecastDetail* CoolDownDetail => ActionIdHelper.GetCoolDownDetail(CoolDownGroup);

    internal float RecastTimeOneChargeRaw => (float)ActionManager.GetAdjustedRecastTime(ActionType.Action, _action.Info.AdjustedID) / 1000f;


    private unsafe float RecastTime => CoolDownDetail == null ? 0 : CoolDownDetail->Total;
    private float RecastTimeRemain => RecastTime - RecastTimeElapsedRaw;
    private float RecastTimeRemainOneChargeRaw => RecastTimeRemain % RecastTimeOneChargeRaw;
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
}
