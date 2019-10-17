using Bolt;
using UnityEngine;

namespace BetterBlueprints.Override.QuickBuild
{
    class ItemHolderEx : ItemHolder
    {
        private float _nextAddItem = Time.time;

        protected override void Update()
        {
            if (BoltNetwork.isServer)
            {
                state.ItemCount = Items;
            }
            else if (BoltNetwork.isClient)
            {
                Items = state.ItemCount;
            }
            bool takeIcon = Items > 0;
            if (TheForest.Utils.Input.GetButtonDown("Take") ||
                (TheForest.Utils.Input.GetButton("Take") && _nextAddItem < Time.time))
            {
                if (TheForest.Utils.Input.GetButtonDown("Take")) _nextAddItem = Time.time + 0.25f;
                else _nextAddItem = Time.time + 0.05f;
                if (Items > 0 && TheForest.Utils.LocalPlayer.Inventory.AddItem(_itemid, 1, false, false,
                        (!_bonusManager)
                            ? TheForest.Items.Inventory.ItemProperties.Any
                            : _bonusManager.GetItemProperties(Items - 1)))
                {
                    TheForest.Utils.LocalPlayer.Sfx.PlayItemCustomSfx(_itemid);
                    if (BoltNetwork.isRunning)
                    {
                        ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
                        itemHolderTakeItem.Target = entity;
                        itemHolderTakeItem.Player = TheForest.Utils.LocalPlayer.Entity;
                        itemHolderTakeItem.Send();
                    }
                    else
                    {
                        Items--;
                        ItemsRender[Items].SetActive(false);
                        if (_bonusManager)
                        {
                            _bonusManager.UnsetItemProperties(Items);
                        }
                    }

                }
            }
            bool flag = Items < ItemsRender.Length && TheForest.Utils.LocalPlayer.Inventory.Owns(_itemid);
            if (flag)
            {
                if (TheForest.Utils.Input.GetButtonDown("Craft") ||
                    (TheForest.Utils.Input.GetButton("Craft") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Craft")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    if (addItemEvent.Length > 0)
                    {
                        FMODCommon.PlayOneshot(addItemEvent, transform);
                    }
                    else
                    {
                        TheForest.Utils.LocalPlayer.Sfx.PlayPutDown(gameObject);
                    }
                    if (TheForest.Utils.LocalPlayer.Inventory.RemoveItem(_itemid))
                    {
                        if (BoltNetwork.isRunning)
                        {
                            ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
                            itemHolderAddItem.Target = entity;
                            itemHolderAddItem.Send();
                        }
                        else
                        {
                            Items++;
                            ItemsRender[Items - 1].SetActive(true);
                            if (_bonusManager)
                            {
                                int index = TheForest.Utils.LocalPlayer.Inventory.AmountOf(_itemid);
                                _bonusManager.SetItemProperties(Items - 1, _itemid,
                                    TheForest.Utils.LocalPlayer.Inventory.InventoryItemViewsCache[_itemid][index]
                                        .Properties);
                            }
                        }
                    }
                }
            }
            TheForest.Utils.Scene.HudGui.HolderWidgets[(int)_type].Show(takeIcon, flag, TakeIcon.transform);
        }
    }
}
