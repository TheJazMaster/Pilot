// AMove
using System;
using System.Collections.Generic;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class ACampanellaAttack : CardAction
{
	public required AAttack attack;
	public Campanella? campanella = null;

	public override void Begin(G g, State s, Combat c)
	{
		timer = 0;
		if (campanella != null) {
			attack.fromDroneX = campanella.x;
			c.QueueImmediate(attack);
		} else {
			foreach (Campanella campanella in CampanellaManager.GetCampanellas(c, attack.targetPlayer)) {
				var newAttack = Mutil.DeepCopy(attack);
				newAttack.fromDroneX = campanella.x;
				c.QueueImmediate(newAttack);
			}
		}
	}


	public override Icon? GetIcon(State s) => null;
	

	public override List<Tooltip> GetTooltips(State s) => [];
}
