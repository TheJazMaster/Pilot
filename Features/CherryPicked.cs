using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace TheJazMaster.Pilot.Features;

static class CherryPickedManager
{
	internal static ModEntry Instance => ModEntry.Instance;
	internal static IModCards CardsHelper => ModEntry.Instance.Helper.Content.Cards;

	internal static ICardTraitEntry CherryPickedTrait { get; private set; } = null!;

	public static void Initialize(IPluginPackage<IModManifest> package, IModHelper helper) {
		CherryPickedTrait = ModEntry.Instance.Helper.Content.Cards.RegisterTrait("CherryPicked", new() {
            Icon = (s, _) => ModEntry.Instance.CherryPickedIcon,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["trait", "cherryPicked"]).Localize,
            Tooltips = (s, _) => [
                new GlossaryTooltip($"trait.{MethodBase.GetCurrentMethod()!.DeclaringType!.Namespace!}::CherryPicked")
				{
					Icon = Instance.CherryPickedIcon,
					TitleColor = Colors.action,
					Title = ModEntry.Instance.Localizations.Localize(["trait", "cherryPicked", "name"]),
					Description = ModEntry.Instance.Localizations.Localize(["trait", "cherryPicked", "description"]),
				}
            ]
        });
	}
	
	public static void Return(State s, Combat c) {
		for (int i = 0; i < c.exhausted.Count; i++) {
			Card card = c.exhausted[i];
			if (CardsHelper.IsCardTraitActive(s, card, CherryPickedTrait)) {
				s.RemoveCardFromWhereverItIs(card.uuid);
				s.SendCardToDeck(card, true, true);
				i--;
			}
		}
	}
}
