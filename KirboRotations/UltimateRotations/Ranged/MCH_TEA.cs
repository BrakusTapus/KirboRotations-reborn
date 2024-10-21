#pragma warning disable S1066 // Mergeable "if" statements should be combined

using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using KirboRotations.Helpers;

namespace KirboRotations.UltimateRotations.Ranged;

[BetaRotation]
[Rotation("MCH TEA",
    CombatType.PvE,
    GameVersion = $"v.\notation： v...1\n\n",
    Description = $"")]
[SourceCode(Path = "")]
[Api(4)]
public sealed class MCH_TEA : MachinistRotation
{
    #region Config Options
    //[RotationConfig(CombatType.PvE, Name = "Skip Queen Logic and uses Rook Autoturret/Automaton Queen immediately whenever you get 50 battery")]
    //public bool SkipQueenLogic { get; set; } = true;

    private byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }

    private bool OpenerHasFinishedDummy { get; set; } = false;
    private bool OpenerHasFinished { get; set; } = false;
    private int OpenerStep { get; set; } = 0;
    const float universalFailsafeThreshold = 5.0f;
    private bool OpenerAvailable { get; set; } = false;
    public bool OpenerInProgress { get; private set; }
    private bool StartOpener { get; set; } = false;

    private bool InBurst => Player.HasStatus(true, StatusID.Wildfire_1946);
    private bool IsSecond0GCD => WeaponRemain >= 0.59f && WeaponRemain <= 0.80f && CustomRotationEx.GetCurrentAnimationLock() == 0;
    #endregion

    #region Countdown logic
    // Defines logic for actions to take during the countdown before combat starts.
    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < 4.85f)
        {
            if (ReassemblePvE.CanUse(out var act) && !Player.HasStatus(true, StatusID.Reassembled))
            {
                return act;
            }
        }
        if (remainTime < 0.4f)
        {
            if (AirAnchorPvE.CanUse(out var act))
            {
                StartOpener = true;
                return act;
            }
        }
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region oGCD Logic
    // Determines emergency actions to take based on the next planned GCD action.
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (OpenerInProgress)
        {
            return Opener(out act);
        }

        act = null;
        bool isJagdDollAndLowHP = Target.Name.ToString() == "Jagd Doll" && Target.GetHealthRatio() < 0.25;
        if (isJagdDollAndLowHP)
        {
            return false;
        }

        // Reassemble Logic
        // Check next GCD action and conditions for Reassemble.
        bool isReassembleUsable = nextGCD.IsTheSameTo(false, DrillPvE) || nextGCD.IsTheSameTo(ActionID.DrillPvE) || nextGCD.IsTheSameTo(ActionID.AirAnchorPvE);

        // Use Barrel Stabilizer on CD if won't cap
        if (BarrelStabilizerPvE.CanUse(out act) && Target != Player && Target.GetHealthRatio() >= 0.25f)
        {
            return true;
        }

        bool isJagdDoll = Target.Name.ToString() == "Jagd Doll";
        if (Battery >= 50 && RookAutoturretPvE.CanUse(out act) && !isJagdDoll)
        {
            return true;
        }


        // Attempt to use Reassemble if it's ready
        if (isReassembleUsable)
        {
            if (ReassemblePvE.CanUse(out act, skipComboCheck: true, usedUp: true)) return true;
        }

        // Keeps Ricochet and Gauss cannon Even
        bool isRicochetMore = RicochetPvE.EnoughLevel && GaussRoundPvE.Cooldown.CurrentCharges <= RicochetPvE.Cooldown.CurrentCharges;
        bool isGaussMore = !RicochetPvE.EnoughLevel || GaussRoundPvE.Cooldown.CurrentCharges > RicochetPvE.Cooldown.CurrentCharges;

        // Use Ricochet
        if (isRicochetMore && (!IsLastAction(true, GaussRoundPvE, RicochetPvE) && IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE) || !IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE)))
        {
            if (RicochetPvE.CanUse(out act, skipAoeCheck: true, usedUp: true))
                return true;
        }

        // Use Gauss
        if (isGaussMore && (!IsLastAction(true, GaussRoundPvE, RicochetPvE) && IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE) || !IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE)))
        {
            if (GaussRoundPvE.CanUse(out act, usedUp: true))
                return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    // Logic for using attack abilities outside of GCD, focusing on burst windows and cooldown management.
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (OpenerInProgress)
        {
            return Opener(out act);
        }

        act = null;
        bool isJagdDollAndLowHP = Target.Name.ToString() == "Jagd Doll" && Target.GetHealthRatio() < 0.25;
        bool isJagdDoll = Target.Name.ToString() == "Jagd Doll";

        if (isJagdDoll || isJagdDollAndLowHP)
        {
            return false;
        }

        // If Wildfire is active, use Hypercharge.....Period
        if (Player.HasStatus(true, StatusID.Wildfire_1946))
        {
            return HyperchargePvE.CanUse(out act);
        }

        // Burst setting in RSR
        if (IsBurst)
        {
            if ((IsLastAbility(false, HyperchargePvE) || Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged))
                && CanUseHyperchargePvE(out _) && WildfirePvE.CanUse(out act))
            {
                return true;
            }

        }
        // Use Hypercharge if at least 12 seconds of combat and (if wildfire will not be up in 30 seconds or if you hit 100 heat)
        if (!Player.HasStatus(true, StatusID.Reassembled) && (!WildfirePvE.Cooldown.WillHaveOneCharge(30) || Heat == 100))
        {
            if (CanUseHyperchargePvE(out act))
            {
                return true;
            }
        }


        if (nextGCD.IsTheSameTo(true, CleanShotPvE, HotShotPvE) && Battery == 100 && Target.GetHealthRatio() >= 0.25)
        {
            if (RookAutoturretPvE.CanUse(out act))
            {
                return true;
            }
        }

        // Keeps Ricochet and Gauss cannon Even
        bool isRicochetMore = RicochetPvE.EnoughLevel && GaussRoundPvE.Cooldown.CurrentCharges <= RicochetPvE.Cooldown.CurrentCharges;
        bool isGaussMore = !RicochetPvE.EnoughLevel || GaussRoundPvE.Cooldown.CurrentCharges > RicochetPvE.Cooldown.CurrentCharges;

        // Use Ricochet
        if (isRicochetMore && (!IsLastAction(true, GaussRoundPvE, RicochetPvE) && IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE) || !IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE)))
        {
            if (RicochetPvE.CanUse(out act, skipAoeCheck: true, usedUp: true))
                return true;
        }

        // Use Gauss
        if (isGaussMore && (!IsLastAction(true, GaussRoundPvE, RicochetPvE) && IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE) || !IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE)))
        {
            if (GaussRoundPvE.CanUse(out act, usedUp: true))
                return true;
        }
        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    // Defines the general logic for determining which global cooldown (GCD) action to take.
    protected override bool GeneralGCD(out IAction? act)
    {
        if (OpenerInProgress)
        {
            return Opener(out act);
        }
        act = null;
        bool isJagdDollAndLowHP = Target.Name.ToString() == "Jagd Doll" && Target.GetHealthRatio() < 0.25;
        if (isJagdDollAndLowHP)
        {
            return false;
        }

        if (HeatBlastPvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }

        // Check if AirAnchor can be used
        if (AirAnchorPvE.CanUse(out act))
        {
            return true;
        }

        // Check if Drill can be used
        bool isJagdDoll = Target.Name.ToString() == "Jagd Doll";
        if (DrillPvE.CanUse(out act, usedUp: true) && !isJagdDoll)
        {
            return true;
        }

        if (SpreadShotPvE.CanUse(out act))
        {
            return true;
        }

        // Single target actions: CleanShot, SlugShot, and SplitShot based on their usability.
        if (CleanShotPvE.CanUse(out act))
        {
            return true;
        }

        if (SlugShotPvE.CanUse(out act))
        {
            return true;
        }

        if (SplitShotPvE.CanUse(out act))
        {
            return true;
        }

        return base.GeneralGCD(out act);
    }
    #endregion

    #region Extra Methods
    protected override void UpdateInfo()
    {
        OpenerReady();
        OpenerStarter();
    }

    private void OpenerStarter()
    {
        if (OpenerHasFinished)
        {
            StartOpener = false;
            OpenerHasFinished = false;
        }
        if (StartOpener)
        {
            OpenerInProgress = true;
        }
        else
        {
            OpenerInProgress = false;
        }
    }

    private bool OpenerReady()
    {
        var Lvl80 = Player.Level == 80;
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
        bool Openerstep0 = OpenerStep == 0;

        OpenerAvailable = Lvl80
                                    && HasAirAnchor
                                    && HasDrill
                                    && DrillCharges == 1
                                    && HasBarrelStabilizer
                                    && RCcharges == 2
                                    && GRcharges == 2
                                    && HasWildfire
                                    && ReassembleOneCharge
                                    && NoResources
                                    && Openerstep0;
        return false;
    }

    private bool OpenerController(bool lastAction, bool nextAction)
    {
        if (lastAction)
        {
            OpenerStep++;
            return false;
        }
        return nextAction;
    }

    private bool Opener(out IAction? act)
    {
        // Universal failsafe for opener inactivity
        if (TimeSinceLastAction.TotalSeconds > universalFailsafeThreshold && OpenerStep > 0)
        {
            act = null;
            OpenerHasFinished = true;  // Stop the opener
            return false;  // Stop further action
        }

        switch (OpenerStep)
        {
            case 0:
                return OpenerController(IsLastGCD(true, AirAnchorPvE), AirAnchorPvE.CanUse(out act));

            case 1:
                return OpenerController(IsLastAbility(false, BarrelStabilizerPvE), BarrelStabilizerPvE.CanUse(out act, usedUp: true));

            case 2:
                return OpenerController(IsLastAbility(false, WildfirePvE), WildfirePvE.CanUse(out act));

            case 3:
                return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

            case 4:
                return OpenerController(IsLastAbility(false, HyperchargePvE), HyperchargePvE.CanUse(out act, usedUp: true));

            case 5:
                OpenerHasFinished = true;
                OpenerStep = 0;
                break;
        }
        act = null;
        return OpenerHasFinishedDummy = false;
    }

    // Logic for Hypercharge
    private bool CanUseHyperchargePvE(out IAction? act)
    {
        if (IsLastGCD(ActionID.FullMetalFieldPvE) && IsLastAbility(ActionID.WildfirePvE) && (Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged)))
        {
            return HyperchargePvE.CanUse(out act);
        }

        float REST_TIME = 6f;
        if
                     //Cannot AOE
                     (!SpreadShotPvE.CanUse(out _)
                     &&
                     // AirAnchor Enough Level % AirAnchor 
                     (AirAnchorPvE.EnoughLevel && AirAnchorPvE.Cooldown.WillHaveOneCharge(REST_TIME)
                     ||
                     // HotShot Charge Detection
                     !AirAnchorPvE.EnoughLevel && HotShotPvE.EnoughLevel && HotShotPvE.Cooldown.WillHaveOneCharge(REST_TIME)
                     ||
                     // Drill Charge Detection
                     DrillPvE.EnoughLevel && DrillPvE.Cooldown.WillHaveOneCharge(REST_TIME)))
        {
            act = null;
            return false;
        }
        else
        {
            // Use Hypercharge
            return HyperchargePvE.CanUse(out act);
        }
    }
    #endregion

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
        ImGui.Text("OpenerAvailable: " + OpenerAvailable.ToString());
        ImGui.Text("StartOpener: " + StartOpener.ToString());
        ImGui.Text("OpenerInProgress: " + OpenerInProgress.ToString());
        ImGui.Text("OpenerHasFinished: " + OpenerHasFinished.ToString());
        ImGui.Text("OpenerStep: " + OpenerStep.ToString());
    }
}