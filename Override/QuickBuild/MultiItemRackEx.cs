using Bolt;
using TheForest.Buildings.World;
using UnityEngine;

namespace BetterBlueprints.Override.QuickBuild
{
    class MultiItemRackEx : MultiItemRack
    {
        private float _nextAddItem = Time.time;

        protected override void Update()
        {
            if (IsSlotOccupied)
            {
                TheForest.Utils.Scene.HudGui.RackWidgets[(int)_type].ShowSingle(_currentTakeItem, CurrentTakeItemId, _slots[CurrentSlot]._slotTr, TheForest.UI.SideIcons.Take);
                if (TheForest.Utils.Input.GetButtonDown("Take") || (TheForest.Utils.Input.GetButton("Take") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Take")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    if ((TheForest.Utils.LocalPlayer.Inventory.AddItem(CurrentTakeItemId) || TheForest.Utils.LocalPlayer.Inventory.FakeDrop(CurrentTakeItemId)))
                    {
                        TheForest.Utils.LocalPlayer.Sfx.PlayItemCustomSfx(CurrentTakeItemId);
                        CurrentTakeIcon.SetActive(false);
                        if (BoltNetwork.isRunning)
                        {
                            ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
                            itemHolderTakeItem.Target = entity;
                            itemHolderTakeItem.Player = TheForest.Utils.LocalPlayer.Entity;
                            itemHolderTakeItem.ContentType = CurrentSlot;
                            itemHolderTakeItem.ContentValue = CurrentTakeItemId;
                            itemHolderTakeItem.Send();
                            if (BoltNetwork.isClient)
                            {
                                CurrentTakeItemId = 0;
                            }
                        }
                        else
                        {
                            CurrentTakeItemId = 0;
                            UpdateRenderers();
                        }
                    }
                }
            }
            int num = TheForest.Utils.LocalPlayer.Inventory.OwnsWhich(CurrentAddItemId, _allowFallback);
            if (!IsSlotOccupied && num > -1)
            {
                if (_slots[CurrentSlot]._items.Length > 1)
                {
                    TheForest.Utils.Scene.HudGui.RackWidgets[(int)_type].ShowList(CurrentAddItemId, num, _slots[CurrentSlot]._slotTr, TheForest.UI.SideIcons.Craft);
                }
                else
                {
                    TheForest.Utils.Scene.HudGui.RackWidgets[(int)_type].ShowSingle(CurrentAddItemId, num, _slots[CurrentSlot]._slotTr, TheForest.UI.SideIcons.Craft);
                }
                if (TheForest.Utils.Input.GetButtonDown("Craft") || (TheForest.Utils.Input.GetButton("Craft") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Craft")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    TheForest.Audio.Sfx.Play(TheForest.Audio.SfxInfo.SfxTypes.AddItem, CurrentAddIcon.transform);
                    if (TheForest.Utils.LocalPlayer.Inventory.RemoveItem(num))
                    {
                        CurrentTakeItemId = num;
                        _currentTakeItem = CurrentAddItemId;
                        if (BoltNetwork.isRunning)
                        {
                            ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
                            itemHolderAddItem.Target = entity;
                            itemHolderAddItem.ContentType = CurrentSlot;
                            itemHolderAddItem.ContentInfo = num;
                            itemHolderAddItem.Send();
                        }
                        else
                        {
                            UpdateRenderers();
                        }
                    }
                }
            }
            bool flag = CanToggleNextAddItem();
            if (flag && (TheForest.Utils.Input.GetButtonDown("Rotate") || num == -1))
            {
                TheForest.Utils.LocalPlayer.Sfx.PlayWhoosh();
                ToggleNextAddItem();
            }
        }
    }
}
