using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MimicAPI.GameAPI
{
    public static class ActorAPI
    {
        public static VPlayer? GetVPlayerInRoom(IVroom? room, int actorID)
        {
            if (room == null)
                return null;
            var dict = ReflectionHelper.GetFieldValue<Dictionary<int, VPlayer>>(room, "_vPlayerDict");
            return dict?.Values.FirstOrDefault(p => p.ObjectID == actorID);
        }

        public static List<VPlayer> GetAllVPlayersInRoom(IVroom? room)
        {
            if (room == null)
                return new List<VPlayer>();
            var dict = ReflectionHelper.GetFieldValue<Dictionary<int, VPlayer>>(room, "_vPlayerDict");
            return dict?.Values.ToList() ?? new List<VPlayer>();
        }

        public static List<VPlayer> GetAlivePlayersInRoom(IVroom? room)
        {
            var players = GetAllVPlayersInRoom(room);
            return players.Where(p => p != null && p.IsAliveStatus()).ToList();
        }

        public static List<VPlayer> GetDeadPlayersInRoom(IVroom? room)
        {
            var players = GetAllVPlayersInRoom(room);
            return players.Where(p => p != null && !p.IsAliveStatus()).ToList();
        }

        public static List<VActor> GetAllVActorsInRoom(IVroom? room)
        {
            if (room == null)
                return new List<VActor>();
            var dict = ReflectionHelper.GetFieldValue<Dictionary<int, VActor>>(room, "_vActorDict");
            return dict?.Values.ToList() ?? new List<VActor>();
        }

        public static VActor? GetVActorInRoom(IVroom? room, int actorID)
        {
            if (room == null)
                return null;
            var dict = ReflectionHelper.GetFieldValue<Dictionary<int, VActor>>(room, "_vActorDict");
            return dict != null && dict.ContainsKey(actorID) ? dict[actorID] : null;
        }

        public static List<VActor> GetMonstersInRoom(IVroom? room)
        {
            var actors = GetAllVActorsInRoom(room);
            return actors.Where(a => a != null && a is VMonster).Cast<VActor>().ToList();
        }

        public static List<VActor> GetAliveMonstersInRoom(IVroom? room)
        {
            var monsters = GetMonstersInRoom(room);
            return monsters.Where(m => m != null && m.IsAliveStatus()).ToList();
        }

        public static List<VActor> GetDeadMonstersInRoom(IVroom? room)
        {
            var monsters = GetMonstersInRoom(room);
            return monsters.Where(m => m != null && !m.IsAliveStatus()).ToList();
        }

        public static List<VActor> GetLootingObjectsInRoom(IVroom? room)
        {
            var actors = GetAllVActorsInRoom(room);
            return actors.Where(a => a != null && a is VLootingObject).Cast<VActor>().ToList();
        }

        public static List<VActor> GetActorsByType<T>(IVroom? room) where T : VActor
        {
            var actors = GetAllVActorsInRoom(room);
            return actors.Where(a => a != null && a is T).Cast<VActor>().ToList();
        }

        public static bool HasAliveMonstersInRoom(IVroom? room)
        {
            return GetAliveMonstersInRoom(room).Any();
        }

        public static bool HasAlivePlayersInRoom(IVroom? room)
        {
            return GetAlivePlayersInRoom(room).Any();
        }
    }
}