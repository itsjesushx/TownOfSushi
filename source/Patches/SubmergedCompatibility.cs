using System.Collections;
using BepInEx;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;

namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
    public static class SubmergedStartPatch
    {
        public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
        {
            if (IsSubmerged())
            {
                Coroutines.Start(WaitMeeting(resetTimers));
            }
        }
    }

    public static class SubmergedCompatibility
    {
        public static class Classes
        {
            public const string ElevatorMover = "ElevatorMover";
        }

        public const string SUBMERGED_GUID = "Submerged";
        public const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)6;

        public static SemanticVersioning.Version Version { get; private set; }
        public static bool SubmergedLoaded { get; private set; }
        public static BasePlugin Plugin { get; private set; }
        public static Assembly Assembly { get; private set; }
        public static Type[] Types { get; private set; }
        public static Dictionary<string, Type> InjectedTypes { get; private set; }

        private static MonoBehaviour _submarineStatus;
        public static MonoBehaviour SubmarineStatus
        {
            get
            {
                if (!SubmergedLoaded) return null;

                if (_submarineStatus is null || _submarineStatus.WasCollected || !_submarineStatus || _submarineStatus == null)
                {
                    if (Ship() is null || Ship().WasCollected || !Ship() || Ship() == null)
                    {
                        return _submarineStatus = null;
                    }
                    else
                    {
                        if (Ship().Type == SUBMERGED_MAP_TYPE)
                        {
                            return _submarineStatus = Ship().GetComponent(Il2CppType.From(SubmarineStatusType))?.TryCast(SubmarineStatusType) as MonoBehaviour;
                        }
                        else
                        {
                            return _submarineStatus = null;
                        }
                    }
                }
                else
                {
                    return _submarineStatus;
                }
            }
        }

        private static Type SubmarineStatusType;
        private static MethodInfo CalculateLightRadiusMethod;

        private static MethodInfo RpcRequestChangeFloorMethod;
        private static Type FloorHandlerType;
        private static MethodInfo GetFloorHandlerMethod;

        private static Type VentPatchDataType;
        private static PropertyInfo InTransitionField;

        private static Type CustomTaskTypesType;
        private static FieldInfo RetrieveOxigenMaskField;
        public static TaskTypes RetrieveOxygenMask;
        private static Type SubmarineOxygenSystemType;
        private static PropertyInfo SubmarineOxygenSystemInstanceField;
        private static MethodInfo RepairDamageMethod;

        private static Type SubmergedExileController;
        private static MethodInfo SubmergedExileWrapUpMethod;

        private static Type SubmarineElevator;
        private static MethodInfo GetInElevator;
        private static MethodInfo GetMovementStageFromTime;
        private static FieldInfo getSubElevatorSystem;

        private static Type ElevatorConsole;
        private static MethodInfo CanUse;

        private static Type SubmarineElevatorSystem;
        private static FieldInfo UpperDeckIsTargetFloor; 

        private static FieldInfo SubmergedInstance;
        private static FieldInfo SubmergedElevators;

        public static void Initialize()
        {
            SubmergedLoaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SUBMERGED_GUID, out PluginInfo plugin);
            if (!SubmergedLoaded) return;

            Plugin = plugin!.Instance as BasePlugin;
            Version = plugin.Metadata.Version;

            Assembly = Plugin!.GetType().Assembly;
            Types = AccessTools.GetTypesFromAssembly(Assembly);

            InjectedTypes = (Dictionary<string, Type>)AccessTools.PropertyGetter(Types.FirstOrDefault(t => t.Name == "ComponentExtensions"), "RegisteredTypes")
                .Invoke(null, Array.Empty<object>());

            SubmarineStatusType = Types.First(t => t.Name == "SubmarineStatus");
            SubmergedInstance = AccessTools.Field(SubmarineStatusType, "instance");
            SubmergedElevators = AccessTools.Field(SubmarineStatusType, "elevators");

            CalculateLightRadiusMethod = AccessTools.Method(SubmarineStatusType, "CalculateLightRadius");

            FloorHandlerType = Types.First(t => t.Name == "FloorHandler");
            GetFloorHandlerMethod = AccessTools.Method(FloorHandlerType, "GetFloorHandler", new Type[] { typeof(PlayerControl) });
            RpcRequestChangeFloorMethod = AccessTools.Method(FloorHandlerType, "RpcRequestChangeFloor");

            VentPatchDataType = Types.First(t => t.Name == "VentPatchData");
            InTransitionField = AccessTools.Property(VentPatchDataType, "InTransition");

            CustomTaskTypesType = Types.First(t => t.Name == "CustomTaskTypes");
            RetrieveOxigenMaskField = AccessTools.Field(CustomTaskTypesType, "RetrieveOxygenMask");
            var retTaskType = AccessTools.Field(CustomTaskTypesType, "taskType");
            RetrieveOxygenMask = (TaskTypes)retTaskType.GetValue(RetrieveOxigenMaskField.GetValue(null));

            SubmarineOxygenSystemType = Types.First(t => t.Name == "SubmarineOxygenSystem");
            SubmarineOxygenSystemInstanceField = AccessTools.Property(SubmarineOxygenSystemType, "Instance");
            RepairDamageMethod = AccessTools.Method(SubmarineOxygenSystemType, "RepairDamage");
            SubmergedExileController = Types.First(t => t.Name == "SubmergedExileController");
            SubmergedExileWrapUpMethod = AccessTools.Method(SubmergedExileController, "WrapUpAndSpawn");

            SubmarineElevator = Types.First(t => t.Name == "SubmarineElevator");
            GetInElevator = AccessTools.Method(SubmarineElevator, "GetInElevator", new Type[] { typeof(PlayerControl) });
            GetMovementStageFromTime = AccessTools.Method(SubmarineElevator, "GetMovementStageFromTime");
            getSubElevatorSystem = AccessTools.Field(SubmarineElevator, "system");

            ElevatorConsole = Types.First(t => t.Name == "ElevatorConsole");
            CanUse = AccessTools.Method(ElevatorConsole, "CanUse");

            SubmarineElevatorSystem = Types.First(t => t.Name == "SubmarineElevatorSystem");
            UpperDeckIsTargetFloor = AccessTools.Field(SubmarineElevatorSystem, "upperDeckIsTargetFloor");
            Harmony _harmony = new Harmony("tou.submerged.patch");
            var exilerolechangePostfix = SymbolExtensions.GetMethodInfo(() => ExileRoleChangePostfix());
            _harmony.Patch(SubmergedExileWrapUpMethod, null, new HarmonyMethod(exilerolechangePostfix));
        }

        public static void CheckOutOfBoundsElevator(PlayerControl player)
        {
            if (!SubmergedLoaded) return;
            if (!IsSubmerged()) return;

            Tuple<bool, object> elevator = GetPlayerElevator(player);
            if (!elevator.Item1) return;
            bool CurrentFloor = (bool)UpperDeckIsTargetFloor.GetValue(getSubElevatorSystem.GetValue(elevator.Item2)); //true is top, false is bottom
            bool PlayerFloor = player.transform.position.y > -7f; //true is top, false is bottom
            
            if (CurrentFloor != PlayerFloor)
            {
                ChangeFloor(CurrentFloor);
            }
        }

        public static void MoveDeadPlayerElevator(PlayerControl player)
        {
            if (!IsSubmerged()) return;
            Tuple<bool, object> elevator = GetPlayerElevator(player);
            if (!elevator.Item1) return;

            int MovementStage = (int)GetMovementStageFromTime.Invoke(elevator.Item2, null);
            if (MovementStage >= 5)
            {
                //Fade to clear
                bool topfloortarget = (bool)UpperDeckIsTargetFloor.GetValue(getSubElevatorSystem.GetValue(elevator.Item2)); //true is top, false is bottom
                bool topintendedtarget = player.transform.position.y > -7f; //true is top, false is bottom
                if (topfloortarget != topintendedtarget)
                {
                    ChangeFloor(!topintendedtarget);
                }
            }
        }

        public static Tuple<bool, object> GetPlayerElevator(PlayerControl player)
        {
            if (!IsSubmerged()) return Tuple.Create(false, (object)null);
            IList elevatorlist = createList(SubmarineElevator);
            elevatorlist = (IList)SubmergedElevators.GetValue(SubmergedInstance.GetValue(null));
            foreach (object elevator in elevatorlist)
            {
                if ((bool)GetInElevator.Invoke(elevator, new object[] { player })) return Tuple.Create(true, elevator);
            }

            return Tuple.Create(false, (object)null);
        }

        public static void ExileRoleChangePostfix()
        {
            Coroutines.Start(WaitMeeting(resetTimers));
        }

        public static IEnumerator WaitMeeting(Action next)
        {
            while (!PlayerControl.LocalPlayer.moveable)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            while (HUDManager().PlayerCam.transform.Find("SpawnInMinigame(Clone)") != null)
            {
                yield return null;
            }       
            next();
        }

        public static void resetTimers()
        {
            ResetCustomTimers();
        }

        public static MonoBehaviour AddSubmergedComponent(this GameObject obj, string typeName)
        {
            if (!SubmergedLoaded) return obj.AddComponent<MissingSubmergedBehaviour>();
            bool validType = InjectedTypes.TryGetValue(typeName, out Type type);
            return validType ? obj.AddComponent(Il2CppType.From(type)).TryCast<MonoBehaviour>() : obj.AddComponent<MissingSubmergedBehaviour>();
        }

        public static float GetSubmergedNeutralLightRadius(bool isImpostor)
        {
            if (!SubmergedLoaded) return 0;
            return (float)CalculateLightRadiusMethod.Invoke(SubmarineStatus, new object[] { null, true, isImpostor });
        }

        public static void ChangeFloor(bool toUpper)
        {
            if (!SubmergedLoaded) return;
            MonoBehaviour _floorHandler = ((Component)GetFloorHandlerMethod.Invoke(null, new object[] { PlayerControl.LocalPlayer })).TryCast(FloorHandlerType) as MonoBehaviour;
            RpcRequestChangeFloorMethod.Invoke(_floorHandler, new object[] { toUpper });
        }

        public static bool getInTransition()
        {
            if (!SubmergedLoaded) return false;
            return (bool)InTransitionField.GetValue(null);
        }


        public static void RepairOxygen()
        {
            if (!SubmergedLoaded) return;
            try
            {
                Ship().RpcUpdateSystem((SystemTypes)130, 64);
                RepairDamageMethod.Invoke(SubmarineOxygenSystemInstanceField.GetValue(null), new object[] { PlayerControl.LocalPlayer, 64 });
            }
            catch (System.NullReferenceException)
            {
                
            }

        }

        public static bool IsSubmerged() => SubmergedLoaded && Ship() && Ship().Type == SUBMERGED_MAP_TYPE;
    }

    public class MissingSubmergedBehaviour : MonoBehaviour
    {
        static MissingSubmergedBehaviour() => ClassInjector.RegisterTypeInIl2Cpp<MissingSubmergedBehaviour>();
        public MissingSubmergedBehaviour(IntPtr ptr) : base(ptr) { }
    }
}