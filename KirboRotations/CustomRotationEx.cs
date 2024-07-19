using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace KirboRotations;
internal class CustomRotationEx
{
    #region LB
    [Description("Bar Count")]
    internal unsafe static byte CurrentBarCount
    {
        get
        {
            LimitBreakController limitBreakController = UIState.Instance()->LimitBreakController;
            byte barCount = *&limitBreakController.BarCount;

            return barCount;
        }
    }

    [Description("Limit Break Level")]
    internal unsafe static byte CurrentLimitBreakLevel
    {
        get
        {
            LimitBreakController limitBreakController = UIState.Instance()->LimitBreakController;
            ushort currentUnits = *&limitBreakController.CurrentUnits;

            if (currentUnits >= 9000)
            {
                return 3;
            }
            else if (currentUnits >= 6000)
            {
                return 2;
            }
            else if (currentUnits >= 3000)
            {
                return 1;
            }
            else
            {
                return 0; // Assuming 0 is the default or undefined state.
            }
        }
    }

    [Description("Current Units")]
    internal unsafe static ushort CurrentCurrentUnits
    {
        get
        {
            LimitBreakController limitBreakController = UIState.Instance()->LimitBreakController;
            ushort currentUnits = *&limitBreakController.CurrentUnits;

            return currentUnits;
        }
    }

    [Description("Is PvP")]
    internal unsafe static bool IsCurrentPvP
    {
        get
        {
            LimitBreakController limitBreakController = UIState.Instance()->LimitBreakController;
            bool isPvP = *&limitBreakController.IsPvP;

            return isPvP;
        }
    }

    /// <summary>
    /// PLD
    /// </summary>
    internal static IBaseAction PhalanxPvP { get; } = new BaseAction((ActionID)29069);

    /// <summary>
    /// WAR
    /// </summary>
    internal static IBaseAction PrimalScreamPvP { get; } = new BaseAction((ActionID)29083);

    /// <summary>
    /// DRK
    /// </summary>
    internal static IBaseAction EvenTidePvP { get; } = new BaseAction((ActionID)29097);

    /// <summary>
    /// GNB
    /// </summary>
    internal static IBaseAction TerminalTriggerPvP { get; } = new BaseAction((ActionID)29469);

    /// <summary>
    /// WHM
    /// </summary>
    internal static IBaseAction AfflatusPurgationPvP { get; } = new BaseAction((ActionID)29230);

    /// <summary>
    /// AST
    /// </summary>
    internal static IBaseAction CelestialRiverPvP { get; } = new BaseAction((ActionID)29255);

    /// <summary>
    /// SCH
    /// </summary>
    internal static IBaseAction SummonSeraphPvP { get; } = new BaseAction((ActionID)29237);

    /// <summary>
    /// SCH
    /// </summary>
    internal static IBaseAction SeraphFlightPvP { get; } = new BaseAction((ActionID)29239);

    /// <summary>
    /// SGE
    /// </summary>
    internal static IBaseAction MesotesPvP { get; } = new BaseAction((ActionID)29266);

    /// <summary>
    /// NIN
    /// </summary>
    internal static IBaseAction SeitonTenchuPvP { get; } = new BaseAction((ActionID)29515);

    /// <summary>
    /// MNK
    /// </summary>
    internal static IBaseAction MeteoDivePvP { get; } = new BaseAction((ActionID)29485);

    /// <summary>
    /// DRG
    /// </summary>
    internal static IBaseAction SkyHighPvP { get; } = new BaseAction((ActionID)29497);

    /// <summary>
    /// DRG
    /// </summary>
    internal static IBaseAction SkyShatterPvP { get; } = new BaseAction((ActionID)29499);

    /// <summary>
    /// SAM
    /// </summary>
    internal static IBaseAction ZantetsukenPvP { get; } = new BaseAction((ActionID)29537);

    /// <summary>
    /// RPR
    /// </summary>
    internal static IBaseAction TenebraelemuruPvP { get; } = new BaseAction((ActionID)29553);

    /// <summary>
    /// BRD
    /// </summary>
    internal static IBaseAction FinalFantasiaPvP { get; } = new BaseAction((ActionID)29401);

    /// <summary>
    /// MCH
    /// </summary>
    internal static IBaseAction MarksmansSpitePvP { get; } = new BaseAction((ActionID)29415);

    /// <summary>
    /// DNC
    /// </summary>
    internal static IBaseAction ContraDancePvP { get; } = new BaseAction((ActionID)29432);

    /// <summary>
    /// BLM
    /// </summary>
    internal static IBaseAction SoulResonancePvP { get; } = new BaseAction((ActionID)29662);


    /// <summary>
    /// SMN
    /// </summary>
    internal static IBaseAction SummonBahamutPvP { get; } = new BaseAction((ActionID)29673);

    /// <summary>
    /// SMN
    /// </summary>
    internal static IBaseAction SummonPhoenixPvP  { get; } = new BaseAction((ActionID)29678);

    /// <summary>
    /// SMN
    /// </summary>
    internal static IBaseAction MegaflarePvP { get; } = new BaseAction((ActionID)29675);

    /// <summary>
    /// SMN
    /// </summary>
    internal static IBaseAction EverlastingPvP { get; } = new BaseAction((ActionID)29680);

    /// <summary>
    /// RDM
    /// </summary>
    internal static IBaseAction SouthernCrossPvP { get; } = new BaseAction((ActionID)29704);
    #endregion
}
