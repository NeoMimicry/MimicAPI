using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MimicAPI.GameAPI
{
    public static class ActorAPI
    {
        public static List<object> GetAllVPlayersInRoom(object? room)
        {
            if (room == null)
                return new List<object>();

            var dict = ReflectionHelper.GetFieldValue(room, "_vPlayerDict") as IDictionary;
            return dict == null ? new List<object>() : dict.Values.Cast<object>().ToList();
        }

        public static object? GetVPlayerInRoom(object? room, int actorID)
        {
            return GetAllVPlayersInRoom(room).FirstOrDefault(p => ReflectionHelper.GetFieldValue<int>(p, "ObjectID") == actorID);
        }

        public static List<object> GetAlivePlayersInRoom(object? room) => GetAllVPlayersInRoom(room).Where(p => IsAlive(p)).ToList();

        public static List<object> GetDeadPlayersInRoom(object? room) => GetAllVPlayersInRoom(room).Where(p => !IsAlive(p)).ToList();

        public static List<object> GetAllVActorsInRoom(object? room)
        {
            if (room == null)
                return new List<object>();

            var dict = ReflectionHelper.GetFieldValue(room, "_vActorDict") as IDictionary;
            return dict == null ? new List<object>() : dict.Values.Cast<object>().ToList();
        }

        public static object? GetVActorInRoom(object? room, int actorID)
        {
            return GetAllVActorsInRoom(room).FirstOrDefault(a => ReflectionHelper.GetFieldValue<int>(a, "ObjectID") == actorID);
        }

        public static List<object> GetMonstersInRoom(object? room) => GetAllVActorsInRoom(room).Where(a => a?.GetType().Name == "VMonster").ToList();

        public static List<object> GetAliveMonstersInRoom(object? room) => GetMonstersInRoom(room).Where(m => IsAlive(m)).ToList();

        public static List<object> GetDeadMonstersInRoom(object? room) => GetMonstersInRoom(room).Where(m => !IsAlive(m)).ToList();

        public static List<object> GetLootingObjectsInRoom(object? room) => GetAllVActorsInRoom(room).Where(a => a?.GetType().Name == "VLootingObject").ToList();

        public static List<object> GetActorsByTypeName(object? room, string typeName) => GetAllVActorsInRoom(room).Where(a => a?.GetType().Name == typeName).ToList();

        public static bool HasAliveMonstersInRoom(object? room) => GetAliveMonstersInRoom(room).Any();

        public static bool HasAlivePlayersInRoom(object? room) => GetAlivePlayersInRoom(room).Any();

        private static bool IsAlive(object? actor) => actor != null && ReflectionHelper.InvokeMethod(actor, "IsAliveStatus") is bool b && b;
    }
}
