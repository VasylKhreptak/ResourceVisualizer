using Plugins.Banks.Integer;

namespace Infrastructure.Data.Persistent
{
    public class PlayerData
    {
        public readonly IntegerBank Coins = new IntegerBank(0);
    }
}