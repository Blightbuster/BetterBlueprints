using Bolt;
using TheForest.Buildings.World;
using UnityEngine;

namespace BetterBlueprints.Override.QuickBuild
{
    class MultiItemHolderEx : MultiItemHolder
    {
        private float _nextAddItem = Time.time;

        protected override void Update()
        {
            if (CurrentItemAmount > 0 && TheForest.Utils.LocalPlayer.Inventory.HasRoomFor(CurrentItemId))
            {
                CurrentTakeIcon.SetActive(true);
                if (TheForest.Utils.Input.GetButtonDown("Take") || (TheForest.Utils.Input.GetButton("Take") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Take")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    if (TheForest.Utils.LocalPlayer.Inventory.AddItem(CurrentItemId))
                    {
                        TheForest.Utils.LocalPlayer.Sfx.PlayItemCustomSfx(CurrentItemId);
                        if (BoltNetwork.isRunning)
                        {
                            ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
                            itemHolderTakeItem.Target = entity;
                            itemHolderTakeItem.Player = TheForest.Utils.LocalPlayer.Entity;
                            itemHolderTakeItem.ContentType = Current;
                            itemHolderTakeItem.Send();
                        }
                        else
                        {
                            CurrentItemAmount--;
                            UpdateRenderers();
                        }
                    }
                }
            }
            else if (CurrentTakeIcon.activeSelf)
            {
                CurrentTakeIcon.SetActive(false);
            }
            if (CurrentItemAmount < CurrentItemMaxCapacity && TheForest.Utils.LocalPlayer.Inventory.Owns(CurrentItemId))
            {
                CurrentAddIcon.SetActive(true);
                if (TheForest.Utils.Input.GetButtonDown("Craft") || (TheForest.Utils.Input.GetButton("Craft") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Craft")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    if (_addItemEvent.Length > 0)
                    {
                        FMODCommon.PlayOneshot(_addItemEvent, transform);
                    }
                    else
                    {
                        TheForest.Utils.LocalPlayer.Sfx.PlayPutDown(gameObject);
                    }
                    if (TheForest.Utils.LocalPlayer.Inventory.RemoveItem(CurrentItemId))
                    {
                        if (BoltNetwork.isRunning)
                        {
                            ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
                            itemHolderAddItem.Target = entity;
                            itemHolderAddItem.ContentType = Current;
                            itemHolderAddItem.Send();
                        }
                        else
                        {
                            CurrentItemAmount++;
                            UpdateRenderers();
                        }
                    }
                }
            }
            else
            {
                CurrentAddIcon.SetActive(false);
            }
        }
    }
}
