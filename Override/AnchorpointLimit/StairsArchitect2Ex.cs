using TheForest.Buildings.Creation;

namespace BetterBlueprints.Override.AnchorpointLimit
{
    internal class StairsArchitect2Ex : StairsArchitect2
    {
        protected override void Awake()
        {
            _maxPoints = BetterBlueprintsCore.MaxAnchorPoints;
            base.Awake();
        }
    }
}