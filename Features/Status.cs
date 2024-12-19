using System;
using System.Collections.Generic;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using TheJazMaster.Pilot.Actions;
using static TheJazMaster.Pilot.Features.CampanellaManager;

namespace TheJazMaster.Pilot.Features;

[HarmonyPatch]
public class StatusManager : IStatusLogicHook
{
    private static ModEntry Instance => ModEntry.Instance;

    public static void Initialize(IPluginPackage<IModManifest> package, IModHelper helper) {
        Instance.KokoroApi.RegisterStatusLogicHook(new StatusManager(), 1);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AAttack), nameof(AAttack.ApplyAutododge))]
    private static void AAttack_ApplyAutododge_Postfix(ref bool __result, AAttack __instance, Combat c, Ship target, RaycastResult ray) {
        if (!ray.hitShip || __instance.isBeam || __result == true || __instance.fromDroneX.HasValue) return;
        if (target.Get(ModEntry.Instance.SwerveStatus) <= 0) return;

        List<Campanella> campanellas = GetCampanellas(c, !target.isPlayerShip);
        
        foreach (Campanella campanella in campanellas) {
            int diff = Math.Abs(campanella.x - ray.worldX);
            if (diff <= GetCampanellasSwerveDistance(target)) {
                c.QueueImmediate([
                    new AStatus {
                        status = ModEntry.Instance.SwerveStatus,
                        statusAmount = -1,
                        targetPlayer = target.isPlayerShip,
                        timer = 0
                    },
                    new AMoveCampanella {
                        x = ray.worldX,
                        statusPulse = ModEntry.Instance.SwerveStatus,
                        campanella = campanella
                    },
                    __instance
                ]);
                __instance.timer = 0;
                __result = true;
                return;
            }
        }
    }

    public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
	{
        if (status == ModEntry.Instance.PAceStatus && amount > 0 && timing == StatusTurnTriggerTiming.TurnStart) {
            combat.Queue(new AStatus {
                status = ModEntry.Instance.SwerveStatus,
                statusAmount = amount,
                targetPlayer = ship.isPlayerShip,
                statusPulse = status
            });
        }
        return false;
    }
}