// AMove
using System.Collections.Generic;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class ADestroyCampanella : CardAction
{
	public bool spawnThree;
	public StuffBase? thingToSpawn;
	public Campanella? campanella = null;

	public override void Begin(G g, State s, Combat c)
	{
		List<int> positions = CampanellaManager.DestroyCampanella(s, c, null, campanella);
		if (thingToSpawn != null) {
			foreach (int position in positions) {
				for (int i = spawnThree ? -1 : 0; i <= (spawnThree ? 1 : 0); i++) {
					if (!c.stuff.ContainsKey(position + i)) {
						StuffBase thing = Mutil.DeepCopy(thingToSpawn);
						thing.x = position + i; thing.xLerped = thing.x;
						c.stuff.Add(position + i, thing);
					}
				}
			}
		}
	}

	public override Icon? GetIcon(State s) => null;
	

	public override List<Tooltip> GetTooltips(State s) => [
		new CustomTTGlossary(
				CustomTTGlossary.GlossaryType.midrow,
				() => ModEntry.Instance.CampanellaIcon,
				() => ModEntry.Instance.Localizations.Localize(["midrow", "campanella", "name"]),
				() => ModEntry.Instance.Localizations.Localize(["midrow", "campanella", "description"]),
				key: "midrow.campanella"),
		.. thingToSpawn?.GetTooltips() ?? []
	];
}
