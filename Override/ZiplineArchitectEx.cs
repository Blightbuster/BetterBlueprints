using TheForest.Buildings.Creation;
using TheForest.Utils;
using UnityEngine;

namespace BetterBlueprints.Override
{
    class ZiplineArchitectEx : ZiplineArchitect
    {
        private bool CheckLockGateInfinite()
        {
            bool flag = this._gate2.transform.parent && LocalPlayer.Create.BuildingPlacer.ClearOfCollision;
            if (flag)
            {
                bool flag2 = !this._gate1.transform.parent;
                if (flag2)
                {
                    float maxDistance = Vector3.Distance(this._gate1.transform.position, this._gate2.transform.position);
                    RaycastHit raycastHit;
                    if (Physics.SphereCast(this.Gate1RopePosition + Vector3.down, 1.5f, this.Gate2RopePosition - this.Gate1RopePosition, out raycastHit, maxDistance, LocalPlayer.Create.BuildingPlacer.FloorLayers | LayerMask.GetMask(new string[]
                    {
                        "treeMid",
                        "Blocker",
                        "PickUp"
                    }), QueryTriggerInteraction.Collide))
                    {
                        return false;
                    }
                }
                if (TheForest.Utils.Input.GetButtonDown("Fire1"))
                {
                    if (!flag2)
                    {
                        this._gate1.transform.parent = null;
                    }
                    else
                    {
                        this._gate2.transform.parent = null;
                        if (this._ziplineRoot)
                        {
                            UnityEngine.Object.Destroy(this._ziplineRoot.gameObject);
                        }
                        this._ziplineRoot = this.CreateZipline(this.Gate1RopePosition, this.Gate2RopePosition);
                    }
                }
            }
            return flag;
        }

        protected override void Update()
        {
            bool flag = !this._gate1.transform.parent;
            bool flag2 = !this._gate2.transform.parent;
            if (flag)
            {
                if (!flag2)
                {
                    Vector3 gate2RopePosition = this.Gate2RopePosition;
                    if (Vector3.Distance(this.Gate1RopePosition, gate2RopePosition) > 1f)
                    {
                        Transform ziplineRoot = this.CreateZipline(this.Gate1RopePosition, gate2RopePosition);
                        if (this._ziplineRoot)
                        {
                            UnityEngine.Object.Destroy(this._ziplineRoot.gameObject);
                        }
                        this._ziplineRoot = ziplineRoot;
                        if (!this._gate2.activeSelf)
                        {
                            this._gate2.SetActive(true);
                        }
                        this._gate1.transform.LookAt(new Vector3(this._gate2.transform.position.x, this._gate1.transform.position.y, this._gate2.transform.position.z));
                        this._gate2.transform.LookAt(this.Gate2LookAtTarget);
                    }
                    else if (this._gate2.activeSelf)
                    {
                        this._gate2.SetActive(false);
                    }
                }
            }
            else if (this._ziplineRoot)
            {
                UnityEngine.Object.Destroy(this._ziplineRoot.gameObject);
                this._ziplineRoot = null;
                this._ropePool.Clear();
            }
            // Stuff happends here
            bool flag3 = BetterBlueprintsCore.InfiniteZiplineToggle ? CheckLockGateInfinite() : CheckLockGate();

            this.CheckUnlockGate();
            bool flag4 = flag2;
            if (flag3 || flag4)
            {
                this.SetRopeMaterial(LocalPlayer.Create.BuildingPlacer.ClearMat);
                LocalPlayer.Create.BuildingPlacer.SetClear();
            }
            else
            {
                this.SetRopeMaterial(LocalPlayer.Create.BuildingPlacer.RedMat);
                LocalPlayer.Create.BuildingPlacer.SetNotclear();
            }
            if (LocalPlayer.Create.BuildingPlacer.Clear != flag4 || Scene.HudGui.RotateIcon.activeSelf == flag)
            {
                Scene.HudGui.RotateIcon.SetActive(!flag);
                LocalPlayer.Create.BuildingPlacer.Clear = flag4;
            }
            bool showManualfillLockIcon = !flag && flag3;
            bool canLock = flag && flag3;
            bool canUnlock = flag;
            Scene.HudGui.RoofConstructionIcons.Show(showManualfillLockIcon, false, false, flag4, canLock, canUnlock, false);
        }
    }
}