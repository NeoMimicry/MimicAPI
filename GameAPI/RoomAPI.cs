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

        public static int GetRoomPlayerCount(IVroom? room)
        {
            if (room == null)
                return 0;
            var dict = ReflectionHelper.GetFieldValue<Dictionary<int, VPlayer>>(room, "_vPlayerDict");
            return dict?.Count ?? 0;
        }

        public static int GetRoomActorCount(IVroom? room)
        {
            if (room == null)
                return 0;
            var dict = ReflectionHelper.GetFieldValue<Dictionary<int, VActor>>(room, "_vActorDict");
            return dict?.Count ?? 0;
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
