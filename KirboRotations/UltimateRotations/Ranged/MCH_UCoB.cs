//#pragma warning disable S1066 // Mergeable "if" statements should be combined

//using FFXIVClientStructs.FFXIV.Client.Game;
//using FFXIVClientStructs.FFXIV.Client.UI.Misc;
//using KirboRotations.Helpers;
//using KirboRotations.IllegalHelpers;

//namespace KirboRotations.UltimateRotations.Ranged;

//[BetaRotation]
//[Rotation("MCH UCoB",
//    CombatType.PvE,
//    GameVersion = $"v.\notation： v...\n\n",
//    Description = $"┏━━━━━━━━┓\n" +
//                   "┃       v...     ┃\n" +
//                   "┃                 ┃\n" +
//                   "┗∩━━━━━━∩┛\n" +
//                   "        \\ (´･ω･｀) ﾉ")]
//[SourceCode(Path = "")]
//[Api(4)]
//public sealed class MCH_UCoB : MachinistRotation
//{
//    #region Config Options
//    [RotationConfig(CombatType.PvE, Name = "Skip Queen Logic and uses Rook Autoturret/Automaton Queen immediately whenever you get 50 battery")]
//    public bool SkipQueenLogic { get; set; } = true;

//    //[RotationConfig(CombatType.PvE, Name = "Use LvL 100 Opener")]
//    //public bool UseLv100Opener { get; set; } = false;

//    [RotationConfig(CombatType.PvE, Name = "Automatic 2nd tincture")]
//    public bool UseAuto2ndTincture { get; set; } = false;

//    [RotationConfig(CombatType.PvE, Name = "Enable experimental features.")]
//    public bool ExperimentalFeature { get; set; } = false;

//    private byte HeatStacks
//    {
//        get
//        {
//            byte stacks = Player.StatusStack(true, StatusID.Overheated);
//            return stacks == byte.MaxValue ? (byte)5 : stacks;
//        }
//    }

//    private bool InBurst { get; set; } = false;
//    private bool StartOpener { get; set; } = false;
//    private bool OpenerHasFinished { get; set; } = false;
//    private bool OpenerHasFinishedDummy { get; set; } = false;
//    private bool OpenerAvailable { get; set; } = false;
//    private int OpenerStep { get; set; } = 0;

//    private bool IsSecond0GCD = false;
//    #endregion

//    #region Countdown logic
//    // Defines logic for actions to take during the countdown before combat starts.
//    protected override IAction? CountDownAction(float remainTime)
//    {
//        if (remainTime < 4.85f)
//        {
//            if (ReassemblePvE.CanUse(out var act) && !Player.HasStatus(true, StatusID.Reassembled))
//            {
//                return act;
//            }
//        }
//        if (remainTime < 1.5f)
//        {
//            if (UseBurstMedicine(out var act))
//            {
//                return act;
//            }
//        }
//        return base.CountDownAction(remainTime);
//    }
//    #endregion

//    #region oGCD Logic
//    // Determines emergency actions to take based on the next planned GCD action.
//    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
//    {
//        if (StartOpener)
//        {
//            return Opener(out act);
//        }

//        if (UseAuto2ndTincture && ShouldUseBurstMedicine(out act))
//        {
//            return true;
//        }

//        // Reassemble Logic
//        // Check next GCD action and conditions for Reassemble.
//        bool isReassembleUsable =
//            //Reassemble current # of charges and double proc protection
//            ReassemblePvE.Cooldown.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Reassembled) &&
//            (nextGCD.IsTheSameTo(true, [ChainSawPvE, ExcavatorPvE, AirAnchorPvE]) ||
//            !ChainSawPvE.EnoughLevel && nextGCD.IsTheSameTo(true, DrillPvE) ||
//            !DrillPvE.EnoughLevel && nextGCD.IsTheSameTo(true, CleanShotPvE) ||
//            //HotShot Logic
//            !CleanShotPvE.EnoughLevel && nextGCD.IsTheSameTo(true, HotShotPvE));

