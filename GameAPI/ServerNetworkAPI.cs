using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MimicAPI.GameAPI
{
    public static class ServerNetworkAPI
    {
        private static object? _serverSocket = null;
        private static Type? _serverSocketType = null;

        public static object? GetServerSocket()
        {
            if (_serverSocket != null)
                return _serverSocket;

            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Contains("FishySteamworks"));

                if (assembly == null)
                    return null;

                _serverSocketType = assembly.GetTypes().FirstOrDefault(t => t.Name == "ServerSocket");

                if (_serverSocketType == null)
                    return null;

                var instanceField = _serverSocketType.GetField("instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceField != null)
                {
                    _serverSocket = instanceField.GetValue(null);
                    return _serverSocket;
                }

                var sField = _serverSocketType.GetField("s", BindingFlags.Public | BindingFlags.Static);
                if (sField != null)
                {
                    _serverSocket = sField.GetValue(null);
                    return _serverSocket;
                }
            }
            catch { }

            return null;
        }

        public static int GetMaximumClients()
        {
            object? socket = GetServerSocket();
            if (socket == null)
                return 0;

            object? result = ReflectionHelper.InvokeMethod(socket, "GetMaximumClients");
            return result is int intValue ? intValue : 0;
        }

        public static void SetMaximumClients(object serverSocket, int value)
        {
            if (serverSocket == null)
                return;
            ReflectionHelper.SetFieldValue(serverSocket, "_maximumClients", value);
        }

        public static int GetCurrentClientCount()
        {
            object? socket = GetServerSocket();
            if (socket == null)
                return 0;

            object? result = ReflectionHelper.InvokeMethod(socket, "GetClientCount");
            return result is int intValue ? intValue : 0;
        }

        public static bool IsServerRunning()
        {
            return GetServerSocket() != null;
        }

        public static object? GetVRoomManager()
        {
            return CoreAPI.GetVRoomManager();
        }

        public static object? GetWaitingRoom()
        {
            var roomManager = GetVRoomManager();
            if (roomManager == null)
                return null;

            var vrooms = ReflectionHelper.GetFieldValue(roomManager, "_vrooms");
            if (vrooms == null)
                return null;

            var vroomsDict = vrooms as IDictionary;
            if (vroomsDict == null)
                return null;

            foreach (var room in vroomsDict.Values)
            {
                if (room.GetType().Name == "VWaitingRoom")
                    return room;
            }

            return null;
        }

        public static object? GetMaintenanceRoom()
        {
            var roomManager = GetVRoomManager();
            if (roomManager == null)
                return null;

            var vrooms = ReflectionHelper.GetFieldValue(roomManager, "_vrooms");
            if (vrooms == null)
                return null;

            var vroomsDict = vrooms as IDictionary;
            if (vroomsDict == null)
                return null;

            foreach (var room in vroomsDict.Values)
            {
                if (room.GetType().Name == "MaintenanceRoom")
                    return room;
            }

            return null;
        }

        public static int GetWaitingRoomMemberCount()
        {
            object? waitingRoom = GetWaitingRoom();
            if (waitingRoom == null)
                return 0;

            object? result = ReflectionHelper.InvokeMethod(waitingRoom, "GetMemberCount");
            return result is int intValue ? intValue : 0;
        }

        public static int GetWaitingRoomMaxPlayers()
        {
            object? waitingRoom = GetWaitingRoom();
            if (waitingRoom == null)
                return 0;

            return ReflectionHelper.GetFieldValue<int>(waitingRoom, "_maxPlayers");
        }

        public static int GetMaintenanceRoomMemberCount()
        {
            object? maintenanceRoom = GetMaintenanceRoom();
            if (maintenanceRoom == null)
                return 0;

            object? result = ReflectionHelper.InvokeMethod(maintenanceRoom, "GetMemberCount");
            return result is int intValue ? intValue : 0;
        }

        public static int GetMaintenanceRoomMaxPlayers()
        {
            object? maintenanceRoom = GetMaintenanceRoom();
            if (maintenanceRoom == null)
                return 0;

            return ReflectionHelper.GetFieldValue<int>(maintenanceRoom, "_maxPlayers");
        }

        public static bool CanPlayerEnterWaitingRoom(long playerUID)
        {
            object? waitingRoom = GetWaitingRoom();
            if (waitingRoom == null)
                return false;

            try
            {
                var result = ReflectionHelper.InvokeMethod(waitingRoom, "CanEnterChannel", playerUID);
                if (result == null)
                    return false;

                var msgErrorCodeType = result.GetType();
                if (msgErrorCodeType.IsEnum)
                {
                    string resultName = Enum.GetName(msgErrorCodeType, result);
                    return resultName == "Success";
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static List<object> GetAllConnectedPlayers()
        {
            var result = new List<object>();

            try
            {
                object? socket = GetServerSocket();
                if (socket == null)
                    return result;

                var connectionsField = socket.GetType().GetField("_connections", BindingFlags.NonPublic | BindingFlags.Instance);
                if (connectionsField == null)
                    return result;

                object? connections = connectionsField.GetValue(socket);
                if (connections is IDictionary connDict)
                {
                    result.AddRange(connDict.Values.Cast<object>());
                }
            }
            catch { }

            return result;
        }

        public static Type? GetMsgErrorCodeType()
        {
            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Contains("FishySteamworks"));
                if (assembly == null)
                    return null;

                return assembly.GetTypes().FirstOrDefault(t => t.Name == "MsgErrorCode");
            }
            catch
            {
                return null;
            }
        }

        public static object? CreateErrorCodeEnum(string value)
        {
            var msgErrorCodeType = GetMsgErrorCodeType();
            if (msgErrorCodeType == null || !msgErrorCodeType.IsEnum)
                return null;

            return Enum.Parse(msgErrorCodeType, value);
        }

        public static void SetRoomMaxPlayers(object? room, int maxPlayers)
        {
            if (room == null)
                return;
            ReflectionHelper.SetFieldValue(room, "_maxPlayers", maxPlayers);
        }

        public static int GetRoomPlayerCount(object? room)
        {
            if (room == null)
                return 0;

            var vPlayerDict = ReflectionHelper.GetFieldValue(room, "_vPlayerDict");
            if (vPlayerDict is IDictionary dict)
                return dict.Count;

            return 0;
        }

        public static IDictionary? GetRoomPlayerDictionary(object? room)
        {
            if (room == null)
                return null;
            return ReflectionHelper.GetFieldValue(room, "_vPlayerDict") as IDictionary;
        }

        public static Assembly? GetServerAssembly()
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Contains("FishySteamworks"));
            }
            catch
            {
                return null;
            }
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

        public static Type? GetServerSocketType()
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;
            return assembly.GetTypes().FirstOrDefault(t => t.Name == "ServerSocket");
        }
    }
}
