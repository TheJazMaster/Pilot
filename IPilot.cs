using System.Collections.Generic;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot;

public interface IPilotApi
{
	public Deck PilotDeck { get; }
	public Status SwerveStatus { get; }
	public Status PShieldStatus { get; }
	public Status CoffeeStatus { get; }
	public Status RunAndGun { get; }
	public Status PAceStatus { get; }

	public List<Campanella> GetCampanellas(Combat c, bool? targetPlayer = null);
	public bool IsCampanellaInvincible(State s, Combat c, bool targetPlayer);
	public bool IsCampanellaAllowed(State s, Combat c);
	public int CampanellaAllowedCount(State s, Combat c);
	public int GetCampanellasSwerveDistance(Ship s);
	public List<int> DestroyCampanella(State s, Combat c, bool? targetPlayer = null, Campanella? campanella = null);
	public bool MoveCampanella(State s, Combat c, int x, bool blastIntruders = false, bool? targetPlayer = null, Campanella? campanella = null);
}