//        // Keeps Ricochet and Gauss cannon Even
//        bool isRicochetMore = RicochetPvE.EnoughLevel && GaussRoundPvE.Cooldown.CurrentCharges <= RicochetPvE.Cooldown.CurrentCharges;
//        bool isGaussMore = !RicochetPvE.EnoughLevel || GaussRoundPvE.Cooldown.CurrentCharges > RicochetPvE.Cooldown.CurrentCharges;

//        bool inRaids = TerritoryContentType.Equals(TerritoryContentType.Raids);
//        bool inTrials = TerritoryContentType.Equals(TerritoryContentType.Trials);

//        if (/*(inRaids || inTrials) &&*/ CombatElapsedLessGCD(10))
//        {
//            if (!CombatElapsedLessGCD(5) && IsSecond0GCD)
//            {
//                if (WildfirePvE.CanUse(out act, true))
//                {
//                    return true;
//                }
//            }

//            if (IsLastGCD(ActionID.DrillPvE) && BarrelStabilizerPvE.CanUse(out act))
//            {
//                return true;
//            }

//            if (Battery >= 50 && IsLastGCD(ActionID.ExcavatorPvE, ActionID.ChainSawPvE) && AutomatonQueenPvE.CanUse(out act, false, true, true, true))
//            {
//                return true;
//            }
//        }

//        // Attempt to use Reassemble if it's ready
//        if (isReassembleUsable)
//        {
//            if (ReassemblePvE.CanUse(out act, skipComboCheck: true, usedUp: true)) return true;
//        }

//        // Use Ricochet
//        if (isRicochetMore && (!IsLastAction(true, GaussRoundPvE, RicochetPvE) && IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE) || !IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE)))
//        {
//            if (RicochetPvE.CanUse(out act, skipAoeCheck: true, usedUp: true))
//                return true;
//        }

//        // Use Gauss
//        if (isGaussMore && (!IsLastAction(true, GaussRoundPvE, RicochetPvE) && IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE) || !IsLastGCD(true, HeatBlastPvE, AutoCrossbowPvE)))
//        {
//            if (GaussRoundPvE.CanUse(out act, usedUp: true))
//                return true;
//        }
//        return base.EmergencyAbility(nextGCD, out act);
//    }

//    // Logic for using attack abilities outside of GCD, focusing on burst windows and cooldown management.
//    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
//    {
//        // Check for not burning Hypercharge below level 52 on AOE
//        bool LowLevelHyperCheck = !AutoCrossbowPvE.EnoughLevel && SpreadShotPvE.CanUse(out _);

//        // If Wildfire is active, use Hypercharge.....Period
//        if (Player.HasStatus(true, StatusID.Wildfire_1946))
//        {
//            return HyperchargePvE.CanUse(out act);
//        }
//        // Burst
//        if (IsBurst)
//        {
//            if (UseBurstMedicine(out act))
//            {
//                return true;
//            }


//            if ((IsLastAbility(false, HyperchargePvE) || Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged))
//                && !CombatElapsedLess(10) && CanUseHyperchargePvE(out _) && !LowLevelHyperCheck && WildfirePvE.CanUse(out act))
//            {
//                return true;
//            }

//        }
//        // Use Hypercharge if at least 12 seconds of combat and (if wildfire will not be up in 30 seconds or if you hit 100 heat)
//        if (!LowLevelHyperCheck && !CombatElapsedLess(12) && !Player.HasStatus(true, StatusID.Reassembled) && (!WildfirePvE.Cooldown.WillHaveOneCharge(30) || Heat == 100))
//        {
//            if (CanUseHyperchargePvE(out act))
//            {
//                return true;
//            }
//        }
//        // Rook Autoturret/Queen Logic
//        if (!IsLastGCD(true, HeatBlastPvE, BlazingShotPvE) && CanUseQueenMeow(out act))
//        {
//            return true;
//        }

