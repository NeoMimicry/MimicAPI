using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MimicAPI.GameAPI
{
    public static class RoomAPI
    {
        private static IDictionary? GetRoomDictionary()
        {
            var roomManager = CoreAPI.GetVRoomManager();
            if (roomManager == null)
                return null;

            return ReflectionHelper.GetFieldValue(roomManager, "_vrooms") as IDictionary;
        }

        public static object? GetRoom(long roomID)
        {
            var rooms = GetRoomDictionary();
            if (rooms == null)
                return null;

            foreach (var key in rooms.Keys)
            {
                if (key is long id && id == roomID)
                    return rooms[key];
            }
            return null;
        }

        public static object[] GetAllRooms()
        {
            var rooms = GetRoomDictionary();
            if (rooms == null)
                return System.Array.Empty<object>();

            var result = new object[rooms.Count];
            int i = 0;
            foreach (var room in rooms.Values)
                result[i++] = room;
            return result;
        }

        public static List<long> GetAllRoomIDs()
        {
            var rooms = GetRoomDictionary();
            if (rooms == null)
                return new List<long>();

            var ids = new List<long>();
            foreach (var key in rooms.Keys)
            {
                if (key is long id)
                    ids.Add(id);
            }
            return ids;
        }

        public static bool RoomExists(long roomID) => GetRoom(roomID) != null;

        public static object? GetCurrentRoom()
        {
            return GetAllRooms().FirstOrDefault(r => r != null && IsRoomPlayable(r));
        }

        public static List<object> GetAllPlayableRooms() => GetAllRooms().Where(r => r != null && IsRoomPlayable(r)).ToList();

        public static long GetRoomID(object? room) => room == null ? 0L : ReflectionHelper.GetFieldValue<long>(room, "RoomID");

        public static int GetRoomMasterID(object? room) => room == null ? 0 : ReflectionHelper.GetFieldValue<int>(room, "MasterID");

        public static string GetRoomName(object? room) => room == null ? "Unknown" : room.GetType().Name;

        public static object? GetRoomType(object? room) => room == null ? null : ReflectionHelper.InvokeMethod(room, "GetToMoveRoomType");

        public static object? GetRoomProperty(object? room) => room == null ? null : ReflectionHelper.InvokeMethod(room, "GetProperty");

        public static bool IsRoomPlayable(object? room) => room != null && ReflectionHelper.InvokeMethod(room, "IsPlayable") is bool b && b;

        public static int GetCurrentGameDay(object? room) => room == null ? 0 : ReflectionHelper.GetFieldValue<int>(room, "_currentDay");

        public static int GetCurrentSessionCycle(object? room) => room == null ? 0 : ReflectionHelper.GetFieldValue<int>(room, "_currentSessionCount");

        public static long GetCurrentTick(object? room) => room == null ? 0L : ReflectionHelper.GetFieldValue<long>(room, "_currentTick");

        public static bool IsAllPlayerDead(object? room) => room != null && ReflectionHelper.InvokeMethod(room, "IsAllPlayerDead") is bool b && b;

        public static bool IsAllPlayerWastedOrDead(object? room) => room != null && ReflectionHelper.InvokeMethod(room, "IsAllPlayerWastedOrDead") is bool b && b;

        public static int GetDeadPlayerCount(object? room) => room == null ? 0 : (ReflectionHelper.InvokeMethod(room, "GetDeadPlayerCount") is int n ? n : 0);

        public static int GetMemberCount(object? room) => room == null ? 0 : (ReflectionHelper.InvokeMethod(room, "GetMemberCount") is int n ? n : 0);

        public static List<object> GetRoomPlayers(object? room)
        {
            if (room == null)
                return new List<object>();

            var dict = ReflectionHelper.GetFieldValue(room, "_vPlayerDict") as IDictionary;
            if (dict == null)
                return new List<object>();

            return dict.Values.Cast<object>().ToList();
        }

        public static List<object> GetRoomActors(object? room)
        {
            if (room == null)
                return new List<object>();

            var dict = ReflectionHelper.GetFieldValue(room, "_vActorDict") as IDictionary;
            if (dict == null)
                return new List<object>();

            return dict.Values.Cast<object>().ToList();
        }

        public static IDictionary? GetRoomLevelObjects(object? room)
        {
            if (room == null)
                return null;

            return ReflectionHelper.GetFieldValue(room, "_levelObjects") as IDictionary;
        }

        public static int GetRoomCurrency(object? room) => room == null ? 0 : ReflectionHelper.GetFieldValue<int>(room, "<Currency>k__BackingField");

        public static long GetContaRecoveryRate(object? room) => room == null ? 0L : (ReflectionHelper.InvokeMethod(room, "GetContaRecoveryRate") is long v ? v : 0L);

        public static int GetRoomPlayerCount(object? room) => room == null ? 0 : ServerNetworkAPI.GetRoomPlayerCount(room);

        public static IDictionary? GetRoomPlayerDictionary(object? room) => room == null ? null : ServerNetworkAPI.GetRoomPlayerDictionary(room);
    }
}
