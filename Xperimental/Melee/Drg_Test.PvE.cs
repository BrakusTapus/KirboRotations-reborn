#if DEBUG
#pragma warning disable CS8601 // 'act' can be null it's okay, dont be so worried for me IDE, you're not even my real dad!
#endif

using Xperimental.Common;

namespace Xperimental.Melee;

[BetaRotation]
[Api(3)]
[Rotation($"Example Custom Rotation", CombatType.PvE, GameVersion = "0x0x0x0", Description = "An example description for the example Custom Rotation, for example: 'This is just an example!'")]
[LinkDescription("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR3BCylKUHNtb9jmnJi4OMQB4zImS_swIKg9Q&s")]
public sealed class Drg_Test : DragoonRotation
{

    public unsafe override void DisplayStatus()
    {
        if (ImGui.BeginChild("The CustomRotation's status window", new Vector2(ImGui.GetContentRegionAvail().X - ImGui.GetStyle().WindowPadding.X * 2, ImGui.GetContentRegionAvail().Y - 50), border: true, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            ImGui.Text("Rotation: " + Name + " "); ImGui.SameLine(); ImGui.TextDisabled(Description);

            //TODO- Add GroupBox with a border that lists all available actions.
            //TODO- Add GroupBox with a border that lists general help information for debugging.

            //var ActionManager = ActionManagerInstance.InstanceActionManager;

            //float defaultTotalGCDtime = ActionManager->Cooldowns..GetDefaultRecastTime();
            //float elapsedGCDTime = ActionCoolDown.GetDefaultRecastTimeElapsed();
            //float remainingGCDTimeTillNextGCD = ActionCoolDown.TimeTillNextGCD;

            //ImGui.Text($"GCD:{defaultTotalGCDtime.ToString()}" + "\"" + "{remainingGCDTimeTillNextGCD.ToString()}");
            //ImGui.Text($"AnimationLockTime: {ActionManagerInstance.InstanceActionManager->AnimationLock}");

            float speed = Player.DataId-;
            float gcdTotal = DataBase.DefaultGCDRemain;
            float gcdRemain = DataBase.DefaultGCDRemain;
            float gcdTotalElapsed = DataBase.DefaultGCDElapsed;
            float animationLockremain = ActionManagerHelper.GetCurrentAnimationLock();

            // Calculate fractions
            float gcdFraction = gcdRemain / gcdTotal;
            float elapsedFraction = gcdTotalElapsed / gcdTotal;
            //float animationLockFraction = animationLockRemain / gcdTotal; // Assuming the same total
            Vector2 size = new Vector2(200, 20); // Example size

            ImGui.Text("GCD Total: " + gcdTotal.ToString("F2"));
            ImGui.Text("GCD Remain: " + gcdRemain.ToString("F2"));
            ImGui.Text("GCD Elapsed: " + gcdTotalElapsed.ToString("F2"));
            //ImGui.Text("Calculated Action Ahead: " + DataCenter.CalculatedActionAhead.ToString());
            //ImGui.Text("Actual Action Ahead: " + DataCenter.ActionAhead.ToString());
            ImGui.Text("Animation Lock Delay: " + animationLockremain.ToString("F2"));


        ImGui.ProgressBar(gcdFraction, size, "GCD Remaining");
        ImGui.ProgressBar(elapsedFraction, size, "GCD Elapsed");
       // ImGui.ProgressBar(animationLockFraction, size, "Animation Lock Remaining");

            if (ImGui.Button(ActionID.PelotonPvE.ToString()))
            {
                Serilog.Log.Logger.Warning("[Experimental] muuh LOG!");
            }

        }
        ImGui.EndChild();


    }

    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Use Doom Spike for damage uptime if out of melee range even if it breaks combo")]
    public bool DoomSpikeWhenever { get; set; } = true;
    #endregion

    #region Additional oGCD Logic

    [RotationDesc]
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.WingedGlidePvE)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (WingedGlidePvE.CanUse(out act))
        {
            return true;
        }

