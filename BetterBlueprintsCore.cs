using System;
using System.IO;
using BetterBlueprints.Override;
using ModAPI;
using ModAPI.Attributes;
using TheForest.Buildings.Creation;
using TheForest.Player;
using TheForest.Utils;
using UnityEngine;

namespace BetterBlueprints
{
    internal class BetterBlueprintsCore : MonoBehaviour
    {
        public static int MaxAnchorPoints = 10000;
        public static Color TintColor;
        public static bool BuildAnywhereToggle;
        public static bool InfiniteZiplineToggle;
        public static BlueprintSnap BlueprintSnap;

        private const string ConfigPath = @"Mods/BetterBlueprints.settings";
        private readonly Config _config = new Config(ConfigPath, "BetterBlueprints");
        // private DynamicBookLinks book;

        [ExecuteOnGameStart]
        private static void AddMeToScene()
        {
            if (Application.loadedLevelName.ToLower().Contains("forest"))
            {
                GameObject gameObject = new GameObject("__BetterBlueprints__");
                gameObject.AddComponent<BetterBlueprintsCore>();
                gameObject.AddComponent<BlueprintSnap>();
                BlueprintSnap = gameObject.GetComponent<BlueprintSnap>();
            }
        }

        private void Awake()
        {
            ReadConfig();
            InvokeRepeating("SetMaxAnchorpointsWall", 1f, 5f);
            InvokeRepeating("ReadConfig", 1f, 2f);
            //book = UnityEngine.Resources.FindObjectsOfTypeAll<DynamicBookLinks>()[0];
        }

        private void Update()
        {
            LocalPlayer.Create.BuildingPlacer.ClearMat.SetColor("_TintColor", TintColor);
            /*
            {
                // Book Scrolling
                if (book == null)
                {
                    ModAPI.Console.Write("Could'nt find book");
                }
                if (Input.mouseScrollDelta.y < 0)
                {
                    ModAPI.Console.Write("Previous");
                    book._pages[GetActivePageIndex()]._prevButton.OnClick();
                    ModAPI.Console.Write("1");
                }
                else if (Input.mouseScrollDelta.y > 0)
                {
                    ModAPI.Console.Write("Next");
                    book._pages[GetActivePageIndex()]._nextButton.OnClick();
                    ModAPI.Console.Write("2");
                }
            }
            catch (Exception e) { ModAPI.Console.Write(e.ToString()); */
        }

        /*private int GetActivePageIndex()
        {
            for (int pageIndex = 0; pageIndex <= book._pages.Length; pageIndex++)
            {
                if (book._pages[pageIndex]._go.activeSelf)
                {
                    ModAPI.Console.Write("Active: " + pageIndex);
                    return pageIndex;
                }
            }
            ModAPI.Console.Write("0");
            return 0;
        }*/

        private void SetMaxAnchorpointsWall()
        {
            var objects = (WallArchitect)FindObjectOfType(typeof(WallArchitect));
            objects._maxPoints = MaxAnchorPoints;
        }

        private void CheckConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                Log.Write("Configfile is missing");
                CreateConfig();
            }
        }

        private void CreateConfig()
        {
            Log.Write("Creating new default config...");
            _config.WriteInt("red", 0, "BlueprintColor");
            _config.WriteInt("green", 0);
            _config.WriteInt("blue", 100);
            _config.WriteInt("alpha", 25);
            _config.WriteBool("buildAnywhere", true, "Blueprints");
            _config.WriteBool("infiniteZipline", true, "Blueprints");
        }

        private void ReadConfig()
        {
            CheckConfig();

            try
            {
                TintColor.r = _config.ReadFloat("red", 0, "BlueprintColor") / 100;
                TintColor.g = _config.ReadFloat("green") / 100;
                TintColor.b = _config.ReadFloat("blue", 100) / 100;
                TintColor.a = _config.ReadFloat("alpha", 25) / 100;
                BuildAnywhereToggle = _config.ReadBool("buildAnywhere", true, "Blueprints");
                InfiniteZiplineToggle = _config.ReadBool("infiniteZipline", true, "Blueprints");
            }
            catch (Exception e)
            {
                Log.Write("Error while reading the config from: " + ConfigPath);
                Log.Write(e.Message);
                Log.Write(e.StackTrace);

                if (File.Exists(ConfigPath))
                {
                    File.Move(ConfigPath, ConfigPath + ".old");
                }

                CreateConfig();
                ReadConfig();
            }
        }
    }
}