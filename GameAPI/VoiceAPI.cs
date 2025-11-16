using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dissonance;
using Dissonance.Audio.Playback;
using Dissonance.Integrations.FishNet;
using UnityEngine;

namespace MimicAPI.GameAPI
{
    public static class VoiceAPI
    {
        private static Dictionary<string, float> _voicePitchCache = new Dictionary<string, float>();

        public static bool AddVoicePitch(string playerId, float pitchValue)
        {
            if (string.IsNullOrEmpty(playerId))
                return false;

            var voiceManager = ManagerAPI.GetUIManager();
            if (voiceManager == null)
                return false;

            try
            {
                var dissonanceComms = DissonanceFishNetComms.Instance?.Comms;
                if (dissonanceComms == null)
                    return false;

                var voicePlayerState = dissonanceComms.FindPlayer(playerId);
                if (voicePlayerState == null)
                    return false;

                var customPlayback = voicePlayerState.Playback as CustomVoicePlayback;
                if (customPlayback?.AudioSource == null)
                    return false;

                if (!_voicePitchCache.ContainsKey(playerId))
                {
                    _voicePitchCache[playerId] = customPlayback.AudioSource.pitch;
                }

                customPlayback.AudioSource.pitch += pitchValue;
                return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool RemoveVoicePitch(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
                return false;

            try
            {
                var dissonanceComms = DissonanceFishNetComms.Instance?.Comms;
                if (dissonanceComms == null)
                    return false;

                var voicePlayerState = dissonanceComms.FindPlayer(playerId);
                if (voicePlayerState == null)
                    return false;

                var customPlayback = voicePlayerState.Playback as CustomVoicePlayback;
                if (customPlayback?.AudioSource == null || !_voicePitchCache.ContainsKey(playerId))
                    return false;

                customPlayback.AudioSource.pitch = _voicePitchCache[playerId];
                _voicePitchCache.Remove(playerId);
                return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static float GetVoicePitchBefore(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
                return 1f;

            if (_voicePitchCache.TryGetValue(playerId, out var originalPitch))
                return originalPitch;

            return 1f;
        }

        public static float GetVoicePitchAfter(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
                return 1f;

            try
            {
                var dissonanceComms = DissonanceFishNetComms.Instance?.Comms;
                if (dissonanceComms == null)
                    return 1f;

                var voicePlayerState = dissonanceComms.FindPlayer(playerId);
                if (voicePlayerState == null)
                    return 1f;

                var audioSource = ReflectionHelper.GetFieldValue<AudioSource>(voicePlayerState.Playback, "AudioSource");
                return audioSource?.pitch ?? 1f;
            }
            catch
            {
                return 1f;
            }
        }

        public static bool SetVoicePitch(string playerId, float pitchValue)
        {
            if (string.IsNullOrEmpty(playerId))
                return false;

            try
            {
                var dissonanceComms = DissonanceFishNetComms.Instance?.Comms;
                if (dissonanceComms == null)
                    return false;

                var voicePlayerState = dissonanceComms.FindPlayer(playerId);
                if (voicePlayerState == null)
                    return false;

                var customPlayback = voicePlayerState.Playback as CustomVoicePlayback;
                if (customPlayback?.AudioSource == null)
                    return false;

                if (!_voicePitchCache.ContainsKey(playerId))
                {
                    _voicePitchCache[playerId] = customPlayback.AudioSource.pitch;
                }

                customPlayback.AudioSource.pitch = Mathf.Clamp(pitchValue, 0.1f, 3f);
                return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> GetAllVoicePlayers()
        {
            var players = new List<string>();

            try
            {
                var dissonanceComms = DissonanceFishNetComms.Instance?.Comms;
                if (dissonanceComms == null)
                    return players;

                var allPlayers = ReflectionHelper.GetFieldValue<Dictionary<string, VoicePlayerState>>(dissonanceComms, "_players");
                if (allPlayers != null)
                {
                    players.AddRange(allPlayers.Keys);
                }
            }
            catch { }

            return players;
        }

        public static bool IsVoicePitchModified(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
                return false;

            var originalPitch = GetVoicePitchBefore(playerId);
            var currentPitch = GetVoicePitchAfter(playerId);

            return !Mathf.Approximately(originalPitch, currentPitch);
        }

        public static void ClearVoicePitchCache()
        {
            _voicePitchCache.Clear();
        }
    }
}
