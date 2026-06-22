using BepInEx;
using BepInEx.Configuration;
using CreatureManager;
using HarmonyLib;
using ItemManager;
using LocalizationManager;
using ServerSync;
using System.Reflection;
using UnityEngine;


namespace BatsyBat
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class BatsyBat : BaseUnityPlugin
    {
        private const string ModName = "BatsyBat";
        private const string ModVersion = "0.0.7";
        private const string ModGUID = "org.bepinex.plugins.batsybat";

        private static readonly ConfigSync configSync = new(ModName) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        private static ConfigEntry<Toggle> serverConfigLocked = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => config(group, name, value, new ConfigDescription(description), synchronizedSetting);

        private enum Toggle
        {
            On = 1,
            Off = 0
        }


        public void Awake()
        {
            Localizer.Load();

            {
                Item BottledBatsyBomb = new("batsybat", "BottledBatsyBomb");  //assetbundle name, Asset Name
                BottledBatsyBomb.Crafting.Add(CraftingTable.Cauldron, 1);
                BottledBatsyBomb.RequiredItems.Add("Bloodbag", 2);
                BottledBatsyBomb.RequiredItems.Add("SurtlingCore", 1);
                BottledBatsyBomb.CraftAmount = 1;

                Creature WildBatsyBat = new("batsybat", "WildBatsyBat")            //add creature
                {
                    Biome = Heightmap.Biome.Plains,
                    CanSpawn = true,
                    CanBeTamed = true,
                    FoodItems = "Raspberry, Blueberries, Cloudberry, Pukeberries",
                    SpawnChance = 20,
                    GroupSize = new Range(1, 2),
                    CheckSpawnInterval = 300,
                    SpecificSpawnTime = SpawnTime.Night,
                    Maximum = 1
                };
                WildBatsyBat.Drops["LeatherScraps"].Amount = new Range(1, 2);
                WildBatsyBat.Drops["LeatherScraps"].DropChance = 100f;
                WildBatsyBat.Drops["BoneFragments"].Amount = new Range(1, 2);
                WildBatsyBat.Drops["BoneFragments"].DropChance = 10f;
                WildBatsyBat.Drops["Guck"].Amount = new Range(1, 2);
                WildBatsyBat.Drops["Guck"].DropChance = 20f;

                GameObject BottledBatsyProjectile = ItemManager.PrefabManager.RegisterPrefab("batsybat", "BottledBatsyProjectile"); //register projectile
                GameObject MissBatsyBat = ItemManager.PrefabManager.RegisterPrefab("batsybat", "MissBatsyBat");
                GameObject fx_bottled_bat_spawn = ItemManager.PrefabManager.RegisterPrefab("batsybat", "fx_bottled_bat_spawn");

                Assembly assembly = Assembly.GetExecutingAssembly();
                Harmony harmony = new(ModGUID);
                harmony.PatchAll(assembly);

            }
        }
        [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.Start))]
        static class MonsterAI_Start_Patch
        {
            static void Postfix(MonsterAI __instance)
            {
                if (Player.m_localPlayer && __instance.gameObject.name.Contains("MissBatsyBat") && __instance.m_nview.IsOwner())
                {
                    __instance.ResetPatrolPoint();
                    __instance.SetFollowTarget(Player.m_localPlayer.gameObject);
                }


            }
        }
    }

}