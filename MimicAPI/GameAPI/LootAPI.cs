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

        public static LootingLevelObject[] GetInactiveLoot()
        {
            return FilterLoot(l => !l.gameObject.activeInHierarchy);
        }

        public static LootingLevelObject[] GetLootNearby(float maxDistance, Vector3? searchCenter = null)
        {
            Vector3 center = searchCenter.HasValue ? searchCenter.Value : PlayerAPI.GetLocalPlayerPosition();
            if (center == Vector3.zero && PlayerAPI.GetLocalPlayer() == null)
                center = Vector3.zero;
            return FilterLoot(l => l.gameObject.activeInHierarchy && Vector3.Distance(l.transform.position, center) <= maxDistance);
        }

        public static LootingLevelObject[] GetLootByName(string name)
        {
            return FilterLoot(l => l.gameObject.name.Contains(name));
        }

        public static LootingLevelObject? GetNearestLoot(Vector3? searchCenter = null)
        {
            Vector3 center = searchCenter.HasValue ? searchCenter.Value : PlayerAPI.GetLocalPlayerPosition();
            if (center == Vector3.zero && PlayerAPI.GetLocalPlayer() == null)
                center = Vector3.zero;
            var allLoot = GetAllLoot();
            if (allLoot.Length == 0)
                return null;

            return allLoot.OrderBy(l => Vector3.Distance(l.transform.position, center)).FirstOrDefault();
        }

        public static LootingLevelObject? GetNearestLootInRange(float maxDistance, Vector3? searchCenter = null)
        {
            return GetLootNearby(maxDistance, searchCenter).FirstOrDefault();
        }

        public static bool HasLoot()
        {
            return GetAllLoot().Length > 0;
        }

        public static bool HasLootNearby(float maxDistance, Vector3? searchCenter = null)
        {
            return GetLootNearby(maxDistance, searchCenter).Length > 0;
        }

        public static Vector3 GetLootPosition(LootingLevelObject loot)
        {
            return loot == null ? Vector3.zero : loot.transform.position;
        }

        public static float GetDistanceToLoot(LootingLevelObject loot, Vector3? center = null)
        {
            if (loot == null)
                return 0f;
            Vector3 searchCenter = center.HasValue ? center.Value : PlayerAPI.GetLocalPlayerPosition();
            if (searchCenter == Vector3.zero && PlayerAPI.GetLocalPlayer() == null)
                searchCenter = Vector3.zero;
            return Vector3.Distance(loot.transform.position, searchCenter);
        }

        public static LootingLevelObject[] FilterLootByDistance(float minDistance, float maxDistance, Vector3? searchCenter = null)
        {
            Vector3 center = searchCenter.HasValue ? searchCenter.Value : PlayerAPI.GetLocalPlayerPosition();
            if (center == Vector3.zero && PlayerAPI.GetLocalPlayer() == null)
                center = Vector3.zero;
            return FilterLoot(l =>
            {
                float distance = Vector3.Distance(l.transform.position, center);
                return l.gameObject.activeInHierarchy && distance >= minDistance && distance <= maxDistance;
            });
        }

        private static LootingLevelObject[] FilterLoot(Func<LootingLevelObject, bool> predicate)
        {
            return UnityEngine.Object.FindObjectsByType<LootingLevelObject>(UnityEngine.FindObjectsSortMode.None).Where(l => l != null && predicate(l)).ToArray();
        }
    }
}
