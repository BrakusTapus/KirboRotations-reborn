//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace KirboRotations.IllegalHelpers;

//internal static unsafe class PlayerEx
//{
//    public const int MaxLevel = 100;
//    public static IPlayerCharacter Object => Svc.ClientState.LocalPlayer;
//    public static bool Available => Svc.ClientState.LocalPlayer != null;
//    public static bool Interactable => Available && Object.IsTargetable;
//    public static ulong CID => Svc.ClientState.LocalContentId;
//    public static StatusList Status => Svc.ClientState.LocalPlayer.StatusList;
//    public static string Name => Svc.ClientState.LocalPlayer?.Name.ToString();
//    public static string NameWithWorld => GetNameWithWorld(Svc.ClientState.LocalPlayer);
//    public static int Level => Svc.ClientState.LocalPlayer?.Level ?? 0;
//    public static bool IsInHomeWorld => Svc.ClientState.LocalPlayer.HomeWorld.Id == Svc.ClientState.LocalPlayer.CurrentWorld.Id;
//    public static bool IsInHomeDC => Svc.ClientState.LocalPlayer.CurrentWorld.GameData.DataCenter.Row == Svc.ClientState.LocalPlayer.HomeWorld.GameData.DataCenter.Row;
//    public static string HomeWorld => Svc.ClientState.LocalPlayer?.HomeWorld.GameData.Name.ToString();
//    public static string CurrentWorld => Svc.ClientState.LocalPlayer?.CurrentWorld.GameData.Name.ToString();
//    public static Character* Character => (Character*)Svc.ClientState.LocalPlayer.Address;
//    public static BattleChara* BattleChara => (BattleChara*)Svc.ClientState.LocalPlayer.Address;
//    public static GameObject* GameObject => (GameObject*)Svc.ClientState.LocalPlayer.Address;
//    [Obsolete("Please use GameObject")]
//    public static GameObject* IGameObject => (GameObject*)Svc.ClientState.LocalPlayer.Address;
//    public static uint Territory => Svc.ClientState.TerritoryType;
//    public static Job Job => GetJob(Svc.ClientState.LocalPlayer);
//    public static GrandCompany GrandCompany => (GrandCompany)PlayerState.InstanceActionManager()->GrandCompany;
//    public static string GetNameWithWorld(this IPlayerCharacter pc) => pc == null ? null : (pc.Name.ToString() + "@" + pc.HomeWorld.GameData.Name);
//    public static Job GetJob(this IPlayerCharacter pc) => (Job)pc.ClassJob.Id;
//    public static Vector3 Position => Object.Position;
//    public static float Rotation => Object.Rotation;
//    public static float AnimationLock => *(float*)((nint)ActionManager.InstanceActionManager() + 8);
//    public static bool IsAnimationLocked => AnimationLock > 0;
//}
