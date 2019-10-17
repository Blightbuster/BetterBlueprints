using TheForest.Buildings.Creation;

namespace BetterBlueprints.Override.AnchorpointLimit
{
    internal class FoundationArchitectEx : FoundationArchitect
    {
        protected override void Awake()
        {
            _maxPoints = BetterBlueprintsCore.MaxAnchorPoints;
            base.Awake();
        }
    }
}