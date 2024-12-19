using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FSPRO;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using TheJazMaster.Pilot.Actions;
using TheJazMaster.Pilot.Artifacts;

namespace TheJazMaster.Pilot.Features;

[HarmonyPatch]
static class CampanellaManager
{
	internal static ModEntry Instance => ModEntry.Instance;

	internal static readonly CampanellaParticles combatCampanella = new();

	public static void Initialize(IPluginPackage<IModManifest> package, IModHelper helper) {
		
	}

	public static bool IsCampanellaInvincible(State s, Combat c, bool forPlayer) {
		Ship ship = forPlayer ? s.ship : c.otherShip;
		return ship.Get(ModEntry.Instance.PShieldStatus) > 0;
	}
	
	public static List<Campanella> GetCampanellas(Combat c, bool? targetPlayer = null) {
		var pairs = c.stuff.Where(kvp => kvp.Value is Campanella && (targetPlayer == null || kvp.Value.targetPlayer == targetPlayer));
		return pairs.Select(kvp => (Campanella)kvp.Value).ToList();
	}

	public static int GetCampanellasSwerveDistance(Ship s) {
		return s.Get(ModEntry.Instance.CoffeeStatus) + 1;
	}
	public static int GetCampanellasSwerveDistance(State s) {
		return GetCampanellasSwerveDistance(s.ship);
	}

	public static bool IsCampanellaAllowed(State s, Combat c) {
		int max = 1;
		if (s.EnumerateAllArtifacts().Any(item => item is IsabellArtifact)) max++;

		return max - c.stuff.Where(thing => thing.Value is Campanella).Count() > 0;
	}

	public static bool MoveCampanella(State s, Combat c, int xToMoveTo, bool blastIntruders = false, bool? targetPlayer = null, Campanella? campanella = null) {
		List<Campanella> campanellas = campanella == null ? GetCampanellas(c, targetPlayer) : [campanella];

		foreach (Campanella camp in campanellas) {
			int x = xToMoveTo;
			int oldX = camp.x;		
			c.stuff.Remove(camp.x);
			if (!blastIntruders)
				while (c.stuff.ContainsKey(x)) {
					x++;
				}

			camp.x = x;
			c.DestroyDroneAt(s, x, !camp.targetPlayer);
			c.stuff.Add(x, camp);
			Audio.Play(Event.Move);
			if (x < c.leftWall || x >= c.rightWall)
			{
				Audio.Play(Event.Hits_HitDrone);
				c.DestroyDroneAt(s, x, true);
			} else {
				int runAndGun = s.ship.Get(ModEntry.Instance.RunAndGunStatus);
				if (runAndGun > 0 && oldX != camp.x) {
					c.QueueImmediate(new ACampanellaAttack {
						campanella = camp,
						attack = new AAttack {
							damage = runAndGun,
							fast = true,
							storyFromStrafe = true,
							targetPlayer = camp.targetPlayer,
							statusPulse = ModEntry.Instance.RunAndGunStatus
						}
					});
				}
			}
		}

		return true;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(ADroneMove), nameof(ADroneMove.Begin))]
	private static void ADroneMove_Begin_Postfix(G g, State s, Combat c, ADroneMove __instance) {
		foreach (Campanella camp in GetCampanellas(c)) {		
			int runAndGun = s.ship.Get(ModEntry.Instance.RunAndGunStatus);
			if (runAndGun > 0) {
				c.QueueImmediate(new ACampanellaAttack {
					campanella = camp,
					attack = new AAttack {
						damage = runAndGun,
						fast = true,
						storyFromStrafe = true,
						targetPlayer = camp.targetPlayer,
						statusPulse = ModEntry.Instance.RunAndGunStatus
					}
				});
			}
		}
	}

	public static List<int> DestroyCampanella(State s, Combat c, bool? targetPlayer = null, Campanella? campanella = null) {
		
		List<Campanella> campanellas = campanella == null ? GetCampanellas(c, targetPlayer) : [campanella];

		List<int> destroyed = [];
		foreach (Campanella camp in campanellas) {
			c.DestroyDroneAt(s, camp.x, true);
			Audio.Play(Event.Hits_DroneCollision);
			destroyed.Add(camp.x);
		}

		return destroyed;
	}

	// [HarmonyPostfix]
	// [HarmonyPatch(typeof(ASpawn), nameof(ASpawn.GetWorldX))]
	// private static void ASpawn_GetWorldX_Prefix(ASpawn __instance, ref int __result, State s, Combat c) {
	// 	if (__instance.thing is Campanella) {
	// 		while (c.stuff.ContainsKey(__result + __instance.offset)) {
	// 			if (c.rightWall.HasValue && __result >= c.rightWall) {
	// 				break;
	// 			}
	// 			__instance.offset++;
	// 		}
	// 	}
	// 	if (c.stuff.TryGetValue(__result + __instance.offset, out var thing) && thing is Campanella campanella) {
	// 		MoveCampanella(s, c, __result + __instance.offset + 1, campanella: campanella);
	// 	}
	// }

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Combat), nameof(Combat.RenderBehindCockpit))]
	private static void Combat_RenderBehindCockpit_Prefix(G g, Combat __instance) {
		Rect rect = default(Rect) + __instance.GetCamOffset() + Combat.arenaPos;
		Box box = g.Push(null, rect);
		combatCampanella.Render(g.dt, box.rect.xy);
		g.Pop();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(PFX), nameof(PFX.ClearAll))]
	private static void PFX_ClearAll_Postfix() {
		combatCampanella.Clear();
	}

	[HarmonyTranspiler]
	[HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
	private static IEnumerable<CodeInstruction> AMove_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions).Find(
                ILMatches.Ldloc<StuffBase>(originalMethod).CreateLdlocInstruction(out var ldLoc),
                ILMatches.Isinst(typeof(AttackDrone))
            )
			.EncompassUntil(SequenceMatcherPastBoundsDirection.After, [
				ILMatches.Stfld("pulse")
			])
			.Encompass(SequenceMatcherEncompassDirection.After, 1)
			.PointerMatcher(SequenceMatcherRelativeElement.Last).ExtractLabels(out var labels)
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
				ldLoc.Value.WithLabels(labels),
				new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CampanellaManager), nameof(PulseCampanella))),
			])
			.AllElements();
	}

	private static void PulseCampanella(StuffBase thing) {
		if (thing is Campanella) {
			thing.pulse = 1;
		}
	}
}



