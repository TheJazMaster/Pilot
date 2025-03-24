using Nickel;
using TheJazMaster.Pilot.Features;

namespace TheJazMaster.Pilot.Actions;

public class ACardSelectAddCherryPicked : CardAction
{
	public static IModCards CardsHelper => ModEntry.Instance.Helper.Content.Cards;

	public override Route? BeginWithRoute(G g, State s, Combat c)
	{
		if (selectedCard != null)
		{
			CardsHelper.SetCardTraitOverride(s, selectedCard, CherryPickedManager.CherryPickedTrait, true, true);
			return new CustomShowCards
			{
				message = ModEntry.Instance.Localizations.Localize(["action", "addCherryPicked", "showCardText"]),
				cardIds = [selectedCard.uuid]
			};
		}
		return null;
	}

	public override string? GetCardSelectText(State s)
	{
		return ModEntry.Instance.Localizations.Localize(["action", "addCherryPicked", "cardSelectText"]);
	}
}