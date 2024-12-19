// AMove
using System;
using System.Collections.Generic;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class AMoveCampanella : CardAction
{
	public required int x;
	public bool blastIntruders;
	public bool targetPlayer;
	public Campanella? campanella = null;

	public override void Begin(G g, State s, Combat c)
	{
		CampanellaManager.MoveCampanella(s, c, x, blastIntruders, targetPlayer, campanella);
	}

	public override Icon? GetIcon(State s) => null;

	public override List<Tooltip> GetTooltips(State s) => [];
}
