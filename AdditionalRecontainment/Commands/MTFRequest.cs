using CommandSystem;
using RemoteAdmin;
using System;
using Exiled.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.Patches.Events.Cassie;
using UnityEngine;
using MEC;
using System.Threading;

namespace AdditionalRecontainment.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class MTFRequest : ICommand
    {
        public string Command { get; } = "request";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "request mtf helicopter";
        private List<Player> AliveMTF = new List<Player>();
        private List<Player> AliveSCP = new List<Player>();
        private Player ScpTarget;
        sbyte count=2;
        Vector3 mtfposition = new Vector3(163, 990, -52);
        Vector3 mtfposition2 = new Vector3(193,995,-68);

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player_requester = Player.Get((sender as CommandSender)?.SenderId);
            if (!player_requester.IsHuman)
            {
                player_requester.Broadcast(5, "Вы не человек");
                response = "Вы не человек";
                return false;
            }
            if (!player_requester.IsNTF)
            {
                player_requester.Broadcast(5, "Вы не член отряда мтф");
                response = "Вы не член отряда мтф";
                return true;
            }
            if (!(player_requester.Position.x > mtfposition.x && player_requester.Position.x < mtfposition2.x
                && player_requester.Position.y > mtfposition.y && player_requester.Position.y < mtfposition2.y
                && player_requester.Position.z < mtfposition.z && player_requester.Position.z > mtfposition2.z))
            {
                player_requester.Broadcast(5, "Вы не на вертолётной площадке");
                response = "failed";
                return true;
            }
            AliveMTF = Player.Get(Team.MTF).ToList();
            AliveSCP = Player.Get(Team.SCP).ToList();
            foreach (Player scp in AliveSCP.Where(x => x.Role == RoleType.Scp0492 || x.Role == RoleType.Scp079))
                AliveSCP.Remove(scp);
            foreach (Player scp in AliveSCP.Where(x => Vector3.Distance(player_requester.Position, x.Position) <= Plugin.PluginItem.Config.Distance))
            {
                ScpTarget = scp;
                break;
            }
            if (ScpTarget == null)
            {
                player_requester.Broadcast(5, "С вами нет объекта");
                response = "С вами нет объекта";
                return true;
            }
            Log.Info("Ждем");
            for (int i = 0; i <= 2; i++)
            {
                Thread.Sleep(10000);
                foreach (Player ply in AliveMTF.Where(x => Vector3.Distance(ScpTarget.Position, x.Position) > Plugin.PluginItem.Config.Distance))
                    count++;
                if (count < Plugin.PluginItem.Config.PersonCount)
                {
                    foreach (Player ply in AliveMTF.Where(x => Vector3.Distance(ScpTarget.Position, x.Position) < Plugin.PluginItem.Config.Distance))
                        ply.Broadcast(5, "Слишком мало людей для эвакуации объекта");
                    ScpTarget = null;
                    player_requester = null;
                    response = "Слишком мало людей для эвакуации объекта";
                    return true;
                }
                else
                {
                    foreach (Player ply in AliveMTF.Where(x => Vector3.Distance(ScpTarget.Position, x.Position) < Plugin.PluginItem.Config.Distance))
                        ply.Broadcast(5, "Пока все успешно");
                }
            }
            Respawn.PlayEffect(RespawnEffectType.SummonNtfChopper);
            Log.Info("Ждем вертолёт");
            Thread.Sleep(18000);
            ScpTarget.Role = RoleType.Spectator;
            Cassie.GlitchyMessage("scp containment successfully g a y", 10, 10);
            response = "Объект успешно эвакуирован";
            ScpTarget = null;
            player_requester = null;
            return true;
        }
    }
}
