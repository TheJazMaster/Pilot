using Nickel;
using TheJazMaster.Pilot.Actions;
using System.Collections.Generic;
using System.Reflection;
using TheJazMaster.Pilot.Features;
using Microsoft.Extensions.Logging;

namespace TheJazMaster.Pilot.Cards;


internal sealed class BlastoffCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.BlastoffCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AGoCampanella(),
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AGoCampanella(),
			new AStatus {
				status = Status.droneShift,
				statusAmount = upgrade == Upgrade.A ? 2 : 1,
				targetPlayer = true
			}
		]
	};
}


internal sealed class SwerveShotCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.SwerveShotCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1)
		},
		new AStatus {
			status = ModEntry.Instance.SwerveStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		}
	];
}


internal sealed class RallyCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.BlastoffCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		retain = upgrade == Upgrade.A,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AGoCampanella(),
		new AAttack {
			damage = GetDmg(s, 3),
			piercing = upgrade == Upgrade.B
		}
	];
}


internal sealed class BootSystemCard : Card, IPilotCard, IHasCustomCardTraits
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.BootSystemCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry> { CherryPickedManager.CherryPickedTrait };

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.PShieldStatus,
			statusAmount = 2,
			targetPlayer = true
		},
		new AStatus {
			status = ModEntry.Instance.SwerveStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		}
	];
}


internal sealed class SwivelCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.SwivelCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		infinite = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.SwerveStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.evade,
				statusAmount = 1,
				targetPlayer = true
			},
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.SwerveStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true
			},
		]
	};
}


internal sealed class CupOJoeCard : Card, IPilotCard, IHasCustomCardTraits
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.CupOJoeCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) =>
		upgrade == Upgrade.A ? new HashSet<ICardTraitEntry> { CherryPickedManager.CherryPickedTrait } : [];

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.CoffeeStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
		new AStatus {
			status = ModEntry.Instance.SwerveStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
	];
}


internal sealed class QuickLaunchCard : Card, IPilotCard, IHasCustomCardTraits
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.BlastoffCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) =>
		upgrade == Upgrade.A ? new HashSet<ICardTraitEntry> { CherryPickedManager.CherryPickedTrait } : [];

	public override CardData GetData(State state) => new() {
		cost = 0,
		exhaust = true,
		buoyant = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AGoCampanella(),
			new AStatus {
				status = ModEntry.Instance.SwerveStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = 1,
				targetPlayer = true
			},
		],
		_ => [
			new AGoCampanella()
		]
	};
}


internal sealed class DualShieldCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.DualShieldCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.PShieldStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
		new AStatus {
			status = Status.shield,
			statusAmount = upgrade == Upgrade.A ? 2 : 1,
			targetPlayer = true
		},
	];
}


internal sealed class DriftCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.DriftCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		retain = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new ADroneMove {
				dir = -3
			},
			new AStatus {
				status = Status.droneShift,
				statusAmount = 1,
				targetPlayer = true
			},
		],
		_ => [
			new ADroneMove {
				dir = -3
			}
		]
	};
}


internal sealed class TheTrialOfArrowCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.TrialCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		retain = upgrade == Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", CardName, "description"]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ACampanellaAttack {
			attack = new AAttack {	
				damage = 4,
				piercing = true
			}
		},
		new ADestroyCampanella(),
	];
}


internal sealed class TheTrialOfBombCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.TrialCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		retain = upgrade == Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", CardName, "description"]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ADestroyCampanella {
			thingToSpawn = new SpaceMine {
				bigMine = true
			}
		},
	];
}


internal sealed class TheTrialOfStoneCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.TrialCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		retain = upgrade == Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", CardName, "description"]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ADestroyCampanella {
			spawnThree = true,
			thingToSpawn = new Asteroid()
		},
	];
}


internal sealed class HotJavaCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.CupOJoeCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.CoffeeStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
		new AStatus {
			status = ModEntry.Instance.SwerveStatus,
			statusAmount = upgrade == Upgrade.A ? 3 : 2,
			targetPlayer = true
		}
	];
}


internal sealed class VengeanceCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.VengeanceCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		exhaust = true,
		description = ModEntry.Instance.Localizations.Localize(["card", CardName, "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAddCard {
			card = new TakeThatCard {
				upgrade = upgrade == Upgrade.B ? Upgrade.B : Upgrade.None
			},
			destination = CardDestination.Hand
		}
	];
}


