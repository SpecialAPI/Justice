using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Justice
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.justice";
        public const string NAME = "Justice";
        public const string VERSION = "1.0.0";
        
        public void Awake()
        {
            new Harmony(GUID).PatchAll();
        }

        [HarmonyPatch(typeof(RewardManager), nameof(RewardManager.GetMultiplierForItem))]
        [HarmonyPostfix]
        public static void ChangeActiveRarity(ref float __result, PickupObject prefab, PlayerController player)
        {
            if(prefab != null && player != null && prefab is PlayerItem)
            {
                var item = prefab as PlayerItem;
                int addcapacity = 0;
                if (item.passiveStatModifiers != null && item.passiveStatModifiers.Length > 0)
                {
                    foreach(var mod in item.passiveStatModifiers)
                    {
                        if(mod != null && mod.statToBoost == PlayerStats.StatType.AdditionalItemCapacity)
                        {
                            addcapacity += Mathf.RoundToInt(mod.amount);
                        }
                    }
                }
                if (item is TeleporterPrototypeItem)
                {
                    for (int k = 0; k < player.activeItems.Count; k++)
                    {
                        if (player.activeItems[k] is ChestTeleporterItem)
                        {
                            addcapacity++;
                            break;
                        }
                    }
                }
                else if (item is ChestTeleporterItem)
                {
                    for (int l = 0; l < player.activeItems.Count; l++)
                    {
                        if (player.activeItems[l] is TeleporterPrototypeItem)
                        {
                            addcapacity++;
                            break;
                        }
                    }
                }
                int maxItems = player.MAX_ITEMS_HELD + (int)player.stats.GetStatValue(PlayerStats.StatType.AdditionalItemCapacity) + addcapacity;
                if (player.activeItems.Count >= maxItems)
                {
                    __result *= 0.1f;
                }
            }
        }
    }
}
