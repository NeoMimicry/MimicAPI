using System;
using System.Collections.Generic;
using System.Text;
using Mimic;
using Mimic.InputSystem;
using ReluProtocol;
using UnityEngine;

namespace MimicAPI.GameAPI
{
    public static class CoreAPI
    {
        public static Hub GetHub()
        {
            return Hub.s;
        }

        public static VWorld? GetVWorld()
        {
            Hub? hub = GetHub();
            return hub == null ? null : ReflectionHelper.GetFieldValue<VWorld>(hub, "vworld");
        }

        public static VRoomManager? GetVRoomManager()
        {
            VWorld? vworld = GetVWorld();
            return vworld == null ? null : ReflectionHelper.GetFieldValue<VRoomManager>(vworld, "VRoomManager");
        }

        public static Hub.PersistentData? GetPersistentData()
        {
            Hub? hub = GetHub();
            return hub == null ? null : ReflectionHelper.GetFieldValue<Hub.PersistentData>(hub, "pdata");
        }
    }
}