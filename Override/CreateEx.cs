using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheForest.Buildings.Creation;
using UnityEngine;

namespace BetterBlueprints.Override
{
    class CreateEx : Create
    {
        protected override void Update()
        {
            base.Update();
            return;
            // Snap To Grid
            BlueprintSnap bps = BetterBlueprintsCore.BlueprintSnap;
            if (bps.snap)
            {
                Vector3 oldPos = base._currentGhost.transform.position;
                Vector3 newPos = new Vector3();
                newPos.x = Mathf.RoundToInt(oldPos.x / bps.gridSize) * bps.gridSize;
                newPos.y = Mathf.RoundToInt(oldPos.y / bps.gridSize) * bps.gridSize;
                newPos.z = Mathf.RoundToInt(oldPos.z / bps.gridSize) * bps.gridSize;
                base._currentGhost.transform.position = newPos;
                ModAPI.Console.Write(newPos.x.ToString());
            }
        }
    }
}
