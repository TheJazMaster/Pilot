// AMove
using System;
using System.Collections.Generic;
using System.Linq;
using TheJazMaster.Pilot.Cards;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class AGoCampanella : DynamicWidthCardAction
{

	public override void Begin(G g, State s, Combat c)
	{
		if (!CampanellaManager.IsCampanellaAllowed(s, c)) {
			c.QueueImmediate(new ARecenterCampanella());
			c.QueueImmediate(new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = 1,
				targetPlayer = true
			});
		} else {
			bool second = CampanellaManager.GetCampanellas(c).Count > 0;
			c.QueueImmediate(new ASpawn {
				thing = new Campanella {
					skin = second ? ModEntry.Instance.Campanella2Sprite : ModEntry.Instance.CampanellaSprite,
					color = second ? new("C8C8C8") : new("fe626e"),
					bubbleSkin = second ? ModEntry.Instance.Campanella2Shield : ModEntry.Instance.CampanellaShield,
				},
				fromPlayer = true,
				multiBayVolley = true
			});
			foreach (Card card in c.hand.Concat(s.deck).Concat(c.discard)) {
				if (card is CoffeeBreakCard) c.QueueImmediate(new AExhaustOtherCard {
					uuid = card.uuid
				});
			}
		}
	}

	public override Icon? GetIcon(State s) {
		return new Icon(ModEntry.Instance.GoCampanellaIcon, null, Colors.textMain);
	}

	public override List<Tooltip> GetTooltips(State s) {
		if (s.route is Combat combat && !CampanellaManager.IsCampanellaAllowed(s, combat)) {
			foreach (StuffBase value in combat.stuff.Values)
			{
				if (value is Campanella)
				{
					value.hilight = 2;
				}
			}
		}

		foreach (Part part in s.ship.parts)
		{
			if (part.type == PType.missiles && part.active)
			{
				part.hilight = true;
				break;
			}
		}

		return [
			new CustomTTGlossary(
				CustomTTGlossary.GlossaryType.action,
				() => ModEntry.Instance.GoCampanellaIcon,
				() => ModEntry.Instance.Localizations.Localize(["action", "goCampanella", "name"]),
				() => ModEntry.Instance.Localizations.Localize(["action", "goCampanella", "description"]),
				key: "action.goCampanella"),
			new TTGlossary("action.spawn"),
		];
	}
}
