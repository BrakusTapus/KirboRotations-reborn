#pragma warning disable S1066 // Mergeable "if" statements should be combined

using System.Globalization;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Xperimental.Common;
using Action = Lumina.Excel.GeneratedSheets.Action;
namespace Xperimental.Ranged;

[BetaRotation]
[Api(3)]
[Rotation($"PvE TESTING ONLY", CombatType.PvE, GameVersion = "0x0x0x2", Description = "DO NOT USE LIKE REGULAR")]
public sealed class MchTestPvE : MachinistRotation
{
    public unsafe override void DisplayStatus()
    {
        float paddingX = ImGui.GetStyle().WindowPadding.X;
        DisplayStatusHelper.BeginPaddedChild("The CustomRotation's status window", true, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

        ImGui.Text("Rotation: " + Name + " ");
        ImGui.SameLine();
        ImGui.TextDisabled(Description);

        DisplayStatusHelper.DisplayGCDStatus();

        //var gameobjectID = DataBase.DisplayPlayerGameObjectId();

        if (ImGui.Button(nameof(ActionID.PelotonPvE)))
        {
            ActionManagerHelper.Instance.InstanceActionManager->UseAction(ActionType.Action, (uint)ActionID.PelotonPvE);
        }

        DisplayStatusHelper.EndPaddedChild();
    }

    #region Config Options
    //[RotationConfig(CombatType.PvE, Name = "(Warning: Queen logic is new and untested, uncheck to test new logic) Skip Queen Logic and uses Rook Autoturret/Automaton Queen immediately whenever you get 50 battery")]
    //private bool SkipQueenLogic { get; set; } = true;
    #endregion

    #region Countdown Logic
    // Defines logic for actions to take during the countdown before combat starts.
    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < 5)
        {
            if (ReassemblePvE.CanUse(out var act)) return act;
        }
        if (remainTime < 2)
        {
            if (UseBurstMedicine(out var act)) return act;
        }
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region oGCD Logic 
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (BarrelStabilizerPvE.CanUse(out act, false, false, false, false, false))
        {
            return true;
        }
        if (WildfirePvE.CanUse(out act, true, false, false, false, true))
        {
            return true;
        }
        if (IsLastAbility(ActionID.WildfirePvE))
        {
            if (HyperchargePvE.CanUse(out act, false, false, false, false, false))
            {
                return true;
            }
        }
        if (GaussRoundPvE.CanUse(out act, false, false, false, false, true))
        {
            return true;
        }
        if (RicochetPvE.CanUse(out act, true, false, false, false, true))
        {
            return true;
        }

        //if (ShouldUseGaussroundOrRicochet(out act)) { return true; }

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        //// Check if Hypercharge should be used based on tool availability
        //if (ToolChargeSoon(out act))
        //{
        //    return true; // Hypercharge can be used
        //}

