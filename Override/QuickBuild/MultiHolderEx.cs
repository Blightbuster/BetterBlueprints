using Bolt;
using TheForest.Audio;
using TheForest.Buildings.World;
using TheForest.Utils;
using UnityEngine;
using Scene = TheForest.Utils.Scene;

namespace BetterBlueprints.Override.QuickBuild
{
    class MultiHolderEx : MultiHolder
    {
        private float _nextAddItem = Time.time;

        protected override void RockContentUpdate(ref ContentTypes showAddIcon)
        {
            bool takeIcon = false;
            if (_contentActual > 0)
            {
                takeIcon = true;
                if (TheForest.Utils.Input.GetButtonDown("Take") || (TheForest.Utils.Input.GetButton("Take") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Take")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    if (BoltNetwork.isRunning)
                    {
                        ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
                        itemHolderTakeItem.ContentType = (int)_contentTypeActual;
                        itemHolderTakeItem.Target = entity;
                        itemHolderTakeItem.Player = LocalPlayer.Entity;
                        itemHolderTakeItem.Send();
                    }
                    else if (LocalPlayer.Inventory.AddItem(RockItemId) || LocalPlayer.Inventory.FakeDrop(RockItemId))
                    {
                        RockRender[_contentActual - 1].SetActive(false);
                        _contentActual--;
                        if (_contentActual == 0)
                        {
                            takeIcon = false;
                            _contentTypeActual = ContentTypes.None;
                        }
                        RefreshMassAndDrag();
                    }
                }
            }
            if (_contentActual < RockRender.Length && LocalPlayer.Inventory.Owns(RockItemId) && (_content == ContentTypes.Rock || _addingContentType == ContentTypes.Rock))
            {
                showAddIcon = ContentTypes.Rock;
                if (TheForest.Utils.Input.GetButtonDown("Craft") || (TheForest.Utils.Input.GetButton("Craft") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Craft")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    LocalPlayer.Inventory.RemoveItem(RockItemId);
                    Sfx.Play(SfxInfo.SfxTypes.AddLog, RockRender[_contentActual].transform);
                    if (BoltNetwork.isRunning)
                    {
                        ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
                        itemHolderAddItem.ContentType = 3;
                        itemHolderAddItem.Target = entity;
                        itemHolderAddItem.Send();
                    }
                    else
                    {
                        _contentTypeActual = ContentTypes.Rock;
                        _contentActual++;
                        RockRender[_contentActual - 1].SetActive(true);
                        RefreshMassAndDrag();
                    }
                }
            }
            Scene.HudGui.HolderWidgets[1].Show(takeIcon, _contentTypeActual == ContentTypes.Rock && showAddIcon == ContentTypes.Rock, TakeIcon.transform);
        }
        
        protected override void StickContentUpdate(ref ContentTypes showAddIcon)
        {
            bool takeIcon = false;
            if (_contentActual > 0)
            {
                takeIcon = true;
                if (TheForest.Utils.Input.GetButtonDown("Take") || (TheForest.Utils.Input.GetButton("Take") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Take")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    if (BoltNetwork.isRunning)
                    {
                        ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
                        itemHolderTakeItem.ContentType = (int)_contentTypeActual;
                        itemHolderTakeItem.Target = entity;
                        itemHolderTakeItem.Player = LocalPlayer.Entity;
                        itemHolderTakeItem.Send();
                    }
                    else if (LocalPlayer.Inventory.AddItem(StickItemId) || LocalPlayer.Inventory.FakeDrop(StickItemId))
                    {
                        StickRender[_contentActual - 1].SetActive(false);
                        _contentActual--;
                        if (_contentActual == 0)
                        {
                            takeIcon = false;
                            _contentTypeActual = ContentTypes.None;
                        }
                        RefreshMassAndDrag();
                    }
                }
            }
            if (_contentActual < StickRender.Length && LocalPlayer.Inventory.Owns(StickItemId) && (_content == ContentTypes.Stick || _addingContentType == ContentTypes.Stick))
            {
                showAddIcon = ContentTypes.Stick;
                if (TheForest.Utils.Input.GetButtonDown("Craft") || (TheForest.Utils.Input.GetButton("Craft") && _nextAddItem < Time.time))
                {
                    if (TheForest.Utils.Input.GetButtonDown("Craft")) _nextAddItem = Time.time + 0.25f;
                    else _nextAddItem = Time.time + 0.05f;
                    LocalPlayer.Inventory.RemoveItem(StickItemId);
                    Sfx.Play(SfxInfo.SfxTypes.AddLog, StickRender[_contentActual].transform);
                    if (BoltNetwork.isRunning)
                    {
                        ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
                        itemHolderAddItem.ContentType = 4;
                        itemHolderAddItem.Target = entity;
                        itemHolderAddItem.Send();
                    }
                    else
                    {
                        _contentTypeActual = ContentTypes.Stick;
                        _contentActual++;
                        StickRender[_contentActual - 1].SetActive(true);
                        RefreshMassAndDrag();
                    }
                }
            }
            Scene.HudGui.HolderWidgets[0].Show(takeIcon, _contentTypeActual == ContentTypes.Stick && showAddIcon == ContentTypes.Stick, TakeIcon.transform);
        }
    }
}
