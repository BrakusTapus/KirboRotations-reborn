
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using FFXIVClientStructs.FFXIV.Client.Game;
//using FFXIVClientStructs.FFXIV.Client.Game.Control;
//using FFXIVClientStructs.FFXIV.Client.Game.Object;

//namespace KirboRotations.IllegalHelpers;


//public sealed class TargetSystemSingleton
//{
//    private unsafe static readonly Lazy<TargetSystem> instanceActionManager = new Lazy<TargetSystem>(() =>
//    {
//        TargetSystem* ptr = TargetSystem.StaticAddressPointers.pInstance;
//        if (ptr == null)
//        {
//            throw new InvalidOperationException("TargetSystem instanceActionManager is null");
//        }
//        return *ptr;
//    });

//    public static TargetSystem InstanceActionManager =>  instanceActionManager.Value;

//    private TargetSystemSingleton() { }
//}

//public unsafe class TargetSystemManager
//{
//    public unsafe GameObjectId GetTargetObjectId()
//    {
//        TargetSystem* ts = (TargetSystem*)Unsafe.AsPointer(ref TargetSystemSingleton.InstanceActionManager);
//        if (TargetSystem.MemberFunctionPointers.GetTargetObjectId == (delegate* unmanaged<TargetSystem*, GameObjectId>)null)
//        {
//            throw new InvalidOperationException("GetTargetObjectId function pointer is null");
//        }
//        return TargetSystem.MemberFunctionPointers.GetTargetObjectId(ts);
//    }

//    public unsafe GameObject* GetTargetObject()
//    {
//        TargetSystem* ts = (TargetSystem*)Unsafe.AsPointer(ref TargetSystemSingleton.InstanceActionManager);
//        if (TargetSystem.MemberFunctionPointers.GetTargetObject == (delegate* unmanaged<TargetSystem*, GameObject*>)null)
//        {
//            throw new InvalidOperationException("GetTargetObject function pointer is null");
//        }
//        return TargetSystem.MemberFunctionPointers.GetTargetObject(ts);
//    }

//    public unsafe bool IsObjectInViewRange(GameObject* obj)
//    {
//        TargetSystem* ts = (TargetSystem*)Unsafe.AsPointer(ref TargetSystemSingleton.InstanceActionManager);
//        if (TargetSystem.MemberFunctionPointers.IsObjectInViewRange == (delegate* unmanaged<TargetSystem*, GameObject*, bool>)null)
//        {
//            throw new InvalidOperationException("IsObjectInViewRange function pointer is null");
//        }
//        return TargetSystem.MemberFunctionPointers.IsObjectInViewRange(ts, obj);
//    }

//    public unsafe bool IsObjectOnScreen(GameObject* obj)
//    {
//        TargetSystem* ts = (TargetSystem*)Unsafe.AsPointer(ref TargetSystemSingleton.InstanceActionManager);
//        if (TargetSystem.MemberFunctionPointers.IsObjectOnScreen == (delegate* unmanaged<TargetSystem*, GameObject*, bool>)null)
//        {
//            throw new InvalidOperationException("IsObjectOnScreen function pointer is null");
//        }
//        return TargetSystem.MemberFunctionPointers.IsObjectOnScreen(ts, obj);
//    }

//    public unsafe ulong InteractWithObject(GameObject* obj, bool checkLineOfSight = true)
//    {
//        TargetSystem* ts = (TargetSystem*)Unsafe.AsPointer(ref TargetSystemSingleton.InstanceActionManager);
//        if (TargetSystem.MemberFunctionPointers.InteractWithObject == (delegate* unmanaged<TargetSystem*, GameObject*, bool, ulong>)null)
//        {
//            throw new InvalidOperationException("InteractWithObject function pointer is null");
//        }
//        return TargetSystem.MemberFunctionPointers.InteractWithObject(ts, obj, checkLineOfSight);
//    }

//    public unsafe void OpenObjectInteraction(GameObject* obj)
//    {
//        TargetSystem* ts = (TargetSystem*)Unsafe.AsPointer(value: ref TargetSystemSingleton.InstanceActionManager);
//        if (TargetSystem.MemberFunctionPointers.OpenObjectInteraction == (delegate* unmanaged<TargetSystem*, GameObject*, void>)null)
//        {
//            throw new InvalidOperationException("OpenObjectInteraction function pointer is null");
//        }
//        TargetSystem.MemberFunctionPointers.OpenObjectInteraction(ts, obj);
//    }

//    public unsafe GameObject* GetMouseOverObject(int x, int y, GameObjectArray* objectArray, Camera* camera)
//    {
//        TargetSystem* ts = (TargetSystem*)Unsafe.AsPointer(ref TargetSystemSingleton.InstanceActionManager);
//        if (TargetSystem.MemberFunctionPointers.GetMouseOverObject == (delegate* unmanaged<TargetSystem*, int, int, GameObjectArray*, Camera*, GameObject*>)null)
//        {
//            throw new InvalidOperationException("GetMouseOverObject function pointer is null");
//        }
//        return TargetSystem.MemberFunctionPointers.GetMouseOverObject(ts, x, y, objectArray, camera);
//    }
//}