using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MimicAPI.GameAPI
{
    public static class ServerNetworkAPI
    {
        private static object? GetVWorld()
        {
            var hub = CoreAPI.GetHub();
            if (hub == null)
                return null;
            return ReflectionHelper.GetFieldValue(hub, "<vworld>k__BackingField");
        }

        private static object? GetSessionManager()
        {
            var vworld = GetVWorld();
            if (vworld == null)
                return null;
            return ReflectionHelper.GetFieldValue(vworld, "_sessionManager");
        }

        public static object? GetRudpServer()
        {
            var vworld = GetVWorld();
            if (vworld == null)
                return null;
            return ReflectionHelper.GetFieldValue(vworld, "_rudpServer");
        }

        public static object? GetSdrServer()
        {
            var vworld = GetVWorld();
            if (vworld == null)
                return null;
            return ReflectionHelper.GetFieldValue(vworld, "_sdrServer");
        }

        public static object? GetServerSocket() => GetSdrServer() ?? GetRudpServer();

        public static bool IsServerRunning() => GetVWorld() != null;

        public static int GetSessionCount()
        {
            var sm = GetSessionManager();
            if (sm == null)
                return 0;
            var contexts = ReflectionHelper.GetFieldValue(sm, "m_Contexts") as IDictionary;
            return contexts?.Count ?? 0;
        }

        public static int GetCurrentClientCount() => GetSessionCount();

        public static int GetMaximumClients()
        {
            var vrm = CoreAPI.GetVRoomManager();
            if (vrm == null)
                return 0;
            return ReflectionHelper.InvokeMethod(vrm, "GetPlayerCountInSession") is int n ? n : 0;
        }

        public static void SetMaximumClients(object serverSocket, int value)
        {
            ReflectionHelper.SetFieldValue(serverSocket, "_maximumClients", value);
        }

        public static List<object> GetAllConnectedPlayers()
        {
            var sm = GetSessionManager();
            if (sm == null)
                return new List<object>();

            var contexts = ReflectionHelper.GetFieldValue(sm, "m_Contexts") as IDictionary;
            if (contexts == null)
                return new List<object>();

            return contexts.Values.Cast<object>().ToList();
        }

        public static object? GetWaitingRoom() => RoomAPI.GetAllRooms().FirstOrDefault(r => r?.GetType().Name == "VWaitingRoom");

        public static object? GetMaintenanceRoom() => RoomAPI.GetAllRooms().FirstOrDefault(r => r?.GetType().Name == "MaintenanceRoom");

        public static int GetWaitingRoomMemberCount() => RoomAPI.GetMemberCount(GetWaitingRoom());

        public static int GetWaitingRoomMaxPlayers() => RoomAPI.GetMemberCount(GetWaitingRoom());

        public static int GetMaintenanceRoomMemberCount() => RoomAPI.GetMemberCount(GetMaintenanceRoom());

        public static int GetMaintenanceRoomMaxPlayers() => RoomAPI.GetMemberCount(GetMaintenanceRoom());

        public static bool CanPlayerEnterWaitingRoom(long playerUID)
        {
            var room = GetWaitingRoom();
            if (room == null)
                return false;
            try
            {
                var result = ReflectionHelper.InvokeMethod(room, "CanEnterChannel", playerUID);
                if (result?.GetType().IsEnum == true)
                    return Enum.GetName(result.GetType(), result) == "Success";
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPlayerInRoom(object room, long playerUID)
        {
            var dict = GetRoomPlayerDictionary(room);
            if (dict == null)
                return false;
            foreach (var player in dict.Values)
            {
                if (ReflectionHelper.GetPropertyValue<long>(player, "UID") == playerUID)
                    return true;
            }
            return false;
        }

        public static int GetRoomPlayerCount(object? room)
        {
            if (room == null)
                return 0;
            var dict = ReflectionHelper.GetFieldValue(room, "_vPlayerDict") as IDictionary;
            return dict?.Count ?? 0;
        }

        public static IDictionary? GetRoomPlayerDictionary(object? room)
        {
            if (room == null)
                return null;
            return ReflectionHelper.GetFieldValue(room, "_vPlayerDict") as IDictionary;
        }

        public static Assembly? GetGameAssembly()
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");
            }
            catch
            {
                return null;
            }
        }

        public static Type? GetIVroomType() => GetGameAssembly()?.GetType("IVroom");

        public static Type? GetGameSessionInfoType() => GetGameAssembly()?.GetType("GameSessionInfo");
    }
}
