using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using TheJazMaster.Pilot.Cards;
using TheJazMaster.Pilot.Artifacts;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null!;

    internal Harmony Harmony { get; }
	internal IKokoroApi KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; }
	internal IDraculaApi? DraculaApi { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal IPlayableCharacterEntryV2 PilotCharacter { get; }

    internal Deck PilotDeck { get; }

	internal Status RunAndGunStatus { get; }
	internal Status PAceStatus { get; }
    internal Status PShieldStatus { get; }
	internal Status CoffeeStatus { get; }
	internal Status SwerveStatus { get; }

    internal Spr PilotPortrait { get; }
    internal Spr PilotPortraitMini { get; }
    internal Spr PilotFrame { get; }
    internal Spr PilotCardBorder { get; }

	internal Spr CampanellaSprite { get; }
	internal Spr Campanella2Sprite { get; }
	internal Spr[] CampanellaParticles { get; }
	internal Spr CampanellaShield { get; }
	internal Spr Campanella2Shield { get; }

	internal Spr CampanellaIcon { get; }
    internal Spr GoCampanellaIcon { get; }
	internal Spr CherryPickedIcon { get; }
	internal Spr AddCherryPickedIcon { get; }

	internal Spr BlastoffCardArt { get; }
	internal Spr CupOJoeCardArt { get; }
	internal Spr CoffeeBreakCardArt { get; }
	internal Spr ReflexiveCoatingCardArt { get; }
	internal Spr DriftCardArt { get; }
	internal Spr BootSystemCardArt { get; }
	internal Spr TrialCardArt { get; }
	internal Spr VengeanceCardArt { get; }
	internal Spr DualShieldCardArt { get; }
	internal Spr SwerveShotCardArt { get; }
	internal Spr KeepPaceCardArt { get; }
	internal Spr SwivelCardArt { get; }
	internal Spr RunAndGunCardArt { get; }


	internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
		typeof(BlastoffCard),
		typeof(SwerveShotCard),
		typeof(RallyCard),
		typeof(BootSystemCard),
		typeof(SwivelCard),
		typeof(CupOJoeCard),
		typeof(QuickLaunchCard),
		typeof(DualShieldCard),
        typeof(DriftCard),

		typeof(PilotExeCard)
	];

	internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
		typeof(TheTrialOfArrowCard),
		typeof(TheTrialOfBombCard),
		typeof(TheTrialOfStoneCard),
		typeof(HotJavaCard),
		typeof(VengeanceCard),
		typeof(CoffeeBreakCard),
		typeof(MoonShardAlloyCard),
	];

	internal static IReadOnlyList<Type> RareCardTypes { get; } = [
		typeof(KeepPaceCard),
		typeof(ChannelCard),
		typeof(ReflexiveCoatingCard),
		typeof(SharingIsCaringCard),
		typeof(RunAndGunCard),
	];


	internal static IReadOnlyList<Type> SecretCardTypes { get; } = [
		typeof(TakeThatCard),
	];

    internal static IEnumerable<Type> AllCardTypes
		=> CommonCardTypes
			.Concat(UncommonCardTypes)
			.Concat(RareCardTypes)
			.Concat(SecretCardTypes);

    internal static IReadOnlyList<Type> CommonArtifacts { get; } = [
		typeof(CaffeinePillArtifact),
		typeof(CherryPieArtifact),
		typeof(YoYoArtifact),
		typeof(NailGunArtifact),
	];

	internal static IReadOnlyList<Type> BossArtifacts { get; } = [
		typeof(SeeingRedArtifact),
		typeof(IsabellArtifact),
	];

	internal static IEnumerable<Type> AllArtifactTypes
		=> CommonArtifacts.Concat(BossArtifacts);

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties")!;
		DraculaApi = helper.ModRegistry.GetApi<IDraculaApi>("Shockah.Dracula")!;
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;

        Harmony.PatchAll();

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

		PilotFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Panel.png")).Sprite;
        PilotCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Cards/CardBorder.png")).Sprite;

        CoffeeStatus = helper.Content.Statuses.RegisterStatus("Coffee", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Coffee.png")).Sprite,
				color = new("471F16"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Coffee", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Coffee", "description"]).Localize,
		}).Status;
		DraculaApi?.RegisterBloodTapOptionProvider(CoffeeStatus, (_, _, status) => [
            new AHurt { targetPlayer = true, hurtAmount = 1 },
            new AStatus { targetPlayer = true, status = status, statusAmount = 2 }
        ]);

        SwerveStatus = helper.Content.Statuses.RegisterStatus("Swerve", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Swerve.png")).Sprite,
				color = new("D10C4D"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Swerve", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Swerve", "description"]).Localize
		}).Status;
		DraculaApi?.RegisterBloodTapOptionProvider(SwerveStatus, (_, _, status) => [
            new AHurt { targetPlayer = true, hurtAmount = 1 },
            new AStatus { targetPlayer = true, status = status, statusAmount = 3 }
        ]);

        RunAndGunStatus = helper.Content.Statuses.RegisterStatus("RunAndGun", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/RunAndGun.png")).Sprite,
				color = new("D71751"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "RunAndGun", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "RunAndGun", "description"]).Localize
		}).Status;
		DraculaApi?.RegisterBloodTapOptionProvider(RunAndGunStatus, (_, _, status) => [
            new AHurt { targetPlayer = true, hurtAmount = 2 },
            new AStatus { targetPlayer = true, status = status, statusAmount = 1 },
            new AStatus { targetPlayer = true, status = SwerveStatus, statusAmount = 2 }
        ]);

        PAceStatus = helper.Content.Statuses.RegisterStatus("PAce", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/PAce.png")).Sprite,
				color = new("FE6270"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "PAce", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "PAce", "description"]).Localize
		}).Status;
		DraculaApi?.RegisterBloodTapOptionProvider(PAceStatus, (_, _, status) => [
            new AHurt { targetPlayer = true, hurtAmount = 1 },
            new AStatus { targetPlayer = true, status = status, statusAmount = 1 }
        ]);

        PShieldStatus = helper.Content.Statuses.RegisterStatus("PShield", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/PShield.png")).Sprite,
				color = new("FE6270"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "PShield", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "PShield", "description"]).Localize
		}).Status;
		DraculaApi?.RegisterBloodTapOptionProvider(PShieldStatus, (_, _, status) => [
            new AHurt { targetPlayer = true, hurtAmount = 1 },
            new AStatus { targetPlayer = true, status = status, statusAmount = 3 }
        ]);

		CampanellaSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Campanella.png")).Sprite;
		Campanella2Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Campanella2.png")).Sprite;
		CampanellaParticles = [
			helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Particle1.png")).Sprite,
			helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Particle2.png")).Sprite,
			helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Particle3.png")).Sprite,
			helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Particle4.png")).Sprite,
			0
		];
		CampanellaParticles[4] = CampanellaParticles[3];
		CampanellaShield = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Bubble.png")).Sprite;
		Campanella2Shield = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Midrow/Bubble2.png")).Sprite;
		
		CampanellaIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Campanella.png")).Sprite;
		GoCampanellaIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/GoCampanella.png")).Sprite;
		CherryPickedIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/CherryPicked.png")).Sprite;
		AddCherryPickedIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/AddCherryPicked.png")).Sprite;

		BlastoffCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/Blastoff.png")).Sprite;
		TrialCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/Trial.png")).Sprite;
		BootSystemCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/BootSystem.png")).Sprite;
		CoffeeBreakCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/CoffeeBreak.png")).Sprite;
		CupOJoeCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/CupOJoe.png")).Sprite;
		ReflexiveCoatingCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/ReflexiveCoating.png")).Sprite;
		VengeanceCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/Vengeance.png")).Sprite;
		DriftCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/Drift.png")).Sprite;
		DualShieldCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/DualShield.png")).Sprite;
		SwerveShotCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/SwerveShot.png")).Sprite;
		KeepPaceCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/KeepPace.png")).Sprite;
		SwivelCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/Swivel.png")).Sprite;
		RunAndGunCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Cards/RunAndGun.png")).Sprite;
		
		PilotDeck = helper.Content.Decks.RegisterDeck("Pilot", new()
		{
			Definition = new() { color = new Color("fe626e"), titleColor = Colors.black },
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = PilotCardBorder,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		}).Deck;

        foreach (var cardType in AllCardTypes)
			AccessTools.DeclaredMethod(cardType, nameof(IPilotCard.Register))?.Invoke(null, [helper]);
		foreach (var artifactType in AllArtifactTypes)
			AccessTools.DeclaredMethod(artifactType, nameof(IPilotArtifact.Register))?.Invoke(null, [helper]);

		MoreDifficultiesApi?.RegisterAltStarters(PilotDeck, new StarterDeck {
            cards = {
				new RallyCard(),
				new BootSystemCard()
            }
        });
		
		StatusManager.Initialize(package, helper);
		CherryPickedManager.Initialize(package, helper);
		CampanellaManager.Initialize(package, helper);

		ICardTraitEntry exhaust = Helper.Content.Cards.ExhaustCardTrait;
		helper.Content.Cards.OnGetFinalDynamicCardTraitOverrides += (_, data) => {
			foreach (Artifact item in data.State.EnumerateAllArtifacts()) {
				if (item is SeeingRedArtifact) {
					if (!data.TraitStates[exhaust].IsActive) {
						data.SetOverride(exhaust, true);
						data.SetOverride(CherryPickedManager.CherryPickedTrait, true);
					}
					break;
				}
			}
		};
		
        PilotCharacter = helper.Content.Characters.V2.RegisterPlayableCharacter("Pilot", new()
		{
			Deck = PilotDeck,
			Description = AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = PilotFrame,
			Starters = new StarterDeck {
				cards = [
					new BlastoffCard(),
					new SwerveShotCard()
				]
			},
			ExeCardType = typeof(PilotExeCard),
			NeutralAnimation = RegisterAnimation(helper, "Neutral").Configuration,
			MiniAnimation = RegisterAnimation(helper, "Mini").Configuration
		});

		RegisterAnimation(helper, "Squint");
		RegisterAnimation(helper, "Gameover");
    }

	private ICharacterAnimationEntryV2 RegisterAnimation(IModHelper helper, string name)
    {
        var files = Instance.Package.PackageRoot.GetRelative($"Sprites/Character").AsDirectory!.GetFilesRecursively()
			.Where(f => f.Name.Contains($"Pilot{name}") && f.Name.EndsWith(".png"));

		List<Spr> sprites = [];
		if (files != null) {
			foreach (IFileInfo file in files) {
				sprites.Add(Instance.Helper.Content.Sprites.RegisterSprite(file).Sprite);
			}
		}
		
		return helper.Content.Characters.V2.RegisterCharacterAnimation(name, new()
		{
			CharacterType = PilotDeck.Key(),
			LoopTag = name.ToLower(),
			Frames = sprites
		});
    }
}