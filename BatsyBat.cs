using BepInEx;
using ItemManager;
using UnityEngine;
using CreatureManager;
using ServerSync;
using HarmonyLib;


namespace BatsyBat
{
	[BepInPlugin(ModGUID, ModName, ModVersion)]
	public class BatsyBat : BaseUnityPlugin
	{
		private const string ModName = "BatsyBat";
		private const string ModVersion = "0.0.1";
		private const string ModGUID = "org.bepinex.plugins.batsybat";

		public void Awake()

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
				RequiredWeather = Weather.ClearSkies,
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

			GameObject MissBatsyBat = ItemManager.PrefabManager.RegisterPrefab("batsybat", "MissBatsyBat"); //register projectile


			new Harmony(ModName).PatchAll();			

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