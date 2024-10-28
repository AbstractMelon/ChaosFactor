using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChaosFactor
{
    [BepInPlugin("com.Abstractmelon.ChaosFactor", "Chaos Factor", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static new readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("ChaosFactor");
        private float timer = 0f;
        public static Plugin instance;
        private const float interval = 5f; 
        private List<IMod> mods = new List<IMod>();
        private List<IMod> activeMods = new List<IMod>();

        private void Awake()
        {
            instance = this;
            
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            RegisterMods();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void RegisterMods()
        {
            mods.Add(new LowerAbilityCooldown());
        }

        private void Update()
        {
            if (!IsCurrentSceneValid()) return; // Only run if the scene is valid
            
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                ActivateRandomMod();
                timer = 0f; // Reset timer
            }

            // Update active mods
            UpdateActiveMods();
        }

        private void ActivateRandomMod()
        {
            if (mods.Count == 0) return;

            // Select a random mod
            var randomMod = mods[UnityEngine.Random.Range(0, mods.Count)];
            if (!activeMods.Contains(randomMod))
            {
                randomMod.Activate();
                activeMods.Add(randomMod);
                Logger.LogInfo($"Activated mod: {randomMod.GetType().Name}");
            }
        }

        private void UpdateActiveMods()
        {
            foreach (var mod in activeMods)
            {
                // mod.UpdateState();
            }
        }

        public void DeactivateAllMods()
        {
            foreach (var mod in activeMods)
            {
                mod.Deactivate();
                Logger.LogInfo($"Deactivated mod: {mod.GetType().Name}");
            }
            activeMods.Clear();
        }

        private bool IsCurrentSceneValid()
        {
            var currentScene = SceneManager.GetActiveScene();
            return currentScene.name.StartsWith("Level", StringComparison.OrdinalIgnoreCase);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.LogInfo($"Loaded scene: {scene.name}");
            
            if (!IsCurrentSceneValid())
            {
                Logger.LogInfo("Deactivating all mods due to invalid scene.");
                DeactivateAllMods();
            }
            else
            {
                Logger.LogInfo("Scene is valid, checking for active mods.");
            }
        }

        [HarmonyPatch(typeof(GameSessionHandler), nameof(GameSessionHandler.LoadNextLevelScene))]
        [HarmonyPrefix]
        private static void GameSessionHandler_LoadNextLevelScene()
        {
            instance.DeactivateAllMods();
        }
        

        [HarmonyPatch(typeof(GameSession), nameof(GameSession.Init))]
        [HarmonyPrefix]
        private static void GameSession_Init()
        {
            instance.DeactivateAllMods();
        }
    }

    public interface IMod
    {
        void Activate();
        void Deactivate();
        void UpdateState(); 
    }
} 
