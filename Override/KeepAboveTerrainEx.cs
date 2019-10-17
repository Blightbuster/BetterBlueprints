using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;
using Input = TheForest.Utils.Input;

namespace BetterBlueprints.Override
{
    internal class KeepAboveTerrainEx : KeepAboveTerrain
    {
        //private float _size = 1;

        protected override void Awake()
        {
            base.Awake();
            maxBuildingHeight = 10000f;
            maxAirBorneHeight = 10000f;
        }

        public override void SetColliding()
        {
            base.SetColliding();
            if (BetterBlueprintsCore.BuildAnywhereToggle) ClearOfCollision = true;
        }

        protected override void Update()
        {
            /*
            // Controll blueprint size
            _size += UnityEngine.Input.mouseScrollDelta.y;
            var finalSize = Mathf.Abs(_size) * _size;
            // Modifie Size
            LocalPlayer.Create.CurrentGhost.transform.localScale = new Vector3(finalSize, finalSize, finalSize);
            transform.localScale = new Vector3(finalSize, finalSize, finalSize);
            ModAPI.Console.Write("Size: " + finalSize);
            */
            BlueprintSnap bps = BetterBlueprintsCore.BlueprintSnap;
            
            // This is all for rotation
            if (!TheForest.Utils.Input.IsGamePad)
            {
                if (TheForest.Utils.Input.GetButtonUp("Rotate"))
                {
                    turn = 0f;
                }
                else if (TheForest.Utils.Input.GetButton("Rotate"))
                {
                    turn = 60f * Time.deltaTime * (float)((!TheForest.Utils.Input.player.GetButtonTimedPress("Rotate", 0.5f)) ? 1 : 3);
                }

                // Reverse Rotate
                if (ModAPI.Input.GetButtonUp("ReverseRotateKey"))
                {
                    turn = 0f;
                }
                else if (ModAPI.Input.GetButton("ReverseRotateKey"))
                {
                    turn = 60f * Time.deltaTime * -1;
                }

            }
            else
            {
                float num = Mathf.Clamp(TheForest.Utils.Input.GetAxis("Rotate"), 0f, 1f);
                float axisTimeActive = TheForest.Utils.Input.player.GetAxisTimeActive("Rotate");
                if (!Mathf.Approximately(num, 0f))
                {
                    turn = num * 60f * Time.deltaTime * (float)((axisTimeActive <= 0.5f) ? 1 : 3);
                }
                else
                {
                    turn = 0f;
                }
            }
            curDir = (curDir + turn + 360f) % 360f;
            CurrentRotation = Quaternion.Euler(0f, curDir, 0f);

            // This is all for the placement of the blueprint
            if (!Locked && LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World)
            {
                //transform = LocalPlayer.Transform;  // Used for blueprint snapping
                Vector3 vector = transform.parent.TransformPoint(startLocalPosition);
                _shouldDoSmallStructureCheck = false;
                _shouldDoDynamicBuildingCheck = false;
                _lockDynamicBuilding = false;
                _shouldDoPreventConstructioCheck = false;

                // Bottom left coordinate of the default collision box
                Vector3 bl = boxCollider.center - boxCollider.size / 2f;
                // Upper right coordinate of default collision box
                Vector3 ur = boxCollider.center + boxCollider.size / 2f;

                // Defines the how large in percent the final collision box should be in relation to the d
                float cornersRaycastDistanceAlpha = LocalPlayer.Create.CurrentBlueprint._cornersRaycastDistanceAlpha;

                // Create corner vectors for final collision box. cBoxCorner[1-4] are all the lower corners of the box
                Vector3 cBoxCornerBL = transform.TransformPoint(Vector3.Lerp(boxCollider.center, bl, cornersRaycastDistanceAlpha));                              // 0,0,0
                Vector3 cBoxCornerTL = transform.TransformPoint(Vector3.Lerp(boxCollider.center, new Vector3(bl.x, bl.y, ur.z), cornersRaycastDistanceAlpha));   // 0,0,1
                Vector3 cBoxCornerBR = transform.TransformPoint(Vector3.Lerp(boxCollider.center, new Vector3(ur.x, bl.y, bl.z), cornersRaycastDistanceAlpha));   // 1,0,0
                Vector3 cBoxCornerTR = transform.TransformPoint(Vector3.Lerp(boxCollider.center, new Vector3(ur.x, bl.y, ur.z), cornersRaycastDistanceAlpha));   // 1,0,1

                // Increase maxBuildingHeight by 300 when player is in sinkhole
                sinkHolePos.y = transform.position.y;
                IsInSinkHole = (Vector3.Distance(sinkHolePos, vector) < 190f);
                float maxDistance = (!IsInSinkHole) ? maxBuildingHeight : (maxBuildingHeight + 300f);

                Vector3 position = LocalPlayer.MainCamTr.position;
                bool hitFloor = false;
                Vector3 point;
                if (MathEx.ClosestPointsOnTwoLines(out lookingAtPointCam, out point, position, LocalPlayer.MainCamTr.forward, vector, Vector3.up))
                {
                    float distanceToGhost = Vector3.Distance(position, lookingAtPointCam) * 0.96f;

                    // Make Blueprint be over ground
                    if (!LocalPlayer.IsInCaves && !IsInSinkHole)
                    {
                        point.y = Mathf.Max(point.y, Terrain.activeTerrain.SampleHeight(vector));
                    }

                    // Calculate ghost position
                    Vector3 ghostPosition;
                    if (IgnoreLookAtCollision || !Physics.Raycast(position, LocalPlayer.MainCamTr.forward, out hit, distanceToGhost, GetValidLayers(FloorLayersFinal.value)))
                    {
                        ghostPosition = LocalPlayer.MainCamTr.position + LocalPlayer.MainCamTr.forward * distanceToGhost;
                        LastHit = null;
                        if (Airborne)
                        {
                            cBoxCornerBL.y = point.y;
                            cBoxCornerTL.y = point.y;
                            cBoxCornerBR.y = point.y;
                            cBoxCornerTR.y = point.y;
                        }
                        else
                        {
                            cBoxCornerBL.y = point.y + 2f;
                            cBoxCornerTL.y = point.y + 2f;
                            cBoxCornerBR.y = point.y + 2f;
                            cBoxCornerTR.y = point.y + 2f;
                        }
                    }
                    else
                    {
                        ghostPosition = hit.point;
                        // Adjust y coordinate of hitpoint
                        float y;
                        if (!LocalPlayer.IsInCaves)
                        {
                            y = Mathf.Max(LocalPlayer.Transform.position.y + 2f, Terrain.activeTerrain.SampleHeight(vector) + 2f);
                        }
                        else
                        {
                            y = LocalPlayer.Transform.position.y + 2f;
                        }
                        LastHit = new RaycastHit?(hit);
                        point = hit.point;
                        if (hit.normal.y > 0f)
                        {
                            hitFloor = true;
                            point.y = y;
                            cBoxCornerBL.y = y;
                            cBoxCornerTL.y = y;
                            cBoxCornerBR.y = y;
                            cBoxCornerTR.y = y;
                        }
                        else if (!Airborne)
                        {
                            point.y += maxBuildingHeight / 2f;
                            cBoxCornerBL.y = hit.point.y + maxBuildingHeight / 2f;
                            cBoxCornerTL.y = hit.point.y + maxBuildingHeight / 2f;
                            cBoxCornerBR.y = hit.point.y + maxBuildingHeight / 2f;
                            cBoxCornerTR.y = hit.point.y + maxBuildingHeight / 2f;
                        }
                        ParentingRulesLookup();
                    }
                    // Horizontal plane vector which removes the y-component of a vector when scaled with
                    Vector3 hPlane = new Vector3(1f, 0f, 1f);

                    float ghostHorzDist = Vector3.Distance(Vector3.Scale(ghostPosition, hPlane), Vector3.Scale(LocalPlayer.Transform.position, hPlane));
                    float transformHorzDist = Vector3.Distance(Vector3.Scale(transform.position, hPlane), Vector3.Scale(LocalPlayer.Transform.position, hPlane));

                    // Move collision box to ghostPosition
                    if (!Mathf.Approximately(ghostHorzDist, transformHorzDist))
                    {
                        Vector3 offset = LocalPlayer.Transform.forward * (ghostHorzDist - transformHorzDist);
                        // Snap To Grid
                        if (bps.snap)
                        {
                            Vector3 newPos = transform.position + offset;
                            newPos.x = Mathf.RoundToInt(transform.position.x / bps.gridSize) * bps.gridSize;
                            newPos.z = Mathf.RoundToInt(transform.position.z / bps.gridSize) * bps.gridSize;
                            transform.position = newPos;
                        }
                        else transform.position += offset;

                        if (transform.position.sqrMagnitude > 10_000_000)
                        {
                            transform.position = LocalPlayer.Transform.position;
                            offset = Vector3.zero;
                        }

                        cBoxCornerBL += offset;
                        cBoxCornerTL += offset;
                        cBoxCornerBR += offset;
                        cBoxCornerTR += offset;
                    }
                }

                // Move collision box to ground
                RaycastHit? raycastHit1 = null;
                RaycastHit? raycastHit2 = null;
                RaycastHit? raycastHit3 = null;
                RaycastHit? raycastHit4 = null;
                int iterations = 0;
                float radius = 0.2f;
                if (Physics.SphereCast(cBoxCornerBL, radius, Vector3.down, out hit, maxDistance, GetValidLayers(FloorLayersFinal.value)))
                {
                    if (!MatchingExclusionGroup(hit.collider, true))
                    {
                        raycastHit1 = new RaycastHit?(hit);
                    }
                    else
                    {
                        SetColliding();
                    }
                    cBoxCornerBL.y = hit.point.y;
                    if ((1 << hit.collider.gameObject.layer & WaterLayers) != 0)
                    {
                        iterations++;
                    }
                    ParentingRulesLookup();
                }
                else if (LocalPlayer.IsInEndgame)
                {
                    cBoxCornerBL.y = LocalPlayer.Transform.position.y - 2f;
                    SetColliding();
                }
                else
                {
                    cBoxCornerBL.y = activeTerrain.SampleHeight(cBoxCornerBL) + activeTerrain.transform.position.y;
                }
                if (Physics.SphereCast(cBoxCornerTL, radius, Vector3.down, out hit, maxDistance, GetValidLayers(FloorLayersFinal.value)))
                {
                    if (!MatchingExclusionGroup(hit.collider, true))
                    {
                        raycastHit2 = new RaycastHit?(hit);
                    }
                    else
                    {
                        SetColliding();
                    }
                    cBoxCornerTL.y = hit.point.y;
                    if ((1 << hit.collider.gameObject.layer & WaterLayers) != 0)
                    {
                        iterations++;
                    }
                    ParentingRulesLookup();
                }
                else if (LocalPlayer.IsInEndgame)
                {
                    cBoxCornerTL.y = LocalPlayer.Transform.position.y - 2f;
                    SetColliding();
                }
                else
                {
                    cBoxCornerTL.y = activeTerrain.SampleHeight(cBoxCornerTL) + activeTerrain.transform.position.y;
                }
                if (Physics.SphereCast(cBoxCornerBR, radius, Vector3.down, out hit, maxDistance, GetValidLayers(FloorLayersFinal.value)))
                {
                    if (!MatchingExclusionGroup(hit.collider, true))
                    {
                        raycastHit4 = new RaycastHit?(hit);
                    }
                    else
                    {
                        SetColliding();
                    }
                    cBoxCornerBR.y = hit.point.y;
                    if ((1 << hit.collider.gameObject.layer & WaterLayers) != 0)
                    {
                        iterations++;
                    }
                    ParentingRulesLookup();
                }
                else if (LocalPlayer.IsInEndgame)
                {
                    cBoxCornerBR.y = LocalPlayer.Transform.position.y - 2f;
                    SetColliding();
                }
                else
                {
                    cBoxCornerBR.y = activeTerrain.SampleHeight(cBoxCornerBR) + activeTerrain.transform.position.y;
                }
                if (Physics.SphereCast(cBoxCornerTR, radius, Vector3.down, out hit, maxDistance, GetValidLayers(FloorLayersFinal.value)))
                {
                    if (!MatchingExclusionGroup(hit.collider, true))
                    {
                        raycastHit3 = new RaycastHit?(hit);
                    }
                    else
                    {
                        SetColliding();
                    }
                    cBoxCornerTR.y = hit.point.y;
                    if ((1 << hit.collider.gameObject.layer & WaterLayers) != 0)
                    {
                        iterations++;
                    }
                    ParentingRulesLookup();
                }
                else if (LocalPlayer.IsInEndgame)
                {
                    cBoxCornerTR.y = LocalPlayer.Transform.position.y - 2f;
                    SetColliding();
                }
                else
                {
                    cBoxCornerTR.y = activeTerrain.SampleHeight(cBoxCornerTR) + activeTerrain.transform.position.y;
                }

                bool mainTerrain = false;
                Vector3 curPosition = transform.position;
                curPosition.y = point.y + 1f;
                float newY;
                // Call SetCollide when ghost intersects stuff
                if (Physics.SphereCast(curPosition, radius, Vector3.down, out hit, maxDistance, GetValidLayers(FloorLayersFinal.value)))
                {
                    newY = hit.point.y;
                    if (!MatchingExclusionGroup(hit.collider, true))
                    {
                        if (LastHit == null)
                        {
                            LastHit = new RaycastHit?(hit);
                        }
                    }
                    else
                    {
                        SetColliding();
                    }
                    if (hit.transform.name.Equals("MainTerrain") || hit.transform.gameObject.layer == 17)
                    {
                        mainTerrain = true;
                    }
                    if ((1 << hit.collider.gameObject.layer & WaterLayers) != 0)
                    {
                        iterations++;
                    }
                    ParentingRulesLookup();
                }
                else if (LocalPlayer.IsInEndgame)
                {
                    newY = LocalPlayer.Transform.position.y - 2f;
                    SetColliding();
                }
                else
                {
                    newY = activeTerrain.SampleHeight(vector) + activeTerrain.transform.position.y;
                }

                // Adjust y of collision box once more
                if (CheckForHole(cBoxCornerBL, cBoxCornerTL, cBoxCornerBR, cBoxCornerTR))
                {
                    if (newY < cBoxCornerBL.y)
                    {
                        if (raycastHit1 != null)
                        {
                            LastHit = new RaycastHit?(hit);
                        }
                        newY = cBoxCornerBL.y;
                    }
                }
                else if (CheckForHole(cBoxCornerTL, cBoxCornerBL, cBoxCornerTR, cBoxCornerBR) && newY < cBoxCornerTL.y)
                {
                    if (raycastHit2 != null)
                    {
                        LastHit = new RaycastHit?(hit);
                    }
                    newY = cBoxCornerTL.y;
                }
                if (CheckForHole(cBoxCornerTR, cBoxCornerTL, cBoxCornerBR, cBoxCornerBL) && newY < cBoxCornerTR.y)
                {
                    if (raycastHit3 != null)
                    {
                        LastHit = new RaycastHit?(hit);
                    }
                    newY = cBoxCornerTR.y;
                }
                if (CheckForHole(cBoxCornerBR, cBoxCornerTR, cBoxCornerBL, cBoxCornerTL) && newY < cBoxCornerBR.y)
                {
                    if (raycastHit4 != null)
                    {
                        LastHit = new RaycastHit?(hit);
                    }
                    newY = cBoxCornerBR.y;
                }
                // Save the changes to y when the floor was hit
                if (hitFloor)
                {
                    point.y = newY;
                }

                // Adjust height of all collision box corner vectors
                float averageCBoxCornerHeight = (cBoxCornerBL.y + cBoxCornerTL.y + cBoxCornerBR.y + cBoxCornerTR.y + newY) / 5f;
                if (!mainTerrain)
                {
                    if (Mathf.Abs((cBoxCornerTL.y + cBoxCornerBR.y + cBoxCornerTR.y + newY) / 4f - cBoxCornerBL.y) > 1)
                    {
                        averageCBoxCornerHeight = (cBoxCornerTL.y + cBoxCornerBR.y + cBoxCornerTR.y + newY) / 4f;
                        cBoxCornerBL.y = averageCBoxCornerHeight;
                    }
                    if (Mathf.Abs((cBoxCornerBL.y + cBoxCornerBR.y + cBoxCornerTR.y + newY) / 4f - cBoxCornerTL.y) > 1)
                    {
                        averageCBoxCornerHeight = (cBoxCornerBL.y + cBoxCornerBR.y + cBoxCornerTR.y + newY) / 4f;
                        cBoxCornerTL.y = averageCBoxCornerHeight;
                    }
                    if (Mathf.Abs((cBoxCornerBL.y + cBoxCornerTL.y + cBoxCornerTR.y + newY) / 4f - cBoxCornerBR.y) > 1)
                    {
                        averageCBoxCornerHeight = (cBoxCornerBL.y + cBoxCornerTL.y + cBoxCornerTR.y + newY) / 4f;
                        cBoxCornerBR.y = averageCBoxCornerHeight;
                    }
                    if (Mathf.Abs((cBoxCornerBL.y + cBoxCornerTL.y + cBoxCornerBR.y + newY) / 4f - cBoxCornerTR.y) > 1)
                    {
                        averageCBoxCornerHeight = (cBoxCornerBL.y + cBoxCornerTL.y + cBoxCornerBR.y + newY) / 4f;
                        cBoxCornerTR.y = averageCBoxCornerHeight;
                    }
                }

                // Limit ghost height
                if (Airborne)
                {
                    if (IsInSinkHole)
                    {
                        float ghostMaxHeight = LocalPlayer.Transform.position.y + maxAirBorneHeight;
                        if (point.y > ghostMaxHeight)
                        {
                            point.y = ghostMaxHeight;
                        }
                    }
                    else if (point.y - averageCBoxCornerHeight > maxAirBorneHeight)
                    {
                        point.y = averageCBoxCornerHeight + maxAirBorneHeight;
                    }
                }

                // Set (not) clear if waterborne
                if (WaterborneExclusive)
                {
                    if (iterations == 5)
                    {
                        SetClear();
                    }
                    else
                    {
                        SetNotclear();
                    }
                }
                else if (Hydrophobic && iterations > 0)
                {
                    SetColliding();
                    SetNotclear();
                }

                Vector3 cross1 = Vector3.Cross(cBoxCornerTL - cBoxCornerBL, cBoxCornerBR - cBoxCornerBL);
                Vector3 cross2 = Vector3.Cross(cBoxCornerBR - cBoxCornerTR, cBoxCornerTL - cBoxCornerTR);
                curNormal = Vector3.Normalize((cross1 + cross2) / 2f);
                MinHeight = Mathf.Min(new float[]
                {
                    cBoxCornerBL.y,
                    cBoxCornerTL.y,
                    cBoxCornerBR.y,
                    cBoxCornerTR.y,
                    newY
                });
                if (!AllowFoundation || Vector3.Angle(Vector3.up, curNormal) < FoundationMinSlope)
                {
                    AirBorneHeight = Mathf.Max(point.y, Mathf.Min(newY, averageCBoxCornerHeight));
                    RegularHeight = Mathf.Min(newY, averageCBoxCornerHeight);
                    if (curNormal.y < 0.5f)
                    {
                        curNormal = Vector3.up;
                        if (Airborne)
                        {
                            SetClear();
                        }
                    }
                    curGroundTilt = Quaternion.FromToRotation(Vector3.up, curNormal);
                    ApplyLeaningPosRot(transform, Airborne, TreeStructure);
                }
                else
                {
                    AirBorneHeight = Mathf.Max(new float[]
                    {
                        cBoxCornerBL.y,
                        cBoxCornerTL.y,
                        cBoxCornerBR.y,
                        cBoxCornerTR.y,
                        newY,
                        point.y
                    });
                    RegularHeight = Mathf.Max(new float[]
                    {
                        cBoxCornerBL.y,
                        cBoxCornerTL.y,
                        cBoxCornerBR.y,
                        cBoxCornerTR.y,
                        newY
                    });
                    // Set height
                    Vector3 newPosition = new Vector3(transform.position.x, (!Airborne) ? RegularHeight : AirBorneHeight, transform.position.z);

                    // Snap height
                    if (bps.snap)
                    {
                        newPosition.y = Mathf.RoundToInt(newPosition.y / bps.gridSize) * bps.gridSize;
                    }
                    transform.position = newPosition;

                    // Set rotation
                    transform.rotation = CurrentRotation;
                }
                ParentingRulesCheck();
            }
            if (TreeStructure)
            {
                LocalPlayer.Create.CurrentGhost.transform.rotation = CurrentRotation;
                if (Locked != Clear)
                {
                    if (Locked && ClearOfCollision)
                    {
                        SetClear();
                    }
                    else
                    {
                        SetNotclear();
                    }
                }
            }

            // Build Anywhere
            if (BetterBlueprintsCore.BuildAnywhereToggle)
            {
                _clearInternal = true;
                _clearDynamicBuilding = true;
                _clearSmallStructures = true;
                ClearOfCollision = true;
            }
        }
    }
}