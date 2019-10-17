using TheForest.Buildings.Creation;

namespace BetterBlueprints.Override.AnchorpointLimit
{
    internal class RoofArchitectEx : RoofArchitect
    {
        protected override void Awake()
        {
            _maxPoints = BetterBlueprintsCore.MaxAnchorPoints;
            base.Awake();
        }
    }
}