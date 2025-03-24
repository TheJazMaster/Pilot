// AMove
using System;
using System.Collections.Generic;
using System.Linq;
using TheJazMaster.Pilot.Cards;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class AGoCampanella : DynamicWidthCardAction
{
	public bool targetPlayer = false;
	public int? i;

	public override void Begin(G g, State s, Combat c)
	{
		Ship ship = targetPlayer ? c.otherShip : s.ship;

		if (i.HasValue) {

		}

		int allowedCount = CampanellaManager.CampanellaAllowedCount(s, c);
		if (allowedCount == 0) {
			c.QueueImmediate(new ARecenterCampanella());
			c.QueueImmediate(new AStatus {
				status = ModEntry.Instance.PShieldStatus,
				statusAmount = 1,
				targetPlayer = !targetPlayer
			});
		}
		else {
			Stack<int> indices = [];
			for (int i = 0, j = 0; i < ship.parts.Count && allowedCount - j > 0; i++) {
				Part part = ship.parts[i];
				if (!part.active || part.type != PType.missiles) continue;
				
				indices.Push(i);
				j++;
			}

			int cNum = 0;
			while (indices.Count > 0) {
				bool second = CampanellaManager.GetCampanellas(c).Count + indices.Count > 1;
				c.QueueImmediate(new ASpawn {
					thing = new Campanella {
						altSkin = second,
						color = second ? new("C8C8C8") : new("fe626e"),
						altBubbleSkin = second,
					},
					fromX = indices.Pop(),
					fromPlayer = !targetPlayer,
					multiBayVolley = true
				});
				cNum++;
			}
			foreach (Card card in c.hand.Concat(s.deck).Concat(c.discard)) {
				if (card is CoffeeBreakCard) c.QueueImmediate(new AExhaustOtherCardFr {
					uuid = card.uuid
				});
			}
		}
	}

	public override Icon? GetIcon(State s) {
		return new Icon(ModEntry.Instance.GoCampanellaIcon, null, Colors.textMain);
	}

	public override List<Tooltip> GetTooltips(State s) {
		if (s.route is Combat combat && CampanellaManager.CampanellaAllowedCount(s, combat) == 0) {
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
