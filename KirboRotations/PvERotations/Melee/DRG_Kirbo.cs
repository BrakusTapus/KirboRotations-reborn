//#pragma warning disable CS0618 // Type or member is obsolete

//namespace DefaultRotations.Melee;

//[Rotation("Kirbo's DRG", CombatType.PvE, GameVersion = "7.01")]
//[SourceCode(Path = "main/DefaultRotations/Melee/DRG_Default.cs")]
//[Api(3)]

//public sealed class DRG_Kirbo : DragoonRotation
//{
//    #region Config Options
//    [RotationConfig(CombatType.PvE, Name = "Use Doom Spike for damage uptime if out of melee range even if it breaks combo")]
//    public bool DoomSpikeWhenever { get; set; } = true;
//    #endregion

//    #region Properties
//    private bool StartOpener { get; set; } = false;
//    private bool OpenerHasFinished { get; set; } = false;
//    private bool StartOpenerDummy { get; set; } = true;
//    private bool OpenerAvailable { get; set; } = false;
//    private int Openerstep { get; set; } = 0;

//    #endregion

//    #region oGCD Logic

//    [RotationDesc]
//    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
//    {
//        //if (StartOpenerDummy)
//        //{
//        //    Opener(out act);
//        //}

//        bool inRaids = TerritoryContentType.Equals(TerritoryContentType.Raids);
//        bool inTrials = TerritoryContentType.Equals(TerritoryContentType.Trials);
//        if (Target != Player && Target.HasStatus(true, StatusID.ChaoticSpring) && BattleLitanyPvE.CanUse(out act))
//        {
//            return true;
//        }

//        if (IsBurst && InCombat)
//        {
//            if ((Player.HasStatus(true, StatusID.BattleLitany) || Player.HasStatus(true, StatusID.LanceCharge) || LOTDEndAfter(1000)) && nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE)
//            || (Player.HasStatus(true, StatusID.BattleLitany) && Player.HasStatus(true, StatusID.LanceCharge) && LOTDEndAfter(1000) && nextGCD.IsTheSameTo(true, ChaoticSpringPvE, LanceBarragePvE, WheelingThrustPvE, FangAndClawPvE))
//            || (nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE) && (LanceChargePvE.IsInCooldown || BattleLitanyPvE.IsInCooldown)))
//            {
//                if (LifeSurgePvE.CanUse(out act, usedUp: true)) return true;
//            }

//            if (LanceChargePvE.CanUse(out act)) return true;

//            //if (BattleLitanyPvE.CanUse(out act)) return true;
//        }

//        if (Player.HasStatus(true, StatusID.LanceCharge))
//        {
//            if (GeirskogulPvE.CanUse(out act)) return true;
//        }

//        if (BattleLitanyPvE.EnoughLevel && Player.HasStatus(true, StatusID.BattleLitany) && Player.HasStatus(true, StatusID.LanceCharge)
//            || !BattleLitanyPvE.EnoughLevel && Player.HasStatus(true, StatusID.LanceCharge))
//        {
//            if (DragonfireDivePvE.CanUse(out act)) return true;
//        }

//        if ((Player.HasStatus(true, StatusID.BattleLitany) || Player.HasStatus(true, StatusID.LanceCharge) || LOTDEndAfter(1000))
//            || nextGCD.IsTheSameTo(true, RaidenThrustPvE, DraconianFuryPvE))
//        {
//            if (WyrmwindThrustPvE.CanUse(out act, usedUp: true)) return true;
//        }

//        if (JumpPvE.CanUse(out act)) return true;
//        if (HighJumpPvE.CanUse(out act)) return true;

//        if (StardiverPvE.CanUse(out act)) return true;
//        if (MirageDivePvE.CanUse(out act)) return true;
//        if (NastrondPvE.CanUse(out act)) return true;
//        if (StarcrossPvE.CanUse(out act)) return true;
//        if (RiseOfTheDragonPvE.CanUse(out act)) return true;

//        return base.EmergencyAbility(nextGCD, out act);
//    }
//    #endregion

//    #region Extra 0GCD
//    [RotationDesc(ActionID.WingedGlidePvE)]
//    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
//    {
//        if (WingedGlidePvE.CanUse(out act)) return true;

//        return false;
//    }

//    //[RotationDesc(ActionID.ElusiveJumpPvE)]
//    //protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
//    //{
//    //    if (ElusiveJumpPvE.CanUse(out act)) return true;

//    //    return false;
//    //}

//    [RotationDesc(ActionID.FeintPvE)]
//    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
//    {
//        if (FeintPvE.CanUse(out act)) return true;
//        return false;
//    }
//    #endregion


//    #region GCD Logic
//    protected override bool GeneralGCD(out IAction? act)
//    {
//        //if (StartOpenerDummy)
//        //{
//        //    Opener(out act);
//        //}

//        bool doomSpikeRightNow = DoomSpikeWhenever;

//        if (CoerthanTormentPvE.CanUse(out act)) return true;
//        if (SonicThrustPvE.CanUse(out act, skipStatusProvideCheck: true)) return true;
//        if (DoomSpikePvE.CanUse(out act, skipComboCheck: doomSpikeRightNow)) return true;

