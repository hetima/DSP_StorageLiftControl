using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HTStorageLiftControl
{
    [BepInPlugin(__GUID__, __NAME__, "1.0.0")]
    public class StorageLiftControl : BaseUnityPlugin
    {
        public const string __NAME__ = "StorageLiftControl";
        public const string __GUID__ = "com.hetima.dsp." + __NAME__;

        new internal static ManualLogSource Logger;
        void Awake()
        {
            Logger = base.Logger;
            //Logger.LogInfo("Awake");

            new Harmony(__GUID__).PatchAll(typeof(Patch));
        }


        public static StorageComponent NextStorageConsiderBans(StorageComponent sc)
        {
            if (sc.bans >= 1 && sc.bans <= 4)
            {
                return null;
            }

            return sc.nextStorage;
        }

        static class Patch
        {

            [HarmonyTranspiler, HarmonyPatch(typeof(PlanetFactory), "InsertInto")]
            public static IEnumerable<CodeInstruction> PlanetFactory_InsertInto_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                //storageComponent = storageComponent.nextStorage;
                //IL_02fe: ldloc.s storageComponent //そのまま引数に使える
                //IL_0300: ldfld   class StorageComponent StorageComponent::nextStorage
                //IL_0305: stloc.s storageComponent
                FieldInfo f = AccessTools.Field(typeof(StorageComponent), nameof(StorageComponent.nextStorage));
                MethodInfo m = typeof(StorageLiftControl).GetMethod(nameof(StorageLiftControl.NextStorageConsiderBans));

                foreach (var ins in instructions)
                {
                    if (ins.opcode == OpCodes.Ldfld && ins.operand is FieldInfo o && o == f)
                    {
                        yield return new CodeInstruction(OpCodes.Call, m);
                    }
                    else
                    {
                        yield return ins;
                    }
                }
            }

        }

    }
}
