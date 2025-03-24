using System;

namespace TheJazMaster.Pilot;

public interface IAppleShipyardApi
{
	void RegisterActionLooksForPartType(Type actionType, PType partType);
}