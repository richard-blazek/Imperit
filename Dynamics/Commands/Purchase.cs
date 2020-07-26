using System.Collections.Generic;
using System.Linq;

namespace Imperit.Dynamics.Commands
{
    public class Purchase : ICommand
    {
        public readonly State.PlayerArmy Army;
        public readonly int Land;
        public readonly uint Price;
        public Purchase(State.PlayerArmy army, int land, uint price)
        {
            Army = army;
            Land = land;
            Price = price;
        }
        public bool Allowed(IReadOnlyList<State.Player> players, State.IProvinces provinces)
            => players[Army.Player.Id].Money >= Price && provinces.NeighborsOf(Land).Any(prov => prov is State.Land land && land.IsAllyOf(Army));

        public (IAction[], State.Province) Perform(State.Province province)
        {
            return province.Id == Land ? province.GiveUpTo(Army).Swap() : (System.Array.Empty<IAction>(), province);
        }
        public (IAction[], State.Player) Perform(State.Player player, State.IProvinces provinces)
        {
            return (System.Array.Empty<IAction>(), player == Army.Player ? player.Pay(Price) : player);
        }
    }
}