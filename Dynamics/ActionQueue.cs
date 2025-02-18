﻿using Imperit.State;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ActionSeq = System.Collections.Immutable.ImmutableSortedSet<Imperit.Dynamics.IAction>;

namespace Imperit.Dynamics
{
	public class ActionQueue : IEnumerable<IAction>
	{
		static readonly IComparer<IAction> cmp = new FnComparer<IAction, byte>(a => a.Priority, allowDuplicates: true);
		static readonly ActionSeq NoActions = ImmutableSortedSet.Create(cmp);
		readonly ActionSeq actions;
		public ActionQueue() => actions = NoActions;
		public ActionQueue(ActionSeq actions) => this.actions = actions;
		public ActionQueue(IEnumerable<IAction> actions) => this.actions = actions.ToImmutableSortedSet(cmp);
		public IEnumerator<IAction> GetEnumerator() => actions.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		static (ActionSeq, Province) ApplyAll(IReadOnlyList<IAction> actions, Province province, Player active)
		{
			return actions.Aggregate((actions: NoActions, province), (acc, action) =>
			{
				var (new_action, new_province) = action.Perform(acc.province, active);
				return (new_action is IAction ? acc.actions.Add(new_action) : acc.actions, new_province);
			});
		}
		static (ActionSeq, Player) ApplyAll(IReadOnlyList<IAction> actions, Player player, Player active, IProvinces provinces)
		{
			return actions.Aggregate((actions: NoActions, player), (acc, action) =>
			{
				var (new_action, new_player) = action.Perform(acc.player, active, provinces);
				return (new_action is IAction ? acc.actions.Add(new_action) : acc.actions, new_player);
			});
		}
		static (IReadOnlyList<IAction>, Provinces) ApplyAllToAll(IProvinces provinces, Player active, IReadOnlyList<IAction> actions)
		{
			var new_provinces = new Province[provinces.Count];
			foreach (var (i, province) in provinces.Index())
			{
				(actions, new_provinces[i]) = ApplyAll(actions, province, active);
			}
			return (actions, provinces.With(new_provinces));
		}
		static (IReadOnlyList<IAction>, Player[]) ApplyAllToAll(IReadOnlyList<Player> players, Player active, IProvinces provinces, IReadOnlyList<IAction> actions)
		{
			var new_players = new Player[players.Count];
			foreach (var (i, player) in players.Index())
			{
				(actions, new_players[i]) = ApplyAll(actions, player, active, provinces);
			}
			return (actions, new_players);
		}
		static (ActionQueue, IReadOnlyList<Player>, IProvinces) DoActions(IReadOnlyList<Player> players, IProvinces provinces, int active, IReadOnlyList<IAction> actions)
		{
			(actions, provinces) = ApplyAllToAll(provinces, players[active], actions);
			(actions, players) = ApplyAllToAll(players, players[active], provinces, actions);
			return (new ActionQueue(actions), players, provinces);
		}
		public (ActionQueue, IReadOnlyList<Player>, IProvinces) EndOfTurn(IReadOnlyList<Player> players, IProvinces provinces, int active)
		{
			return DoActions(players, provinces, active, actions);
		}
		(ActionSeq, bool) Interactions(ICommand command)
		{
			bool allowed = true;
			var new_actions = NoActions;
			int i;
			for (i = 0; i < actions.Count && allowed; ++i)
			{
				var (new_action, ok) = actions[i].Interact(command);
				new_actions = new_action != null ? new_actions.Add(new_action) : new_actions;
				allowed = allowed && ok;
			}
			return (new_actions.AddRange(actions.Skip(i)), allowed);
		}
		bool IsAllowed(ICommand cmd, IReadOnlyList<Player> players, IProvinces provinces)
		{
			return cmd.Allowed(players, provinces) && actions.All(action => action.Allows(cmd, players, provinces));
		}
		public (ActionQueue, Player[], Province[], bool) Add(ICommand command, IReadOnlyList<Player> players, IProvinces provinces)
		{
			if (IsAllowed(command, players, provinces))
			{
				var (actions1, new_players) = players.Select(player => command.Perform(player, provinces)).Unzip();
				var (actions2, new_provinces) = provinces.Select(province => command.Perform(province)).Unzip();
				var (new_actions, interacted) = Interactions(command);
				return (new ActionQueue(interacted ? new_actions.AddRange(actions1.Must()).AddRange(actions2.Must()) : new_actions), new_players.ToArray(), new_provinces.ToArray(), true);
			}
			return (this, players.ToArray(), provinces.ToArray(), false);
		}
	}
}
