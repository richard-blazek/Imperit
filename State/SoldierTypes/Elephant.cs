﻿using System;
using System.Collections.Immutable;

namespace Imperit.State.SoldierTypes
{
	public class Elephant : SoldierType
	{
		public override Description Description { get; }
		public override int AttackPower { get; }
		public override int DefensePower { get; }
		public override int Weight { get; }
		public override int Price { get; }
		public int Capacity { get; }
		public int Speed { get; }
		public ImmutableArray<int> RecruitPlaces { get; }
		public Elephant(int id, Description description, int attackPower, int defensePower, int weight, int price, int capacity, int speed, ImmutableArray<int> recruitPlaces) : base(id)
		{
			Description = description;
			AttackPower = attackPower;
			DefensePower = defensePower;
			Weight = weight;
			Price = price;
			Capacity = capacity;
			Speed = speed;
			RecruitPlaces = recruitPlaces;
		}
		protected override IComparable Identity => (base.Identity, Capacity, Speed, RecruitPlaces);
		int Difficulty(Province to) => to is Land || to is Mountains ? 1 : Speed + 1;
		public override int CanMove(IProvinces provinces, int from, int to)
		{
			return provinces[from] is Land && provinces[to] is Land && provinces.Passable(from, to, Speed, (_, dest) => Difficulty(dest)) ? Weight + Capacity : 0;
		}
		public override int CanSustain(Province province) => province is Land ? Capacity + Weight : Weight;
		public override bool IsRecruitable(Province province) => RecruitPlaces.Contains(province.Id);
	}
}
