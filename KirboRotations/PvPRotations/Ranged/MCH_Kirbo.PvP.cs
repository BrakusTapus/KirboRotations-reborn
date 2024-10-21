#pragma warning disable CS0618 // Type or member is obsolete
using KirboRotations.Helpers;

namespace KirboRotations.Ranged;


[BetaRotation]
[Rotation("Kirbo PvP", CombatType.PvP, GameVersion = "7.06", Description = "Kirbo's Beta Rotation for MCH\nUses LB\nUses Turret")]
[Api(4)]
internal class MCH_TESTERPvE : MachinistRotation
{
    private byte PvP_HeatStacks
    {
        get
        {
            byte pvp_heatstacks = Player.StatusStack(true, StatusID.Heat);
            return pvp_heatstacks == byte.MaxValue ? (byte)5 : pvp_heatstacks;
        }
    }

    private bool IsPvPOverheated => Player.HasStatus(true, StatusID.Overheated_3149);
    private float OverheatedStatusTime => Player.StatusTime(true, StatusID.Overheated_3149);

    private static IBaseAction MarksmansSpitePvP { get; } = new BaseAction((ActionID)29415);

    #region IBaseActions
    // Enemies with our PvE_Wildfire Take highest priority, if none falls back to lowest HP in range
    //private new static IBaseAction BlastChargePvP { get; } = new BaseAction(ActionID.BlastChargePvP)
    //{
    //    ChoiceTarget = (Targets, mustUse) =>
    //    {
    //        // First, prioritize targets with the PvP_WildfireDebuff
    //        var targetWithWildfire = Targets.FirstOrDefault(b => b.HasStatus(true, StatusID.Wildfire_1323));
    //        if (targetWithWildfire != null)
    //        {
    //            return targetWithWildfire;
    //        }