[HarmonyPatch]
public sealed class Campanella : StuffBase
{
	public Spr skin = ModEntry.Instance.CampanellaSprite;
	public Spr bubbleSkin = ModEntry.Instance.CampanellaShield;
	public Color color = new("fe626e");
	private double particlesToEmit = 0;

	public override List<Tooltip> GetTooltips() {
		List<Tooltip> tooltips = [
			new CustomTTGlossary(
				CustomTTGlossary.GlossaryType.midrow,
				() => ModEntry.Instance.CampanellaIcon,
				() => ModEntry.Instance.Localizations.Localize(["midrow", "campanella", "name"]),
				() => ModEntry.Instance.Localizations.Localize(["midrow", "campanella", "description"]),
				key: "midrow.campanella")
		];
		if (Invincible()) {
			tooltips.AddRange(StatusMeta.GetTooltips(ModEntry.Instance.PShieldStatus, MG.inst.g.state.ship.Get(ModEntry.Instance.PShieldStatus)));
		}
		if (bubbleShield) {
			tooltips.Add(new TTGlossary("midrow.bubbleShield"));
		}
		return tooltips;
	}

	public override Spr? GetIcon()
	{
		return ModEntry.Instance.CampanellaIcon;
	}

	public override string GetDialogueTag()
	{
		return "campanella";
	}

	public override List<CardAction>? GetActions(State s, Combat c)
	{
		if (!targetPlayer && s.EnumerateAllArtifacts().FirstOrDefault(item => item is NailGunArtifact) is { } artifact)
			return [
				new ACampanellaAttack {
					campanella = this,
					attack = new AAttack {
						targetPlayer = targetPlayer,
						damage = 1,
						artifactPulse = artifact.Key()
					}
				}
			];
		return [];
	}

	public override bool Invincible()
	{
		State s = MG.inst.g.state;
		if (s.route is Combat c) {
			return CampanellaManager.IsCampanellaInvincible(s, c, true);
		}
		return false;
	}

