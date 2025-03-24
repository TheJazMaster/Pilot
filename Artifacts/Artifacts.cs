using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMOD;
using Nickel;
using TheJazMaster.Pilot.Actions;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Artifacts;

internal sealed class CaffeinePillArtifact : Artifact, IPilotArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.PilotDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "description"]).Localize
		});
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (combat.turn == 1) {
			combat.QueueImmediate(new AStatus
			{
				status = ModEntry.Instance.CoffeeStatus,
				statusAmount = 1,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		if (status == ModEntry.Instance.CoffeeStatus && statusAmount > 0 && mode == AStatusMode.Add) {
			combat.QueueImmediate(new AStatus
			{
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = 1,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}
	
	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(ModEntry.Instance.CoffeeStatus, 1),
		.. StatusMeta.GetTooltips(ModEntry.Instance.PShieldStatus, 1)
	];
}


internal sealed class CherryPieArtifact : Artifact, IPilotArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.PilotDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		state.GetCurrentQueue().QueueImmediate(new ACardSelect
		{
			browseAction = new ACardSelectAddCherryPicked(),
			browseSource = CardBrowse.Source.Deck,
			filterExhaust = true,
			filterTemporary = false
		});
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new CustomTTGlossary(
			CustomTTGlossary.GlossaryType.action,
			() => ModEntry.Instance.AddCherryPickedIcon,
			() => ModEntry.Instance.Localizations.Localize(["action", "addCherryPicked", "name"]),
			() => ModEntry.Instance.Localizations.Localize(["action", "addCherryPicked", "description"]),
			key: "action.addCherryPicked"),
		.. CherryPickedManager.CherryPickedTrait.Configuration.Tooltips!(DB.fakeState, null).ToList()
	];
}


internal sealed class YoYoArtifact : Artifact, IPilotArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.PilotDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "description"]).Localize
		});
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		List<Campanella> campanellas = CampanellaManager.GetCampanellas(combat);

		foreach (Campanella campanella in campanellas) {
			if (campanella.x < state.ship.x) {
				combat.Queue(new AMoveCampanella {
					x = campanella.x + 1,
					artifactPulse = Key(),
					campanella = campanella
				});
			}
			if (campanella.x >= state.ship.x + state.ship.parts.Count) {
				combat.Queue(new AMoveCampanella {
					x = campanella.x - 1,
					artifactPulse = Key(),
					campanella = campanella
				});
			}
		}
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		new Campanella().GetTooltips();
}


internal sealed class NailGunArtifact : Artifact, IPilotArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.PilotDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "description"]).Localize
		});
	}

	public override List<Tooltip>? GetExtraTooltips() =>
		new Campanella().GetTooltips();
}


internal sealed class SeeingRedArtifact : Artifact, IPilotArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.PilotDeck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		state.ship.baseEnergy++;
	}

	public override void OnRemoveArtifact(State state)
	{
		state.ship.baseEnergy--;
	}
	
	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("cardtrait.exhaust"),
		.. CherryPickedManager.CherryPickedTrait.Configuration.Tooltips!(DB.fakeState, null).ToList()
	];
}


internal sealed class IsabellArtifact : Artifact, IPilotArtifact
{
	private static string ArtifactName = null!;
	public static void Register(IModHelper helper)
	{
		ArtifactName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^8];
		helper.Content.Artifacts.RegisterArtifact(ArtifactName, new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.PilotDeck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"Sprites/Artifacts/{ArtifactName}.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", ArtifactName, "description"]).Localize
		});
	}

	public override void OnPlayerSpawnSomething(State state, Combat combat, StuffBase thing)
	{
		if (thing is Campanella) combat.Queue(new AStatus {
			status = ModEntry.Instance.PShieldStatus,
			statusAmount = 1,
			targetPlayer = true,
			artifactPulse = Key()
		});
	}



	
	public override List<Tooltip>? GetExtraTooltips() => [
		.. new Campanella().GetTooltips(),
		.. StatusMeta.GetTooltips(ModEntry.Instance.PShieldStatus, 1)
	];
}