internal sealed class CoffeeBreakCard : Card, IPilotCard, IHasCustomCardTraits
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.CoffeeBreakCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry> { CherryPickedManager.CherryPickedTrait };

	public override CardData GetData(State state) => new() {
		cost = 0,
		description = ModEntry.Instance.Localizations.Localize(["card", CardName, "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.CoffeeStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.SwerveStatus,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.CoffeeStatus,
				statusAmount = upgrade == Upgrade.B ? 2 : 1,
				targetPlayer = true
			},
		],
	};
}


internal sealed class MoonShardAlloyCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.BootSystemCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 2 : 1,
		exhaust = upgrade != Upgrade.B,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = 4,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.SwerveStatus,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = 4,
				targetPlayer = true
			},
		],
	};
}


internal sealed class KeepPaceCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.KeepPaceCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.PAceStatus,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.CoffeeStatus,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.PAceStatus,
				statusAmount = 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class ChannelCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.BootSystemCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 2 : 1,
		retain = upgrade != Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", CardName, "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = s.ship.Get(Status.shield),
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.SwerveStatus,
				statusAmount = s.ship.Get(Status.evade),
				targetPlayer = true
			},
		],
		Upgrade.A => [
			new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = s.ship.Get(Status.shield),
				targetPlayer = true
			},
			new AStatus {
				status = Status.shield,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.SwerveStatus,
				statusAmount = s.ship.Get(Status.evade),
				targetPlayer = true
			},
			new AStatus {
				status = Status.evade,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.CoffeeStatus,
				statusAmount = 1,
				targetPlayer = true
			},
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = s.ship.Get(Status.shield),
				targetPlayer = true
			},
			new AStatus {
				status = Status.shield,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.SwerveStatus,
				statusAmount = s.ship.Get(Status.evade),
				targetPlayer = true
			},
			new AStatus {
				status = Status.evade,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
		],
	};
}


internal sealed class ReflexiveCoatingCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.ReflexiveCoatingCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		retain = upgrade == Upgrade.A,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = Status.reflexiveCoating,
			statusAmount = 1,
			targetPlayer = true
		},
		new AStatus {
			status = Status.shield,
			statusAmount = upgrade == Upgrade.B ? 3 : 1,
			targetPlayer = true
		},
	];
}


internal sealed class SharingIsCaringCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		floppable = true,
		art = flipped ? StableSpr.cards_Adaptability_Bottom : StableSpr.cards_Adaptability_Top,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.SwerveStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true,
			disabled = flipped
		},
		new AStatus {
			status = ModEntry.Instance.PShieldStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true,
			disabled = flipped
		},
		new ADummyAction(),
		new AStatus {
			status = Status.evade,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true,
			disabled = !flipped
		},
		new AStatus {
			status = Status.shield,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true,
			disabled = !flipped
		},
	];
}


internal sealed class RunAndGunCard : Card, IPilotCard, IHasCustomCardTraits
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.RunAndGunCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) =>
		upgrade == Upgrade.A ? new HashSet<ICardTraitEntry> { CherryPickedManager.CherryPickedTrait } : [];

	public override CardData GetData(State state) => new() {
		cost = 3,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.RunAndGunStatus,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
	];
}




internal sealed class TakeThatCard : Card, IPilotCard, IHasCustomCardTraits
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			Art = ModEntry.Instance.SwerveShotCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry> { CherryPickedManager.CherryPickedTrait };

	public override CardData GetData(State state) => new() {
		cost = 0,
		exhaust = true,
		temporary = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AAttack {
			damage = GetDmg(s, upgrade == Upgrade.B ? 3 : 2)
		},
		new ADrawCard {
			count = upgrade == Upgrade.A ? 2 : 1
		}
	];
}


internal sealed class PilotExeCard : Card, IPilotCard
{
	private static string CardName = null!;
	public static void Register(IModHelper helper) {
		CardName = MethodBase.GetCurrentMethod()!.DeclaringType!.Name[..^4];
		helper.Content.Cards.RegisterCard(CardName, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.PilotDeck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true
			},
			Art = ModEntry.Instance.CoffeeBreakCardArt,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", CardName, "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		description = ColorlessLoc.GetDesc(state, upgrade == Upgrade.B ? 3 : 2, ModEntry.Instance.PilotDeck),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new ACardOffering {
			amount = upgrade == Upgrade.B ? 3 : 2,
			limitDeck = ModEntry.Instance.PilotDeck,
			makeAllCardsTemporary = true,
			overrideUpgradeChances = false,
			canSkip = false,
			inCombat = true,
			discount = -1,
			dialogueSelector = ".summonPilot"
		}
	];
}