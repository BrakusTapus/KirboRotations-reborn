//#pragma warning disable S1066 // Mergeable "if" statements should be combined

//using Dalamud.Interface.Style;

//namespace KirboRotations.UltimateRotations.Ranged;

//[BetaRotation]
//[Rotation("MCH AIO Ultimates", CombatType.PvE, Description = "old ported over Rotation\nProlly sucks and would need A LOT of adjustments", GameVersion = "7.05")]
//[Api(4)]
//public sealed class MCH_AIO : MachinistRotation
//{
//    #region Custom Properties
//    bool InBurst { get; set; }
//    bool WillToolSoon { get; set; }
//    bool InUltimate { get; set; }
//    bool IsNailSmall { get; set; }
//    bool IsNailSmallLowHP { get; set; }
//    bool IsNailBig { get; set; }
//    bool IsTargetLahabrea { get; set; }
//    bool IsTargetMagitekBit { get; set; }
//    bool IsTargetTheUltimaWeapon { get; set; }
//    bool IsTargetJagdDoll { get; set; }
//    bool IsTargetJagdDollLowHP { get; set; }
//    bool IsTargetStrikingDummy { get; set; }
//    static int Phase { get; set; } = 0;
//    #endregion

//    #region Debug Window
//    public override void DisplayStatus()
//    {
//        ImGui.Text($"WillToolSoon: {WillToolSoon}");
//        ImGui.Text($"InBurst: {InBurst}");

//        if (CustomRotation.IsInUCoB || CustomRotation.IsInUwU || CustomRotation.IsInTEA)
//        {
//            if (IsInUCoB)
//            {
//                ImGui.Text($"Unending Coil of Bahamut (Ultimate) | Phase: {Phase}");
//                ImGui.Separator();
//            }

//            if (IsInUwU)
//            {
//                ImGui.Text($"Ultimate weapon (Ultimate) | Phase: {Phase}");
//                ImGui.Separator();
//                if (IsTargetLahabrea)
//                {
//                    ImGui.Text($"Is Target Lahabrea: {IsTargetLahabrea}");
//                }
//                else
//                {
//                    ImGui.Text($"Is Target Lahabrea: {IsTargetLahabrea}");
//                }
//                if (IsTargetMagitekBit)
//                {
//                    ImGui.Text($"Is Target Magitek Bit: {IsTargetMagitekBit}");
//                }
//                else
//                {
//                    ImGui.Text($"Is Target Magitek Bit: {IsTargetMagitekBit}");
//                }
//                if (IsTargetTheUltimaWeapon)
//                {
//                    ImGui.Text($"Is Target The Ultima Weapon: {IsTargetTheUltimaWeapon}");
//                }
//                else
//                {
//                    ImGui.Text($"Is Target The Ultima Weapon: {IsTargetTheUltimaWeapon}");
//                }
//                ImGui.Text($"IsNailSmall: {IsNailSmall}");
//                ImGui.Text($"IsNailSmallLowHP: {IsNailSmallLowHP}");
//                ImGui.Text($"IsNailBig: {IsNailBig}");
//            }
//            if (IsInTEA)
//            {
//                ImGui.Text($"The Epic of Alexander (Ultimate) | Phase: {Phase}");
//                ImGui.Separator();
//            }

//        }
//        else
//        {
//            ImGui.Text($"Not In an Ultimate");
//        }
//    }
//    #endregion

