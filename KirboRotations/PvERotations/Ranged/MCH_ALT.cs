#pragma warning disable S1066 // Mergeable "if" statements should be combined
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable S101 // Types should be named in PascalCase
#pragma warning disable S125 // Sections of code should not be commented out
//Note when using the Ability.CanUse(out act, isLastAbility: true)
// NextAbilityToNextGCD - 0.60s 
// 0.60s + isLastAbilityTimer 
// NextAbilityToNextGCD must be equal or lower then 0.60s + isLastAbilityTimer
// NextAbilityToNextGCD ranges from -0.60s ~ 1.90s
// If overheated ranges from -0.60s ~ 0.90s

using System.ComponentModel;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using KirboRotations.Helpers;

namespace KirboRotations.PvERotations.Ranged;

[BetaRotation]
[Rotation(
"┏━━━━━━┓\n" + "┃    ┃\n" + "┃        ┃\n" + "┗━━━━━━┛",
CombatType.PvE,
GameVersion = $"v.\notation： v...9\n\n",
Description = $"┏━━━━━━━━┓\n" + "┃       v...9     ┃\n" + "┃                 ┃\n" + "┗∩━━━━━━∩┛\n" + "        \\ (´･ω･｀) ﾉ")]
[Api(4)]
public sealed class MCH_ALT : MachinistRotation
{
    #region Config Options
    public enum Openers : byte
    {
        [Description("Default-Opener")] Default,

        [Description("Alternative-Opener")] Alternative,

        [Description("Beta-Opener")] Beta,
    }

    [RotationConfig(CombatType.PvE, Name = "Immediately use Rook Autoturret/Automaton Queen if battery is 50+ ")]
    public bool SkipQueenLogic { get; set; } = true;

    [RotationConfig(CombatType.PvE, Name = "Opener")]
    public Openers SelectedOpener { get; set; } = Openers.Default;