    //        // If no target with PvP_WildfireDebuff, use existing logic
    //        Targets = Targets.Where(b => b.YalmDistanceX < 25 &&
    //        !b.HasPvPInvuln() &&
    //        !b.HasStatus(false, (StatusID)1240, (StatusID)1308, (StatusID)2861, (StatusID)3255, (StatusID)3054, (StatusID)3054, (StatusID)3039, (StatusID)1312)).ToArray();
    //        if (Targets.Any())
    //        {
    //            return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
    //        }
    //        return null;
    //    },
    //    ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.Guard),
    //};
    //
    //      // Wont use drill if overheated
    //    private new static IBaseAction DrillPvP { get; } = new BaseAction(ActionID.DrillPvP)
    //    {
    //        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.PvP_Overheated),
    //        StatusNeed = new StatusID[1] { StatusID.DrillPrimed },
    //        StatusProvide = new StatusID[1] { StatusID.BioblasterPvPPrimed },
    //    };
    //
    //    // Will try and switch to a target when in 12yalm range
    //    private new static IBaseAction BioblasterPvP { get; } = new BaseAction(ActionID.BioblasterPvP)
    //    {
    //        ChoiceTarget = (Targets, mustUseEmpty) =>
    //        {
    //            Targets = Targets.Where(b => b.YalmDistanceX <= 12);
    //            if (Targets.Any())
    //            {
    //                return Targets.OrderBy(ObjectHelper.GetHealthRatio).First();
    //            }
    //            return null;
    //        },
    //        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !Player.HasStatus(true, StatusID.PvP_Guard),
    //        StatusNeed = new StatusID[1] { StatusID.BioblasterPvPPrimed },
    //        StatusProvide = new StatusID[1] { StatusID.AirAnchorPvPPrimed },
    //    };
    //
    //    // Wont use AirAnchorPvP if overheated and if target has Guard active
    //    private new static IBaseAction AirAnchorPvP { get; } = new BaseAction(ActionID.AirAnchorPvP)
    //    {
    //        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !Player.HasStatus(true, StatusID.PvP_Guard),
    //        StatusNeed = new StatusID[1] { StatusID.AirAnchorPvPPrimed },
    //        StatusProvide = new StatusID[1] { StatusID.ChainSawPvPPrimed },
    //    };
    //
    //    // Wont use ChainSawPvP if overheated and if target has Guard active
    //    private new static IBaseAction ChainSawPvP { get; } = new BaseAction(ActionID.ChainSawPvP)
    //    {
    //        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !Player.HasStatus(true, StatusID.PvP_Guard),
    //        StatusNeed = new StatusID[1] { StatusID.ChainSawPvPPrimed },
    //        StatusProvide = new StatusID[1] { StatusID.DrillPvPPrimed },
    //        ChoiceTarget = (Targets, mustUse) =>
    //        {
    //            // Filter targets based on health ratio
    //            var suitableTargets = Targets.Where(b => b.GetHealthRatio() <= 0.55).ToArray();
    //
    //            if (suitableTargets.Any())
    //            {
    //                // Optionally, you can add more logic here to choose the best target
    //                // For now, it's selecting the first suitable target found
    //                return suitableTargets.First();
    //            }
    //
    //            return null;
    //        }
    //    };
    //
    //    // Wont use ScattergunPvP if overheated and if target has Guard active
    //    private new static IBaseAction ScattergunPvP { get; } = new BaseAction(ActionID.ScattergunPvP)
    //    {
    //        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.PvP_Overheated) && !Player.HasStatus(true, StatusID.PvP_Guard),
    //    };
    //
    //    // Picks a target who does not have either invulns nor any Mits picks lowest HP target
    //    private new static IBaseAction PvP_MarksmansSpite { get; } = new BaseAction(ActionID.PvP_MarksmansSpite, ActionOption.Attack | ActionOption.RealGCD)
    //    {
    //        ChoiceTarget = (Targets, mustUse) =>
    //        {
    //            // Define HP thresholds for each role
    //            const float rangedMagicalHpThreshold = 30000; // CurrentHp
    //            const float rangedPhysicalHpThreshold = 30000; // CurrentHp
    //            const float healerHpThreshold = 30000; // CurrentHp
    //            const float tankHpThreshold = 15000; // CurrentHp
    //            const float tooLowHpThreshold = 5000; // CurrentHp
    //
    //            // Filter and prioritize targets based on role and HP
    //            var healerTarget = Targets.GetJobCategory(JobRole.Healer)
    //            .Where(b => b.CurrentHp < healerHpThreshold && b.CurrentHp > tooLowHpThreshold)
    //            .OrderBy(ObjectHelper.GetHealthRatio)
    //            .FirstOrDefault();
    //            if (healerTarget != null)
    //            {
    //                return healerTarget;
    //            }
    //
    //            var mageTarget = Targets.GetJobCategory(JobRole.RangedMagical)
    //            .Where(b => b.CurrentHp < rangedMagicalHpThreshold && b.CurrentHp > tooLowHpThreshold)
    //            .OrderBy(ObjectHelper.GetHealthRatio)
    //            .FirstOrDefault();
    //            if (mageTarget != null)
    //            {
    //                return mageTarget;
    //            }
    //
    //            var rangeTarget = Targets.GetJobCategory(JobRole.RangedPhysical)
    //            .Where(b => b.CurrentHp < rangedPhysicalHpThreshold && b.CurrentHp > tooLowHpThreshold)
    //            .OrderBy(ObjectHelper.GetHealthRatio)
    //            .FirstOrDefault();
    //            if (rangeTarget != null)
    //            {
    //                return rangeTarget;
    //            }
    //
    //            var tankTarget = Targets.GetJobCategory(JobRole.Tank)
    //            .Where(b => b.CurrentHp < tankHpThreshold && b.CurrentHp > tooLowHpThreshold)
    //            .OrderBy(ObjectHelper.GetHealthRatio)
    //            .FirstOrDefault();
    //            return tankTarget ?? null;
    //        },
    //        ActionCheck = (b, m) => LimitBreakLevel >= 1
    //    };
    //
    //    // Will pick a Target with around 85% hp or less left
    //    private new static IBaseAction PvP_Wildfire { get; } = new BaseAction(ActionID.PvP_Wildfire, ActionOption.Attack)
    //    {
    //        ChoiceTarget = (Targets, mustUse) =>
    //        {
    //            // target the closest enemy with HP below a certain threshold (e.g., 85%)
    //            var fallbackTarget = Targets
    //            .Where(b => b.YalmDistanceX < 20 && b.GetHealthRatio() < 0.85)
    //            .OrderBy(b => b.YalmDistanceX)
    //            .FirstOrDefault();
    //
    //            return fallbackTarget;
    //        },
    //        ActionCheck = (b, m) => Player.HasStatus(true, StatusID.PvP_Overheated) && !Player.HasStatus(true, StatusID.PvP_Guard),
    //    };
    //
    //    // sample text
    //    private new static IBaseAction AnalysisPvP { get; } = new BaseAction(ActionID.AnalysisPvP, ActionOption.Friendly)
    //    {
    //        StatusProvide = new StatusID[1] { StatusID.AnalysisPvP },
    //        ActionCheck = (b, m) => !Player.HasStatus(true, StatusID.AnalysisPvP) && HasHostilesInRange && AnalysisPvP.Cooldown.CurrentCharges > 0,
    //    };
    //
    //    // Will try and determine the best AoE target based on how many targets are present
    //    private new static IBaseAction PvP_BishopAutoTurret { get; } = new BaseAction(ActionID.PvP_BishopAutoTurret, ActionOption.Attack)
    //    {
    //        ActionCheck = (b, m) => HasHostilesInRange && !Player.HasStatus(true, StatusID.PvP_Guard),
    //        ChoiceTarget = (Targets, mustUse) =>
    //        {
    //            // Combine party members and hostile targets into a single collection
    //            var combinedTargets = PartyMembers.Concat(HostileTargets);
    //
    //            // Filter targets to those within a reasonable range (e.g., 15 yalms)
    //            combinedTargets = combinedTargets.Where(b => b.YalmDistanceX < 15).ToArray();
    //
    //            // Select the target that has the highest number of nearby players
    //            BattleChara bestTarget = null;
    //            int maxNearbyPlayers = 0;
    //            foreach (var target in combinedTargets)
    //            {
    //                int nearbyPlayersCount = combinedTargets.Count(b => b.YalmDistanceX - target.YalmDistanceX < 5); // 5 yalms as an example radius
    //                if (nearbyPlayersCount > maxNearbyPlayers)
    //                {
    //                    maxNearbyPlayers = nearbyPlayersCount;
    //                    bestTarget = target;
    //                }
    //            }
    //
    //            // Return the target that maximizes the AoE effect, or null if no suitable target
    //            return bestTarget;
    //        }
    //    };
    #endregion IBaseActions

