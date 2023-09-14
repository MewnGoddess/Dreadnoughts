using System;
using System.Linq;
using Vehicles;
using RimWorld;
using SmashTools;
using Verse;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(VehicleTurret))]
[HarmonyPatch("TurretRotationTick")]
public static class VehicleTurret_TurretRotationTick
{

    public static void Postfix(VehicleTurret __instance)
    {
        if (__instance.key != "pivotGun") return;
        //TODO 判斷砲塔是主砲(應該是算到grende出錯)
        // /45 = 八角
        if (Mathf.Abs(__instance.vehicle.Angle / 22.5f - __instance.TurretRotation / 22.5f) >= 1)//如果差距大於22.5度
        {
            Log.Warning("" + Mathf.Abs(__instance.vehicle.Angle - __instance.TurretRotation));
            int rotation = (int)Mathf.Abs(__instance.vehicle.Angle - __instance.TurretRotation) / 45;
            __instance.vehicle.FullRotation = Rot8.FromAngle(rotation * 45);
        }
    }
}

namespace Motorization
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main()
        {
            new Harmony("Aoba.MZ.MobileWorker").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
    public class VehiclePivotTurret : VehicleTurret
    {
        protected override bool TurretRotationTick()
        {
            if (currentRotation != rotationTargeted)
            {
                float num = currentRotation + 90f;
                float num2 = rotationTargeted + 90f;
                if (num < 0f)
                {
                    num += 360f;
                }
                else if (num > 360f)
                {
                    num -= 360f;
                }

                if (num2 < 0f)
                {
                    num2 += 360f;
                }
                else if (num2 > 360f)
                {
                    num2 -= 360f;
                }

                if (Math.Abs(num - num2) < turretDef.rotationSpeed)
                {
                    currentRotation = rotationTargeted;
                }
                else
                {
                    int num3 = ((num < num2) ? ((Math.Abs(num - num2) < 180f) ? 1 : (-1)) : ((!(Math.Abs(num - num2) < 180f)) ? 1 : (-1)));
                    currentRotation += turretDef.rotationSpeed * (float)num3;
                    //foreach (VehicleTurret childTurret in childTurrets)
                    //{
                    //    childTurret.currentRotation += turretDef.rotationSpeed * (float)num3;
                    //}
                }

                return true;
            }
            return false;
        }
    }
}
