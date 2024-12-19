using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TheJazMaster.Pilot.Actions;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot;

public class ApiImplementation : IPilotApi
{

	public Deck PilotDeck => ModEntry.Instance.PilotDeck;
	public Status SwerveStatus => ModEntry.Instance.SwerveStatus;
	public Status PShieldStatus => ModEntry.Instance.PShieldStatus;
	public Status CoffeeStatus => ModEntry.Instance.CoffeeStatus;
	public Status RunAndGun => ModEntry.Instance.RunAndGunStatus;
	public Status PAceStatus => ModEntry.Instance.PAceStatus;

	public List<Campanella> GetCampanellas(Combat c, bool? targetPlayer = null) => CampanellaManager.GetCampanellas(c, targetPlayer);
	public bool IsCampanellaInvincible(State s, Combat c, bool targetPlayer) => CampanellaManager.IsCampanellaInvincible(s, c, targetPlayer);
	public bool IsCampanellaAllowed(State s, Combat c) => CampanellaManager.IsCampanellaAllowed(s, c);
	public int GetCampanellasSwerveDistance(Ship s) => CampanellaManager.GetCampanellasSwerveDistance(s);
	public List<int> DestroyCampanella(State s, Combat c, bool? targetPlayer = null, Campanella? campanella = null) => CampanellaManager.DestroyCampanella(s, c, targetPlayer, campanella);
	public bool MoveCampanella(State s, Combat c, int x, bool blastIntruders = false, bool? targetPlayer = null, Campanella? campanella = null) => CampanellaManager.MoveCampanella(s, c, x, blastIntruders, targetPlayer, campanella);

}
