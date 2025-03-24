// AMove
using System;
using System.Collections.Generic;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class ARecenterCampanella : CardAction
{
	public int? index;

	public override void Begin(G g, State s, Combat c)
	{
		Ship ship = s.ship;
		if (!index.HasValue) index = ship.parts.FindIndex((Part p) => p.type == PType.missiles && p.active);
		if (index.Value == -1)
		{
			timer = 0;
			return;
		}
		timer *= 0.5;

		if (!CampanellaManager.MoveCampanella(s, c, index.Value + ship.x)) timer = 0;
	}

	public override Icon? GetIcon(State s) => null;

	public override List<Tooltip> GetTooltips(State s) => [];
}
