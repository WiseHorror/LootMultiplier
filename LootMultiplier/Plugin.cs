using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

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
        public static ConfigEntry<bool> AlwaysMultiply { get; set; }
        internal static ConfigEntry<int> LootMultiplier { get; private set; }
        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo($"Plugin {PluginGuid} is loaded!");

            EnableMod = Config.Bind("Loot Multiplier",
                                    "Enable Loot Multiplier", 
                                    true, 
                                    "Enable loot multiplier.");

            AlwaysMultiply = Config.Bind("Always Multiply",
                                    "Always multiply the loot by the multiplier amount. If false, loot is randomly multiplied between x1 and the value set in the multiplier",
                                    false,
                                    "Always multiplies the loot by the configured amount.");

            LootMultiplier = Config.Bind("Loot Multiplier",
                                         "Loot Multiplier", 
                                         1, 
                                         new ConfigDescription("Set the loot multiplier.", new AcceptableValueRange<int>(1, 10)));
        }

        private void OnEnable()
        {
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            L($"Plugin {PluginName} is loaded!");
        }

        private void OnDisable()
        {
            Harmony.UnpatchSelf();
            L($"Plugin {PluginName} is unloaded!");
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