//    #region Count Down Actions
//    protected override IAction? CountDownAction(float remainTime)
//    {
//        if (CustomRotation.IsInUwU || CustomRotation.IsInUCoB || CustomRotation.IsInTEA || CustomRotation.IsInDSR || CustomRotation.IsInTOP)
//        {
//            if (CustomRotation.IsInUCoB)
//            {
//                if (remainTime <= DrillPvE.AnimationLockTime &&
//                    DrillPvE.CanUse(out _))
//                {
//                    return DrillPvE;
//                }
//                IAction act0;
//                if (remainTime <= DrillPvE.AnimationLockTime && UseBurstMedicine(out act0, false))
//                {
//                    return act0;
//                }
//                // Use ReassemblePvE
//                if (remainTime <= 5f && ReassemblePvE.Cooldown.CurrentCharges >= 1)
//                {
//                    return ReassemblePvE;
//                }
//            }
//            if (CustomRotation.IsInUwU)
//            {
//                if (remainTime <= DrillPvE.AnimationLockTime &&
//                    DrillPvE.CanUse(out _))
//                {
//                    return DrillPvE;
//                }
//                IAction act0;
//                if (remainTime <= DrillPvE.AnimationLockTime && UseBurstMedicine(out act0, false))
//                {
//                    return act0;
//                }
//                // Use ReassemblePvE
//                if (remainTime <= 5f && ReassemblePvE.Cooldown.CurrentCharges >= 1)
//                {
//                    return ReassemblePvE;
//                }
//            }
//            if (CustomRotation.IsInTEA)
//            {
//                if (remainTime <= AirAnchorPvE.AnimationLockTime &&
//                    AirAnchorPvE.CanUse(out _))
//                {
//                    return AirAnchorPvE;
//                }
//                IAction act0;
//                if (remainTime <= AirAnchorPvE.AnimationLockTime && UseBurstMedicine(out act0, false))
//                {
//                    return act0;
//                }
//                // Use ReassemblePvE
//                if (remainTime <= 5f && ReassemblePvE.Cooldown.CurrentCharges >= 1)
//                {
//                    return ReassemblePvE;
//                }
//            }
//            if (CustomRotation.IsInDSR)
//            {
//                if (remainTime <= AirAnchorPvE.AnimationLockTime &&
//                    AirAnchorPvE.CanUse(out _))
//                {
//                    return AirAnchorPvE;
//                }
//                IAction act0;
//                if (remainTime <= AirAnchorPvE.AnimationLockTime && UseBurstMedicine(out act0, false))
//                {
//                    return act0;
//                }
//                // Use ReassemblePvE
//                if (remainTime <= 5f && ReassemblePvE.Cooldown.CurrentCharges >= 1)
//                {
//                    return ReassemblePvE;
//                }
//            }
//            if (CustomRotation.IsInTOP)
//            {
//                if (remainTime <= AirAnchorPvE.AnimationLockTime &&
//                    AirAnchorPvE.CanUse(out _))
//                {
//                    return AirAnchorPvE;
//                }
//                IAction act0;
//                if (remainTime <= AirAnchorPvE.AnimationLockTime && UseBurstMedicine(out act0, false))
//                {
//                    return act0;
//                }
//                // Use ReassemblePvE
//                if (remainTime <= 5f && ReassemblePvE.Cooldown.CurrentCharges >= 1)
//                {
//                    return ReassemblePvE;
//                }
//            }

//        }
//        if (Player.Level == 100)
//        {
//            if (remainTime <= AirAnchorPvE.AnimationLockTime + 0.1 &&
//                AirAnchorPvE.CanUse(out _))
//            {
//                return AirAnchorPvE;
//            }
//            IAction act0;
//            if (remainTime <= AirAnchorPvE.AnimationLockTime &&
//                UseBurstMedicine(out act0, false))
//            {
//                return act0;
//            }
//            if (remainTime <= 5f && ReassemblePvE.Cooldown.CurrentCharges == 2)
//            {
//                return ReassemblePvE;
//            }
//        }
//        if (Player.Level < 100)
//        {
//            if (AirAnchorPvE.EnoughLevel && remainTime <= 0.6 + CountDownAhead &&
//                AirAnchorPvE.CanUse(out _))
//            {
//                return AirAnchorPvE;
//            }
//            if (!AirAnchorPvE.EnoughLevel && DrillPvE.EnoughLevel &&
//                remainTime <= 0.6 + CountDownAhead && DrillPvE.CanUse(out _))
//            {
//                return DrillPvE;
//            }
//            if (!AirAnchorPvE.EnoughLevel && !DrillPvE.EnoughLevel
//                && CleanShotPvE.EnoughLevel && remainTime <= 0.6 + CountDownAhead &&
//                CleanShotPvE.CanUse(out _))
//            {
//                return CleanShotPvE;
//            }
//            if (!AirAnchorPvE.EnoughLevel && !DrillPvE.EnoughLevel &&
//                HotShotPvE.EnoughLevel && remainTime <= 0.6 + CountDownAhead &&
//                HotShotPvE.CanUse(out _))
//            {
//                return HotShotPvE;
//            }

//            if (remainTime < 5f && !Player.HasStatus(true, StatusID.Reassembled))
//            {
//                if (Player.Level >= 84 && ReassemblePvE.Cooldown.CurrentCharges > 1)
//                { return ReassemblePvE; }
//                if (Player.Level < 84 && ReassemblePvE.Cooldown.CurrentCharges > 0)
//                { return ReassemblePvE; }
//            }
//        }
//        return base.CountDownAction(remainTime);
//    }
//    #endregion

