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

        public static DataManager? GetDataManager() => GetManager<DataManager>("<dataman>k__BackingField");

        public static TimeUtil? GetTimeUtil() => GetManager<TimeUtil>("<timeutil>k__BackingField");

        public static NavManager? GetNavManager() => GetManager<NavManager>("<navman>k__BackingField");

        public static DynamicDataManager? GetDynamicDataManager() => GetManager<DynamicDataManager>("<dynamicDataMan>k__BackingField");

        public static UIManager? GetUIManager() => GetManager<UIManager>("<uiman>k__BackingField");

        public static CameraManager? GetCameraManager() => GetManager<CameraManager>("<cameraman>k__BackingField");

        public static AudioManager? GetAudioManager() => GetManager<AudioManager>("<audioman>k__BackingField");

        public static InputManager? GetInputManager() => GetManager<InputManager>("<inputman>k__BackingField");

        public static NetworkManagerV2? GetNetworkManager() => GetManager<NetworkManagerV2>("<netman2>k__BackingField");

        public static APIRequestHandler? GetAPIHandler() => GetManager<APIRequestHandler>("<apihandler>k__BackingField");

        public static L10NManager? GetLocalisationManager() => GetManager<L10NManager>("lcman");

        public static bool IsManagerAvailable<T>(string fieldName)
            where T : class
        {
            return GetManager<T>(fieldName) != null;
        }
    }
}
