using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mimic;
using Mimic.Actors;
using UnityEngine;

namespace MimicAPI.GameAPI
{
    public static class PlayerAPI
    {
        public static ProtoActor? GetLocalPlayer()
        {
            return FindActorWhere(a => a.AmIAvatar());
        }

        public static ProtoActor[]? GetAllPlayers()
        {
            return FindActorsWhere(a => a != null);
        }

        public static ProtoActor[]? GetOtherPlayers()
        {
            return FindActorsWhere(a => !a.AmIAvatar());
        }

        public static ProtoActor? GetPlayerByName(string name)
        {
            return FindActorWhere(a => a.gameObject.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static ProtoActor? GetPlayerByID(uint actorID)
        {
            return FindActorWhere(a => a.ActorID == actorID);
        }

        public static bool IsPlayerValid(ProtoActor? actor)
        {
            return actor != null && actor.gameObject.activeInHierarchy;
        }

        public static bool HasLocalPlayer()
        {
            return GetLocalPlayer() != null;
        }

        public static StatManager? GetLocalStatManager()
        {
            ProtoActor? player = GetLocalPlayer();
            return player?.GetComponent<StatManager>();
        }

        public static StatManager? GetStatManager(ProtoActor? actor)
        {
            return actor?.GetComponent<StatManager>();
        }

        public static MovementController? GetLocalMovementController()
        {
            ProtoActor? player = GetLocalPlayer();
            return player?.GetComponent<MovementController>();
        }

        public static MovementController? GetMovementController(ProtoActor? actor)
        {
            return actor?.GetComponent<MovementController>();
        }

        public static object? GetLocalInventory()
        {
            ProtoActor? player = GetLocalPlayer();
            return player != null ? ReflectionHelper.GetFieldValue(player, "inventory") : null;
        }

        public static object? GetInventory(ProtoActor? actor)
        {
            return actor != null ? ReflectionHelper.GetFieldValue(actor, "inventory") : null;
        }

        public static List<InventoryItem> GetInventoryItems(ProtoActor? actor)
        {
            if (actor == null)
                return new List<InventoryItem>();

            object? inventory = GetInventory(actor);
            return inventory != null ? ReflectionHelper.GetFieldValue<List<InventoryItem>>(inventory, "SlotItems") ?? new List<InventoryItem>() : new List<InventoryItem>();
        }

        public static Vector3 GetPlayerPosition(ProtoActor? actor)
        {
            return actor == null ? Vector3.zero : actor.transform.position;
        }

        public static Vector3 GetLocalPlayerPosition()
        {
            ProtoActor? player = GetLocalPlayer();
            return GetPlayerPosition(player);
        }

        public static float GetDistanceBetweenPlayers(ProtoActor? actor1, ProtoActor? actor2)
        {
            if (actor1 == null || actor2 == null)
                return 0f;
            return Vector3.Distance(actor1.transform.position, actor2.transform.position);
        }

        public static bool IsPlayerAlive(ProtoActor? actor)
        {
            if (actor == null)
                return false;
            var statManager = GetStatManager(actor);
            if (statManager == null)
                return false;
            var isAlive = ReflectionHelper.InvokeMethod(statManager, "IsAliveStatus");
            return isAlive is bool result && result;
        }

        public static bool IsLocalPlayerAlive()
        {
            ProtoActor? player = GetLocalPlayer();
            return IsPlayerAlive(player);
        }

        public static List<ProtoActor> GetAlivePlayersInRange(float range, Vector3? center = null)
        {
            Vector3 searchCenter = center.HasValue ? center.Value : GetLocalPlayerPosition();
            var players = GetAllPlayers();
            if (players == null)
                return new List<ProtoActor>();

            return players
                .Where(p => p != null && IsPlayerValid(p) && IsPlayerAlive(p) && Vector3.Distance(p.transform.position, searchCenter) <= range)
                .ToList();
        }

        public static ProtoActor? GetNearestPlayer(Vector3? center = null)
        {
            Vector3 searchCenter = center.HasValue ? center.Value : GetLocalPlayerPosition();
            var players = GetAllPlayers();
            if (players == null || players.Length == 0)
                return null;

            return players
                .Where(p => p != null && IsPlayerValid(p))
                .OrderBy(p => Vector3.Distance(p.transform.position, searchCenter))
                .FirstOrDefault();
        }

        public static ProtoActor? GetNearestAlivePlayer(Vector3? center = null)
        {
            Vector3 searchCenter = center.HasValue ? center.Value : GetLocalPlayerPosition();
            var players = GetAllPlayers();
            if (players == null || players.Length == 0)
                return null;

            return players
                .Where(p => p != null && IsPlayerValid(p) && IsPlayerAlive(p))
                .OrderBy(p => Vector3.Distance(p.transform.position, searchCenter))
                .FirstOrDefault();
        }

        public static List<ProtoActor> GetPlayersInRange(float range, Vector3? center = null)
        {
            Vector3 searchCenter = center.HasValue ? center.Value : GetLocalPlayerPosition();
            var players = GetAllPlayers();
            if (players == null)
                return new List<ProtoActor>();

            return players
                .Where(p => p != null && IsPlayerValid(p) && Vector3.Distance(p.transform.position, searchCenter) <= range)
                .ToList();
        }

        public static string GetPlayerName(ProtoActor? actor)
        {
            return actor == null ? "Unknown" : actor.gameObject.name;
        }

        private static ProtoActor? FindActorWhere(Func<ProtoActor, bool> predicate)
        {
            try
            {
                ProtoActor[] allActors = UnityEngine.Object.FindObjectsByType<ProtoActor>(FindObjectsSortMode.None);
                return allActors.FirstOrDefault(predicate);
            }
            catch
            {
                return null;
            }
        }

        private static ProtoActor[]? FindActorsWhere(Func<ProtoActor, bool> predicate)
        {
            try
            {
                ProtoActor[] allActors = UnityEngine.Object.FindObjectsByType<ProtoActor>(FindObjectsSortMode.None);
                return allActors.Where(predicate).ToArray();
            }
            catch
            {
                return new ProtoActor[0];
            }
        }
    }
}