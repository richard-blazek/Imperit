﻿using System.Collections.Immutable;

namespace Imperit.State
{
	public class Savage : Player
	{
		public Savage(int id, ImmutableArray<SoldierType> types)
			: base(id, "", new Color(), new Password(""), 0, true, types) { }
		public override Player ChangeMoney(int amount) => new Savage(Id, SoldierTypes);
		public override Player Die() => new Savage(Id, SoldierTypes);
		public override Player ChangeIncome(int change) => new Savage(Id, SoldierTypes);
	}
}
