using ICities;

namespace SpawnPositionInverter
{
    public class SpawnPositionInverter : IUserMod
    {
        public string Name
        {
            get { return "Spawn/Unspawn Positions Swapper"; }
        }

        public string Description
        {
            get { return "Allows to swap trucks spawn and unspawn positions for cargo train and ship stations"; }
        }
    }
}
