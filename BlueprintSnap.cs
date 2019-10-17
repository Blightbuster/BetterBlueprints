using System.Security.Cryptography;
using TheForest.Utils;
using UnityEngine;

namespace BetterBlueprints.Override
{
    class BlueprintSnap : MonoBehaviour
    {
        private GameObject[] snapPoints;
        public bool snap = false; // Whether or not to snap to the grid
        public int gridSize = 5;
        private Vector3 lastPos;

        private void Awake()
        {
            createSnapPoints();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftAlt) ||
                UnityEngine.Input.GetKeyDown(KeyCode.RightAlt) ||
                UnityEngine.Input.GetKeyDown(KeyCode.AltGr))
            {
                snap = !snap;
                if (!snap) hideSnapPoints();
                else showSnapPoints();
            }

            if (snap) updateSnapPoints();
        }

        private void createSnapPoints()
        {
            snapPoints = new GameObject[27];
            for (int i = 0; i < snapPoints.Length; i++)
            {
                GameObject snapPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
                snapPoints[i] = snapPoint;
                Destroy(snapPoint.GetComponent<BoxCollider>());
                Transform transform = snapPoint.transform;
                transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                MeshRenderer meshRenderer = snapPoint.GetComponent<MeshRenderer>();
                meshRenderer.material.color = new Color(0, 0, 1, 0.5f);
            }
        }

        private void updateSnapPoints()
        {
            var pos = LocalPlayer.Transform.localPosition;
            pos.x = Mathf.RoundToInt(pos.x / gridSize) * gridSize;
            pos.y = Mathf.RoundToInt(pos.y / gridSize) * gridSize;
            pos.z = Mathf.RoundToInt(pos.z / gridSize) * gridSize;
            if (lastPos == pos) return;
            lastPos = pos;

            int i = 0;
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        Vector3 snapPointPos = new Vector3(pos.x + gridSize * x, pos.y + gridSize * y, pos.z + gridSize * z);
                        snapPoints[i++].transform.position = snapPointPos;
                    }
                }
            }
        }

        private void showSnapPoints()
        {
            foreach (var sp in snapPoints)
            {
                sp.SetActive(true);
            }
        }

        private void hideSnapPoints()
        {
            foreach (var sp in snapPoints)
            {
                sp.SetActive(false);
            }
        }
    }
}