//        if (nextGCD.IsTheSameTo(true, CleanShotPvE, AirAnchorPvE, ChainSawPvE, ExcavatorPvE) && Battery == 100)
//        {
//            if (RookAutoturretPvE.CanUse(out act))
//            {
//                return true;
//            }
//        }
//        // Use Barrel Stabilizer on CD if won't cap
//        if (BarrelStabilizerPvE.CanUse(out act))
//        {
//            return true;
//        }

//        return base.AttackAbility(nextGCD, out act);
//    }
//    #endregion

//    #region GCD Logic
//    // Defines the general logic for determining which global cooldown (GCD) action to take.
//    protected override bool GeneralGCD(out IAction? act)
//    {
//        bool inRaids = TerritoryContentType.Equals(TerritoryContentType.Raids);
//        bool hasTinctureBuff = Player.HasStatus(true, StatusID.Medicated);

//        // Checks and executes AutoCrossbow or HeatBlast if conditions are met (overheated state).
//        if (AutoCrossbowPvE.CanUse(out act))
//        {
//            return true;
//        }

//        if (HeatBlastPvE.CanUse(out act, skipComboCheck: true))
//        {
//            return true;
//        }

//        bool experimentalFeatureNoGCDsBeyondThisPoint = ExperimentalFeature && IsOverheated;
//        if (experimentalFeatureNoGCDsBeyondThisPoint) return false;

//        // Executes Bioblaster, and then checks for AirAnchor or HotShot, and Drill based on availability and conditions.
//        if (BioblasterPvE.CanUse(out act))
//        {
//            return true;
//        }

//        // Check if SpreadShot cannot be used
//        if (!SpreadShotPvE.CanUse(out _))
//        {
//            // Check if AirAnchor can be used
//            if (AirAnchorPvE.CanUse(out act))
//            {
//                return true;
//            }

//            // If not at the required level for AirAnchor and HotShot can be used
//            if (!AirAnchorPvE.EnoughLevel && HotShotPvE.CanUse(out act))
//            {
//                return true;
//            }

//            // Check if Drill can be used
//            if (DrillPvE.CanUse(out act))
//            {
//                return true;
//            }

//            if (ExcavatorPvE.CanUse(out act, usedUp: true))
//            {
//                return true;
//            }

//            if (ChainSawPvE.CanUse(out act, usedUp: true))
//            {
//                return true;
//            }

//            if (!CombatElapsedLessGCD(3) && DrillPvE.CanUse(out act, usedUp: true))
//            {
//                return true;
//            }

//            if (Player.HasStatus(true, StatusID.FullMetalMachinist) && FullMetalFieldPvE.CanUse(out act, usedUp: true))
//            {
//                return true;
//            }
//        }

//        // Special condition for using ChainSaw outside of AoE checks if no action is chosen within 4 GCDs.
//        if (!CombatElapsedLessGCD(1) && ChainSawPvE.CanUse(out act, skipAoeCheck: true))
//        {
//            return true;
//        }

//        if (ExcavatorPvE.CanUse(out act, skipAoeCheck: true))
//        {
//            return true;
//        }

//        if (!ChainSawPvE.Cooldown.WillHaveOneCharge(6f) && !CombatElapsedLessGCD(6))
//        {
//            if (DrillPvE.CanUse(out act, usedUp: true))
//            {
//                return true;
//            }
//        }

//        // AoE actions: ChainSaw and SpreadShot based on their usability.
//        if (SpreadShotPvE.CanUse(out _))
//        {
//            if (ChainSawPvE.CanUse(out act))
//            {
//                return true;
//            }

//            if (ExcavatorPvE.CanUse(out act))
//            {
//                return true;
//            }
//        }
//        if (FullMetalFieldPvE.CanUse(out act))
//        {
//            return true;
//        }

//        if (SpreadShotPvE.CanUse(out act))
//        {
//            return true;
//        }

//        // Single target actions: CleanShot, SlugShot, and SplitShot based on their usability.
//        if (CleanShotPvE.CanUse(out act))
//        {
//            return true;
//        }

//        if (SlugShotPvE.CanUse(out act))
//        {
//            return true;
//        }