//        if (DrakesbanePvE.CanUse(out act)) return true;

//        if (FangAndClawPvE.CanUse(out act)) return true;
//        if (WheelingThrustPvE.CanUse(out act)) return true;

//        if (FullThrustPvE.CanUse(out act)) return true;
//        if (ChaosThrustPvE.CanUse(out act)) return true;

//        if (SpiralBlowPvE.CanUse(out act)) return true;
//        if (DisembowelPvE.CanUse(out act)) return true;
//        if (LanceBarragePvE.CanUse(out act)) return true;
//        if (VorpalThrustPvE.CanUse(out act)) return true;

//        if (RaidenThrustPvE.CanUse(out act)) return true;
//        if (TrueThrustPvE.CanUse(out act)) return true;

//        if (PiercingTalonPvE.CanUse(out act)) return true;

//        return base.GeneralGCD(out act);
//    }
//    #endregion

//    #region Opener

//    private bool Opener(out IAction? act)
//    {
//        switch (Openerstep)
//        {
//            case 0:
//                return OpenerController(IsLastGCD(false, TrueThrustPvE), TrueThrustPvE.CanUse(out act));

//            case 1:
//                return OpenerController(IsLastAbility(false, LanceChargePvE), LanceChargePvE.CanUse(out act, usedUp: true, skipAoeCheck: true));

//            case 2:
//                return OpenerController(IsLastGCD(false, SpiralBlowPvE), SpiralBlowPvE.CanUse(out act, usedUp: true));

//            case 3:
//                return OpenerController(IsLastGCD(false, ChaoticSpringPvE), ChaoticSpringPvE.CanUse(out act, usedUp: true));

//            case 4:
//                return OpenerController(IsLastGCD(false, WheelingThrustPvE), WheelingThrustPvE.CanUse(out act, usedUp: true));

//            case 5:
//                return OpenerController(IsLastGCD(false, DrakesbanePvE), DrakesbanePvE.CanUse(out act, usedUp: true));

//            case 6:
//                return OpenerController(IsLastGCD(false, RaidenThrustPvE), RaidenThrustPvE.CanUse(out act, usedUp: true));

//            case 7:
//                return OpenerController(IsLastGCD(false, LanceBarragePvE), LanceBarragePvE.CanUse(out act, usedUp: true));

//            case 8:
//                return OpenerController(IsLastGCD(false, HeavensThrustPvE), HeavensThrustPvE.CanUse(out act, usedUp: true));

//            case 9:
//                return OpenerController(IsLastGCD(false, FangAndClawPvE), FangAndClawPvE.CanUse(out act, usedUp: true));

//            case 10:
//                return OpenerController(IsLastGCD(false, DrakesbanePvE), DrakesbanePvE.CanUse(out act, usedUp: true));

//            case 11:
//                return OpenerController(IsLastGCD(false, RaidenThrustPvE), RaidenThrustPvE.CanUse(out act, usedUp: true));

//            case 12:
//                return OpenerController(IsLastGCD(false, SpiralBlowPvE), SpiralBlowPvE.CanUse(out act, usedUp: true));

//            case 13:
//                break;
//        }
//        act = null;
//        return OpenerHasFinished = true;
//    }

//    private bool OpenerController(bool lastAction, bool nextAction)
//    {
//        if (lastAction)
//        {
//            Openerstep++;
//            return false;
//        }
//        return nextAction;
//    }

//    private bool OpenerReady()
//    {
//        bool Lvl100 = Player.Level == 100;
//        ushort lifeCharges = LifeSurgePvE.Cooldown.CurrentCharges;
//        bool hasLifeSurgeCharges = lifeCharges == 2;
//        bool hasBattleLitany = !BattleLitanyPvE.Cooldown.IsCoolingDown;
//        bool hasGeirskogul = !GeirskogulPvE.Cooldown.IsCoolingDown;
//        bool hasLanceCharge = !LanceChargePvE.Cooldown.IsCoolingDown;
//        bool hasHighJump = !HighJumpPvE.Cooldown.IsCoolingDown;
//        bool hasDragonfireDive = !DragonfireDivePvE.Cooldown.IsCoolingDown;
//        bool Openerstep0 = Openerstep == 0;

//        OpenerAvailable = Lvl100
//                                    && hasLifeSurgeCharges
//                                    && hasGeirskogul
//                                    && hasBattleLitany
//                                    && hasLanceCharge
//                                    && hasHighJump
//                                    && hasDragonfireDive
//                                    && Openerstep0;
//        return false;
//    }
//    #endregion

//    #region Extra

//    protected override void UpdateInfo()
//    {
//        OpenerReady();
//    }

//    #endregion

//    public unsafe override void DisplayStatus()
//    {
//        if (ImGui.Button("Reset Opener Step"))
//        {
//            Openerstep = 0;
//        }
//        ImGui.Text("StartOpenerDummy: " + StartOpenerDummy.ToString());
//        ImGui.Text("OpenerAvailable: " + OpenerAvailable.ToString());
//        ImGui.Text("Openerstep: " + Openerstep.ToString());
//        ImGui.Text("OpenerHasFinished: " + OpenerHasFinished.ToString());
//    }
//}
