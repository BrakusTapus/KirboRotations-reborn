#pragma warning disable S1066 // Mergeable "if" statements should be combined

using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using KirboRotations.Helpers;

namespace KirboRotations.UltimateRotations.Ranged;

[BetaRotation]
[Rotation("MCH UWU",
    CombatType.PvE,
    GameVersion = $"v.\notation： v...2\n\n",
    Description = $"┏━━━━━━━━┓\n" +
                   "┃       v...2     ┃\n" +
                   "┃                 ┃\n" +
                   "┗∩━━━━━━∩┛\n" +
                   "        \\ (´･ω･｀) ﾉ")]
[SourceCode(Path = "")]
[Api(4)]
public sealed class MCH_UWU : MachinistRotation
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

    public bool OpenerHasFinishedDummy { get; private set; }
    public bool OpenerHasFinished { get; private set; }
    public int Openerstep { get; private set; }
    public bool OpenerAvailable { get; private set; }
    public bool OpenerInProgress { get; private set; }
    public bool StartOpener { get; private set; }

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
            if (DrillPvE.CanUse(out var act))
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
        bool isNailSmall = Target.Name.ToString() == "Infernal Nail" && !Target.HasStatus(false, StatusID.VulnerabilityDown_1545);
        bool isNailSmallLowHP = Target.Name.ToString() == "Infernal Nail" && !Target.HasStatus(false, StatusID.VulnerabilityDown_1545) && Target.CurrentHp < 13435 && IsInUwU;
        bool isNailBig = Target.Name.ToString() == "Infernal Nail" && Target.HasStatus(false, StatusID.VulnerabilityDown_1545);
        bool isTargetLahabrea = Target.Name.ToString() == "Lahabrea" && IsInUwU;
        bool isTargetMagitekBit = Target.Name.ToString() == "Magitek Bit" && IsInUwU;
        bool inRaids = TerritoryContentType.Equals(TerritoryContentType.Raids);
        bool inTrials = TerritoryContentType.Equals(TerritoryContentType.Trials);
        bool hasTinctureBuff = Player.HasStatus(true, StatusID.Medicated);
        string playerName = Player.Name.ToString();
        bool isTargetNail = Target.Name.ToString() == "Infernal Nail";
        bool isTargetIfrit = Target.Name.ToString() == "Ifrit";
        bool isTargetGaruda = Target.Name.ToString() == "Garuda";
        bool isTargetPlayer = Target.Name.ToString() == playerName;

        if (isNailSmallLowHP || Player.StatusTime(true, StatusID.Transcendent) > 0)
        {
            return false;
        }

        if (isTargetLahabrea || isTargetNail || isTargetMagitekBit || isTargetPlayer)
        {
            return false;
        }

        if (isTargetGaruda && Target.GetHealthRatio() < 0.25)
        {
            return false;
        }

        // Reassemble Logic
        // Check next GCD action and conditions for Reassemble.
        bool isReassembleUsable = nextGCD.IsTheSameTo(true, DrillPvE);

        //// Keeps Ricochet and Gauss cannon Even
        //bool isRicochetMore = RicochetPvE.EnoughLevel && GaussRoundPvE.Cooldown.CurrentCharges <= RicochetPvE.Cooldown.CurrentCharges;
        //bool isGaussMore = !RicochetPvE.EnoughLevel || GaussRoundPvE.Cooldown.CurrentCharges > RicochetPvE.Cooldown.CurrentCharges;

        // Use Barrel Stabilizer on CD if won't cap
        if (BarrelStabilizerPvE.CanUse(out act))
        {
            return true;
        }

        //if (CombatElapsedLessGCD(10))
        //{
        if (WildfirePvE.CanUse(out act, true) &&
            (HeatStacks == 5 ||
            (nextGCD.IsTheSameTo(true, HotShotPvE) &&
            (Player.HasStatus(true, StatusID.Hypercharged) || Heat >= 50))))
        {
            return true;
        }

        if (CombatElapsedLessGCD(5))
        {

            if (BarrelStabilizerPvE.CanUse(out act))
            {
                return true;
            }
            if (WildfirePvE.CanUse(out act))
            {
                return true;
            }
            if (HyperchargePvE.CanUse(out act))
            {
                return true;
            }
        }

        //if (WildfirePvE.CanUse(out act, true) &&
        //    (HeatStacks == 5 ||
        //    (nextGCD.IsTheSameTo(true, DrillPvE) &&
        //    (Player.HasStatus(true, StatusID.Hypercharged) || Heat >= 50))))
        //{
        //    return true;
        //}

        if (Battery >= 50 && RookAutoturretPvE.CanUse(out act))
        {
            return true;
        }
        //}

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
    // HyperchargePvE
    // RookAutoturretPvE
    // BarrelStabilizerPvE
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (OpenerInProgress)
        {
            return Opener(out act);
        }

        act = null;
        bool isNailSmall = Target.Name.ToString() == "Infernal Nail" && !Target.HasStatus(false, StatusID.VulnerabilityDown_1545);
        bool isNailSmallLowHP = Target.Name.ToString() == "Infernal Nail" && !Target.HasStatus(false, StatusID.VulnerabilityDown_1545) && Target.CurrentHp < 13435 && IsInUwU;
        bool isNailBig = Target.Name.ToString() == "Infernal Nail" && Target.HasStatus(false, StatusID.VulnerabilityDown_1545);
        bool inRaids = TerritoryContentType.Equals(TerritoryContentType.Raids);
        bool inTrials = TerritoryContentType.Equals(TerritoryContentType.Trials);
        bool hasTinctureBuff = Player.HasStatus(true, StatusID.Medicated);

        string playerName = Player.Name.ToString();

        bool isTargetNail = Target.Name.ToString() == "Infernal Nail";
        bool isTargetIfrit = Target.Name.ToString() == "Ifrit";
        bool isTargetGaruda = Target.Name.ToString() == "Garuda";
        bool isTargetPlayer = Target.Name.ToString() == playerName;
        bool isTargetLahabrea = Target.Name.ToString() == "Lahabrea" && IsInUwU;
        bool isTargetMagitekBit = Target.Name.ToString() == "Magitek Bit" && IsInUwU;

        if (isTargetLahabrea || isTargetNail || isTargetMagitekBit || isTargetPlayer)
        {
            return false;
        }

        if (isTargetGaruda && Target.GetHealthRatio() < 0.25)
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
                && CanUseHyperchargePvE(out _) && WildfirePvE.CanUse(out act, true))
            {
                return true;
            }

        }
        // Use Hypercharge if at least 12 seconds of combat and (if wildfire will not be up in 30 seconds or if you hit 100 heat)
        if (/*!CombatElapsedLess(12) && */!Player.HasStatus(true, StatusID.Reassembled) && (!WildfirePvE.Cooldown.WillHaveOneCharge(30) || Heat == 100))
        {
            if (CanUseHyperchargePvE(out act))
            {
                return true;
            }
        }
        // Rook Autoturret/Queen Logic
        //if (!IsLastGCD(true, HeatBlastPvE, BlazingShotPvE) && CanUseQueenMeow(out act))
        //{
        //    return true;
        //}

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
        bool isNailSmall = Target.Name.ToString() == "Infernal Nail" && !Target.HasStatus(false, StatusID.VulnerabilityDown_1545);
        bool isNailSmallLowHP = Target.Name.ToString() == "Infernal Nail" && !Target.HasStatus(false, StatusID.VulnerabilityDown_1545) && Target.CurrentHp < 13435 && IsInUwU;
        bool isNailBig = Target.Name.ToString() == "Infernal Nail" && Target.HasStatus(false, StatusID.VulnerabilityDown_1545);
        bool isTargetLahabrea = Target.Name.ToString() == "Lahabrea" && IsInUwU;
        bool isTargetMagitekBit = Target.Name.ToString() == "Magitek Bit" && IsInUwU;
        bool inRaids = TerritoryContentType.Equals(TerritoryContentType.Raids);
        bool hasTinctureBuff = Player.HasStatus(true, StatusID.Medicated);
        string playerName = Player.Name.ToString();
        bool isTargetNail = Target.Name.ToString() == "Infernal Nail";
        bool isTargetIfrit = Target.Name.ToString() == "Ifrit";
        bool isTargetPlayer = Target.Name.ToString() == playerName;

        if (isNailSmallLowHP)
        {
            return false;
        }

        // Checks and executes AutoCrossbow or HeatBlast if conditions are met (overheated state).
        if (AutoCrossbowPvE.CanUse(out act) && !isTargetIfrit && !isTargetPlayer && !isTargetNail && !isTargetLahabrea && !isTargetMagitekBit)
        {
            return true;
        }

        if (HeatBlastPvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }

        //bool experimentalFeatureNoGCDsBeyondThisPoint = ExperimentalFeature && IsOverheated;
        //if (experimentalFeatureNoGCDsBeyondThisPoint) return false;

        // Check if SpreadShot cannot be used
        if ((!SpreadShotPvE.CanUse(out _) || SpreadShotPvE.CanUse(out _)) && !isTargetPlayer && !isTargetNail && !isTargetLahabrea && !isTargetMagitekBit)
        {
        if (AirAnchorPvE.EnoughLevel && AirAnchorPvE.CanUse(out act) && !isTargetPlayer && !isTargetNail && !isTargetLahabrea && !isTargetMagitekBit)
        {
            return true;
        }

        if (!AirAnchorPvE.EnoughLevel && HotShotPvE.CanUse(out act) && !isTargetPlayer && !isTargetNail && !isTargetLahabrea && !isTargetMagitekBit)
        {
            return true;
        }

        // Check if Drill can be used
        if (DrillPvE.EnoughLevel && DrillPvE.CanUse(out act) && !isTargetPlayer && !isTargetNail && !isTargetLahabrea && !isTargetMagitekBit)
        {
            return true;
        }
        }

        if (SpreadShotPvE.CanUse(out act) && !isTargetPlayer && !isTargetNail && !isTargetLahabrea && !isTargetMagitekBit && !isTargetIfrit)
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
        var Lvl70 = Player.Level == 70;
        bool HasHotShot = !HotShotPvE.Cooldown.IsCoolingDown;
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

        OpenerAvailable = Lvl70
                                    && HasHotShot
                                    && HasDrill
                                    && HasBarrelStabilizer
                                    && RCcharges >= 2
                                    && GRcharges >= 2
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
            Openerstep++;
            return false;
        }
        return nextAction;
    }

    private bool Opener(out IAction? act)
    {
        switch (Openerstep)
        {
            case 0:
                return OpenerController(IsLastAbility(false, BarrelStabilizerPvE), BarrelStabilizerPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 1:
                return OpenerController(IsLastAbility(false, WildfirePvE), WildfirePvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 2:
                return OpenerController(IsLastGCD(false, HotShotPvE), HotShotPvE.CanUse(out act, usedUp: true));

            case 3:
                return OpenerController(IsLastAbility(false, HyperchargePvE), HyperchargePvE.CanUse(out act, usedUp: true));

            case 4:
                OpenerHasFinished = true;
                Openerstep = 0;
                break;
        }
        act = null;
        return OpenerHasFinishedDummy = false;
    }

    // Logic for Hypercharge
    private bool CanUseHyperchargePvE(out IAction? act)
    {
        if (IsLastAbility(ActionID.WildfirePvE) && (Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged) || HeatStacks == 5))
        {
            return HyperchargePvE.CanUse(out act);
        }

        float REST_TIME = 6f;
        if
                     //Cannot AOE
                     (!SpreadShotPvE.CanUse(out _)
                     &&
                     // HotShot Charge Detection
                     (HotShotPvE.EnoughLevel && HotShotPvE.Cooldown.WillHaveOneCharge(REST_TIME)
                     ||
                     // Drill Charge Detection
                     DrillPvE.EnoughLevel && DrillPvE.Cooldown.WillHaveOneCharge(REST_TIME)
                     ||
                     Target.GetHealthRatio() <= 0.20))
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
        ImGui.Text("Openerstep: " + Openerstep.ToString());
    }
}