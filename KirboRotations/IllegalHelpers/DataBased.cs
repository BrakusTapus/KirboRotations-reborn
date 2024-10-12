using FFXIVClientStructs.FFXIV.Client.Game.Character;
using KirboRotations.Common;
using System.Runtime.InteropServices;
using CSGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using DGameObject = Dalamud.Game.ClientState.Objects.Types.IGameObject;

namespace KirboRotations.IllegalHelpers;
internal class DataBased
{
    public static unsafe float DefaultGCDTotal => ActionManagerHelper.GetDefaultRecastTime();
    public static unsafe float DefaultGCDRemain => ActionManagerHelper.GetDefaultRecastTime() - ActionManagerHelper.GetDefaultRecastTimeElapsed();
    public static unsafe float DefaultGCDElapsed => ActionManagerHelper.GetDefaultRecastTimeElapsed();
    public const float MinAnimationLock = 0.6f;
    public static float NextAbilityToNextGCD => Math.Max(0, DefaultGCDRemain - Math.Max(ActionManagerHelper.GetCurrentAnimationLock(), MinAnimationLock));

    // Define the OnSecondAbilitySlot boolean property
    public static bool OnSecondAbilitySlot => CustomRotation.InCombat && (DefaultGCDRemain > 0.60f && DefaultGCDRemain <= 0.82f);

    public static void DisplayPlayerGameObjectId(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject player)
    {
        // Retrieve the GameObjectId for the player
        FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectId gameObjectId = player.GetGameObjectId();

        // Display the GameObjectId
        Serilog.Log.Logger.Debug($"Player's GameObjectId: ObjectId = {gameObjectId.ObjectId}, Type = {gameObjectId.Type}");
    }
    //public static byte GetStatus(DGameObject actor) => Marshal.ReadByte(actor.Address + 0x1980);
    //public static bool InCombat(DGameObject actor) => (GetStatus(actor) & 2) > 0;
    //public static bool InParty(DGameObject actor) => (GetStatus(actor) & 16) > 0;
    //public static bool InAlliance(DGameObject actor) => (GetStatus(actor) & 32) > 0;
}

//internal static class ObjectExtensions
//{
//    public static unsafe BattleChara* BattleChara(this DGameObject obj) => (BattleChara*)obj.Address;
//    public static unsafe Character* Character(this DGameObject obj) => (Character*)obj.Address;

//    public static unsafe BattleChara* BattleChara(this CSGameObject obj) => (BattleChara*)&obj;
//    public static unsafe Character* Character(this CSGameObject obj) => (Character*)&obj;

//    //public static bool IsTargetingPlayer(this DGameObject obj) => obj.TargetObjectId == Player.Object.GameObjectId;
//}