//        if (SplitShotPvE.CanUse(out act))
//        {
//            return true;
//        }

//        return base.GeneralGCD(out act);
//    }
//    #endregion

//    #region Extra Methods
//    protected override void UpdateInfo()
//    {
//        IsInSecond0GCD();
//        OpenerReady();
//        BurstChecker();
//    }

//    private void BurstChecker()
//    {
//        bool hasWildfire = Player.HasStatus(true, StatusID.Wildfire_1946);
//        InBurst = hasWildfire;
//    }

//    // 1946
//    private void IsInSecond0GCD()
//    {
//        float remainingGCD = DataBased.DefaultGCDRemain;

//        if (remainingGCD >= 0.6f && remainingGCD <= 1.2f)
//        {
//            IsSecond0GCD = true;
//        }
//        else
//        {
//            IsSecond0GCD = false;
//        }
//    }

//    // Logic for Hypercharge
//    private bool CanUseHyperchargePvE(out IAction? act)
//    {
//        if (IsLastGCD(ActionID.FullMetalFieldPvE) && IsLastAbility(ActionID.WildfirePvE) && (Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged)))
//        {
//            return HyperchargePvE.CanUse(out act);
//        }

//        float REST_TIME = 6f;
//        if
//                     //Cannot AOE
//                     (!SpreadShotPvE.CanUse(out _)
//                     &&
//                     // AirAnchor Enough Level % AirAnchor 
//                     (AirAnchorPvE.EnoughLevel && AirAnchorPvE.Cooldown.WillHaveOneCharge(REST_TIME)
//                     ||
//                     // HotShot Charge Detection
//                     !AirAnchorPvE.EnoughLevel && HotShotPvE.EnoughLevel && HotShotPvE.Cooldown.WillHaveOneCharge(REST_TIME)
//                     ||
//                     // Drill Charge Detection
//                     DrillPvE.EnoughLevel && DrillPvE.Cooldown.WillHaveOneCharge(REST_TIME)
//                     ||
//                     // Chainsaw Charge Detection
//                     ChainSawPvE.EnoughLevel && ChainSawPvE.Cooldown.WillHaveOneCharge(REST_TIME)))
//        {
//            act = null;
//            return false;
//        }
//        else
//        {
//            // Use Hypercharge
//            return HyperchargePvE.CanUse(out act);
//        }
//    }

//    private bool CanUseQueenMeow(out IAction? act)
//    {
//        // Define conditions under which the Rook Autoturret/Queen can be used.
//        bool NoQueenLogic = SkipQueenLogic;
//        bool QueenOne = Battery >= 60 && CombatElapsedLess(25f);
//        bool QueenTwo = Battery >= 90 && !CombatElapsedLess(58f) && CombatElapsedLess(78f);
//        bool QueenThree = Battery >= 100 && !CombatElapsedLess(111f) && CombatElapsedLess(131f);
//        bool QueenFour = Battery >= 50 && !CombatElapsedLess(148f) && CombatElapsedLess(168f);
//        bool QueenFive = Battery >= 60 && !CombatElapsedLess(178f) && CombatElapsedLess(198f);
//        bool QueenSix = Battery >= 100 && !CombatElapsedLess(230f) && CombatElapsedLess(250f);
//        bool QueenSeven = Battery >= 50 && !CombatElapsedLess(268f) && CombatElapsedLess(288f);
//        bool QueenEight = Battery >= 70 && !CombatElapsedLess(296f) && CombatElapsedLess(316f);
//        bool QueenNine = Battery >= 100 && !CombatElapsedLess(350f) && CombatElapsedLess(370f);
//        bool QueenTen = Battery >= 50 && !CombatElapsedLess(388f) && CombatElapsedLess(408f);
//        bool QueenEleven = Battery >= 80 && !CombatElapsedLess(416f) && CombatElapsedLess(436f);
//        bool QueenTwelve = Battery >= 100 && !CombatElapsedLess(470f) && CombatElapsedLess(490f);
//        bool QueenThirteen = Battery >= 50 && !CombatElapsedLess(505f) && CombatElapsedLess(525f);
//        bool QueenFourteen = Battery >= 60 && !CombatElapsedLess(538f) && CombatElapsedLess(558f);
//        bool QueenFifteen = Battery >= 100 && !CombatElapsedLess(590f) && CombatElapsedLess(610f);

