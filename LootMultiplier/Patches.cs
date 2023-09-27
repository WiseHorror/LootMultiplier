using CharacterIDEnum;
using GlobalEnum;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LootMultiplier
{
    [HarmonyPatch]
    public static class Patches
    {
        private static List<int> hitObjects = new List<int>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleCalculator), nameof(BattleCalculator.Calculator))]
        public static void BattleCalculator_Calculator_Postfix(CharacterType characterType, PlayerCharacter player, Object obj)
        {
            if (!Plugin.EnableMod.Value) return;
            if (hitObjects.Contains(obj.GetInstanceID())) return;
            hitObjects.Add(obj.GetInstanceID());

            if (characterType == CharacterType.ENEMY)
            {
                var enemy = (Enemy)obj;

                if (enemy.loots.Count == 0) return;

                foreach (var loot in enemy.loots.ToList())
                {
                    var key = loot.Key;
                    var value = loot.Value;
                    var randomValue = value * Plugin.LootMultiplier.Value;
                    if (!Plugin.AlwaysMultiply.Value) randomValue = Random.Range(value, 1 + (value * Plugin.LootMultiplier.Value));

                    if (enemy.loots[key] >= Plugin.LootMultiplier.Value) return;

                    Plugin.L($"LootMultiplier: Added quantity {randomValue} of item {key.name} to loot table of monster {enemy.name}");
                    enemy.loots[key] = randomValue;
                }
            }
            if (characterType == CharacterType.RESOURCES)
            {
                var resource = (ResourcesObject)obj;

                if (resource.loots.Count == 0) return;

                foreach (var loot in resource.loots.ToList())
                {
                    var key = loot.Key;
                    var value = loot.Value;
                    var randomValue = value * Plugin.LootMultiplier.Value;
                    if (!Plugin.AlwaysMultiply.Value) randomValue = Random.Range(value, 1 + (value * Plugin.LootMultiplier.Value));

                    if (resource.loots[key] >= Plugin.LootMultiplier.Value && resource.currentstatus.Health != resource.baseStatus.Health) return;

                    Plugin.L($"LootMultiplier: Added quantity {randomValue} of item {key.name} to loot table of resource {resource.name}");
                    resource.loots[key] = randomValue;
                }
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MoundObject), nameof(MoundObject.DropLoots))]
        public static bool MoundObject_DropLoots(ref MoundObject __instance)
        {
            if (!Plugin.EnableMod.Value) return true;
            var randomValue = Plugin.LootMultiplier.Value;
            if (!Plugin.AlwaysMultiply.Value) randomValue = Random.Range(1, 1 + Plugin.LootMultiplier.Value);

            Plugin.L($"LootMultiplier: MoundObject {__instance.name} loot amount: {randomValue}");

            for (int i = 0; i < randomValue; i++)
            {
                BattleController.Instance.lootDropPoolingController.SpawnLootDrop(__instance.itemID, __instance.gameObject.transform.position, default(Vector3));
            }
            BattleController.Instance.particlePoolingController.SpawnParticle(ParticleID.particle_SmokeForPlants, __instance.gameObject.transform.position, default(Vector3), default(Vector3), 22, false);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerCharacter), nameof(PlayerCharacter.OnDayStart))]
        public static void PlayerCharacter_OnDayStart()
        {
            if (!Plugin.EnableMod.Value) return;
            Plugin.L($"LootMultiplier: OnDayStart Cleared hitEnemies list");
            hitObjects.Clear();
        }
    }
}