    //[RotationConfig(CombatType.PvE, Name = "Automatic 2nd tincture")]
    //public bool UseAuto2ndTincture { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Enable UwU Checker.")]
    public bool EnableUwUChecker { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Enable TEA Checker.")]
    public bool EnableTEAChecker { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Enable experimental features.")]
    public bool ExperimentalFeature { get; set; } = false;
    #endregion

    #region Properties
    private bool CountDownActive { get; set; } = false;

    private bool StartOpener { get; set; } = false;
    public bool OpenerInProgress { get; private set; }
    private bool OpenerHasFinished { get; set; } = false;
    private bool OpenerHasFinishedDummy { get; set; } = false;
    private bool OpenerAvailable { get; set; } = false;
    private int OpenerStep { get; set; } = 0;
    const float universalFailsafeThreshold = 5.0f;
    private bool InBurst => Player.HasStatus(true, StatusID.Wildfire_1946);
    private bool IsSecond0GCD => WeaponRemain >= 0.59f && WeaponRemain <= 0.80f && CustomRotationEx.GetCurrentAnimationLock() == 0;
    #endregion

    #region Countdown logic
    /// <summary>
    /// Defines logic for actions to take during the countdown before combat starts.
    /// </summary>
    /// <param name="remainTime"></param>
    /// <returns></returns>
    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < 4.95f)
        {
            if (ReassemblePvE.CanUse(out var act) && !Player.HasStatus(true, StatusID.Reassembled))
            {
                return act;
            }
        }
        if (remainTime < 1.1f)
        {
            if (UseBurstMedicine(out var act))
            {
                return act;
            }
        }
        if (remainTime < 0.5f)
        {
            if (AirAnchorPvE.CanUse(out var act))
            {
                StartOpener = true;
                return act;
            }
        }
        if (remainTime > 0.1f)
        {
            CountDownActive = true;
        }
        if (remainTime <= 0.1f)
        {
            CountDownActive = false;
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

        //if (UseAuto2ndTincture && ShouldUseBurstMedicine() && UseBurstMedicine(out act) && WildfirePvE.Cooldown.ElapsedAfter(115))
        //{
        //    return true;
        //}

        // Reassemble Logic
        // Check next GCD action and conditions for Reassemble.
        bool isReassembleUsable =
            //Reassemble current # of charges and double proc protection
            ReassemblePvE.Cooldown.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Reassembled) &&
            (nextGCD.IsTheSameTo(true, [ChainSawPvE, ExcavatorPvE, AirAnchorPvE]) ||
            !ChainSawPvE.EnoughLevel && nextGCD.IsTheSameTo(true, DrillPvE) ||
            !DrillPvE.EnoughLevel && nextGCD.IsTheSameTo(true, CleanShotPvE) ||
            //HotShot Logic
            !CleanShotPvE.EnoughLevel && nextGCD.IsTheSameTo(true, HotShotPvE));

        // Keeps Ricochet and Gauss cannon Even
        bool isRicochetMore = RicochetPvE.EnoughLevel && GaussRoundPvE.Cooldown.CurrentCharges <= RicochetPvE.Cooldown.CurrentCharges;
        bool isGaussMore = !RicochetPvE.EnoughLevel || GaussRoundPvE.Cooldown.CurrentCharges > RicochetPvE.Cooldown.CurrentCharges;

        bool inRaids = TerritoryContentType.Equals(TerritoryContentType.Raids);
        bool inTrials = TerritoryContentType.Equals(TerritoryContentType.Trials);
        bool lateWeave = WeaponRemain >= 0.59f && WeaponRemain <= 0.80f;

        if (/*(inRaids || inTrials) &&*/ CombatElapsedLessGCD(10))
        {
            if (!CombatElapsedLessGCD(5) && lateWeave)
            {
                if (WildfirePvE.CanUse(out act))
                {
                    return true;
                }
            }

            if (IsLastGCD(ActionID.DrillPvE) && BarrelStabilizerPvE.CanUse(out act))
            {
                return true;
            }

            if (Battery >= 50 && IsLastGCD(ActionID.ExcavatorPvE, ActionID.ChainSawPvE) && RookAutoturretPvE.CanUse(out act, false, true, true, true))
            {
                return true;
            }
        }

        // Attempt to use Reassemble if it's ready
        if (isReassembleUsable)
        {
            if (ReassemblePvE.CanUse(out act, skipComboCheck: true, usedUp: true)) return true;
        }

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

        // Check for not burning Hypercharge below level 52 on AOE
        bool lateWeave = WeaponRemain >= 0.59f && WeaponRemain <= 0.80f;
        bool LowLevelHyperCheck = !AutoCrossbowPvE.EnoughLevel && SpreadShotPvE.CanUse(out _);
        //if (lateWeave && (IsLastAbility(false, HyperchargePvE) || Player.HasStatus(true, StatusID.Hypercharged)))
        //{
        //    return WildfirePvE.CanUse(out act);
        //}

        // If Wildfire is active, use Hypercharge.....Period
        if (Player.HasStatus(true, StatusID.Wildfire_1946) && !nextGCD.IsTheSameTo(true, FullMetalFieldPvE)) // could be ruining things
        {
            return HyperchargePvE.CanUse(out act);
        }
        // Burst
        if (IsBurst)
        {
            if (UseBurstMedicine(out act))
            {
                return true;
            }

            if (lateWeave &&
                ((IsLastAbility(false, HyperchargePvE) || Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged)) &&
                !CombatElapsedLess(10) &&
                CanUseHyperchargePvE(out _) &&
                !LowLevelHyperCheck &&
                WildfirePvE.CanUse(out act)))
            {
                return true;
            }

            //bool lateWeave = WeaponRemain >= 0.59f && WeaponRemain <= 0.80f;
            //if (((IsLastAbility(false, HyperchargePvE)) || Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged))
            //    && !CombatElapsedLess(10) && CanUseHyperchargePvE(out _) && !LowLevelHyperCheck && WildfirePvE.CanUse(out act))
            //{
            //    return true;
            //}

        }
        // Use Hypercharge if at least 12 seconds of combat and (if wildfire will not be up in 30 seconds or if you hit 100 heat)
        if (!LowLevelHyperCheck && !CombatElapsedLess(12) && !Player.HasStatus(true, StatusID.Reassembled) && (!WildfirePvE.Cooldown.WillHaveOneCharge(30) || Heat == 100))
        {
            if (CanUseHyperchargePvE(out act))
            {
                return true;
            }
        }
        // Rook Autoturret/Queen Logic
        if (!IsLastGCD(true, HeatBlastPvE, BlazingShotPvE) && CanUseQueenMeow(out act))
        {
            return true;
        }