//        if (NoQueenLogic || QueenOne || QueenTwo || QueenThree || QueenFour || QueenFive || QueenSix || QueenSeven || QueenEight || QueenNine || QueenTen || QueenEleven || QueenTwelve || QueenThirteen || QueenFourteen || QueenFifteen)
//        {
//            if (RookAutoturretPvE.CanUse(out act))
//            {
//                return true;
//            }
//        }
//        act = null;
//        return false;
//    }

//    private bool OpenerReady()
//    {
//        var Lvl100 = Player.Level == 100;
//        bool HasChainSaw = !ChainSawPvE.Cooldown.IsCoolingDown;
//        bool HasAirAnchor = !AirAnchorPvE.Cooldown.IsCoolingDown;
//        var DrillCharges = DrillPvE.Cooldown.CurrentCharges;
//        bool HasDrill = !DrillPvE.Cooldown.IsCoolingDown;
//        bool HasBarrelStabilizer = !BarrelStabilizerPvE.Cooldown.IsCoolingDown;
//        var RCcharges = RicochetPvE.Cooldown.CurrentCharges;
//        bool HasWildfire = !WildfirePvE.Cooldown.IsCoolingDown;
//        var GRcharges = GaussRoundPvE.Cooldown.CurrentCharges;
//        bool ReassembleOneCharge = ReassemblePvE.Cooldown.CurrentCharges >= 1;
//        bool NoHeat = Heat == 0;
//        bool NoBattery = Battery == 0;
//        bool NoResources = NoHeat && NoBattery;
//        bool Openerstep0 = OpenerStep == 0;

//        OpenerAvailable = Lvl100
//                                    && HasChainSaw
//                                    && HasAirAnchor
//                                    && HasDrill
//                                    && DrillCharges == 2
//                                    && HasBarrelStabilizer
//                                    && RCcharges == 3
//                                    && GRcharges == 3
//                                    && HasWildfire
//                                    && ReassembleOneCharge
//                                    && NoResources
//                                    && Openerstep0;
//        return false;
//    }

//    private bool OpenerController(bool lastAction, bool nextAction)
//    {
//        if (lastAction)
//        {
//            OpenerStep++;
//            return false;
//        }
//        return nextAction;
//    }

//    private bool Opener(out IAction? act)
//    {
//        switch (OpenerStep)
//        {
//            case 0:
//                return OpenerController(IsLastGCD(false, AirAnchorPvE), AirAnchorPvE.CanUse(out act));

//            case 1:
//                return OpenerController(IsLastAbility(false, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 2:
//                return OpenerController(IsLastAbility(false, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 3:
//                return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

//            case 4:
//                return OpenerController(IsLastAbility(false, BarrelStabilizerPvE), BarrelStabilizerPvE.CanUse(out act, usedUp: true));

//            case 5:
//                return OpenerController(IsLastGCD(true, ChainSawPvE), ChainSawPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 6:
//                return OpenerController(IsLastGCD(true, ExcavatorPvE), ExcavatorPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 7:
//                return OpenerController(IsLastAbility(false, RookAutoturretPvE), RookAutoturretPvE.CanUse(out act, usedUp: true));

//            case 8:
//                return OpenerController(IsLastAbility(false, ReassemblePvE), ReassemblePvE.CanUse(out act, usedUp: true));

//            case 9:
//                return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

//            case 10:
//                return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 11:
//                return OpenerController(IsLastAbility(false, WildfirePvE), WildfirePvE.CanUse(out act, usedUp: true));

//            case 12:
//                return OpenerController(IsLastGCD(false, FullMetalFieldPvE), HyperchargePvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 13:
//                return OpenerController(IsLastAbility(false, HyperchargePvE), HyperchargePvE.CanUse(out act, usedUp: true));

