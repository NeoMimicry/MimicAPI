using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MimicAPI.GameAPI
{
    public static class WeatherAPI
    {
        public static SkyAndWeatherSystem.eWeatherPreset GetCurrentWeatherPreset(IVroom? room)
        {
            if (room == null)
                return SkyAndWeatherSystem.eWeatherPreset.Sunny;

            try
            {
                var dungeonWeather = ReflectionHelper.GetFieldValue<DungeonWeather>(room, "_dungeonWeather");
                if (dungeonWeather == null)
                    return SkyAndWeatherSystem.eWeatherPreset.Sunny;

                TimeUtil? timeUtil = ManagerAPI.GetTimeUtil();
                if (timeUtil == null)
                    return SkyAndWeatherSystem.eWeatherPreset.Sunny;

                int currentHour = GetCurrentHour(timeUtil);
                return dungeonWeather.GetWeatherPreset(currentHour);
            }
            catch
            {
                return SkyAndWeatherSystem.eWeatherPreset.Sunny;
            }
        }

        public static int GetCurrentWeatherMasterID(IVroom? room)
        {
            if (room == null)
                return 0;

            try
            {
                var dungeonWeather = ReflectionHelper.GetFieldValue<DungeonWeather>(room, "_dungeonWeather");
                if (dungeonWeather == null)
                    return 0;

                TimeUtil? timeUtil = ManagerAPI.GetTimeUtil();
                if (timeUtil == null)
                    return 0;

                int currentHour = GetCurrentHour(timeUtil);
                return dungeonWeather.GetWeatherMasterID(currentHour);
            }
            catch
            {
                return 0;
            }
        }

        public static int GetWeatherForecastMasterID(IVroom? room)
        {
            if (room == null)
                return 0;

            try
            {
                var dungeonWeather = ReflectionHelper.GetFieldValue<DungeonWeather>(room, "_dungeonWeather");
                if (dungeonWeather == null)
                    return 0;

                TimeUtil? timeUtil = ManagerAPI.GetTimeUtil();
                if (timeUtil == null)
                    return 0;

                int currentHour = GetCurrentHour(timeUtil);
                return dungeonWeather.GetWeatherForecastMasterID(currentHour);
            }
            catch
            {
                return 0;
            }
        }

        public static bool UpdateWeather(IVroom? room, int newWeatherID, int startHour, int durationHour, bool forecast)
        {
            if (room == null)
                return false;

            try
            {
                var dungeonWeather = ReflectionHelper.GetFieldValue<DungeonWeather>(room, "_dungeonWeather");
                if (dungeonWeather == null)
                    return false;

                dungeonWeather.SetWeatherOverride(newWeatherID, startHour, durationHour, forecast);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdateWeatherAll(IVroom? room, int newWeatherID)
        {
            if (room == null)
                return false;

            try
            {
                var dungeonWeather = ReflectionHelper.GetFieldValue<DungeonWeather>(room, "_dungeonWeather");
                if (dungeonWeather == null)
                    return false;

                dungeonWeather.SetWeatherOverrideAll(newWeatherID);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static float GetCurrentContaminationRate(IVroom? room)
        {
            if (room == null)
                return 0f;

            try
            {
                var dungeonWeather = ReflectionHelper.GetFieldValue<DungeonWeather>(room, "_dungeonWeather");
                if (dungeonWeather == null)
                    return 0f;

                TimeUtil? timeUtil = ManagerAPI.GetTimeUtil();
                if (timeUtil == null)
                    return 0f;

                int currentHour = GetCurrentHour(timeUtil);
                return dungeonWeather.GetCurrentContaRate(currentHour);
            }
            catch
            {
                return 0f;
            }
        }

        public static bool IsRandomWeatherOccurred(IVroom? room)
        {
            if (room == null)
                return false;

            try
            {
                var dungeonWeather = ReflectionHelper.GetFieldValue<DungeonWeather>(room, "_dungeonWeather");
                if (dungeonWeather == null)
                    return false;

                return dungeonWeather.IsRandomOccured;
            }
            catch
            {
                return false;
            }
        }

        public static List<int> GetAllWeatherByHour(IVroom? room)
        {
            if (room == null)
                return new List<int>();

            try
            {
                var dungeonWeather = ReflectionHelper.GetFieldValue<DungeonWeather>(room, "_dungeonWeather");
                if (dungeonWeather == null)
                    return new List<int>();

                return dungeonWeather.GetAllWeather();
            }
            catch
            {
                return new List<int>();
            }
        }

        private static int GetCurrentHour(TimeUtil timeUtil)
        {
            object? currentTimeObj = ReflectionHelper.GetFieldValue(timeUtil, "_currentTime");
            if (currentTimeObj is long currentTime)
            {
                int hours = (int)((currentTime / 1000) / 3600) % 24;
                return hours;
            }
            return 0;
        }
    }
}
