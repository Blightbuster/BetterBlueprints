using TheForest.Buildings.Creation;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

namespace BetterBlueprints.Override.QuickBuild
{
    class CraftStructureEx : Craft_Structure
    {
        protected override void Update()
        {
            if (!_initialized) return;
            CheckText();
            CheckNeeded();
            Scene.HudGui.DestroyIcon.gameObject.SetActive(true);
            if (_swapTo)
            {
                if (!Scene.HudGui.ToggleVariationIcon.activeSelf)
                {
                    Scene.HudGui.ToggleVariationIcon.SetActive(true);
                }
                if (TheForest.Utils.Input.GetButtonDown("Rotate"))
                {
                    SwapToNextGhost();
                    return;
                }
            }
            if (TheForest.Utils.Input.GetButtonAfterDelay("Craft", 0.25f))
            {
                CancelBlueprint();
                return;
            }
            if (_lockBuild || (CustomLockCheck != null && CustomLockCheck()))
            {
                int num = 0;
                for (int i = 0; i < _requiredIngredients.Count; i++)
                {
                    num += _requiredIngredients[i]._amount - _presentIngredients[i]._amount;
                }
                if (num == 1)
                {
                    AllOff();
                    Scene.HudGui.CantPlaceIcon.SetActive(true);
                    return;
                }
            }
            Scene.HudGui.CantPlaceIcon.SetActive(false);
            bool flag = false;
            for (int j = 0; j < _requiredIngredients.Count; j++)
            {
                HudGui.BuildingIngredient icons = GetIcons(_requiredIngredients[j]._itemID);
                if (_requiredIngredients[j]._amount != _presentIngredients[j]._amount)
                {
                    BuildIngredients buildIngredients = _requiredIngredients[j];
                    ReceipeIngredient receipeIngredient = _presentIngredients[j];
                    if (buildIngredients._amount > receipeIngredient._amount)
                    {
                        if (!LocalPlayer.Inventory.Owns(_requiredIngredients[j]._itemID) && !Cheats.Creative)
                        {
                            icons.SetMissingIngredientColor();
                        }
                        else
                        {
                            icons.SetAvailableIngredientColor();
                            if (_nextAddItem < Time.time && !flag)
                            {
                                flag = true;
                                if (!Scene.HudGui.AddBuildingIngredientIcon.activeSelf)
                                {
                                    Scene.HudGui.AddBuildingIngredientIcon.SetActive(true);
                                }
                                Scene.HudGui.AddBuildingIngredientIcon.transform.localPosition = icons._iconGo.transform.localPosition + new Vector3(0f, -50f, 0f);
                                Scene.HudGui.AddBuildingIngredientIcon.transform.rotation = icons._iconGo.transform.rotation;
                                if (TheForest.Utils.Input.GetButtonDown("Build"))
                                {
                                    _nextAddItem = Time.time + 0.25f;
                                    AddIngredient(j);
                                    break;
                                }
                                if (TheForest.Utils.Input.GetButton("Build") && (_nextAddItem < Time.time))
                                {
                                    _nextAddItem = Time.time + 0.05f;
                                    AddIngredient(j);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (!flag && Scene.HudGui.AddBuildingIngredientIcon.activeSelf)
            {
                Scene.HudGui.AddBuildingIngredientIcon.SetActive(false);
            }
            if (Vector3.Dot(LocalPlayer.Transform.forward, _uiFollowTarget.forward) < 0.75f || LocalPlayer.Transform.InverseTransformPoint(_uiFollowTarget.position).z < 0f)
            {
                SetUpIcons();
            }
        }
    }
}
