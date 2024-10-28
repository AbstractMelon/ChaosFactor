using BepInEx.Logging;
using BoplFixedMath;
using HarmonyLib;

namespace ChaosFactor
{
    public class LowerAbilityCooldown : IMod
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("LowerAbilityCooldown");

        public void Activate()
        {
            var harmony = new Harmony("com.ChaosFactor.LowerCooldowns");
            harmony.PatchAll();
            Logger.LogInfo("Low cooldown activated: Ability cooldown set to 0.3 seconds.");
        }
        
        public void Deactivate()
        {
            var harmony = new Harmony("com.ChaosFactor.LowerCooldowns");
            harmony.UnpatchSelf();
            Logger.LogInfo("Low cooldown deactivated.");
        }

        public void UpdateState()
        {
            // Do nothing
        }

        [HarmonyPatch(typeof(Ability), "Awake")]
        class Patch
        {
            static void Postfix(Ability __instance)
            {
                __instance.Cooldown = (Fix)0.3f;
            }
        }
    }
}