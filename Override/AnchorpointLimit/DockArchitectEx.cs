using TheForest.Buildings.Creation;

namespace BetterBlueprints.Override.AnchorpointLimit
{
    internal class DockArchitectEx : DockArchitect
    {
        protected override void Awake()
        {
            _maxPoints = BetterBlueprintsCore.MaxAnchorPoints;
            base.Awake();
        }
    }
}