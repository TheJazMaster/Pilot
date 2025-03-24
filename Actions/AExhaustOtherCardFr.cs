// AMove
using System;
using System.Collections.Generic;
using FSPRO;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class AExhaustOtherCardFr : CardAction
{
	public int uuid;

	private const double delayTimer = 0.3;

	public override void Begin(G g, State s, Combat c)
	{
		timer = 0.0;
		Card? card = s.FindCard(uuid);
		if (card != null)
		{
			card.ExhaustFX();
			Audio.Play(Event.CardHandling);
			s.RemoveCardFromWhereverItIs(uuid);
			c.SendCardToExhaust(s, card);
			timer = delayTimer;
		}
	}


	public override Icon? GetIcon(State s) => null;
	

	public override List<Tooltip> GetTooltips(State s) => [];
}
