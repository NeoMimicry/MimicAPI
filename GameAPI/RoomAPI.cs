using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mimic.Actors;

namespace MimicAPI.GameAPI
{
    public static class RoomAPI
    {
        public static IVroom? GetRoom(long roomID)
        {
            IDictionary? roomDict = GetRoomDictionary();
            if (roomDict == null)
                return null;

            return roomDict[roomID] as IVroom;
        }

        public static IVroom? GetCurrentRoom()
        {
            ProtoActor? player = PlayerAPI.GetLocalPlayer();
            if (player == null)
                return null;

            IVroom[] rooms = GetAllRooms();
            return rooms.FirstOrDefault(room => room != null && IsRoomPlayable(room));
        }

        public static IVroom[] GetAllRooms()
        {
            IDictionary? roomDict = GetRoomDictionary();
            if (roomDict == null)
                return new IVroom[0];

            IVroom[] rooms = new IVroom[roomDict.Count];
            int i = 0;
            foreach (var room in roomDict.Values)
            {
                rooms[i++] = (IVroom)room;
            }
            return rooms;
        }

        public static List<IVroom> GetAllPlayableRooms()
        {
            return GetAllRooms().Where(r => r != null && IsRoomPlayable(r)).ToList();
        }

        public static long GetRoomID(IVroom? room)
        {
            return room == null ? 0 : ReflectionHelper.GetFieldValue<long>(room, "RoomID");
        }

        public static int GetRoomMasterID(IVroom? room)
        {
            return room == null ? 0 : ReflectionHelper.GetFieldValue<int>(room, "MasterID");
        }

        public static bool IsRoomPlayable(IVroom? room)
        {
            if (room == null)
                return false;
            return ReflectionHelper.InvokeMethod(room, "IsPlayable") is bool result && result;
        }

        public static int GetCurrentGameDay(IVroom? room)
        {
            return room == null ? 0 : ReflectionHelper.GetFieldValue<int>(room, "_currentDay");
        }

        public static int GetCurrentSessionCycle(IVroom? room)
        {
            return room == null ? 0 : ReflectionHelper.GetFieldValue<int>(room, "_currentSessionCount");
        }

        public static Dictionary<int, ILevelObjectInfo>? GetRoomLevelObjects(IVroom? room)
        {
            if (room == null)
                return new Dictionary<int, ILevelObjectInfo>();
            return ReflectionHelper.GetFieldValue<Dictionary<int, ILevelObjectInfo>>(room, "_levelObjects") ?? new Dictionary<int, ILevelObjectInfo>();
        }

        public static List<VPlayer> GetRoomPlayers(IVroom? room)
        {
            if (room == null)
                return new List<VPlayer>();
            var dict = ReflectionHelper.GetFieldValue<Dictionary<int, VPlayer>>(room, "_vPlayerDict");
            return dict?.Values.ToList() ?? new List<VPlayer>();
        }

        public static List<VActor> GetRoomActors(IVroom? room)
        {
            if (room == null)
                return new List<VActor>();
            var dict = ReflectionHelper.GetFieldValue<Dictionary<int, VActor>>(room, "_vActorDict");
            return dict?.Values.ToList() ?? new List<VActor>();
        }

        public static List<long> GetAllRoomIDs()
        {
            IDictionary? roomDict = GetRoomDictionary();
            if (roomDict == null)
                return new List<long>();

            var ids = new List<long>();
            foreach (var key in roomDict.Keys)
            {
                if (key is long longKey)
                    ids.Add(longKey);
            }
            return ids;
        }

        public static IVroom[] GetAllRoomsOfType<T>() where T : IVroom
        {
            return GetAllRooms().Where(r => r is T).ToArray();
        }

        public static bool RoomExists(long roomID)
        {
            return GetRoom(roomID) != null;
        }

        public static int GetRoomMaxPlayers(IVroom? room)
        {
            if (room == null)
                return 0;
            return ReflectionHelper.GetFieldValue<int>(room, "_maxPlayers");
        }

        public static int GetRoomMaxPlayers(object? room)
        {
            if (room == null)
                return 0;
            return ReflectionHelper.GetFieldValue<int>(room, "_maxPlayers");
        }

        public static void SetRoomMaxPlayers(IVroom? room, int maxPlayers)
        {
            if (room == null)
                return;
            ReflectionHelper.SetFieldValue(room, "_maxPlayers", maxPlayers);
        }

        public static void SetRoomMaxPlayers(object? room, int maxPlayers)
        {
            if (room == null)
                return;
            ReflectionHelper.SetFieldValue(room, "_maxPlayers", maxPlayers);
        }

        public static string GetRoomName(IVroom? room)
        {
            if (room == null)
                return "Unknown";
            return room.GetType().Name;
        }

        private static IDictionary? GetRoomDictionary()
        {
            VRoomManager? roomManager = CoreAPI.GetVRoomManager();
            if (roomManager == null)
                return null;

            object? roomDict = ReflectionHelper.GetFieldValue(roomManager, "_roomDict");
            return roomDict as IDictionary;
        }
    }
}