//    #region GCD
//    protected override bool GeneralGCD(out IAction? act)
//    {
//        act = null;
//        //Overheated
//        if (HeatBlastPvE.CanUse(out act))
//        { return true; }

//        //Air Anchor
//        if (AirAnchorPvE.EnoughLevel)
//        {
//            if (!AirAnchorPvE.Cooldown.IsCoolingDown && !HotShotPvE.Cooldown.IsCoolingDown && AirAnchorPvE.CanUse(out act))
//            { return true; }
//        }

//        //HotShotPvE
//        if (HotShotPvE.EnoughLevel && !AirAnchorPvE.EnoughLevel)
//        {
//            if (!HotShotPvE.Cooldown.IsCoolingDown && HotShotPvE.CanUse(out act))
//            { return true; }
//        }

//        //DrillPvE
//        if (DrillPvE.EnoughLevel && DrillPvE.CanUse(out act) && !IsTargetLahabrea && !IsTargetMagitekBit && !IsTargetJagdDoll && !IsNailSmall)
//        { return true; }

//        //ChainSawPvE
//        if (ChainSawPvE.EnoughLevel && ChainSawPvE.CanUse(out act))
//        { return true; }

//        //Aoe
//        if (SpreadShotPvE.CanUse(out act))
//        { return true; }

//        //Single
//        if (CleanShotPvE.CanUse(out act))
//        { return true; }

//        if (SlugShotPvE.CanUse(out act))
//        { return true; }

//        if (SplitShotPvE.CanUse(out act))
//        { return true; }

//        return base.GeneralGCD(out act);
//    }
//    #endregion

//    #region 0GCD
//    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
//    {
//        act = null;

//        // BarrelStabilizerPvE
//        if (BarrelStabilizerPvE.CanUse(out act))
//        { return true; }

//        // WildfirePvE
//        if (WildfirePvE.CanUse(out act) && WeaponRemain >= 0.58f && WeaponRemain <= 1f)
//        {
//            if (IsTargetMagitekBit || IsTargetLahabrea)
//            { return false; }

//            if (Target.GetHealthRatio() <= 0.10)
//            { return false; }

//            if (Heat >= 45 && (nextGCD == SplitShotPvE || nextGCD == SlugShotPvE || nextGCD == CleanShotPvE))
//            { return true; }

//            if (Heat >= 50 || (CustomRotation.IsLastAbility(ActionID.HyperchargePvE) && OverheatedStacks > 4))
//            { return true; }

//            if (Heat >= 50 && (nextGCD == DrillPvE || nextGCD == AirAnchorPvE || nextGCD == HotShotPvE || nextGCD == ChainSawPvE))
//            { return true; }

//        }

//        //ReassemblePvE
//        if (ReassemblePvE.CanUse(out act, usedUp: true) && ReassemblePvE.Cooldown.CurrentCharges > 0 && CustomRotation.AverageTimeToKill >= 30)
//        {
//            if (IsTargetMagitekBit || IsTargetLahabrea)
//            { return false; }

//            if (Target.GetHealthRatio() <= 0.15)
//            { return false; }

//            if (WildfirePvE.Cooldown.RecastTimeRemainOneCharge > 5 && WildfirePvE.Cooldown.RecastTimeRemainOneCharge <= 20)
//            { return false; }

//            if (CustomRotation.IsInUCoB || CustomRotation.IsInUwU)
//            {
//                if (nextGCD == DrillPvE)
//                { return true; }
//            }

//            if (IsInTEA)
//            {
//                if (nextGCD == DrillPvE || nextGCD == AirAnchorPvE)
//                { return true; }
//            }
//            if (ReassemblePvE.Cooldown.CurrentCharges > 1 || (ReassemblePvE.Cooldown.CurrentCharges == 1 && ReassemblePvE.Cooldown.RecastTimeRemainOneCharge < 20))
//            {
//                if (nextGCD == ChainSawPvE || nextGCD == AirAnchorPvE || nextGCD == DrillPvE)
//                {
//                    return true;
//                }
//            }
//        }

//        // RookAutoturretPvE
//        if (Target.GetHealthRatio() <= 0.15 && RookAutoturretPvE.CanUse(out act))
//        {
//            if (IsTargetMagitekBit || IsTargetLahabrea || IsTargetJagdDoll)
//            { return false; }

//            if (InBurst)
//            { return true; }

