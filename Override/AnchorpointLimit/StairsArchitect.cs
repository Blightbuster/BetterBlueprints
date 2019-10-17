using TheForest.Buildings.Creation;

namespace BetterBlueprints.Override.AnchorpointLimit
{
    internal class StairsArchitectEx : StairsArchitect
    {
        protected override void Awake()
        {
            _maxPoints = BetterBlueprintsCore.MaxAnchorPoints;
            base.Awake();
        }
    }
}