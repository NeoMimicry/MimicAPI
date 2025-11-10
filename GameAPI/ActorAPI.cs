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

        public static List<VPlayer> FindAlivePlayersInRoom(IVroom? room)
        {
            var players = GetAllVPlayersInRoom(room);
            return players.Where(p => p != null && p.IsAliveStatus()).ToList();
        }

        public static List<VPlayer> FindPlayersInRoomByName(IVroom? room, string name)
        {
            var players = GetAllVPlayersInRoom(room);
            return players.Where(p => p != null).ToList();
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

        public static List<VActor> FindMonstersInRoom(IVroom? room)
        {
            var actors = GetAllVActorsInRoom(room);
            return actors.Where(a => a != null && a is VMonster).Cast<VActor>().ToList();
        }

        public static List<VActor> FindAliveMonstersInRoom(IVroom? room)
        {
            var monsters = FindMonstersInRoom(room);
            return monsters.Where(m => m != null && m.IsAliveStatus()).ToList();
        }

        public static List<VActor> FindLootingObjectsInRoom(IVroom? room)
        {
            var actors = GetAllVActorsInRoom(room);
            return actors.Where(a => a != null && a is VLootingObject).Cast<VActor>().ToList();
        }
    }
}