    #region Rotation Config
    [RotationConfig(CombatType.PvP, Name = "Emergency Healing")]
    public bool EmergencyHealing { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "Recuperate")]
    public int Recuperate { get; set; } = 37500;

    [RotationConfig(CombatType.PvP, Name = "Guard")]
    public int Guard { get; set; } = 27500;

    [RotationConfig(CombatType.PvP, Name = "LowHPThreshold")]
    public int LowHPThreshold { get; set; } = 20000;

    [RotationConfig(CombatType.PvP, Name = "AnalysisOnDrill")]
    public bool AnalysisOnDrill { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "AnalysisOnAirAnchor")]
    public bool AnalysisOnAirAnchor { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "AnalysisOnBioBlaster")]
    public bool AnalysisOnBioBlaster { get; set; } = false;

    [RotationConfig(CombatType.PvP, Name = "AnalysisOnChainsaw")]
    public bool AnalysisOnChainsaw { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "GuardCancel")]
    public bool GuardCancel { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "PreventActionWaste")]
    public bool PreventActionWaste { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "SafetyCheck")]
    public bool SafetyCheck { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "DrillOnGuard")]
    public bool DrillOnGuard { get; set; } = true;

    [RotationConfig(CombatType.PvP, Name = "LowHPNoBlastCharge")]
    public bool LowHPNoBlastCharge { get; set; } = true;
    #endregion Rotation Config

    #region Status Display
    public override void DisplayStatus()
    {
        ImGui.TextWrapped("LimitBreakLevel: " + CustomRotationEx.CurrentLimitBreakLevel);
        ImGuiToolTipsKirbo.HoveredTooltip("CurrentUnits: " + CustomRotationEx.CurrentCurrentUnits);

        ImGui.TextWrapped("HeatStacks: " + PvP_HeatStacks);
        ImGui.TextWrapped("IsPvPOverheated: " + IsPvPOverheated);
        ImGui.TextWrapped("Overheated StatusTime: " + Player.StatusTime(true, StatusID.Overheated_3149).ToString("F1"));


        ImGui.TextWrapped("BlastChargePvP Target: " + BlastChargePvP.Target.Target?.ToString());
        ImGui.TextWrapped("BishopAutoturretPvP Target: " + BishopAutoturretPvP.Target.Target?.ToString());
        ImGui.TextWrapped("BioblasterPvP Target: " + BioblasterPvP.Target.Target?.ToString());
        ImGui.TextWrapped("Distance" + Target.DistanceToPlayer().ToString() + "y");
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        act = null;
        if (EmergencyHealing && EmergencyLowHP(out act))
        {
            return true;
        }
        if (MarksmansSpitePvP.CanUse(out act) && CustomRotationEx.CurrentLimitBreakLevel == 1 && !Target.HasStatus(false, StatusID.Guard))
        {
            //if (CurrentTarget.Name.ToString() == "Striking Dummy") return true;
            //if (CurrentTarget == Player || CurrentTarget == null) return false;
            //if (CurrentTarget.HasStatus(false, StatusID.Guard)) return false;
            //if (CurrentTarget.CurrentHp == CurrentTarget.MaxHp) return false;
            // OverheatedStatusTime
            if (Target.HasStatus(true, StatusID.Wildfire_1323) && IsPvPOverheated && OverheatedStatusTime < 1 && CurrentTarget.CurrentHp <= 50000) return true;
            if (Target.HasStatus(false, StatusID.Mortared) && CurrentTarget.CurrentHp <= 39600) return true;
            if (CurrentTarget.IsJobCategory(JobRole.Healer) && CurrentTarget.CurrentHp >= 15000 && CurrentTarget.CurrentHp <= 36000) return true;
            if (CurrentTarget.IsJobCategory(JobRole.RangedMagical) && CurrentTarget.CurrentHp >= 15000 && CurrentTarget.CurrentHp <= 36000) return true;
            if (CurrentTarget.IsJobCategory(JobRole.RangedPhysical) && CurrentTarget.CurrentHp >= 15000 && CurrentTarget.CurrentHp <= 36000) return true;
            if (CurrentTarget.IsJobCategory(JobRole.Melee) && CurrentTarget.CurrentHp >= 10000 && CurrentTarget.CurrentHp <= 25000) return true;
            if (CurrentTarget.IsJobCategory(JobRole.Tank) && CurrentTarget.CurrentHp >= 10000 && CurrentTarget.CurrentHp <= 20000) return true;

        }

        // Status checks
        bool targetIsNotPlayer = Target != Player;
        bool playerHasGuard = Player.HasStatus(true, StatusID.Guard);
        bool targetHasGuard = Target.HasStatus(false, StatusID.Guard) && targetIsNotPlayer;
        bool hasChiten = Target.HasStatus(false, StatusID.Chiten) && targetIsNotPlayer;
        bool hasHallowedGround = Target.HasStatus(false, StatusID.HallowedGround_1302) && targetIsNotPlayer;
        bool hasUndeadRedemption = Target.HasStatus(false, StatusID.UndeadRedemption) && targetIsNotPlayer;

        // Configurations checks
        int guardthreshold = Guard;
        bool guardCancel = GuardCancel;
        bool lowHPNoBlastCharge = LowHPNoBlastCharge;
        int lowHPThreshold = LowHPThreshold;
        bool preventActionWaste = PreventActionWaste;
        bool safetyCheck = SafetyCheck;
        bool drillOnGuard = DrillOnGuard;

        // Should prevent any actions if the option 'guardCancel' is enabled and Player has the Guard buff up
        if (guardCancel && Player.HasStatus(true, StatusID.Guard))
        {
            return false;
        }

        if (NumberOfHostilesInRange > 0 && !Player.HasStatus(true, StatusID.Analysis) && AnalysisPvP.CanUse(out act, usedUp: true) && Player.HasStatus(true, StatusID.DrillPrimed) && DrillPvP.CanUse(out _))
        {
            return AnalysisPvP.CanUse(out act, usedUp: true);
        }

        // A Analysis buffed PvE_Drill should be used if target has Guard
        if (drillOnGuard && targetHasGuard && DrillPvP.CanUse(out act, usedUp: true) && Player.HasStatus(true, StatusID.DrillPrimed) && !Player.HasStatus(true, StatusID.Analysis))
        {
            if (AnalysisPvP.Cooldown.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Analysis))
            {
                return AnalysisPvP.CanUse(out act, usedUp: true);
            }
            if (Player.HasStatus(true, StatusID.Analysis))
            {
                return DrillPvP.CanUse(out act, usedUp: true);
            }
        }

        // At the moment just prevents attacks if a SAM has Chiten
        // Note: Try to add cast checks for various enemy LB's
        if (safetyCheck && hasChiten)
        {
            return false;
        }

        // Prevent action Waste aims to not use vital skills on targets with invulns
        // Note: currently just blocks attacks, consider just blocking valueable skills and allowing Basic attacks
        if (preventActionWaste && (targetHasGuard || hasHallowedGround || hasUndeadRedemption))
        {
            return false;
        }

        // Marks Man should already be taking into invulns into account
        //bool toolow = Target.CurrentHp < 5000 ;
        //bool toomuch = Target.CurrentHp >= 25000;
        //bool oktouse = !toolow && !toomuch;
        //if (oktouse && !IsPvPOverheated && MarksmansSpitePvP.CanUse(out act) && CustomRotationEx.CurrentLimitBreakLevel == 1)
        //{
        //    return true;
        //}

        // When PvE_Drill can be used we first check if we can buff it with analysis
        // Note: PvE_Drill should always be buffed tbh
        if (DrillPvP.CanUse(out act, usedUp: true) && Target != Player && !IsPvPOverheated && Player.HasStatus(true, StatusID.DrillPrimed))
        {
            if (AnalysisPvP.Cooldown.CurrentCharges > 0 && !Player.HasStatus(true, StatusID.Analysis))
            {
                return AnalysisPvP.CanUse(out act, usedUp: true);
            }
            if (Player.HasStatus(true, StatusID.Analysis))
            {
                return true;
            }
            if (AnalysisPvP.Cooldown.CurrentCharges == 0)
            {
                return true;
            }
        }

        // Uses BioBlaster automatically when a Target is in range
        if (!IsPvPOverheated && BioblasterPvP.CanUse(out act, usedUp: true) && Target != Player && Player.HasStatus(true, StatusID.BioblasterPrimed) && Target.DistanceToPlayer() < 12)
        {
            return true;
        }

        // Air Anchor is used if Player is not overheated and available
        if (!IsPvPOverheated && AirAnchorPvP.CanUse(out act, usedUp: true) && Player.HasStatus(true, StatusID.AirAnchorPrimed))
        {
            return true;
        }

        // Chain Saw is used if Player is not overheated and available
        // Note: Analysis will be used to buff Chain Saw if Target has around half of their HP
        if (!IsPvPOverheated && ChainSawPvP.CanUse(out act, usedUp: true, skipAoeCheck: true) && Player.HasStatus(true, StatusID.ChainSawPrimed))
        {
            return true;
        }

        // Scattergun is used if Player is not overheated and available
        if (!IsPvPOverheated && ScattergunPvP.CanUse(out act, usedUp: true, skipAoeCheck: true) && ScattergunPvP.Target.Target.DistanceToPlayer() <= 3)
        {
            return true;
        }

        // Blast Charge is used if available
        // Note: Stop Using Blast Charge if Player's HP is low + moving + not overheated (since our movement slows down a lot we do this to be able retreat)
        if (BlastChargePvP.CanUse(out act, skipCastingCheck: true))
        {
            if (guardCancel && playerHasGuard)
            {
                return false;
            }
            if (Player.CurrentHp <= lowHPThreshold && lowHPNoBlastCharge && IsMoving && !IsPvPOverheated) // Maybe add InCombat as well
            {
                return false;
            }
            return true;
        }

        return base.GeneralGCD(out act);
    }

    #endregion GCD Logic

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (EmergencyHealing && EmergencyLowHP(out act))
        {
            return true;
        }
        //var highPriority = availableCharas.Where(ObjectHelper.IsTopPriorityHostile);
        //if (highPriority.Any())
        //{
        //    availableCharas = highPriority;
        //}

        // Status checks
        bool targetIsNotPlayer = Target != Player;
        bool playerHasGuard = Player.HasStatus(true, StatusID.Guard);
        bool targetHasGuard = Target.HasStatus(false, StatusID.Guard) && targetIsNotPlayer;
        bool hasChiten = Target.HasStatus(false, StatusID.Chiten) && targetIsNotPlayer;
        bool hasHallowedGround = Target.HasStatus(false, StatusID.HallowedGround_1302) && targetIsNotPlayer;
        bool hasUndeadRedemption = Target.HasStatus(false, StatusID.UndeadRedemption) && targetIsNotPlayer;

        // Configurations checks
        bool analysisOnDrill = AnalysisOnDrill;
        bool analysisOnAirAnchor = AnalysisOnAirAnchor;
        bool analysisOnBioBlaster = AnalysisOnBioBlaster;
        bool analysisOnChainsaw = AnalysisOnChainsaw;
        bool guardCancel = GuardCancel;
        bool preventActionWaste = PreventActionWaste;
        bool safetyCheck = SafetyCheck;
        bool drillOnGuard = DrillOnGuard;
        int recuperateThreshold = Recuperate;
        int guardThreshold = Guard;

        // Should prevent any actions if the option 'guardCancel' is enabled and Player has the Guard buff up
        if (guardCancel && playerHasGuard)
        {
            return false;
        }

        // Uses Recuperate to heal if HP falls below threshold  and have minimum required amount of MP to cast Recuperate
        if (Player.CurrentHp <= recuperateThreshold && Player.CurrentMp >= 2500 && RecuperatePvP.CanUse(out act, true, true, true, true, true))
        {
            if (!playerHasGuard)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Will use Analysis to buff PvE_Drill if Analysis has charges and Player has PvE_Drill Primed status
        if (!playerHasGuard && drillOnGuard && targetHasGuard && Player.HasStatus(true, StatusID.DrillPrimed) && (DrillPvP.CanUse(out act, usedUp: true) || DrillPvP.Cooldown.WillHaveOneCharge(5)))
        {
            return AnalysisPvP.CanUse(out act, usedUp: true);
        }

        /*if (drillOnGuard && targetHasGuard)
        {
            if (!Player.HasStatus(true, StatusID.DrillPrimed))
            {
                return false;
            }
            if (Player.HasStatus(true, StatusID.DrillPrimed) && AnalysisPvP.Cooldown.CurrentCharges == 0 && !Player.HasStatus(true, StatusID.AnalysisPvP))
            {
                return false;
            }
            if (Player.HasStatus(true, StatusID.DrillPrimed) && AnalysisPvP.Cooldown.CurrentCharges >= 1 && AnalysisPvP.CanUse(out act, usedUp: true))
            {
                return true;
            }
        }*/

        // At the moment just prevents attacks if a SAM has Chiten
        // Note: Try to add cast checks for various enemy LB's
        if (safetyCheck && hasChiten)
        {
            return false;
        }

        // Prevent action Waste aims to not use vital skills on targets with invulns
        // Note: currently just blocks attacks, consider just blocking valueable skills and allowing Basic attacks
        if (preventActionWaste && (targetHasGuard || hasHallowedGround || hasUndeadRedemption))
        {
            return false;
        }

        // PvE_Wildfire Should be used only right after getting the 5th Heat Stacks
        if (IsPvPOverheated && !Player.WillStatusEnd(3.5f, true, StatusID.Overheated_3149) && WildfirePvP.CanUse(out act))
        {
            return true;
        }

        // Bishop Turret should be used off cooldown
        // Note: Could prolly be improved using 'ChoiceTarget' in the IBaseAction
        if (BishopAutoturretPvP.CanUse(out act, skipAoeCheck: true)) // Without MustUse, returns CastType 7 invalid // BishopAutoturretPvP.action.CastType
        {
            BishopAutoturretPvP.Action.CastType = 3; // TODO: try 3/4/10 
            BishopAutoturretPvP.Setting.TargetType = TargetType.Self;
            return true;
        }

        // Analysis should be used on any of the tools depending on which options are enabled
        if (AnalysisPvP.CanUse(out act, usedUp: true) && NumberOfAllHostilesInRange > 0 && !IsPvPOverheated)
        {
            if (Player.HasStatus(true, StatusID.Analysis))
            {
                return false;
            }
            //if (AnalysisPvP.Cooldown.CurrentCharges > 0 && Player.HasStatus(true, StatusID.DrillPrimed) && PvP_HeatStacks <= 4 && !WildfirePvP.Cooldown.WillHaveOneCharge(10))
            //{
            //    return true;
            //}
            if (analysisOnDrill && nextGCD.IsTheSameTo(false, DrillPvP) && Player.HasStatus(true, StatusID.DrillPrimed))
            {
                return true;
            }
            else if (analysisOnChainsaw && nextGCD.IsTheSameTo(false, ChainSawPvP) && Target != Player && Target.GetHealthRatio() <= 0.50 && Player.HasStatus(true, StatusID.ChainSawPrimed))
            {
                return true;
            }
            else if (analysisOnBioBlaster && nextGCD.IsTheSameTo(false, BioblasterPvP) && Player.HasStatus(true, StatusID.BioblasterPrimed))
            {
                return true;
            }
            else if (analysisOnAirAnchor && nextGCD.IsTheSameTo(false, AirAnchorPvP) && Player.HasStatus(true, StatusID.AirAnchorPrimed))
            {
                return true;
            }
        }

        return base.EmergencyAbility(nextGCD, out act);
    }
    #endregion oGCD Logic

    private bool EmergencyLowHP(out IAction? act)
    {
        if (Player.HasStatus(true, StatusID.Guard))
        {
            act = null;
            return false;
        }

        if (Player.CurrentHp <= 25000 && GuardPvP.CanUse(out _) && !Player.HasStatus(true, StatusID.Guard) && NumberOfHostilesInMaxRange >= 1)
        {
            return GuardPvP.CanUse(out act);
        }

        if (Player.CurrentMp == Player.MaxMp && Player.CurrentHp <= 37500 && !Player.HasStatus(true, StatusID.Guard) && RecuperatePvP.CanUse(out _))
        {
            return RecuperatePvP.CanUse(out act);
        }

        if (Player.CurrentMp >= 7500 && Player.CurrentHp <= 37500 && !Player.HasStatus(true, StatusID.Guard) && RecuperatePvP.CanUse(out _))
        {
            return RecuperatePvP.CanUse(out act);
        }

        if (Player.CurrentMp >= 5000 && Player.CurrentHp <= 32000 && !Player.HasStatus(true, StatusID.Guard) && RecuperatePvP.CanUse(out _))
        {
            return RecuperatePvP.CanUse(out act);
        }

        if (Player.CurrentMp >= 2500 && Player.CurrentHp <= 25000 && GuardPvP.Cooldown.IsCoolingDown && !Player.HasStatus(true, StatusID.Guard) && RecuperatePvP.CanUse(out _))
        {
            return RecuperatePvP.CanUse(out act);
        }
        act = null;
        return false;
    }
}


