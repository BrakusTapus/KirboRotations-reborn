using KirboRotations.Helpers;

namespace KirboRotations.Melee;

[Rotation("Kirbo's PVP", CombatType.PvP, GameVersion = "7.0.1", Description = "")]
[Api(4)]

public class VPR_Kirbo_PvP : ViperRotation
{
    #region Settings
    [RotationConfig(CombatType.PvP, Name = "Use Sprint out of combat?")]
    public bool UseSprint { get; set; } = true;
    #endregion

    private static IBaseAction WorldSwallowerPvP { get; } = new BaseAction((ActionID)39190);

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (Player.HasStatus(true, StatusID.Guard)) return false;
        if (Player.GetHealthRatio() < 0.7 && RecuperatePvP.CanUse(out act)) return true;
        if (Player.GetHealthRatio() < 0.2 && SnakeScalesPvP.CanUse(out act, true, true, true, true, true) && CurrentMp < 2500 && HasHostilesInRange && InCombat) return true;

        if (Player.HasStatus(true, StatusID.Reawakened, StatusID.Reawakened_4094) && nextGCD.IsTheSameTo((ActionID)39190, ActionID.OuroborosPvP) && SlitherPvP.CanUse(out act, true, true, true, true, true)) return true;

        if ((UncoiledFuryPvP.Cooldown.RecastTimeRemainOneCharge >= 5 || IsLastGCD(ActionID.UncoiledFuryPvP)) && RattlingCoilPvP.CanUse(out act, true, true, true, true, true)) return true;

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (Player.HasStatus(true, StatusID.Guard)) return false;
        if (IsLastGCD((ActionID)FirstGenerationPvP.ID) && FirstLegacyPvP.CanUse(out act, skipAoeCheck: true)) return true;
        if (IsLastGCD((ActionID)SecondGenerationPvP.ID) && SecondLegacyPvP.CanUse(out act, skipAoeCheck: true)) return true;
        if (IsLastGCD((ActionID)ThirdGenerationPvP.ID) && ThirdLegacyPvP.CanUse(out act, skipAoeCheck: true)) return true;
        if (IsLastGCD((ActionID)FourthGenerationPvP.ID) && FourthLegacyPvP.CanUse(out act, skipAoeCheck: true)) return true;

        if (IsLastGCD((ActionID)UncoiledFuryPvP.ID) && UncoiledTwinfangPvP.CanUse(out act, skipAoeCheck: true)) return true;
        if (IsLastGCD((ActionID)HuntersSnapPvP.ID) && TwinfangBitePvP.CanUse(out act)) return true;
        if (IsLastGCD((ActionID)SwiftskinsCoilPvP.ID) && TwinbloodBitePvP.CanUse(out act)) return true;
        if (IsLastGCD((ActionID)BarbarousBitePvP.ID, (ActionID)RavenousBitePvP.ID) && DeathRattlePvP.CanUse(out act)) return true;

        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        if (Player.HasStatus(true, StatusID.Guard)) return false;
        if (UseSprint) { if (!InCombat && SprintPvP.CanUse(out act)) return true; }

        return base.GeneralAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        act = null;
        if (CustomRotationEx.CurrentLimitBreakLevel == 1 && WorldSwallowerPvP.CanUse(out act, true, false, true, true, true, true,true, 1) && HasHostilesInRange && InCombat) return true;

        if (Player.HasStatus(true, StatusID.Guard)) return false;

        if (Player.HasStatus(true, StatusID.Reawakened_4094))
        {
            if (Player.WillStatusEnd(4, true, StatusID.Reawakened_4094))
            {
                if (OuroborosPvP.CanUse(out act, skipAoeCheck: true)) return true;
            }

            if (IsLastAction((ActionID)FourthLegacyPvP.ID) && OuroborosPvP.CanUse(out act, skipAoeCheck: true)) return true;

        }

        if (!Player.HasStatus(true, StatusID.Reawakened_4094))
        {
            if (SwiftskinsCoilPvP.CanUse(out act, usedUp: true)) return true;
            if (HuntersSnapPvP.CanUse(out act, usedUp: true)) return true;
        }


        if (UncoiledFuryPvP.CanUse(out act, skipAoeCheck: true)) return true;

        if (RavenousBitePvP.CanUse(out act)) return true;
        if (SwiftskinsStingPvP.CanUse(out act)) return true;
        if (PiercingFangsPvP.CanUse(out act)) return true;
        if (BarbarousBitePvP.CanUse(out act)) return true;
        if (HuntersStingPvP.CanUse(out act)) return true;
        if (SteelFangsPvP.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }

    public override void DisplayStatus()
    {
        ImGui.TextWrapped("LimitBreakLevel: " + CustomRotationEx.CurrentLimitBreakLevel);
        ImGuiToolTipsKirbo.HoveredTooltip("CurrentUnits: " + CustomRotationEx.CurrentCurrentUnits);
    }

}