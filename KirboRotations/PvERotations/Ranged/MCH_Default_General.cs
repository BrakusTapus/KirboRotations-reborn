namespace KirboRotations.Ranged;

#pragma warning disable S1066 // Mergeable "if" statements should be combined

[Api(4)]
[Rotation("┏━━━━━━┓\n" +
               "┃    ┃\n" +
               "┗━━━━━━┛",
    CombatType.PvE, 
    GameVersion = "v.", 
    Description = $"┳┻|\n" + 
                $"\r┻┳|\n" +
                $"\r┳┻|_∧               Ｒｏｔａｔｉｏｎ： v...\n" +
                $"\r┻┳|ω･)              \n" +
                $"\r┳┻|⊂ﾉ\n" +
                $"\r┻┳| Ｊ"
    )]

public sealed class MCH_Default_General : MachinistRotation
{
    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Skip Queen Logic and uses Rook Autoturret/Automaton Queen immediately whenever you get 50 battery")]
    public bool SkipQueenLogic { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Use LvL 100 Opener")]
    public bool UseLv100Opener { get; set; } = false;

    private byte HeatStacks
    {
        get
        {
            byte stacks = Player.StatusStack(true, StatusID.Overheated);
            return stacks == byte.MaxValue ? (byte)5 : stacks;
        }
    }
    private bool StartOpener { get; set; } = false;
    private bool OpenerHasFinished { get; set; } = false;
    private bool OpenerHasFinishedDummy { get; set; } = false;
    private bool OpenerAvailable { get; set; } = false;
    private int Openerstep { get; set; } = 0;
    #endregion

    #region Countdown logic
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
        if (remainTime < 3 && UseLv100Opener)
        {
            StartOpener = true;
        }
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region oGCD Logic
    // Determines emergency actions to take based on the next planned GCD action.
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (StartOpener)
        {
            return Opener(out act);
        }

        // Reassemble Logic
        // Check next GCD action and conditions for Reassemble.
        bool isReassembleUsable =
            //Reassemble current # of charges and double proc protection
            ReassemblePvE.Cooldown.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Reassembled) &&
            ((ExcavatorPvE.EnoughLevel && nextGCD.IsTheSameTo(true, ExcavatorPvE)) ||
            //Chainsaw Level Check and NextGCD Check
            (ChainSawPvE.EnoughLevel && nextGCD.IsTheSameTo(true, ChainSawPvE)) ||
            //AirAnchor Logic
            (AirAnchorPvE.EnoughLevel && nextGCD.IsTheSameTo(true, AirAnchorPvE)) ||
            //Drill Logic
            (DrillPvE.EnoughLevel && !ChainSawPvE.EnoughLevel && nextGCD.IsTheSameTo(true, DrillPvE)) ||
            //Cleanshot Logic
            (!DrillPvE.EnoughLevel && CleanShotPvE.EnoughLevel && nextGCD.IsTheSameTo(true, CleanShotPvE)) ||
            //HotShot Logic
            (!CleanShotPvE.EnoughLevel && nextGCD.IsTheSameTo(true, HotShotPvE)));

        // Keeps Ricochet and Gauss cannon Even
        bool isRicochetMore = RicochetPvE.EnoughLevel && GaussRoundPvE.Cooldown.CurrentCharges <= RicochetPvE.Cooldown.CurrentCharges;
        bool isGaussMore = !RicochetPvE.EnoughLevel || GaussRoundPvE.Cooldown.CurrentCharges > RicochetPvE.Cooldown.CurrentCharges;

        bool inRaids = CustomRotation.TerritoryContentType.Equals(TerritoryContentType.Raids);

        if (inRaids && CombatElapsedLessGCD(10) && Battery >= 50 && AutomatonQueenPvE.CanUse(out act, true, true, true, true))
        {
            return true;
        }

        // Attempt to use Reassemble if it's ready
        if (isReassembleUsable)
        {
            if (ReassemblePvE.CanUse(out act, skipComboCheck: true, usedUp: true)) return true;
        }

        // Use Ricochet
        if (isRicochetMore && ((!IsLastAction(true, new[] { GaussRoundPvE, RicochetPvE }) && IsLastGCD(true, new[] { HeatBlastPvE, AutoCrossbowPvE })) || !IsLastGCD(true, new[] { HeatBlastPvE, AutoCrossbowPvE })))
        {
            if (RicochetPvE.CanUse(out act, skipAoeCheck: true, usedUp: true))
                return true;
        }

