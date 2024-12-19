// AMove
using System;
using System.Collections.Generic;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class AKillOtherCampanellas : CardAction
{
	public bool wasPlayer;

	public override void Begin(G g, State s, Combat c)
	{
		foreach (Campanella campanella in CampanellaManager.GetCampanellas(c, true))
			c.DestroyDroneAt(s, campanella.x, wasPlayer);
		foreach (Campanella campanella in CampanellaManager.GetCampanellas(c, false))
			c.DestroyDroneAt(s, campanella.x, wasPlayer);
	}

	public override Icon? GetIcon(State s) {
		return null;
	}
}
