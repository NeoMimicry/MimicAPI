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

        public static FieldInfo? GetMaximumClientsField()
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;

            var serverSocketType = assembly.GetTypes().FirstOrDefault(t => t.Name == "ServerSocket");
            if (serverSocketType == null)
                return null;

            return serverSocketType.GetField("_maximumClients", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static IEnumerable<MethodBase> GetAllServerSocketMethods()
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return new MethodBase[0];

            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == "ServerSocket");
            if (type == null)
                return new MethodBase[0];

            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => !m.IsAbstract && m.DeclaringType == type);
        }

        public static MethodInfo? GetVRoomManagerMethod(string methodName)
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;

            var vroomManagerType = assembly.GetTypes().FirstOrDefault(t => t.Name == "VRoomManager");
            if (vroomManagerType == null)
                return null;

            return vroomManagerType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MethodInfo? GetRoomMethod(string roomTypeName, string methodName)
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;

            var roomType = assembly.GetTypes().FirstOrDefault(t => t.Name == roomTypeName);
            if (roomType == null)
                return null;

            return roomType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MethodInfo? GetSteamInviteMethod(string methodName)
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;

            var steamInviteType = assembly.GetTypes().FirstOrDefault(t => t.Name == "SteamInviteDispatcher");
            if (steamInviteType == null)
                return null;

            return steamInviteType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }

        public static MethodInfo? GetUIMethod(string uiTypeName, string methodName)
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;

            var uiType = assembly.GetTypes().FirstOrDefault(t => t.Name == uiTypeName);
            if (uiType == null)
                return null;

            return uiType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MethodBase? GetServerSocketMethod(string methodName)
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;

            var serverSocketType = assembly.GetTypes().FirstOrDefault(t => t.Name == "ServerSocket");
            if (serverSocketType == null)
                return null;

            return serverSocketType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static MethodBase? GetServerMethod(string typeName, string methodName)
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;

            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
            if (type == null)
                return null;

            return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static MethodBase? GetIVroomMethod(string methodName)
        {
            var csharpAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");
            if (csharpAssembly == null)
                return null;

            var ivroomType = csharpAssembly.GetType("IVroom");
            if (ivroomType == null)
                return null;

            return ivroomType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static MethodBase? GetAssemblyMethod(string assemblyName, string typeName, string methodName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            if (assembly == null)
                return null;

            var type = assembly.GetType(typeName);
            if (type == null)
                return null;

            return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static Type? GetMsgErrorCodeType()
        {
            var assembly = GetServerAssembly();
            if (assembly == null)
                return null;

            return assembly.GetTypes().FirstOrDefault(t => t.Name == "MsgErrorCode");
        }

        public static int GetRoomMemberCount(object room)
        {
            if (room == null)
                return 0;

            object? result = ReflectionHelper.InvokeMethod(room, "GetMemberCount");
            return result is int intValue ? intValue : 0;
        }

        private static Assembly? GetServerAssembly()
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
    }
}
