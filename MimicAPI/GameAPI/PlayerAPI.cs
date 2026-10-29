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
            var main = CoreAPI.GetPersistentData()?.main;
            return main != null ? main.GetMyAvatar() : null;
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
            return GetStatManager(GetLocalPlayer());
        }

        public static StatManager? GetStatManager(ProtoActor? actor)
        {
            var controller = GetServerActor(actor)?.StatControlUnit;
            return controller == null ? null : ReflectionHelper.GetFieldValue<StatManager>(controller, "StatManager");
        }

        public static MovementController? GetLocalMovementController()
        {
            return GetMovementController(GetLocalPlayer());
        }

        public static MovementController? GetMovementController(ProtoActor? actor)
        {
            return GetServerActor(actor)?.MovementControlUnit;
        }

        // These controllers belong to the host's server actor, not to a Unity component.
        internal static VActor? GetServerActor(ProtoActor? actor)
        {
            if (actor == null)
                return null;
            var room = CoreAPI.GetVRoomManager()?.FindRoomByActorObjectID(actor.ActorID);
            var serverActor = room?.FindActorByObjectID(actor.ActorID);
            return serverActor?.UID == actor.UID ? serverActor : null;
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
            return inventory != null ? ReflectionHelper.GetPropertyValue<List<InventoryItem>>(inventory, "SlotItems") ?? new List<InventoryItem>() : new List<InventoryItem>();
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
            return actor != null && !actor.dead;
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

            return players.Where(p => p != null && IsPlayerValid(p) && IsPlayerAlive(p) && Vector3.Distance(p.transform.position, searchCenter) <= range).ToList();
        }

        public static ProtoActor? GetNearestPlayer(Vector3? center = null)
        {
            Vector3 searchCenter = center.HasValue ? center.Value : GetLocalPlayerPosition();
            var players = GetAllPlayers();
            if (players == null || players.Length == 0)
                return null;

            return players.Where(p => p != null && IsPlayerValid(p)).OrderBy(p => Vector3.Distance(p.transform.position, searchCenter)).FirstOrDefault();
        }

        public static ProtoActor? GetNearestAlivePlayer(Vector3? center = null)
        {
            Vector3 searchCenter = center.HasValue ? center.Value : GetLocalPlayerPosition();
            var players = GetAllPlayers();
            if (players == null || players.Length == 0)
                return null;

            return players.Where(p => p != null && IsPlayerValid(p) && IsPlayerAlive(p)).OrderBy(p => Vector3.Distance(p.transform.position, searchCenter)).FirstOrDefault();
        }

        public static List<ProtoActor> GetPlayersInRange(float range, Vector3? center = null)
        {
            Vector3 searchCenter = center.HasValue ? center.Value : GetLocalPlayerPosition();
            var players = GetAllPlayers();
            if (players == null)
                return new List<ProtoActor>();

            return players.Where(p => p != null && IsPlayerValid(p) && Vector3.Distance(p.transform.position, searchCenter) <= range).ToList();
        }

        public static string GetPlayerName(ProtoActor? actor)
        {
            return actor == null ? "Unknown" : actor.gameObject.name;
        }

        private static ProtoActor? FindActorWhere(Func<ProtoActor, bool> predicate)
        {
            return FindActorsWhere(predicate).FirstOrDefault();
        }

        private static ProtoActor[] FindActorsWhere(Func<ProtoActor, bool> predicate)
        {
            var main = CoreAPI.GetPersistentData()?.main;
            return main == null ? Array.Empty<ProtoActor>() : main.GetAllPlayers().Where(predicate).ToArray();
        }
    }
}