	private List<CardAction> GetActionsOnHurtWhileInvincible(State s, Combat c, bool wasPlayer) {
		return [
			new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = -1,
				timer = 0,
				targetPlayer = fromPlayer
			},
			new AMoveCampanella {
				x = x + 1,
				campanella = this
			},
		];	
	}

	public override List<CardAction>? GetActionsOnBonkedWhileInvincible(State s, Combat c, bool wasPlayer, StuffBase thing)
	{
		return GetActionsOnHurtWhileInvincible(s, c, wasPlayer);
	}

	public override List<CardAction>? GetActionsOnShotWhileInvincible(State s, Combat c, bool wasPlayer, int damage)
	{
		return GetActionsOnHurtWhileInvincible(s, c, wasPlayer);
	}

	public override double GetWiggleAmount()
	{
		return 1.0;
	}

	public override double GetWiggleRate()
	{
		return 1.0;
	}

	public override void Render(G g, Vec v)
	{
		double speed = xLerped - x;
		double absSpeed = Math.Abs(speed);
		particlesToEmit += g.dt/0.3 * (1 + 4 * absSpeed);
		while (particlesToEmit >= 1)
		{
			CampanellaManager.combatCampanella.Add(new CampanellaParticle
			{
				pos = new Vec(xLerped*16 + 12.5, v.y + 11.5 + (targetPlayer ? -7.5 : 7.5)) + GetOffset(g),
				sizeMode = SizeMode.Constant,
				vel = new Vec(speed*5, (targetPlayer ? -2 : 2) - absSpeed * 2),
				lifetime = 0.375
			});
			particlesToEmit -= 1;
		}
		DrawWithHilight(g, skin, v + GetOffset(g), false, targetPlayer);
	}

	public override List<CardAction>? GetActionsOnDestroyed(State s, Combat c, bool wasPlayer, int worldX)
	{
		return [
			new AStatus {
				status = ModEntry.Instance.CoffeeStatus,
				statusAmount = 0,
				targetPlayer = true,
				mode = AStatusMode.Set,
				timer = 0
			},
			new AStatus {
				status = ModEntry.Instance.RunAndGunStatus,
				statusAmount = 0,
				targetPlayer = true,
				mode = AStatusMode.Set,
				timer = 0
			},
			new AReturnCherryPicked(),
			new AKillOtherCampanellas {
				wasPlayer = wasPlayer
			}
		];
	}

	
	const double height = 24; const double length = 4;
	private static void DrawLine(G g, double x, double y, Color color) {
		double offset = (g.state.time * 6 % (2*length)) - 2*length;
		while (offset < height) {
			double alpha = Math.Min(1, (height/2 - Math.Abs(offset - height/2)) / 8);
			Draw.Line(x, y + offset - length/2, x, y + offset + length/2, 1, color.fadeAlpha(alpha));
			offset += 2*length;
		}
	}

	public void RenderStuff(G g) {
		Rect? rect = GetGetRect();
		Box box = g.Push(null, rect);
		Vec offset = GetOffset(g);
		if (Invincible()) {
			Draw.Sprite(bubbleSkin, box.rect.x - 5.0 + offset.x, box.rect.y + 3.0 + offset.y);

		}
		if (g.state.ship.Get(ModEntry.Instance.SwerveStatus) > 0) {
			int swerveDist = CampanellaManager.GetCampanellasSwerveDistance(g.state);
			DrawLine(g, box.rect.x - 0.5 - 16*swerveDist, box.rect.y + 1.0, color);
			DrawLine(g, box.rect.x + 15.5 + 16*swerveDist, box.rect.y + 1.0, color);
		}

		g.Pop();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Combat), nameof(Combat.RenderDroneShields))]
	private static void Combat_RenderDroneShields_Postfix(Combat __instance, G g) {
		Rect? rect = default(Rect) + Combat.arenaPos + __instance.GetCamOffset();
		g.Push(null, rect);
		List<Campanella> campanellas = [];
		foreach (Campanella campanella in CampanellaManager.GetCampanellas(__instance))
		{
			campanellas.Add(campanella);
			campanella.RenderStuff(g);
		}
		g.Pop();
	}
}


class CampanellaParticle : Particle {
	private static Spr[] CampanellaParticles => ModEntry.Instance.CampanellaParticles;
	public void ChangeSprite(double dt) {
		sprite = ModEntry.Instance.CampanellaParticles[Math.Min((int)(ageCoef * CampanellaParticles.Length), CampanellaParticles.Length - 1)];
	}
}

[HarmonyPatch]
class CampanellaParticles : ParticleSystem {
	[HarmonyPrefix]
	[HarmonyPatch(typeof(ParticleSystem), nameof(Render))]
	private static void ParticleSystem_Render_Prefix(ParticleSystem __instance, double dt, Vec? offset) {
		if (__instance is CampanellaParticles) {
			foreach (Particle particle in __instance.particles) {
				if (particle is CampanellaParticle cp) cp.ChangeSprite(dt);
			}
		}
	}
}