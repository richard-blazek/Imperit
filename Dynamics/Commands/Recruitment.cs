using System.Collections.Generic;

namespace Imperit.Dynamics.Commands
{
    public class Recruitment : ICommand
    {
        public readonly int Player;
        public readonly int Land;
        public readonly State.IArmy Army;
        public Recruitment(int player, int land, State.IArmy army)
        {
            Player = player;
            Land = land;
            Army = army;
        }

        public (IAction[], State.Player) Perform(State.Player player, State.Provinces provinces)
        {
            return player.Id == Player
                ? (new[] { new Actions.Reinforcement(Land, Army) }, player.Pay(Army.Soldiers))
                : (System.Array.Empty<IAction>(), player);
        }
        public bool Allowed(IReadOnlyList<State.Player> players, State.Provinces provinces)
        {
            return provinces[Land].IsControlledBy(Player) && players[Player].Money >= Army.Soldiers && Army.Soldiers > 0;
        }
    }
}