        return base.AttackAbility(nextGCD, out act);
    }

    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        if (HeatBlastPvE.CanUse(out act))
            return true;

        if (PowerfulPewPews(out act))
            return true;

        if (BasicPewPews(out act))
            return true;

        return base.GeneralGCD(out act);
    }
    #endregion

    #region Extra Methods
    public int Openerstep { get; private set; }
    public bool OpenerAvailable { get; private set; }
    public bool WillhaveToolSoon { get; private set; }

    // Could be usefull, just dont know how yet
    // Black Mage GCD Logic methods might be interesting

    private bool ShouldUseGaussroundOrRicochet(out IAction? act)
    {
        act = null;

        if (!GaussRoundPvE.Cooldown.HasOneCharge && RicochetPvE.Cooldown.HasOneCharge)
        {
            return false;
        }

        if (!GaussRoundPvE.Cooldown.HasOneCharge && !RicochetPvE.Info.EnoughLevel)
        {
            return false;
        }

        if (!RicochetPvE.EnoughLevel)
        {
            return GaussRoundPvE.CanUse(out act, usedUp: true);
        }

        if (GaussRoundPvE.Cooldown.CurrentCharges == RicochetPvE.Cooldown.CurrentCharges)
        {
            if (GaussRoundPvE.Cooldown.RecastTimeRemainOneCharge <= RicochetPvE.Cooldown.RecastTimeRemainOneCharge)
            {
                return GaussRoundPvE.CanUse(out act, usedUp: true);
            }
            else if (GaussRoundPvE.Cooldown.RecastTimeRemainOneCharge >= RicochetPvE.Cooldown.RecastTimeRemainOneCharge)
            {
                return RicochetPvE.CanUse(out act, usedUp: true);
            }
        }

        if (GaussRoundPvE.Cooldown.CurrentCharges >= RicochetPvE.Cooldown.CurrentCharges)
        {
            return GaussRoundPvE.CanUse(out act, usedUp: true);
        }

        if (RicochetPvE.Cooldown.CurrentCharges >= GaussRoundPvE.Cooldown.CurrentCharges)
        {
            return RicochetPvE.Cooldown.HasOneCharge &&
                   RicochetPvE.CanUse(out act, usedUp: true);
        }

        return GaussRoundPvE.CanUse(out act, usedUp: true);
    }

    private bool PowerfulPewPews(out IAction? act)
    {
        act = null;

        if (FullMetalFieldPvE.EnoughLevel && FullMetalFieldPvE.CanUse(out act))
        {
            return true; // L100
        }

        if (ExcavatorPvE.EnoughLevel && ExcavatorPvE.CanUse(out act))
        {
            return true; // L96
        }

        if (ChainSawPvE.EnoughLevel && ChainSawPvE.CanUse(out act))
        {
            return true; // L90
        }

        if (AirAnchorPvE.EnoughLevel && AirAnchorPvE.CanUse(out act))
        {
            return true; // L76
        }

        if (DrillPvE.EnoughLevel && DrillPvE.CanUse(out act))
        {
            return true; // L58
        }

        if (!AirAnchorPvE.EnoughLevel && HotShotPvE.EnoughLevel && HotShotPvE.CanUse(out act))
        {
            return true; // L4
        }

        return false;
    }

    private bool BasicPewPews(out IAction? act)
    {
        act = null;

        if (HeatedCleanShotPvE.EnoughLevel && HeatedCleanShotPvE.CanUse(out act))
        {
            return true; // L64
        }

        if (HeatedSlugShotPvE.EnoughLevel && HeatedSlugShotPvE.CanUse(out act))
        {
            return true; // L60
        }

        if (HeatedSplitShotPvE.EnoughLevel && HeatedSplitShotPvE.CanUse(out act))
        {
            return true; // L64
        }

        if (CleanShotPvE.EnoughLevel && CleanShotPvE.CanUse(out act))
        {
            return true; // L26
        }

        if (SlugShotPvE.EnoughLevel && SlugShotPvE.CanUse(out act))
        {
            return true; // L2
        }

        if (SplitShotPvE.CanUse(out act))
        {
            return true; // L1
        }

        return false;
    }

    private bool ToolKitCheck()
    {
        bool WillFullMetalField = Player.HasStatus(true, StatusID.FullMetalMachinist) || BarrelStabilizerPvE.Cooldown.JustUsedAfter(1);
        bool WillExcavator = !ChainSawPvE.Cooldown.JustUsedAfter(1) || Player.HasStatus(true, StatusID.ExcavatorReady);
        bool WillHaveChainSaw = !ChainSawPvE.Cooldown.IsCoolingDown || ChainSawPvE.Cooldown.RecastTimeRemainOneCharge + ActionCoolDown.DefaultGCDRemain < 7.5f;
        bool WillHaveAirAnchor = !AirAnchorPvE.Cooldown.IsCoolingDown || AirAnchorPvE.Cooldown.RecastTimeRemainOneCharge  + ActionCoolDown.DefaultGCDRemain < 7.5f;
        bool WillHaveDrill = !DrillPvE.Cooldown.IsCoolingDown || DrillPvE.Cooldown.RecastTimeRemainOneCharge + ActionCoolDown.DefaultGCDRemain < 7.5f;
        bool WillHaveHotShot = !HotShotPvE.Cooldown.IsCoolingDown || HotShotPvE.Cooldown.RecastTimeRemainOneCharge + ActionCoolDown.DefaultGCDRemain < 7.5f;

        if (FullMetalFieldPvE.EnoughLevel || Player.Level >= 100)
        {
            WillhaveToolSoon = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw || WillExcavator || WillFullMetalField;
        }
        else if (ExcavatorPvE.EnoughLevel || Player.Level >= 96 && Player.Level < 100)
        {
            WillhaveToolSoon = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw || WillExcavator;
        }
        else if (ChainSawPvE.EnoughLevel || Player.Level >= 90 && Player.Level < 96)
        {
            WillhaveToolSoon = WillHaveDrill || WillHaveAirAnchor || WillHaveChainSaw;
        }
        else if (AirAnchorPvE.EnoughLevel || Player.Level >= 76 && Player.Level < 90)
        {
            WillhaveToolSoon = WillHaveDrill || WillHaveAirAnchor;
        }
        else if (DrillPvE.EnoughLevel || Player.Level >= 58 && Player.Level < 76)
        {
            WillhaveToolSoon = WillHaveDrill || WillHaveHotShot;
        }
        else if (HotShotPvE.EnoughLevel && Player.Level < 58)
        {
            WillhaveToolSoon = WillHaveHotShot;
        }

        return WillhaveToolSoon;
    }

    private bool ToolChargeSoon(out IAction? act)
    {
        act = null;

        // Use Hypercharge if ToolKitCheck() indicates no tools will be available soon
        if (ToolKitCheck())
        {
            // If tools are available soon, return false
            return false;
        }

        // If tools are not available soon, proceed to check if Hypercharge can be used
        return HyperchargePvE.CanUse(out act);
    }


    private bool OpenerReady() // resources needed for LvL 100 opener
    {
        var Lvl100 = Player.Level == 100;
        bool HasChainSaw = !ChainSawPvE.Cooldown.IsCoolingDown;
        bool HasAirAnchor = !AirAnchorPvE.Cooldown.IsCoolingDown;
        var DrillCharges = DrillPvE.Cooldown.CurrentCharges;
        bool HasDrill = !DrillPvE.Cooldown.IsCoolingDown;
        bool HasBarrelStabilizer = !BarrelStabilizerPvE.Cooldown.IsCoolingDown;
        var RCcharges = RicochetPvE.Cooldown.CurrentCharges;
        bool HasWildfire = !WildfirePvE.Cooldown.IsCoolingDown;
        var GRcharges = GaussRoundPvE.Cooldown.CurrentCharges;
        bool ReassembleOneCharge = ReassemblePvE.Cooldown.CurrentCharges >= 1;
        bool NoHeat = Heat == 0;
        bool NoBattery = Battery == 0;
        bool NoResources = NoHeat && NoBattery;
        bool Openerstep0 = Openerstep == 0;

        OpenerAvailable = Lvl100 && HasChainSaw && HasAirAnchor && HasDrill && DrillCharges == 2 && HasBarrelStabilizer &&
                        RCcharges == 3 && GRcharges == 3 && HasWildfire && ReassembleOneCharge && NoResources && Openerstep0;
        return false;
    }
    #endregion

    static Action? Action = new();
    static TargetType? TargetType = null;
    public unsafe bool Use(ActionID actionID)
    {
        ActionManager* actionManager = ActionManager.Instance();
        uint adjustedID = actionManager->GetAdjustedActionId((uint)actionID);

        var loc = new FFXIVClientStructs.FFXIV.Common.Math.Vector3() { X = Target.Position.X, Y = Target.Position.Y, Z = Target.Position.Z };

        if (Action.TargetArea)
        {
            Serilog.Log.Logger.Debug($"Using action: {actionID.ToString()}");
            return ActionManager.Instance()->UseActionLocation(ActionType.Action, (uint)actionID, Player.TargetObjectId, (Vector3*)&loc);
        }
        else if (Target == null)
        {
            return false;
        }
        else
        {
            Serilog.Log.Logger.Debug("Using action: " + actionID.ToString());
            return ActionManager.Instance()->UseAction(ActionType.Action, (uint)actionID, Player.TargetObjectId);
        }
    }
}
