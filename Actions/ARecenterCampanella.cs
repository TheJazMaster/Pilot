// AMove
using System;
using System.Collections.Generic;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class ARecenterCampanella : CardAction
{
	public override void Begin(G g, State s, Combat c)
	{
		Ship ship = s.ship;
		int missilesIndex = ship.parts.FindIndex((Part p) => p.type == PType.missiles && p.active);
		if (missilesIndex == -1)
		{
			timer = 0;
			return;
		}
		timer *= 0.5;

		CampanellaManager.MoveCampanella(s, c, missilesIndex + ship.x);
	}

	public override Icon? GetIcon(State s) => null;

	public override List<Tooltip> GetTooltips(State s) => [];
}