//            case 14:
//                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 4, HeatBlastPvE.CanUse(out act, usedUp: true));

//            case 15:
//                return OpenerController(IsLastAbility(false, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 16:
//                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 3, HeatBlastPvE.CanUse(out act, usedUp: true));

//            case 17:
//                return OpenerController(IsLastAbility(false, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 18:
//                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 2, HeatBlastPvE.CanUse(out act, usedUp: true));

//            case 19:
//                return OpenerController(IsLastAbility(false, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 20:
//                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 1, HeatBlastPvE.CanUse(out act, usedUp: true));

//            case 21:
//                return OpenerController(IsLastAbility(false, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 22:
//                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 0, HeatBlastPvE.CanUse(out act, usedUp: true));

//            case 23:
//                return OpenerController(IsLastAbility(false, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 24:
//                return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

//            case 25:
//                OpenerHasFinished = true;
//                break;
//        }
//        act = null;
//        return OpenerHasFinishedDummy = false;
//    }

//    private bool ShouldUseBurstMedicine(out IAction? act)
//    {
//        act = null;  // Default to null if Tincture cannot be used.

//        // Don't use Tincture if player has a bad status
//        if (Player.HasStatus(false, StatusID.Weakness) || Player.HasStatus(true, StatusID.Transcendent) || Player.HasStatus(true, StatusID.BrinkOfDeath))
//        {
//            return false;
//        }

//        if (WildfirePvE.Cooldown.RecastTimeRemainOneCharge <= 20 && CombatTime > 60 &&
//            NextAbilityToNextGCD > 1.2 &&
//            !Player.HasStatus(true, StatusID.Weakness) &&
//            DrillPvE.Cooldown.RecastTimeRemainOneCharge < 5 &&
//            AirAnchorPvE.Cooldown.RecastTimeRemainOneCharge < 5)
//        {
//            // Attempt to use Burst Medicine.
//            return UseBurstMedicine(out act, false);
//        }

//        //if (WildfirePvE.Cooldown.RecastTimeRemainOneCharge <= 10
//        //    && !Target.HasStatus(true, StatusID.Wildfire)
//        //    && Target != Player
//        //    && CombatTime > 300
//        //    && !Player.HasStatus(true, StatusID.Weakness)
//        //    && AirAnchorPvE.Cooldown.IsCoolingDown
//        //    && IsLastAbility(ActionID.BarrelStabilizerPvE)
//        //    && DrillPvE.Cooldown.RecastTimeRemainOneCharge < 2.5f)
//        //{
//        //    return UseBurstMedicine(out act, false);
//        //}

//        // If the conditions are not met, return false.
//        return false;
//    }
//    #endregion

//    public unsafe override void DisplayStatus()
//    {
//        float paddingX = ImGui.GetStyle().WindowPadding.X;
//        DisplayStatusHelper.BeginPaddedChild("The CustomRotation's status window", true, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

//        ImGui.Text("Rotation: " + Name + " ");
//        ImGui.SameLine();
//        ImGui.TextDisabled(Description);

//        DisplayStatusHelper.DisplayGCDStatus();

//        //var gameobjectID = DataBase.DisplayPlayerGameObjectId();

//        if (ImGui.Button(nameof(ActionID.PelotonPvE)))
//        {
//            ActionManagerHelper.Instance.InstanceActionManager->UseAction(ActionType.Action, (uint)ActionID.PelotonPvE);
//        }

//        ImGui.Spacing();
//        ImGui.Spacing();

//        ImGui.Text("InBurst: " + InBurst.ToString());

//        ImGui.Spacing();
//        ImGui.Spacing();

//        ImGui.Text("IsSecond0GCD: " + IsSecond0GCD.ToString());
//        ImGui.Text("DefaultGCDRemain" + DataBased.DefaultGCDRemain.ToString());
//        ImGui.Text("OpenerStep: " + OpenerStep.ToString());




//        DisplayStatusHelper.EndPaddedChild();
//    }
//}