        // Use Gauss
        if (isGaussMore && ((!IsLastAction(true, new[] { GaussRoundPvE, RicochetPvE }) && IsLastGCD(true, new[] { HeatBlastPvE, AutoCrossbowPvE })) || !IsLastGCD(true, new[] { HeatBlastPvE, AutoCrossbowPvE })))
        {
            if (GaussRoundPvE.CanUse(out act, usedUp: true))
                return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    // Logic for using attack abilities outside of GCD, focusing on burst windows and cooldown management.
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (StartOpener)
        {
            return Opener(out act);
        }

        // Define conditions under which the Rook Autoturret/Queen can be used.
        bool NoQueenLogic = SkipQueenLogic;
        bool OpenerQueen = !CombatElapsedLess(20f) && CombatElapsedLess(25f);
        bool CombatTimeQueen = CombatElapsedLess(60f) && !CombatElapsedLess(45f);
        bool WildfireCooldownQueen = WildfirePvE.Cooldown.IsCoolingDown && WildfirePvE.Cooldown.ElapsedAfter(105f) && Battery == 100 &&
                    (nextGCD.IsTheSameTo(true, AirAnchorPvE) || nextGCD.IsTheSameTo(true, CleanShotPvE) || nextGCD.IsTheSameTo(true, HeatedCleanShotPvE) || nextGCD.IsTheSameTo(true, ChainSawPvE));
        bool BatteryCheckQueen = Battery >= 90 && !WildfirePvE.Cooldown.ElapsedAfter(70f);
        bool LastGCDCheckQueen = Battery >= 80 && !WildfirePvE.Cooldown.ElapsedAfter(77.5f) && IsLastGCD(true, AirAnchorPvE);
        // Check for not burning Hypercharge below level 52 on AOE
        bool LowLevelHyperCheck = !AutoCrossbowPvE.EnoughLevel && SpreadShotPvE.CanUse(out _);

        // If Wildfire is active, use Hypercharge.....Period
        if (Player.HasStatus(true, StatusID.Wildfire_1946))
        {
            return HyperchargePvE.CanUse(out act);
        }
        // Burst
        if (IsBurst)
        {
            if (UseBurstMedicine(out act)) return true;

            {
                if ((IsLastAbility(false, HyperchargePvE) || Heat >= 50 || Player.HasStatus(true, StatusID.Hypercharged)) && !CombatElapsedLess(10) && CanUseHyperchargePvE(out _)
                && !LowLevelHyperCheck && WildfirePvE.CanUse(out act)) return true;
            }
        }
        // Use Hypercharge if at least 12 seconds of combat and (if wildfire will not be up in 30 seconds or if you hit 100 heat)
        if (!LowLevelHyperCheck && !CombatElapsedLess(12) && !Player.HasStatus(true, StatusID.Reassembled) && (!WildfirePvE.Cooldown.WillHaveOneCharge(30) || (Heat == 100)))
        {
            if (CanUseHyperchargePvE(out act)) return true;
        }
        // Rook Autoturret/Queen Logic
        if (NoQueenLogic || OpenerQueen || CombatTimeQueen || WildfireCooldownQueen || BatteryCheckQueen || LastGCDCheckQueen)
        {
            if (RookAutoturretPvE.CanUse(out act)) return true;
        }
        // Use Barrel Stabilizer on CD if won't cap
        if (BarrelStabilizerPvE.CanUse(out act)) return true;

        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    // Defines the general logic for determining which global cooldown (GCD) action to take.
    protected override bool GeneralGCD(out IAction? act)
    {
        if (StartOpener)
        {
            return Opener(out act);
        }

        // Checks and executes AutoCrossbow or HeatBlast if conditions are met (overheated state).
        if (AutoCrossbowPvE.CanUse(out act)) return true;
        if (HeatBlastPvE.CanUse(out act)) return true;

        if (Player.HasStatus(true, StatusID.FullMetalMachinist) && FullMetalFieldPvE.CanUse(out act, usedUp: true))
        {
            return true;
        }

        // Executes Bioblaster, and then checks for AirAnchor or HotShot, and Drill based on availability and conditions.
        if (BioblasterPvE.CanUse(out act)) return true;
        // Check if SpreadShot cannot be used
        if (!SpreadShotPvE.CanUse(out _))
        {
            // Check if AirAnchor can be used
            if (AirAnchorPvE.CanUse(out act)) return true;

            // If not at the required level for AirAnchor and HotShot can be used
            if (!AirAnchorPvE.EnoughLevel && HotShotPvE.CanUse(out act)) return true;

            // Check if Drill can be used
            if (DrillPvE.CanUse(out act, usedUp: true)) return true;

            if (ExcavatorPvE.CanUse(out act, usedUp: true)) return true;

            if (ChainSawPvE.CanUse(out act, usedUp: true)) return true;

            if (FullMetalFieldPvE.CanUse(out act, usedUp: true)) return true;
        }

        // Special condition for using ChainSaw outside of AoE checks if no action is chosen within 4 GCDs.
        if (!CombatElapsedLessGCD(4) && ChainSawPvE.CanUse(out act, skipAoeCheck: true)) return true;
        if (!CombatElapsedLessGCD(4) && ExcavatorPvE.CanUse(out act, skipAoeCheck: true)) return true;

        // AoE actions: ChainSaw and SpreadShot based on their usability.
        if (SpreadShotPvE.CanUse(out _))
        {
            if (FullMetalFieldPvE.CanUse(out act, usedUp: true)) return true;
            if (ChainSawPvE.CanUse(out act)) return true;
            if (ExcavatorPvE.CanUse(out act)) return true;
        }
        if (SpreadShotPvE.CanUse(out act)) return true;

        // Single target actions: CleanShot, SlugShot, and SplitShot based on their usability.
        if (CleanShotPvE.CanUse(out act)) return true;
        if (SlugShotPvE.CanUse(out act)) return true;
        if (SplitShotPvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }
    #endregion

    #region Extra Methods
    protected override void UpdateInfo()
    {
        OpenerReady();
    }

    // Logic for Hypercharge
    private bool CanUseHyperchargePvE(out IAction? act)
    {
        float REST_TIME = 6f;
        if
                     //Cannot AOE
                     ((!SpreadShotPvE.CanUse(out _))
                     &&
                     // AirAnchor Enough Level % AirAnchor 
                     ((AirAnchorPvE.EnoughLevel && AirAnchorPvE.Cooldown.WillHaveOneCharge(REST_TIME))
                     ||
                     // HotShot Charge Detection
                     (!AirAnchorPvE.EnoughLevel && HotShotPvE.EnoughLevel && HotShotPvE.Cooldown.WillHaveOneCharge(REST_TIME))
                     ||
                     // Drill Charge Detection
                     (DrillPvE.EnoughLevel && DrillPvE.Cooldown.WillHaveOneCharge(REST_TIME))
                     ||
                     // Chainsaw Charge Detection
                     (ChainSawPvE.EnoughLevel && ChainSawPvE.Cooldown.WillHaveOneCharge(REST_TIME))))
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
        bool Openerstep0 = Openerstep == 0;

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
                return OpenerController(IsLastGCD(false, AirAnchorPvE), AirAnchorPvE.CanUse(out act));

            case 1:
                return OpenerController(IsLastAbility(false, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 2:
                return OpenerController(IsLastAbility(false, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 3:
                return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

            case 4:
                return OpenerController(IsLastAbility(false, BarrelStabilizerPvE), BarrelStabilizerPvE.CanUse(out act, usedUp: true));

            case 5:
                return OpenerController(IsLastGCD(true, ChainSawPvE), ChainSawPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 6:
                return OpenerController(IsLastGCD(true, ExcavatorPvE), ExcavatorPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 7:
                return OpenerController(IsLastAbility(false, RookAutoturretPvE), RookAutoturretPvE.CanUse(out act, usedUp: true));

            case 8:
                return OpenerController(IsLastAbility(false, ReassemblePvE), ReassemblePvE.CanUse(out act, usedUp: true));

            case 9:
                return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

            case 10:
                return OpenerController(IsLastAbility(true, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 11:
                return OpenerController(IsLastAbility(false, WildfirePvE), WildfirePvE.CanUse(out act, usedUp: true));

            case 12:
                return OpenerController(IsLastGCD(false, FullMetalFieldPvE), HyperchargePvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 13:
                return OpenerController(IsLastAbility(false, HyperchargePvE), HyperchargePvE.CanUse(out act, usedUp: true));

            case 14:
                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 4, HeatBlastPvE.CanUse(out act, usedUp: true));

            case 15:
                return OpenerController(IsLastAbility(false, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 16:
                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 3, HeatBlastPvE.CanUse(out act, usedUp: true));

            case 17:
                return OpenerController(IsLastAbility(false, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 18:
                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 2, HeatBlastPvE.CanUse(out act, usedUp: true));

            case 19:
                return OpenerController(IsLastAbility(false, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 20:
                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 1, HeatBlastPvE.CanUse(out act, usedUp: true));

            case 21:
                return OpenerController(IsLastAbility(false, GaussRoundPvE), GaussRoundPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 22:
                return OpenerController(IsLastGCD(false, HeatBlastPvE) && HeatStacks == 0, HeatBlastPvE.CanUse(out act, usedUp: true));

            case 23:
                return OpenerController(IsLastAbility(false, RicochetPvE), RicochetPvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

            case 24:
                return OpenerController(IsLastGCD(false, DrillPvE), DrillPvE.CanUse(out act, usedUp: true));

            case 25:
                OpenerHasFinished = true;
                break;
        }
        act = null;
        return OpenerHasFinishedDummy = false;
    }
    #endregion
}