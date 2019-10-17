using Bolt;
using TheForest.Buildings.World;
using UnityEngine;

namespace BetterBlueprints.Override.QuickBuild
{
    class MultiThrowerItemHolderEx : MultiThrowerItemHolder
    {
        private float _nextAddItem = Time.time;

        protected override void Update()
        {
            if (BoltNetwork.isRunning)
            {
                UpdateMP();
            }
            if (_nextItemIndex > 0)
            {
                TheForest.Utils.Scene.HudGui.MultiThrowerTakeWidget.ShowSingle(_items[_nextItemIndex - 1],
                    _takeIcon.transform, TheForest.UI.SideIcons.Take);
                if (TheForest.Utils.Input.GetButtonDown("Take") ||
                    (TheForest.Utils.Input.GetButton("Take") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Take")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    TakeItem();
                }
            }
            else
            {
                TheForest.Utils.Scene.HudGui.MultiThrowerTakeWidget.Shutdown();
            }
            if (_nextItemIndex < _renderSlots.Length)
            {
                bool flag = CanToggleNextAddItem();
                if (flag && TheForest.Utils.Input.GetButtonDown("Rotate"))
                {
                    TheForest.Utils.LocalPlayer.Sfx.PlayWhoosh();
                    ToggleNextAddItem();
                }
                if (_currentAddItem >= 0 && _currentAddItem < TheForest.Items.ItemDatabase.Items.Length)
                {
                    int id = TheForest.Items.ItemDatabase.Items[_currentAddItem]._id;
                    bool flag2 = !TheForest.Utils.LocalPlayer.Inventory.IsRightHandEmpty() &&
                                 TheForest.Utils.LocalPlayer.Inventory.RightHand._itemId == id;
                    if (TheForest.Utils.LocalPlayer.Inventory.Owns(id))
                    {
                        TheForest.Utils.Scene.HudGui.MultiThrowerAddWidget.ShowList(TheForest.Items.ItemDatabase.Items[_currentAddItem]._id, _addIcon.transform, TheForest.UI.SideIcons.Craft);
                        if (TheForest.Utils.Input.GetButtonDown("Craft") || (TheForest.Utils.Input.GetButton("Craft") && _nextAddItem < Time.time))
                        {
                            if (TheForest.Utils.Input.GetButtonDown("Craft")) _nextAddItem = Time.time + 0.25f;
                            else _nextAddItem = Time.time + 0.05f;
                            if (flag2 && TheForest.Utils.LocalPlayer.Inventory.ShuffleRemoveRightHandItem() || !flag2 && TheForest.Utils.LocalPlayer.Inventory.RemoveItem(id))
                            {
                                if (_addItemEvent.Length > 0)
                                {
                                    FMODCommon.PlayOneshot(_addItemEvent, transform);
                                }
                                else
                                {
                                    TheForest.Utils.LocalPlayer.Sfx.PlayPutDown(gameObject);
                                }
                                if (BoltNetwork.isRunning)
                                {
                                    ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
                                    itemHolderAddItem.ContentType = id;
                                    itemHolderAddItem.Target = entity;
                                    itemHolderAddItem.Send();
                                }
                                else
                                {
                                    SpawnEquipmentItemView(_renderSlots[_nextItemIndex], id);
                                    _items[_nextItemIndex] = id;
                                    _nextItemIndex++;
                                }
                            }
                        }
                        return;
                    }
                }
                if (flag)
                {
                    ToggleNextAddItem();
                }
                TheForest.Utils.Scene.HudGui.MultiThrowerAddWidget.Shutdown();
            }
            else
            {
                TheForest.Utils.Scene.HudGui.MultiThrowerAddWidget.Shutdown();
            }
        }
    }
}