//            if (Battery == 90 && (nextGCD == AirAnchorPvE || nextGCD == HotShotPvE))
//            { return true; }

//            if (Battery == 100 && (nextGCD == AirAnchorPvE || nextGCD == CleanShotPvE || nextGCD == HotShotPvE))
//            { return true; }

//            if (RookAutoturretPvE.CanUse(out act) && (double)CustomRotation.HostileTarget.GetHealthRatio() > 0.1 && CustomRotation.HostileTarget.IsTargetable && (Battery == 100 || InBurst))
//            { return true; }
//        }

//        // WildfirePvE
//        /*if (WildfirePvE.CanUse(out act, CanUseOption.OnLastAbility, 0, 0) &&
//            (Heat >= 50 || (CustomRotation.IsLastAbility(ActionID.HyperchargePvE) && HeatStacks > 4) ||
//            (Heat >= 45 && !DrillPvE.WillHaveOneCharge(5f) && !HotShotPvE.WillHaveOneCharge(7.5f))))
//        {
//            if (Target.GetTimeToKill(false) <= 20)
//            {
//                return false;
//            }
//            return true;
//        }*/

//        // HyperchargePvE
//        if (HyperchargePvE.CanUse(out act))
//        {
//            if (Target == null || Target == Player)
//            { return false; }

//            if (IsTargetMagitekBit || IsTargetLahabrea)
//            { return false; }

//            //if (Target.GetTimeToKill(false) <= 20)
//            //{ return false; }

//            if (WillToolSoon)
//            { return false; }

//            if (Target.GetHealthRatio() <= 0.10)
//            { return false; }

//            if (InBurst && WildfirePvE.Cooldown.IsCoolingDown && (StatusHelper.StatusTime(Target, true, StatusID.Wildfire, StatusID.Wildfire_1946) > 8 || StatusHelper.StatusTime(Player, true, StatusID.Wildfire, StatusID.Wildfire_1946) > 8))
//            { return true; }

//            if (IsLastGCD(ActionID.SplitShotPvE, ActionID.SlugShotPvE, ActionID.CleanShotPvE, ActionID.HeatedSplitShotPvE, ActionID.HeatedSlugShotPvE, ActionID.HeatedCleanShotPvE, ActionID.HotShotPvE, ActionID.DrillPvE, ActionID.AirAnchorPvE, ActionID.ChainSawPvE) && !WildfirePvE.Cooldown.WillHaveOneCharge(40f))
//            { return true; }

//            if (IsLastAbility(ActionID.WildfirePvE) && Heat >= 45)
//            { return true; }

//            if (Heat >= 100 && WildfirePvE.Cooldown.WillHaveOneCharge(10f))
//            { return true; }

//            if (Heat >= 100 && WildfirePvE.Cooldown.WillHaveOneCharge(40f))
//            { return true; }

//            if (Heat >= 50 && !WildfirePvE.Cooldown.WillHaveOneCharge(40f))
//            { return true; }

//            return false;
//        }

//        // Gaussround and RicochetPvE
//        if (ShouldUseGaussroundOrRicochet(out act))
//        {
//            return true;
//        }

//        return base.EmergencyAbility(nextGCD, out act);
//    }
//    #endregion

//    #region Info Updater
//    protected override void UpdateInfo()
//    {
//        CheckAndUpdatePhase();
//        UwUChecker();
//        TEAChecker();
//    }
//    #endregion

//    #region WildfirePvE Decider (inactive)
//#pragma warning disable S1144 // Unused private types or members should be removed
//    private bool ShouldUseWildfireAbility(IAction nextGCD, out IAction? act)
//    {
//        act = null;

//        if (WildfirePvE.CanUse(out act, true) || CustomRotation.Target.IsBossFromTTK() || CustomRotation.Target.IsBossFromIcon())
//        {
//#pragma warning disable S1066 // Mergeable "if" statements should be combined
//            if ((nextGCD == ChainSawPvE && Heat >= 50)
//                || (IsLastAbility(ActionID.HyperchargePvE) && !IsLastGCD(ActionID.HeatBlastPvE) && MachinistRotation.OverheatedStacks > 4)
//                || (Heat >= 45 && !DrillPvE.Cooldown.WillHaveOneCharge(5) && !AirAnchorPvE.Cooldown.WillHaveOneCharge(5f) && !ChainSawPvE.Cooldown.WillHaveOneCharge(5f)))
//            {
//                return WildfirePvE.CanUse(out act);
//            }
//        }

//        return false;
//    }
//    #endregion