        if ((nextGCD.IsTheSameTo(true, CleanShotPvE) && Battery == 100) ||
            (nextGCD.IsTheSameTo(true, HotShotPvE, AirAnchorPvE, ChainSawPvE, ExcavatorPvE) && Battery >= 90) ||
            InBurst)
        {
            if (RookAutoturretPvE.CanUse(out act))
            {
                return true;
            }
        }
        // Use Barrel Stabilizer on CD if won't cap
        if (BarrelStabilizerPvE.CanUse(out act))
        {
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

        bool inRaids = TerritoryContentType.Equals(TerritoryContentType.Raids);
        bool hasTinctureBuff = Player.HasStatus(true, StatusID.Medicated);

        // Checks and executes AutoCrossbow or HeatBlast if conditions are met (overheated state).
        if (AutoCrossbowPvE.CanUse(out act))
        {
            return true;
        }

        if (HeatBlastPvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }

        bool experimentalFeatureNoGCDsBeyondThisPoint = ExperimentalFeature && IsOverheated;
        if (experimentalFeatureNoGCDsBeyondThisPoint) return false;

        // Executes Bioblaster, and then checks for AirAnchor or HotShot, and Drill based on availability and conditions.
        if (BioblasterPvE.CanUse(out act))
        {
            return true;
        }

        // Check if SpreadShot cannot be used
        if (!SpreadShotPvE.CanUse(out _))
        {
            // Check if AirAnchor can be used
            if (AirAnchorPvE.CanUse(out act))
            {
                return true;
            }

            // If not at the required level for AirAnchor and HotShot can be used
            if (!AirAnchorPvE.EnoughLevel && HotShotPvE.CanUse(out act))
            {
                return true;
            }

            // Check if Drill can be used
            if (DrillPvE.CanUse(out act))
            {
                return true;
            }

            if (ExcavatorPvE.CanUse(out act, usedUp: true))
            {
                return true;
            }

            if (ChainSawPvE.CanUse(out act, usedUp: true))
            {
                return true;
            }

            if (!CombatElapsedLessGCD(3) && DrillPvE.CanUse(out act, usedUp: true))
            {
                return true;
            }

            if (Player.HasStatus(true, StatusID.FullMetalMachinist) && FullMetalFieldPvE.CanUse(out act, usedUp: true))
            {
                return true;
            }
        }

        // Special condition for using ChainSaw outside of AoE checks if no action is chosen within 4 GCDs.
        if (!CombatElapsedLessGCD(1) && ChainSawPvE.CanUse(out act, skipAoeCheck: true))
        {
            return true;
        }

        if (ExcavatorPvE.CanUse(out act, skipAoeCheck: true))
        {
            return true;
        }

        if (!ChainSawPvE.Cooldown.WillHaveOneCharge(6f) && !CombatElapsedLessGCD(6))
        {
            if (DrillPvE.CanUse(out act, usedUp: true))
            {
                return true;
            }
        }

        // AoE actions: ChainSaw and SpreadShot based on their usability.
        if (SpreadShotPvE.CanUse(out _))
        {
            if (ChainSawPvE.CanUse(out act))
            {
                return true;
            }

            if (ExcavatorPvE.CanUse(out act))
            {
                return true;
            }
        }
        if (FullMetalFieldPvE.CanUse(out act))
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
    /// <summary>
    /// Updates the custom fields.
    /// </summary>
    protected override void UpdateInfo()
    {
        OpenerReady();
        OpenerStarter();
    }

    /// <summary>
    /// Logic for Hypercharge
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
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
                     DrillPvE.EnoughLevel && DrillPvE.Cooldown.WillHaveOneCharge(REST_TIME)
                     ||
                     // Chainsaw Charge Detection
                     ChainSawPvE.EnoughLevel && ChainSawPvE.Cooldown.WillHaveOneCharge(REST_TIME)))
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

    /// <summary>
    /// Logic for Rook Autoturret/Queen.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseQueenMeow(out IAction? act)
    {
        // Define conditions under which the Rook Autoturret/Queen can be used.
        bool NoQueenLogic = SkipQueenLogic;
        bool QueenOne = Battery >= 50 && CombatElapsedLess(25f);
        bool QueenTwo = Battery >= 90 && !CombatElapsedLess(58f) && CombatElapsedLess(78f);
        bool QueenThree = Battery >= 100 && !CombatElapsedLess(111f) && CombatElapsedLess(131f);
        bool QueenFour = Battery >= 50 && !CombatElapsedLess(148f) && CombatElapsedLess(168f);
        bool QueenFive = Battery >= 60 && !CombatElapsedLess(178f) && CombatElapsedLess(198f);
        bool QueenSix = Battery >= 100 && !CombatElapsedLess(230f) && CombatElapsedLess(250f);
        bool QueenSeven = Battery >= 50 && !CombatElapsedLess(268f) && CombatElapsedLess(288f);
        bool QueenEight = Battery >= 70 && !CombatElapsedLess(296f) && CombatElapsedLess(316f);
        bool QueenNine = Battery >= 100 && !CombatElapsedLess(350f) && CombatElapsedLess(370f);
        bool QueenTen = Battery >= 50 && !CombatElapsedLess(388f) && CombatElapsedLess(408f);
        bool QueenEleven = Battery >= 80 && !CombatElapsedLess(416f) && CombatElapsedLess(436f);
        bool QueenTwelve = Battery >= 100 && !CombatElapsedLess(470f) && CombatElapsedLess(490f);
        bool QueenThirteen = Battery >= 50 && !CombatElapsedLess(505f) && CombatElapsedLess(525f);
        bool QueenFourteen = Battery >= 60 && !CombatElapsedLess(538f) && CombatElapsedLess(558f);
        bool QueenFifteen = Battery >= 100 && !CombatElapsedLess(590f) && CombatElapsedLess(610f);

        if (NoQueenLogic || QueenOne || QueenTwo || QueenThree || QueenFour || QueenFive || QueenSix || QueenSeven || QueenEight || QueenNine || QueenTen || QueenEleven || QueenTwelve || QueenThirteen || QueenFourteen || QueenFifteen)
        {
            if (RookAutoturretPvE.CanUse(out act))
            {
                return true;
            }
        }
        act = null;
        return false;
    }

    /// <summary>
    /// Manages the initiation and reset logic for the opener sequence. 
    /// If the opener has finished, it resets relevant flags and step counters. 
    /// Otherwise, it toggles the 'OpenerInProgress' state based on whether the opener is set to start.
    /// </summary>
    private void OpenerStarter()
    {
        if (OpenerHasFinished)
        {
            StartOpener = false;
            OpenerStep = 0;
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

    private void ResetOpenerValues()
    {
        StartOpener = false;
        OpenerInProgress = false;
        OpenerHasFinished = false;
        OpenerStep = 0;
    }

    /// <summary>
    /// Method that checks the opener requirements.
    /// </summary>
    /// <returns></returns>
    private bool OpenerReady()
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
        bool openerStep0 = OpenerStep == 0;

        OpenerAvailable = Lvl100
                                    && HasChainSaw
                                    && HasAirAnchor
                                    && HasDrill
                                    && DrillCharges == 2
                                    && HasBarrelStabilizer
                                    && RCcharges == 3
                                    && GRcharges == 3
                                    && HasWildfire
                                    && ReassembleOneCharge
                                    && NoResources
                                    && openerStep0;
        return false;
    }

    /// <summary>
    /// <br>Method that allows using actions in a specific order.</br>
    /// <br>First checks if lastAction used matches specified action, if true, increases openerstep.</br>
    /// <br>If first check is false, then 'nextAction' calls and executes the specified action's 'CanUse' method </br>
    /// </summary>
    /// <param name="lastAction"></param>
    /// <param name="nextAction"></param>
    /// <returns></returns>
    private bool OpenerController(bool lastAction, bool nextAction)
    {
        if (lastAction)
        {
            OpenerStep++;
            return false;
        }
        return nextAction;
    }

    /// <summary>
    /// Opener sequence logic.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool Opener(out IAction? act)
    {
        // Universal failsafe for opener inactivity
        if (TimeSinceLastAction.TotalSeconds > universalFailsafeThreshold && OpenerStep > 0)
        {
            act = null;
            OpenerHasFinished = true;  // Stop the opener
            return false;  // Stop further action
        }

        switch (SelectedOpener)
        {
            case Openers.Default:
                switch (OpenerStep)
                {
                    case 0:
                        return OpenerController(IsLastGCD(true, AirAnchorPvE), AirAnchorPvE.CanUse(out act));

                    case 1:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 2:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 3:
                        return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

                    case 4:
                        return OpenerController(IsLastAbility(false, BarrelStabilizerPvE), BarrelStabilizerPvE.CanUse(out act, usedUp: true));

                    case 5:
                        return OpenerController(IsLastGCD(false, ChainSawPvE), ChainSawPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 6:
                        return OpenerController(IsLastGCD(true, ExcavatorPvE), ExcavatorPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 7:
                        return OpenerController(IsLastAbility(true, RookAutoturretPvE), RookAutoturretPvE.CanUse(out act, usedUp: true));

                    case 8:
                        return OpenerController(IsLastAbility(false, ReassemblePvE), ReassemblePvE.CanUse(out act, usedUp: true));

                    case 9:
                        return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

                    case 10:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 11:
                        // Only proceed if WeaponRemain is between 0.6s and 0.8s
                        if (WeaponRemain >= 0.59f && WeaponRemain <= 0.80f)
                        {
                            return OpenerController(IsLastAbility(false, WildfirePvE), WildfirePvE.CanUse(out act));
                        }
                        else if (WeaponRemain > 0.80f)
                        {
                            // Hold this step until WeaponRemain is within the desired range
                            act = null; // No action is performed, but the step is not advanced
                            return true; // Keep checking the condition on subsequent calls
                        }
                        else
                        {
                            act = null;
                            OpenerHasFinished = true;
                            return false;
                        }

                    case 12:
                        return OpenerController(IsLastGCD(true, FullMetalFieldPvE), FullMetalFieldPvE.CanUse(out act, skipAoeCheck: true));

                    case 13:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 14:
                        return OpenerController(IsLastAbility(false, HyperchargePvE), HyperchargePvE.CanUse(out act, usedUp: true));

                    case 15:
                        OpenerHasFinished = true;
                        OpenerStep = 0;
                        break;
                }
                break;

            case Openers.Alternative:
                switch (OpenerStep)
                {
                    case 0:
                        return OpenerController(IsLastGCD(true, AirAnchorPvE), AirAnchorPvE.CanUse(out act));

                    case 1:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 2:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 3:
                        return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

                    case 4:
                        return OpenerController(IsLastAbility(false, BarrelStabilizerPvE), BarrelStabilizerPvE.CanUse(out act, usedUp: true));

                    case 5:
                        return OpenerController(IsLastGCD(false, ChainSawPvE), ChainSawPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 6:
                        return OpenerController(IsLastAbility(false, ReassemblePvE), ReassemblePvE.CanUse(out act, usedUp: true));

                    case 7:
                        return OpenerController(IsLastGCD(true, ExcavatorPvE), ExcavatorPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 8:
                        return OpenerController(IsLastAbility(true, RookAutoturretPvE), RookAutoturretPvE.CanUse(out act, usedUp: true));

                    case 9:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 10:
                        return OpenerController(IsLastGCD(true, FullMetalFieldPvE), FullMetalFieldPvE.CanUse(out act, skipAoeCheck: true));

                    case 11:
                        return OpenerController(IsLastAbility(false, HyperchargePvE), HyperchargePvE.CanUse(out act, usedUp: true));

                    case 12:
                        // Only proceed if WeaponRemain is between 0.6s and 0.8s
                        if (WeaponRemain >= 0.59f && WeaponRemain <= 0.80f)
                        {
                            return OpenerController(IsLastAbility(false, WildfirePvE), WildfirePvE.CanUse(out act));
                        }
                        else if (WeaponRemain > 0.80f)
                        {
                            // Hold this step until WeaponRemain is within the desired range
                            act = null; // No action is performed, but the step is not advanced
                            return true; // Keep checking the condition on subsequent calls
                        }
                        else
                        {
                            act = null;
                            OpenerHasFinished = true;
                            return false;
                        }

                    //case 12:
                    //    return OpenerController(IsLastAbility(false, WildfirePvE), WildfirePvE.CanUse(out act/*, isLastAbility: true*/) && WeaponRemain >= 0.6 && WeaponRemain <= 1);

                    case 13:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 4, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 14:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 15:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 3, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 16:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 17:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 2, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 18:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 19:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 1, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 20:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 21:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 0, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 22:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 23:
                        return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

                    case 24:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 25:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 26:
                        return OpenerController(IsLastAction(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

                    case 27:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 28:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 29:
                        return OpenerController(IsLastGCD(true, SplitShotPvE), SplitShotPvE.CanUse(out act));

                    case 30:
                        return OpenerController(IsLastGCD(true, SlugShotPvE), SlugShotPvE.CanUse(out act));

                    case 31:
                        return OpenerController(IsLastGCD(true, CleanShotPvE), CleanShotPvE.CanUse(out act));

                    case 32:
                        OpenerHasFinished = true;
                        OpenerStep = 0;
                        break;
                }
                break;

            case Openers.Beta:
                switch (OpenerStep)
                {
                    case 0:
                        return OpenerController(IsLastGCD(true, AirAnchorPvE), AirAnchorPvE.CanUse(out act));

                    case 1:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 2:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 3:
                        return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

                    case 4:
                        return OpenerController(IsLastAbility(false, BarrelStabilizerPvE), BarrelStabilizerPvE.CanUse(out act, usedUp: true));

                    case 5:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 6:
                        return OpenerController(IsLastGCD(false, ChainSawPvE), ChainSawPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 7:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 8:
                        return OpenerController(IsLastAbility(false, ReassemblePvE), ReassemblePvE.CanUse(out act, usedUp: true));

                    case 9:
                        return OpenerController(IsLastGCD(true, ExcavatorPvE), ExcavatorPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 10:
                        return OpenerController(IsLastAbility(true, RookAutoturretPvE), RookAutoturretPvE.CanUse(out act, usedUp: true));

                    case 11:
                        // Only proceed if WeaponRemain is between 0.6s and 0.8s
                        if (WeaponRemain >= 0.59f && WeaponRemain <= 0.80f)
                        {
                            return OpenerController(IsLastAbility(false, WildfirePvE), WildfirePvE.CanUse(out act));
                        }
                        else if (WeaponRemain > 0.80f)
                        {
                            // Hold this step until WeaponRemain is within the desired range
                            act = null; // No action is performed, but the step is not advanced
                            return true; // Keep checking the condition on subsequent calls
                        }
                        else
                        {
                            act = null;
                            OpenerHasFinished = true;
                            return false;
                        }

                    case 12:
                        return OpenerController(IsLastGCD(true, FullMetalFieldPvE), FullMetalFieldPvE.CanUse(out act, skipAoeCheck: true));

                    case 13:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 14:
                        return OpenerController(IsLastAbility(false, HyperchargePvE), HyperchargePvE.CanUse(out act, usedUp: true));

                    case 15:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 4, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 16:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 17:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 3, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 18:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 19:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 2, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 20:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 21:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 1, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 22:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 23:
                        return OpenerController(IsLastGCD(true, HeatBlastPvE) && OverheatedStacks == 0, HeatBlastPvE.CanUse(out act, usedUp: true));

                    case 24:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 25:
                        return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

                    case 26:
                        return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 27:
                        return OpenerController(IsLastAbility(true, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

                    case 28:
                        return OpenerController(IsLastAction(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

                    case 29:
                        return OpenerController(IsLastGCD(true, SplitShotPvE), SplitShotPvE.CanUse(out act));

                    case 30:
                        return OpenerController(IsLastGCD(true, SlugShotPvE), SlugShotPvE.CanUse(out act));

                    case 31:
                        return OpenerController(IsLastGCD(true, CleanShotPvE), CleanShotPvE.CanUse(out act));

                    case 32:
                        OpenerHasFinished = true;
                        OpenerStep = 0;
                        break;
                }
                break;
        }

        act = null;
        return false;
    }

    /// <summary>
    /// Tincture logic.
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool ShouldUseBurstMedicine(out IAction? act)
    {
        act = null;  // Default to null if Tincture cannot be used.

        // Don't use Tincture if player has a bad status
        if (Player.HasStatus(false, StatusID.Weakness) || Player.HasStatus(true, StatusID.Transcendent) || Player.HasStatus(true, StatusID.BrinkOfDeath))
        {
            return false;
        }

        if (WildfirePvE.Cooldown.RecastTimeRemainOneCharge <= 20 && CombatTime > 60 &&
            NextAbilityToNextGCD > 1.2 &&
            !Player.HasStatus(true, StatusID.Weakness) &&
            DrillPvE.Cooldown.RecastTimeRemainOneCharge < 5 &&
            AirAnchorPvE.Cooldown.RecastTimeRemainOneCharge < 5)
        {
            // Attempt to use Burst Medicine.
            return UseBurstMedicine(out act, false);
        }

        // If the conditions are not met, return false.
        return false;
    }
    #endregion

    /// <summary>
    /// Displays extra status information.
    /// </summary>
    public unsafe override void DisplayStatus()
    {
        float paddingX = ImGui.GetStyle().WindowPadding.X;
        DisplayStatusHelper.BeginPaddedChild("The CustomRotation's status window", true, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        ImGui.Text("Rotation: " + Name + " ");
        ImGui.SameLine();
        ImGui.TextDisabled(Description);
        DisplayStatusHelper.DisplayGCDStatus();
        //var gameobjectID = DataBase.DisplayPlayerGameObjectId();

        ImGui.BeginGroup();
        if (ImGui.Button(nameof(ActionID.PelotonPvE)))
        {
            ActionManagerHelper.Instance.InstanceActionManager->UseAction(ActionType.Action, (uint)ActionID.PelotonPvE);
        }
        if (ImGui.Button("Reset Opener values"))
        {
            ResetOpenerValues();
            //StartOpener = false;
            //OpenerInProgress = false;
            //OpenerHasFinished = false;
            //OpenerStep = 0;
        }
        ImGui.EndGroup();

        ImGui.BeginGroup();
        ImGui.Text("SelectedOpener: " + SelectedOpener.ToString());
        ImGui.Text("OpenerAvailable: " + OpenerAvailable.ToString());
        ImGui.Text("StartOpener: " + StartOpener.ToString());
        ImGui.Text("OpenerInProgress: " + OpenerInProgress.ToString());
        ImGui.Text("OpenerHasFinished: " + OpenerHasFinished.ToString());
        ImGui.Text("OpenerStep: " + OpenerStep.ToString());
        ImGui.EndGroup();

        if (InCombat)
        {
            float time = (float)TimeSinceLastAction.TotalSeconds;  // Assuming TimeSinceLastAction is in seconds
            int minutes = (int)(time / 60);    // Extract minutes
            float seconds = time % 60;         // Extract remaining seconds
            ImGui.Text($"TimeSinceLastAction: {minutes:00}:{seconds:00.00}");
        }

        DisplayStatusHelper.EndPaddedChild();
    }

    // Currently no use for.
    /// <summary>
    /// Handles actions when the territory changes.
    /// </summary>
    //public override void OnTerritoryChanged()
    //{
    //CreateSystemWarning("Changed Territory");
    //}
}