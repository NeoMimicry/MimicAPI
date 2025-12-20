using System;
using System.Collections.Generic;
using System.Text;
using Mimic.Audio;
using Mimic.InputSystem;

namespace MimicAPI.GameAPI
{
    public static class ManagerAPI
    {
        public static T? GetManager<T>(string fieldName)
            where T : class
        {
            Hub? hub = CoreAPI.GetHub();
            return hub != null ? ReflectionHelper.GetFieldValue<T>(hub, fieldName) : null;
        }

        public static DataManager? GetDataManager()
        {
            return GetManager<DataManager>("dataman");
        }

        public static TimeUtil? GetTimeUtil()
        {
            return GetManager<TimeUtil>("timeutil");
        }

        public static NavManager? GetNavManager()
        {
            return GetManager<NavManager>("navman");
        }

        public static DynamicDataManager? GetDynamicDataManager()
        {
            return GetManager<DynamicDataManager>("dynamicDataMan");
        }

        public static UIManager? GetUIManager()
        {
            return GetManager<UIManager>("uiman");
        }

        public static CameraManager? GetCameraManager()
        {
            return GetManager<CameraManager>("cameraman");
        }

        public static AudioManager? GetAudioManager()
        {
            return GetManager<AudioManager>("audioman");
        }

        public static InputManager? GetInputManager()
        {
            return GetManager<InputManager>("inputman");
        }

        public static NetworkManagerV2? GetNetworkManager()
        {
            return GetManager<NetworkManagerV2>("netman2");
        }

        public static APIRequestHandler? GetAPIHandler()
        {
            return GetManager<APIRequestHandler>("apihandler");
        }

        public static L10NManager? GetLocalisationManager()
        {
            return GetManager<L10NManager>("lcman");
        }

        public static bool IsManagerAvailable<T>(string fieldName)
            where T : class
        {
            return GetManager<T>(fieldName) != null;
        }
    }
}
