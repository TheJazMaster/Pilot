using Nickel;

namespace TheJazMaster.Pilot;

internal interface IPilotCard
{
	static abstract void Register(IModHelper helper);
}

internal interface IPilotArtifact
{
	static abstract void Register(IModHelper helper);
}