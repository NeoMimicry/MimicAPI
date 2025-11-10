using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MimicAPI.GameAPI
{
    public static class LootAPI
    {
        public static LootingLevelObject[] GetAllLoot()
        {
            return FilterLoot(l => l.gameObject.activeInHierarchy);
        }

        public static LootingLevelObject[] GetLootNearby(float maxDistance, UnityEngine.Vector3? searchCenter = null)
        {
            UnityEngine.Vector3 center = searchCenter ?? PlayerAPI.GetLocalPlayer()?.transform.position ?? UnityEngine.Vector3.zero;
            return FilterLoot(l => l.gameObject.activeInHierarchy && UnityEngine.Vector3.Distance(l.transform.position, center) <= maxDistance);
        }

        public static LootingLevelObject[] GetLootByName(string name)
        {
            return FilterLoot(l => l.gameObject.name.Contains(name));
        }

        private static LootingLevelObject[] FilterLoot(System.Func<LootingLevelObject, bool> predicate)
        {
            return UnityEngine.Object.FindObjectsByType<LootingLevelObject>(UnityEngine.FindObjectsSortMode.None).Where(l => l != null && predicate(l)).ToArray();
        }
    }
}