        return false;
    }

    [RotationDesc(ActionID.ElusiveJumpPvE)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
    {
        if (ElusiveJumpPvE.CanUse(out act))
        {
            return true;
        }

        return false;
    }

    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (FeintPvE.CanUse(out act))
        {
            return true;
        }

        return false;
    }
    #endregion

    #region oGCD Logic
    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        if (IsBurst && InCombat)
        {
            if ((Player.HasStatus(true, StatusID.BattleLitany) || Player.HasStatus(true, StatusID.LanceCharge) || LOTDEndAfter(1000)) && nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE)
            || (Player.HasStatus(true, StatusID.BattleLitany) && Player.HasStatus(true, StatusID.LanceCharge) && LOTDEndAfter(1000) && nextGCD.IsTheSameTo(true, ChaoticSpringPvE, LanceBarragePvE, WheelingThrustPvE, FangAndClawPvE))
            || (nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE) && (LanceChargePvE.IsInCooldown || BattleLitanyPvE.IsInCooldown)))
            {
                if (LifeSurgePvE.CanUse(out act, usedUp: true))
                {
                    return true;
                }
            }

            if (LanceChargePvE.CanUse(out act))
            {
                return true;
            }

            if (BattleLitanyPvE.CanUse(out act))
            {
                return true;
            }
        }

        return base.GeneralAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (Player.HasStatus(true, StatusID.LanceCharge) && GeirskogulPvE.CanUse(out act))
        {
            return true;
        }

        if ((BattleLitanyPvE.EnoughLevel && Player.HasStatus(true, StatusID.BattleLitany) && Player.HasStatus(true, StatusID.LanceCharge)
            || !BattleLitanyPvE.EnoughLevel && Player.HasStatus(true, StatusID.LanceCharge)) && DragonfireDivePvE.CanUse(out act))
        {
            return true;
        }

        if (((Player.HasStatus(true, StatusID.BattleLitany) || Player.HasStatus(true, StatusID.LanceCharge) || LOTDEndAfter(1000))
            || nextGCD.IsTheSameTo(true, RaidenThrustPvE, DraconianFuryPvE)) && WyrmwindThrustPvE.CanUse(out act, usedUp: true))
        {
            return true;
        }

        if (JumpPvE.CanUse(out act))
        {
            return true;
        }

        if (HighJumpPvE.CanUse(out act))
        {
            return true;
        }

        if (StardiverPvE.CanUse(out act))
        {
            return true;
        }

        if (MirageDivePvE.CanUse(out act))
        {
            return true;
        }

        if (NastrondPvE.CanUse(out act))
        {
            return true;
        }

        if (StarcrossPvE.CanUse(out act))
        {
            return true;
        }

        if (RiseOfTheDragonPvE.CanUse(out act))
        {
            return true;
        }

        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    protected override bool GeneralGCD(out IAction? act)
    {
        bool doomSpikeRightNow = DoomSpikeWhenever;

        if (CoerthanTormentPvE.CanUse(out act))
        {
            return true;
        }

        if (SonicThrustPvE.CanUse(out act, skipStatusProvideCheck: true))
        {
            return true;
        }

        if (DoomSpikePvE.CanUse(out act, skipComboCheck: doomSpikeRightNow))
        {
            return true;
        }

        if (DrakesbanePvE.CanUse(out act))
        {
            return true;
        }

        if (FangAndClawPvE.CanUse(out act))
        {
            return true;
        }

        if (WheelingThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (FullThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (ChaosThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (SpiralBlowPvE.CanUse(out act))
        {
            return true;
        }

        if (DisembowelPvE.CanUse(out act))
        {
            return true;
        }

        if (LanceBarragePvE.CanUse(out act))
        {
            return true;
        }

        if (VorpalThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (RaidenThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (TrueThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (PiercingTalonPvE.CanUse(out act))
        {
            return true;
        }

        return base.GeneralGCD(out act);
    }

}