/*
    private bool HasMitigation()
    {
        //var purifyStatuses = new Dictionary<int>
        //{
        //	{ 1343, Use1343PvP },
        //	{ 3219, Use3219PvP },
        //	{ 3022, Use3022PvP },
        //	{ 1348, Use1348PvP },
        //	{ 1345, Use1345PvP },
        //	{ 1344, Use1344PvP },
        //	{ 1347, Use1347PvP }
        //};
        var mitigationStatuses = new Dictionary<StatusID, bool>
        {
            //{ StatusID.Phalanx, Target.HasStatus(false, (StatusID)3210)},
            //{ 3026, Target.HasStatus(false, (StatusID)3026)},
            //{ 3188, Target.HasStatus(false, (StatusID)3188)},
            //{ 3186, Target.HasStatus(false, (StatusID)3186)},
            //{ 3054, Target.HasStatus(false, (StatusID)3054)},
            //{ 1308, Target.HasStatus(false, (StatusID)1308)},
            //{ 3036, Target.HasStatus(false, (StatusID)3036)},
            //{ 3037, Target.HasStatus(false, (StatusID)3037)},
            //{ 3051, Target.HasStatus(false, (StatusID)3051)},
            //{ 3047, Target.HasStatus(false, (StatusID)3047)},
            //{ 3044, Target.HasStatus(false, (StatusID)3044)},
            //{ 1415, Target.HasStatus(false, (StatusID)1415)},
            //{ 3086, Target.HasStatus(false, (StatusID)3086)},
            //{ 3111, Target.HasStatus(false, (StatusID)3111)},
            //{ 3110, Target.HasStatus(false, (StatusID)3110)},
            //{ 3087, Target.HasStatus(false, (StatusID)3087)},
            //{ 3093, Target.HasStatus(false, (StatusID)3093)},
            //{ 2011, Target.HasStatus(false, (StatusID)2011)},
            //{ 3186, Target.HasStatus(false, (StatusID)3186)},
            //{ 1240, Target.HasStatus(false, (StatusID)1240)},
            //{ 3173, Target.HasStatus(false, (StatusID)3173)},
            //{ 4096, Target.HasStatus(false, (StatusID)4096)},
            //{ 4097, Target.HasStatus(false, (StatusID)4097)},
        };

        foreach (var status in mitigationStatuses)
        {
            if (status.Value && Target.HasStatus(false, (StatusID)status.Key))
            {
                return true;
            }
        }

        return false;
    }
*/
