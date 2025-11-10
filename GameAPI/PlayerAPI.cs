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
