using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace LootMultiplier
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {

        private const string PluginGuid = "wisehorror.potionpermit.lootmultiplier";
        private const string PluginName = "Loot Multiplier";
        private const string PluginVersion = "1.0.0";
        private static readonly Harmony Harmony = new(PluginGuid);
        private static ManualLogSource Log { get; set; }

        public static ConfigEntry<bool> EnableMod { get; set; }
        internal static ConfigEntry<int> LootMultiplier { get; private set; }
        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginGuid} is loaded!");

            EnableMod = Config.Bind("Loot Multiplier",
                                    "Enable Loot Manipulation", 
                                    true, 
                                    "Enable loot manipulation.");

            LootMultiplier = Config.Bind("Loot Multiplier",
                                         "Loot Multiplier", 
                                         1, 
                                         new ConfigDescription("Set the loot multiplier.", new AcceptableValueRange<float>(1, 10)));
        }
        internal static void L(string message, bool info = false)
        {
            if (info)
            {
                Log.LogInfo(message);
                return;
            }
            Log.LogWarning(message);
        }
    }
}