//    #region Gauss Round or RicochetPvE decider
//    private bool ShouldUseGaussroundOrRicochet(out IAction? act)
//    {
//        act = null;  // Initialize the action as null.

//        // First, check if both GaussRoundPvE and RicochetPvE do not have at least one
//        // charge. If neither has a charge, we cannot use either, so return false.
//        if (!GaussRoundPvE.Cooldown.HasOneCharge && !RicochetPvE.Cooldown.HasOneCharge)
//        {
//            return false;
//        }

//        if (!GaussRoundPvE.Cooldown.HasOneCharge && !RicochetPvE.EnoughLevel)
//        {
//            return false;
//        }

//        // Second, check if RicochetPvE is not at a sufficient level to be used.
//        // If not, default to GaussRoundPvE (if it can be used).
//        if (!RicochetPvE.EnoughLevel)
//        {
//            return GaussRoundPvE.CanUse(out act, usedUp: true);
//        }

//        if (GaussRoundPvE.Cooldown.CurrentCharges == RicochetPvE.Cooldown.RecastTimeRemainOneCharge)
//        {
//            if (GaussRoundPvE.Cooldown.RecastTimeRemainOneCharge <= RicochetPvE.Cooldown.RecastTimeRemainOneCharge)
//            {
//                return GaussRoundPvE.CanUse(out act, usedUp: true);
//            }
//            else if (GaussRoundPvE.Cooldown.RecastTimeRemainOneCharge >= RicochetPvE.Cooldown.RecastTimeRemainOneCharge)
//            {
//                return RicochetPvE.CanUse(out act, usedUp: true);
//            }
//        }

//        // Third, check if GaussRoundPvE and RicochetPvE have the same number of charges.
//        // If they do, prefer using GaussRoundPvE.
//        if (GaussRoundPvE.Cooldown.CurrentCharges >= RicochetPvE.Cooldown.CurrentCharges)
//        {
//            return GaussRoundPvE.CanUse(out act, usedUp: true);
//        }

//        // Fourth, check if RicochetPvE has more or an equal number of charges compared
//        // to GaussRoundPvE. If so, prefer using RicochetPvE.
//        if (RicochetPvE.Cooldown.CurrentCharges >= GaussRoundPvE.Cooldown.CurrentCharges)
//        {
//            return RicochetPvE.Cooldown.HasOneCharge &&
//                   RicochetPvE.CanUse(out act, usedUp: true);
//        }
//        // If none of the above conditions are met, default to using GaussRoundPvE.
//        // This is a fallback in case other conditions fail to determine a clear
//        // action.
//        return GaussRoundPvE.CanUse(out act, usedUp: true);
//    }
//    #endregion

//    #region Phase Updater
//    public static void CheckAndUpdatePhase()
//    {
//        // Define target names for each phase
//        string UCoBPhase1Name = "Twintania";
//        string UCoBPhase2Name = "Nael";
//        string UCoBPhase3Name = "Bahamut";

//        // Define target names for each phase
//        string UWUPhase1Name = "Garuda";
//        string UWUPhase2Name = "Ifrit";
//        string UWUPhase3Name = "Titan";
//        string UWUPhase4Name = "Lahabrea";
//        string UWUPhase4Name2= "Magitek Bit";
//        string UWUPhase5Name = "The Ultima Weapon";

//        // Define target names for each phase
//        //string TEAPhase1Name = "Living Liquid";
//        //string TEAPhase2Name = "Nael";
//        //string TEAPhase3Name = "Bahamut";
//        //string TEAPhase4Name = "Bahamut";

//        if (IsInUwU)
//        {
//            foreach (var obj in CustomRotation.AllHostileTargets)
//            {
//                // Check if the battleChar is targetable and matches any of the target names
//                if (obj.IsTargetable)
//                {
//                    if (obj.Name.ToString() == UWUPhase1Name)
//                    {
//                        Phase = 1;
//                        return;
//                    }
//                    if (obj.Name.ToString() == UWUPhase2Name && Phase == 1)
//                    {
//                        Phase = 2;
//                        return;
//                    }
//                    if (obj.Name.ToString() == UWUPhase3Name && Phase == 2)
//                    {
//                        Phase = 3;
//                        return;
//                    }
//                    if ((obj.Name.ToString() == UWUPhase4Name || obj.Name.ToString() == UWUPhase4Name2) && Phase == 3)
//                    {
//                        Phase = 4;
//                        return;
//                    }
//                    if (obj.Name.ToString() == UWUPhase5Name && Phase == 4)
//                    {
//                        Phase = 5;
//                        return;
//                    }
//                }
//            }
//        }

