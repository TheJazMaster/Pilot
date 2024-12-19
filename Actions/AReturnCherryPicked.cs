// AMove
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class AReturnCherryPicked : CardAction
{
	public override void Begin(G g, State s, Combat c)
	{
		CherryPickedManager.Return(s, c);
	}

	public override Icon? GetIcon(State s) {
		return null;
	}
}
