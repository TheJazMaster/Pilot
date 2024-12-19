using System;
using System.Collections.Generic;

namespace TheJazMaster.Pilot;

public interface IDraculaApi
{
	void RegisterBloodTapOptionProvider(Status status, Func<State, Combat, Status, List<CardAction>> provider);
	void RegisterBloodTapOptionProvider(IBloodTapOptionProvider provider, double priority = 0);
}

public interface IBloodTapOptionProvider
{
	IEnumerable<Status> GetBloodTapApplicableStatuses(State state, Combat combat, IReadOnlySet<Status> allStatuses);
	IEnumerable<List<CardAction>> GetBloodTapOptionsActions(State state, Combat combat, IReadOnlySet<Status> allStatuses);
}