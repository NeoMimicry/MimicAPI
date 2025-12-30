using System;
using System.Collections.Generic;
using System.Linq;
using Bifrost.ConstEnum;
using MelonLoader;
using MimicAPI.GameAPI;
using shadcnui.GUIComponents.Core;
using shadcnui.GUIComponents.Core.Base;
using shadcnui.GUIComponents.Core.Styling;
using shadcnui.GUIComponents.Layout;
using UnityEngine;

namespace MimicAPI.TestMod
{
    public class TestGUI : MonoBehaviour
    {
        private GUIHelper guiHelper;
        private Rect windowRect = new Rect(20, 20, 1200, 700);
        private Vector2 scrollPosition;
        private int currentTab;
        private Tabs.TabConfig[] tabs;
        private bool showWindow = true;

        private Dictionary<string, bool> toggleStates = new Dictionary<string, bool>();
        private Dictionary<string, float> sliderStates = new Dictionary<string, float>();
        private List<TestResult> testResults = new List<TestResult>();

        void Start()
        {
            try
            {
                guiHelper = new GUIHelper();
                tabs = new Tabs.TabConfig[]
                {
                    new Tabs.TabConfig("Core", DrawCoreTab),
                    new Tabs.TabConfig("Managers", DrawManagersTab),
                    new Tabs.TabConfig("Player", DrawPlayerTab),
                    new Tabs.TabConfig("Room", DrawRoomTab),
                    new Tabs.TabConfig("Actor", DrawActorTab),
                    new Tabs.TabConfig("Loot", DrawLootTab),
                    new Tabs.TabConfig("Network", DrawNetworkTab),
                    new Tabs.TabConfig("Tests", DrawTestsTab),
                };
                MelonLogger.Msg("TestGUI initialized");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"TestGUI Start error: {ex.Message}");
            }
        }

