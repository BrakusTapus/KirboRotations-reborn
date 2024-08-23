using KirboRotations.Common;

namespace KirboRotations.IllegalHelpers;
internal class DataBased
{
    public static unsafe float DefaultGCDTotal => ActionManagerHelper.GetDefaultRecastTime();
    public static unsafe float DefaultGCDRemain => ActionManagerHelper.GetDefaultRecastTime() - ActionManagerHelper.GetDefaultRecastTimeElapsed();
    public static unsafe float DefaultGCDElapsed => ActionManagerHelper.GetDefaultRecastTimeElapsed();

    public static void DisplayPlayerGameObjectId(FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject player)
    {
        // Retrieve the GameObjectId for the player
        FFXIVClientStructs.FFXIV.Client.Game.Object.GameObjectId gameObjectId = player.GetGameObjectId();

        // Display the GameObjectId
        Serilog.Log.Logger.Debug($"Player's GameObjectId: ObjectId = {gameObjectId.ObjectId}, Type = {gameObjectId.Type}");
    }
}