//        if (IsInUCoB)
//        {
//            foreach (var obj in CustomRotation.AllHostileTargets)
//            {
//                // Check if the battleChar is targetable and matches any of the target names
//                if (obj.IsTargetable)
//                {
//                    if (obj.Name.ToString() == UCoBPhase1Name && CombatTime < 150)
//                    {
//                        Phase = 1;
//                        return;
//                    }
//                    if (obj.Name.ToString() == UCoBPhase2Name && CombatTime < 300)
//                    {
//                        Phase = 2;
//                        return;
//                    }
//                    if (obj.Name.ToString() == UCoBPhase3Name && CombatTime < 450)
//                    {
//                        Phase = 3;
//                        return;
//                    }
//                    if (obj.Name.ToString() == UCoBPhase1Name && obj.Name.ToString() == UCoBPhase2Name)
//                    {
//                        Phase = 4;
//                        return;
//                    }
//                    if (obj.Name.ToString() == UCoBPhase3Name && CombatTime > 450)
//                    {
//                        Phase = 5;
//                        return;
//                    }
//                }
//            }
//        }

//        /* if (IsInTEA)
//        {
//            foreach (var obj in DataCenter.AllTargets)
//            {
//                // Check if the battleChar is targetable and matches any of the target names
//                if (obj.IsTargetable)
//                {
//                    if (obj.Name.ToString() == TEAPhase1Name)
//                    {
//                        Phase = 1;
//                        return;
//                    }
//                    if (obj.Name.ToString() == TEAPhase2Name && Phase == 1)
//                    {
//                        Phase = 2;
//                        return;
//                    }
//                    if (obj.Name.ToString() == TEAPhase3Name && Phase == 2)
//                    {
//                        Phase = 3;
//                        return;
//                    }
//                    if (obj.Name.ToString() == TEAPhase4Name && Phase == 3)
//                    {
//                        Phase = 4;
//                        return;
//                    }
//                }
//            }
//        }
//        */
//    }
//    #endregion

//    #region UwU checker
//    private void UwUChecker()
//    {
//        bool isNailSmall = Target.Name.ToString() == "Infernal Nail" && !Target.HasStatus(false, StatusID.VulnerabilityDown_1545);
//        bool isNailSmallLowHP = Target.Name.ToString() == "Infernal Nail" && !Target.HasStatus(false, StatusID.VulnerabilityDown_1545) && Target.CurrentHp < 13435 && IsInUwU;
//        bool isNailBig = Target.Name.ToString() == "Infernal Nail" && Target.HasStatus(false, StatusID.VulnerabilityDown_1545);
//        bool isTargetLahabrea = Target.Name.ToString() == "Lahabrea" && IsInUwU;
//        bool isTargetMagitekBit = Target.Name.ToString() == "Magitek Bit" && IsInUwU;
//        bool isTargetTheUltimaWeapon = Target.Name.ToString() == "The Ultima Weapon" && IsInUwU;

//        IsNailSmall = isNailSmall;
//        IsNailSmallLowHP = isNailSmallLowHP;
//        IsNailBig = isNailBig;
//        IsTargetLahabrea = isTargetLahabrea;
//        IsTargetMagitekBit = isTargetMagitekBit;
//        IsTargetTheUltimaWeapon = isTargetTheUltimaWeapon;
//    }
//    #endregion

//    #region TEA checker
//    private void TEAChecker()
//    {
//        bool isTargetJagdDollLowHP = Target.Name.ToString() == "Jagd Doll" && IsInTEA && Target.GetHealthRatio() < 0.25;
//        bool isTargetJagdDoll = Target.Name.ToString() == "Jagd Doll" && IsInTEA;

//        IsTargetJagdDollLowHP = isTargetJagdDollLowHP;
//        IsTargetJagdDoll = isTargetJagdDoll;
//    }
//    #endregion

//    #region Test checker
//    private void TestChecker()
//    {
//        bool isTargetStrikingDummy = Target.Name.ToString() == "Striking Dummy";
//        bool inUltimate = CustomRotation.IsInUCoB || CustomRotation.IsInUwU || CustomRotation.IsInTEA || CustomRotation.IsInDSR || CustomRotation.IsInTOP;

//        IsTargetStrikingDummy = isTargetStrikingDummy;
//        InUltimate = inUltimate;
//    }
//    #endregion
//}