        void OnGUI()
        {
            try
            {
                if (guiHelper == null)
                {
                    guiHelper = new GUIHelper();
                }

                windowRect = GUI.Window(12345, windowRect, DrawWindow, "MimicAPI Test");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"OnGUI error: {ex.Message}");
            }
        }

        void DrawWindow(int id)
        {
            try
            {
                guiHelper.UpdateGUI(showWindow);
                if (!guiHelper.BeginGUI())
                {
                    GUI.DragWindow();
                    return;
                }

                guiHelper.BeginHorizontalGroup();
                guiHelper.Label("MimicAPI Test", ControlVariant.Default);
                GUILayout.FlexibleSpace();
                if (guiHelper.Button("Run All Tests", ControlVariant.Secondary))
                    RunAllTests();
                guiHelper.EndHorizontalGroup();

                currentTab = guiHelper.Tabs(tabs.Select(t => t.Name).ToArray(), currentTab, DrawTabContent);

                guiHelper.EndGUI();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"DrawWindow error: {ex.Message}");
            }
            GUI.DragWindow();
        }

        void DrawTabContent()
        {
            scrollPosition = guiHelper.ScrollView(
                scrollPosition,
                () =>
                {
                    guiHelper.BeginVerticalGroup(GUILayout.ExpandHeight(true));
                    try
                    {
                        tabs[currentTab].Content?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"Tab error: {ex.Message}");
                    }
                    guiHelper.EndVerticalGroup();
                },
                GUILayout.Height(550)
            );
        }

        void DrawSection(string title, Action content)
        {
            try
            {
                guiHelper.Label(title, ControlVariant.Default);
                guiHelper.HorizontalSeparator();
                content?.Invoke();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Section '{title}' error: {ex.Message}");
            }
            GUILayout.Space(15);
        }

        #region Core Tab
        void DrawCoreTab()
        {
            DrawSection(
                "Hub",
                () =>
                {
                    var hub = CoreAPI.GetHub();
                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Hub Status:");
                    guiHelper.Badge(hub != null ? "Connected" : "Not Found", hub != null ? ControlVariant.Default : ControlVariant.Destructive);
                    guiHelper.EndHorizontalGroup();
                }
            );

            DrawSection(
                "Persistent Data",
                () =>
                {
                    var pdata = CoreAPI.GetPersistentData();
                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Persistent Data:");
                    guiHelper.Badge(pdata != null ? "Loaded" : "Not Loaded", pdata != null ? ControlVariant.Default : ControlVariant.Secondary);
                    guiHelper.EndHorizontalGroup();
                }
            );
        }
        #endregion

        #region Managers Tab
        void DrawManagersTab()
        {
            var managers = new (string Name, Func<object> Getter)[]
            {
                ("DataManager", () => ManagerAPI.GetDataManager()),
                ("TimeUtil", () => ManagerAPI.GetTimeUtil()),
                ("NavManager", () => ManagerAPI.GetNavManager()),
                ("DynamicDataManager", () => ManagerAPI.GetDynamicDataManager()),
                ("UIManager", () => ManagerAPI.GetUIManager()),
                ("CameraManager", () => ManagerAPI.GetCameraManager()),
                ("AudioManager", () => ManagerAPI.GetAudioManager()),
                ("InputManager", () => ManagerAPI.GetInputManager()),
                ("NetworkManager", () => ManagerAPI.GetNetworkManager()),
                ("APIHandler", () => ManagerAPI.GetAPIHandler()),
                ("L10NManager", () => ManagerAPI.GetLocalisationManager()),
            };

            DrawSection(
                "Manager Status",
                () =>
                {
                    foreach (var (name, getter) in managers)
                    {
                        guiHelper.BeginHorizontalGroup();
                        guiHelper.Label($"{name}:", ControlVariant.Default);
                        GUILayout.FlexibleSpace();
                        try
                        {
                            var mgr = getter();
                            guiHelper.Badge(mgr != null ? "OK" : "NULL", mgr != null ? ControlVariant.Default : ControlVariant.Secondary);
                        }
                        catch
                        {
                            guiHelper.Badge("Error", ControlVariant.Destructive);
                        }
                        guiHelper.EndHorizontalGroup();
                    }
                }
            );
        }
        #endregion

        #region Player Tab
        void DrawPlayerTab()
        {
            DrawSection(
                "Local Player",
                () =>
                {
                    var player = PlayerAPI.GetLocalPlayer();
                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Local Player:");
                    guiHelper.Badge(player != null ? PlayerAPI.GetPlayerName(player) : "Not Found", player != null ? ControlVariant.Default : ControlVariant.Destructive);
                    guiHelper.EndHorizontalGroup();

                    if (player != null)
                    {
                        var pos = PlayerAPI.GetLocalPlayerPosition();
                        guiHelper.MutedLabel($"Position: {pos.x:F1}, {pos.y:F1}, {pos.z:F1}");

                        guiHelper.BeginHorizontalGroup();
                        guiHelper.Label("Alive:");
                        guiHelper.Badge(PlayerAPI.IsLocalPlayerAlive() ? "Yes" : "No", PlayerAPI.IsLocalPlayerAlive() ? ControlVariant.Default : ControlVariant.Destructive);
                        guiHelper.EndHorizontalGroup();
                    }
                }
            );

            DrawSection(
                "All Players",
                () =>
                {
                    var players = PlayerAPI.GetAllPlayers();
                    guiHelper.Label($"Total Players: {players?.Length ?? 0}");

                    if (players != null && players.Length > 0)
                    {
                        string[] headers = { "Name", "Position", "Alive" };
                        string[,] data = new string[Math.Min(players.Length, 10), 3];

                        for (int i = 0; i < Math.Min(players.Length, 10); i++)
                        {
                            var p = players[i];
                            var pos = PlayerAPI.GetPlayerPosition(p);
                            data[i, 0] = PlayerAPI.GetPlayerName(p);
                            data[i, 1] = $"{pos.x:F0}, {pos.y:F0}, {pos.z:F0}";
                            data[i, 2] = PlayerAPI.IsPlayerAlive(p) ? "Yes" : "No";
                        }

                        guiHelper.Table(headers, data, ControlVariant.Secondary);
                    }
                }
            );

            DrawSection(
                "Player Search",
                () =>
                {
                    if (!sliderStates.ContainsKey("playerRange"))
                        sliderStates["playerRange"] = 50f;
                    sliderStates["playerRange"] = guiHelper.LabeledSlider("Search Range", sliderStates["playerRange"], 1f, 200f, true);

                    guiHelper.BeginHorizontalGroup();
                    if (guiHelper.Button("Find Players In Range"))
                    {
                        var nearby = PlayerAPI.GetPlayersInRange(sliderStates["playerRange"]);
                        guiHelper.ShowInfoToast("Players", $"Found {nearby.Count} players within {sliderStates["playerRange"]:F0}m");
                    }
                    if (guiHelper.Button("Find Nearest Player"))
                    {
                        var nearest = PlayerAPI.GetNearestPlayer();
                        if (nearest != null)
                            guiHelper.ShowSuccessToast("Nearest", PlayerAPI.GetPlayerName(nearest));
                        else
                            guiHelper.ShowInfoToast("Nearest", "No players found");
                    }
                    guiHelper.EndHorizontalGroup();
                }
            );
        }
        #endregion

        #region Room Tab
        void DrawRoomTab()
        {
            DrawSection(
                "Current Room",
                () =>
                {
                    var room = RoomAPI.GetCurrentRoom();
                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Current Room:");
                    guiHelper.Badge(room != null ? RoomAPI.GetRoomName(room) : "None", room != null ? ControlVariant.Default : ControlVariant.Secondary);
                    guiHelper.EndHorizontalGroup();

                    if (room != null)
                    {
                        guiHelper.MutedLabel($"Room ID: {RoomAPI.GetRoomID(room)}");
                        guiHelper.MutedLabel($"Master ID: {RoomAPI.GetRoomMasterID(room)}");
                        guiHelper.MutedLabel($"Game Day: {RoomAPI.GetCurrentGameDay(room)}");
                        guiHelper.MutedLabel($"Session Cycle: {RoomAPI.GetCurrentSessionCycle(room)}");

                        guiHelper.BeginHorizontalGroup();
                        guiHelper.Label("Playable:");
                        guiHelper.Badge(RoomAPI.IsRoomPlayable(room) ? "Yes" : "No", RoomAPI.IsRoomPlayable(room) ? ControlVariant.Default : ControlVariant.Secondary);
                        guiHelper.EndHorizontalGroup();
                    }
                }
            );

            DrawSection(
                "All Rooms",
                () =>
                {
                    var rooms = RoomAPI.GetAllRooms();
                    guiHelper.Label($"Total Rooms: {rooms.Length}");

                    var playableRooms = RoomAPI.GetAllPlayableRooms();
                    guiHelper.Label($"Playable Rooms: {playableRooms.Count}");

                    if (rooms.Length > 0)
                    {
                        string[] headers = { "Name", "ID", "Playable", "Players" };
                        string[,] data = new string[Math.Min(rooms.Length, 10), 4];

                        for (int i = 0; i < Math.Min(rooms.Length, 10); i++)
                        {
                            var r = rooms[i];
                            data[i, 0] = RoomAPI.GetRoomName(r);
                            data[i, 1] = RoomAPI.GetRoomID(r).ToString();
                            data[i, 2] = RoomAPI.IsRoomPlayable(r) ? "Yes" : "No";
                            data[i, 3] = RoomAPI.GetRoomPlayers(r).Count.ToString();
                        }

                        guiHelper.Table(headers, data, ControlVariant.Secondary);
                    }
                }
            );

            DrawSection(
                "Room Settings",
                () =>
                {
                    var room = RoomAPI.GetCurrentRoom();
                    if (room != null)
                    {
                        int roomMaxPlayers = RoomAPI.GetRoomMaxPlayers(room);
                        guiHelper.Label($"Current Max Players: {roomMaxPlayers}");

                        if (!sliderStates.ContainsKey("roomMaxPlayers"))
                            sliderStates["roomMaxPlayers"] = roomMaxPlayers;
                        sliderStates["roomMaxPlayers"] = guiHelper.LabeledSlider("New Max Players", sliderStates["roomMaxPlayers"], 1f, 20f, 1f, true);

                        if (guiHelper.Button("Set Max Players"))
                        {
                            RoomAPI.SetRoomMaxPlayers(room, (int)sliderStates["roomMaxPlayers"]);
                            guiHelper.ShowSuccessToast("Room", $"Max players set to {(int)sliderStates["roomMaxPlayers"]}");
                        }
                    }
                    else
                    {
                        guiHelper.MutedLabel("No room available to configure");
                    }
                }
            );
        }
        #endregion

        #region Actor Tab
        void DrawActorTab()
        {
            var room = RoomAPI.GetCurrentRoom();

            DrawSection(
                "VActors in Room",
                () =>
                {
                    var actors = ActorAPI.GetAllVActorsInRoom(room);
                    guiHelper.Label($"Total Actors: {actors.Count}");

                    if (actors.Count > 0)
                    {
                        string[] headers = { "Type", "ID", "Alive" };
                        string[,] data = new string[Math.Min(actors.Count, 10), 3];

                        for (int i = 0; i < Math.Min(actors.Count, 10); i++)
                        {
                            var a = actors[i];
                            data[i, 0] = a?.GetType().Name ?? "Unknown";
                            data[i, 1] = a?.ObjectID.ToString() ?? "N/A";
                            data[i, 2] = a?.IsAliveStatus() == true ? "Yes" : "No";
                        }

                        guiHelper.Table(headers, data, ControlVariant.Secondary);
                    }
                }
            );

            DrawSection(
                "Monsters",
                () =>
                {
                    var monsters = ActorAPI.GetMonstersInRoom(room);
                    var aliveMonsters = ActorAPI.GetAliveMonstersInRoom(room);
                    var deadMonsters = ActorAPI.GetDeadMonstersInRoom(room);

                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label($"Total Monsters: {monsters.Count}");
                    guiHelper.Badge($"Alive: {aliveMonsters.Count}", ControlVariant.Destructive);
                    guiHelper.Badge($"Dead: {deadMonsters.Count}", ControlVariant.Secondary);
                    guiHelper.EndHorizontalGroup();

                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Has Alive Monsters:");
                    guiHelper.Badge(ActorAPI.HasAliveMonstersInRoom(room) ? "Yes" : "No", ActorAPI.HasAliveMonstersInRoom(room) ? ControlVariant.Destructive : ControlVariant.Default);
                    guiHelper.EndHorizontalGroup();
                }
            );

            DrawSection(
                "Looting Objects",
                () =>
                {
                    var lootObjs = ActorAPI.GetLootingObjectsInRoom(room);
                    guiHelper.Label($"Looting Objects: {lootObjs.Count}");
                }
            );
        }
        #endregion

        #region Loot Tab
        void DrawLootTab()
        {
            DrawSection(
                "Loot Overview",
                () =>
                {
                    var allLoot = LootAPI.GetAllLoot();
                    var inactiveLoot = LootAPI.GetInactiveLoot();

                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label($"Active Loot: {allLoot.Length}");
                    guiHelper.Label($"Inactive Loot: {inactiveLoot.Length}");
                    guiHelper.EndHorizontalGroup();

                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Has Loot:");
                    guiHelper.Badge(LootAPI.HasLoot() ? "Yes" : "No", LootAPI.HasLoot() ? ControlVariant.Default : ControlVariant.Secondary);
                    guiHelper.EndHorizontalGroup();
                }
            );

            DrawSection(
                "Nearby Loot",
                () =>
                {
                    if (!sliderStates.ContainsKey("lootRange"))
                        sliderStates["lootRange"] = 50f;
                    sliderStates["lootRange"] = guiHelper.LabeledSlider("Search Range", sliderStates["lootRange"], 1f, 200f, true);

                    var nearbyLoot = LootAPI.GetLootNearby(sliderStates["lootRange"]);
                    guiHelper.Label($"Loot within {sliderStates["lootRange"]:F0}m: {nearbyLoot.Length}");

                    if (guiHelper.Button("Find Nearest Loot"))
                    {
                        var nearest = LootAPI.GetNearestLoot();
                        if (nearest != null)
                        {
                            var dist = LootAPI.GetDistanceToLoot(nearest);
                            guiHelper.ShowSuccessToast("Nearest Loot", $"{nearest.gameObject.name} at {dist:F1}m");
                        }
                        else
                        {
                            guiHelper.ShowInfoToast("Loot", "No loot found");
                        }
                    }
                }
            );

            DrawSection(
                "Loot List",
                () =>
                {
                    var allLoot = LootAPI.GetAllLoot();
                    if (allLoot.Length > 0)
                    {
                        string[] headers = { "Name", "Distance", "Position" };
                        string[,] data = new string[Math.Min(allLoot.Length, 10), 3];

                        for (int i = 0; i < Math.Min(allLoot.Length, 10); i++)
                        {
                            var l = allLoot[i];
                            var pos = LootAPI.GetLootPosition(l);
                            var dist = LootAPI.GetDistanceToLoot(l);
                            data[i, 0] = l.gameObject.name;
                            data[i, 1] = $"{dist:F1}m";
                            data[i, 2] = $"{pos.x:F0}, {pos.y:F0}, {pos.z:F0}";
                        }

                        guiHelper.Table(headers, data, ControlVariant.Secondary);
                    }
                    else
                    {
                        guiHelper.MutedLabel("No loot available");
                    }
                }
            );

            DrawSection(
                "Filter Loot",
                () =>
                {
                    if (!sliderStates.ContainsKey("lootMinDist"))
                        sliderStates["lootMinDist"] = 10f;
                    if (!sliderStates.ContainsKey("lootMaxDist"))
                        sliderStates["lootMaxDist"] = 50f;

                    sliderStates["lootMinDist"] = guiHelper.LabeledSlider("Min Distance", sliderStates["lootMinDist"], 0f, 100f, true);
                    sliderStates["lootMaxDist"] = guiHelper.LabeledSlider("Max Distance", sliderStates["lootMaxDist"], 0f, 200f, true);

                    if (guiHelper.Button("Filter by Distance"))
                    {
                        var filtered = LootAPI.FilterLootByDistance(sliderStates["lootMinDist"], sliderStates["lootMaxDist"]);
                        guiHelper.ShowInfoToast("Filtered Loot", $"Found {filtered.Length} items between {sliderStates["lootMinDist"]:F0}m and {sliderStates["lootMaxDist"]:F0}m");
                    }
                }
            );
        }
        #endregion

        #region Network Tab
        void DrawNetworkTab()
        {
            DrawSection(
                "Server Status",
                () =>
                {
                    var isRunning = ServerNetworkAPI.IsServerRunning();
                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Server Running:");
                    guiHelper.Badge(isRunning ? "Yes" : "No", isRunning ? ControlVariant.Default : ControlVariant.Secondary);
                    guiHelper.EndHorizontalGroup();

                    if (isRunning)
                    {
                        var maxClients = ServerNetworkAPI.GetMaximumClients();
                        var currentClients = ServerNetworkAPI.GetCurrentClientCount();

                        guiHelper.MutedLabel($"Max Clients: {maxClients}");
                        guiHelper.MutedLabel($"Current Clients: {currentClients}");

                        guiHelper.Progress((float)currentClients / Math.Max(maxClients, 1), 300);
                    }
                }
            );

            DrawSection(
                "Waiting Room",
                () =>
                {
                    var waitingRoom = ServerNetworkAPI.GetWaitingRoom();
                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Waiting Room:");
                    guiHelper.Badge(waitingRoom != null ? "Available" : "Not Found", waitingRoom != null ? ControlVariant.Default : ControlVariant.Secondary);
                    guiHelper.EndHorizontalGroup();

                    if (waitingRoom != null)
                    {
                        var memberCount = ServerNetworkAPI.GetWaitingRoomMemberCount();
                        var maxPlayers = ServerNetworkAPI.GetWaitingRoomMaxPlayers();

                        guiHelper.MutedLabel($"Members: {memberCount}/{maxPlayers}");
                        guiHelper.Progress((float)memberCount / Math.Max(maxPlayers, 1), 300);
                    }
                }
            );

            DrawSection(
                "Maintenance Room",
                () =>
                {
                    var maintenanceRoom = ServerNetworkAPI.GetMaintenanceRoom();
                    guiHelper.BeginHorizontalGroup();
                    guiHelper.Label("Maintenance Room:");
                    guiHelper.Badge(maintenanceRoom != null ? "Available" : "Not Found", maintenanceRoom != null ? ControlVariant.Default : ControlVariant.Secondary);
                    guiHelper.EndHorizontalGroup();

                    if (maintenanceRoom != null)
                    {
                        var memberCount = ServerNetworkAPI.GetMaintenanceRoomMemberCount();
                        var maxPlayers = ServerNetworkAPI.GetMaintenanceRoomMaxPlayers();

                        guiHelper.MutedLabel($"Members: {memberCount}/{maxPlayers}");
                    }
                }
            );

            DrawSection(
                "Connected Players",
                () =>
                {
                    var connected = ServerNetworkAPI.GetAllConnectedPlayers();
                    guiHelper.Label($"Connected Players: {connected.Count}");
                }
            );

            DrawSection(
                "Server Settings",
                () =>
                {
                    var socket = ServerNetworkAPI.GetServerSocket();
                    if (socket != null)
                    {
                        if (!sliderStates.ContainsKey("maxClients"))
                            sliderStates["maxClients"] = ServerNetworkAPI.GetMaximumClients();
                        sliderStates["maxClients"] = guiHelper.LabeledSlider("Max Clients", sliderStates["maxClients"], 1f, 100f, 1f, true);

                        if (guiHelper.Button("Set Max Clients"))
                        {
                            ServerNetworkAPI.SetMaximumClients(socket, (int)sliderStates["maxClients"]);
                            guiHelper.ShowSuccessToast("Server", $"Max clients set to {(int)sliderStates["maxClients"]}");
                        }
                    }
                    else
                    {
                        guiHelper.MutedLabel("Server socket not available");
                    }
                }
            );
        }
        #endregion

        #region Tests Tab
        void DrawTestsTab()
        {
            DrawSection(
                "Test Controls",
                () =>
                {
                    guiHelper.BeginHorizontalGroup();
                    if (guiHelper.Button("Run All Tests", ControlVariant.Default))
                        RunAllTests();
                    if (guiHelper.Button("Clear Results", ControlVariant.Secondary))
                        testResults.Clear();
                    guiHelper.EndHorizontalGroup();

                    if (testResults.Count > 0)
                    {
                        int passed = testResults.Count(r => r.Passed);
                        int failed = testResults.Count - passed;

                        guiHelper.BeginHorizontalGroup();
                        guiHelper.Badge($"Passed: {passed}", ControlVariant.Default);
                        guiHelper.Badge($"Failed: {failed}", failed > 0 ? ControlVariant.Destructive : ControlVariant.Secondary);
                        guiHelper.EndHorizontalGroup();

                        guiHelper.Progress((float)passed / testResults.Count, 300);
                    }
                }
            );

            DrawSection(
                "Test Results",
                () =>
                {
                    if (testResults.Count > 0)
                    {
                        string[] headers = { "Test", "Status", "Message" };
                        string[,] data = new string[testResults.Count, 3];

                        for (int i = 0; i < testResults.Count; i++)
                        {
                            data[i, 0] = testResults[i].TestName;
                            data[i, 1] = testResults[i].Passed ? "PASS" : "FAIL";
                            data[i, 2] = testResults[i].Message;
                        }

                        guiHelper.Table(headers, data, ControlVariant.Secondary);
                    }
                    else
                    {
                        guiHelper.MutedLabel("No test results. Click 'Run All Tests' to start.");
                    }
                }
            );
        }
        #endregion

        #region Test Methods
        void RunAllTests()
        {
            testResults.Clear();
            TestReflectionHelper();
            TestCoreAPI();
            TestManagerAPI();
            TestPlayerAPI();
            TestRoomAPI();
            TestActorAPI();
            TestLootAPI();
            TestNetworkAPI();

            int passed = testResults.Count(r => r.Passed);
            guiHelper.ShowInfoToast("Tests Complete", $"{passed}/{testResults.Count} tests passed");
        }

        void TestReflectionHelper()
        {
            try
            {
                var testObj = new TestClass { TestField = "Hello", TestProperty = 42 };

                var fieldValue = ReflectionHelper.GetFieldValue(testObj, "TestField");
                AddResult("ReflectionHelper.GetFieldValue", fieldValue?.ToString() == "Hello", $"Got: {fieldValue}");

                var typedValue = ReflectionHelper.GetFieldValue<string>(testObj, "TestField");
                AddResult("ReflectionHelper.GetFieldValue<T>", typedValue == "Hello", $"Got: {typedValue}");

                ReflectionHelper.SetFieldValue(testObj, "TestField", "Modified");
                AddResult("ReflectionHelper.SetFieldValue", testObj.TestField == "Modified", $"Got: {testObj.TestField}");

                var methodResult = ReflectionHelper.InvokeMethod(testObj, "GetTestValue");
                AddResult("ReflectionHelper.InvokeMethod", methodResult?.ToString() == "TestValue", $"Got: {methodResult}");

                var propValue = ReflectionHelper.GetPropertyValue<int>(testObj, "TestProperty");
                AddResult("ReflectionHelper.GetPropertyValue", propValue == 42, $"Got: {propValue}");
            }
            catch (Exception ex)
            {
                AddResult("ReflectionHelper", false, ex.Message);
            }
        }

        void TestCoreAPI()
        {
            try
            {
                AddResult("CoreAPI.GetHub", CoreAPI.GetHub() != null, "Hub check");
            }
            catch (Exception ex)
            {
                AddResult("CoreAPI.GetHub", false, ex.Message);
            }

            try
            {
                AddResult("CoreAPI.GetPersistentData", true, CoreAPI.GetPersistentData() != null ? "Found" : "Null");
            }
            catch (Exception ex)
            {
                AddResult("CoreAPI.GetPersistentData", false, ex.Message);
            }
        }

        void TestManagerAPI()
        {
            var tests = new (string Name, Func<object> Getter)[]
            {
                ("GetDataManager", () => ManagerAPI.GetDataManager()),
                ("GetTimeUtil", () => ManagerAPI.GetTimeUtil()),
                ("GetUIManager", () => ManagerAPI.GetUIManager()),
                ("GetCameraManager", () => ManagerAPI.GetCameraManager()),
                ("GetAudioManager", () => ManagerAPI.GetAudioManager()),
                ("GetInputManager", () => ManagerAPI.GetInputManager()),
                ("GetNetworkManager", () => ManagerAPI.GetNetworkManager()),
            };

            foreach (var (name, getter) in tests)
            {
                try
                {
                    AddResult($"ManagerAPI.{name}", true, getter() != null ? "Found" : "Null");
                }
                catch (Exception ex)
                {
                    AddResult($"ManagerAPI.{name}", false, ex.Message);
                }
            }
        }

        void TestPlayerAPI()
        {
            try
            {
                AddResult("PlayerAPI.HasLocalPlayer", true, $"{PlayerAPI.HasLocalPlayer()}");
            }
            catch (Exception ex)
            {
                AddResult("PlayerAPI.HasLocalPlayer", false, ex.Message);
            }

            try
            {
                AddResult("PlayerAPI.GetLocalPlayer", true, PlayerAPI.GetLocalPlayer() != null ? "Found" : "Null");
            }
            catch (Exception ex)
            {
                AddResult("PlayerAPI.GetLocalPlayer", false, ex.Message);
            }

            try
            {
                AddResult("PlayerAPI.GetAllPlayers", true, $"Count: {PlayerAPI.GetAllPlayers()?.Length ?? 0}");
            }
            catch (Exception ex)
            {
                AddResult("PlayerAPI.GetAllPlayers", false, ex.Message);
            }

            try
            {
                AddResult("PlayerAPI.GetLocalPlayerPosition", true, $"{PlayerAPI.GetLocalPlayerPosition()}");
            }
            catch (Exception ex)
            {
                AddResult("PlayerAPI.GetLocalPlayerPosition", false, ex.Message);
            }
        }

        void TestRoomAPI()
        {
            try
            {
                AddResult("RoomAPI.GetAllRooms", true, $"Count: {RoomAPI.GetAllRooms().Length}");
            }
            catch (Exception ex)
            {
                AddResult("RoomAPI.GetAllRooms", false, ex.Message);
            }

            try
            {
                AddResult("RoomAPI.GetCurrentRoom", true, RoomAPI.GetCurrentRoom() != null ? "Found" : "Null");
            }
            catch (Exception ex)
            {
                AddResult("RoomAPI.GetCurrentRoom", false, ex.Message);
            }

            try
            {
                AddResult("RoomAPI.GetAllPlayableRooms", true, $"Count: {RoomAPI.GetAllPlayableRooms().Count}");
            }
            catch (Exception ex)
            {
                AddResult("RoomAPI.GetAllPlayableRooms", false, ex.Message);
            }
        }

        void TestActorAPI()
        {
            var room = RoomAPI.GetCurrentRoom();

            try
            {
                AddResult("ActorAPI.GetAllVActorsInRoom", true, $"Count: {ActorAPI.GetAllVActorsInRoom(room).Count}");
            }
            catch (Exception ex)
            {
                AddResult("ActorAPI.GetAllVActorsInRoom", false, ex.Message);
            }

            try
            {
                AddResult("ActorAPI.GetMonstersInRoom", true, $"Count: {ActorAPI.GetMonstersInRoom(room).Count}");
            }
            catch (Exception ex)
            {
                AddResult("ActorAPI.GetMonstersInRoom", false, ex.Message);
            }
        }

        void TestLootAPI()
        {
            try
            {
                AddResult("LootAPI.GetAllLoot", true, $"Count: {LootAPI.GetAllLoot().Length}");
            }
            catch (Exception ex)
            {
                AddResult("LootAPI.GetAllLoot", false, ex.Message);
            }

            try
            {
                AddResult("LootAPI.HasLoot", true, $"{LootAPI.HasLoot()}");
            }
            catch (Exception ex)
            {
                AddResult("LootAPI.HasLoot", false, ex.Message);
            }

            try
            {
                AddResult("LootAPI.GetNearestLoot", true, LootAPI.GetNearestLoot() != null ? "Found" : "None");
            }
            catch (Exception ex)
            {
                AddResult("LootAPI.GetNearestLoot", false, ex.Message);
            }
        }

        void TestNetworkAPI()
        {
            try
            {
                AddResult("ServerNetworkAPI.IsServerRunning", true, $"{ServerNetworkAPI.IsServerRunning()}");
            }
            catch (Exception ex)
            {
                AddResult("ServerNetworkAPI.IsServerRunning", false, ex.Message);
            }

            try
            {
                AddResult("ServerNetworkAPI.GetServerSocket", true, ServerNetworkAPI.GetServerSocket() != null ? "Found" : "Null");
            }
            catch (Exception ex)
            {
                AddResult("ServerNetworkAPI.GetServerSocket", false, ex.Message);
            }

            try
            {
                AddResult("ServerNetworkAPI.GetGameAssembly", true, ServerNetworkAPI.GetGameAssembly() != null ? "Found" : "Null");
            }
            catch (Exception ex)
            {
                AddResult("ServerNetworkAPI.GetGameAssembly", false, ex.Message);
            }
        }

        void AddResult(string testName, bool passed, string message)
        {
            testResults.Add(
                new TestResult
                {
                    TestName = testName,
                    Passed = passed,
                    Message = message,
                }
            );
        }
        #endregion

        #region Helper Classes
        class TestResult
        {
            public string TestName { get; set; } = "";
            public bool Passed { get; set; }
            public string Message { get; set; } = "";
        }

        class TestClass
        {
            public string TestField = "";
            public int TestProperty { get; set; }

            public string GetTestValue() => "TestValue";
        }
        #endregion
    }
}
