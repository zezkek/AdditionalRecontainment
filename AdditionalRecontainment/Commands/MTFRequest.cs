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
        public string Command { get; } = "mtfchop";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Вызов эвакуационного вертолёта МОГ";
        private List<Player> AlivePlayer = new List<Player>();
        sbyte ChopperCapacity = 10;
        Vector3 mtfposition = new Vector3(163, 990, -52);
        Vector3 mtfposition2 = new Vector3(193,995,-68);
        Vector3 midpos = new Vector3(178, 992.5f, -60);
        TimeSpan time = Round.ElapsedTime;

        public void Evacuate(List<Player> ReadyToEvacuate, Dictionary<RoleType, sbyte> PlayerWeight, ref sbyte ChopperCapacity)
        {
            Log.Info("Начало первого цикла");
            for(int role = 0; role < Plugin.PluginItem.Config.RoleTypeArray.GetLength(0); role++)
            {
                Log.Info("Итерация " + role+1 + " цикла номер один");
                foreach (Player ply in ReadyToEvacuate.Where(x => x.Role == (RoleType)Plugin.PluginItem.Config.RoleTypeArray[role,0] && x.IsCuffed == Convert.ToBoolean(Plugin.PluginItem.Config.RoleTypeArray[role,1])))
                {
                    Log.Info(ply.DisplayNickname);
                    if (Plugin.PluginItem.Config.PlayerWeight[ply.Role] > ChopperCapacity) break;
                    Log.Info("Проверка вертолёта успешно");
                    ChopperCapacity -= Plugin.PluginItem.Config.PlayerWeight[ply.Role];
                    Log.Info("Удаление игрока из списка эвакуации");
                    ReadyToEvacuate.Remove(ply);
                    Log.Info("Изменение на спектатора");
                    ply.SetRole(RoleType.Spectator);
                }
                if (ChopperCapacity == 0)
                {
                    Log.Info("Капасити 0");
                    break; }
            }
        }
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player_requester = Player.Get((sender as CommandSender)?.SenderId);
            if (!player_requester.IsHuman)
            {
                player_requester.ShowHint("<i>Вы не человек</i>",5 );
                response = "Вы не человек";
                return true;
            }
            if (!player_requester.IsNTF)
            {
                player_requester.ShowHint("<i>Вы не член отряда МОГ</i>",5);
                response = "Вы не член отряда МОГ";
                return true;
            }
            //if (player_requester.Zone == ZoneType.Surface)
            //{
            //    player_requester.ShowHint("[Нет сигнала]", 5);
            //    response = "Нет сигнала";
            //    return true;
            //}
            if (Warhead.IsInProgress && Warhead.DetonationTimer < 36)
            {
                //TODO: Передать группу вызывающего
                player_requester.ShowHint("\"Говорит Пилот Эпсилон-11. Вылет невозможен, буду в зоне поражения Альфа-боеголовки\"", 5);
                response = "В эвакуации отказано";
                return true;
            }
            player_requester.ShowHint("\"Говорит Пилот Эпсилон-11. Буду на месте через 30 секунд\"", 5);
            Thread.Sleep(12000);
            Respawn.PlayEffect(RespawnEffectType.SummonNtfChopper);
            Thread.Sleep(18000);
            List<Player> ReadyToEvac = AlivePlayer.Where(x => Vector3.Distance(x.Position, midpos) <= Plugin.PluginItem.Config.Distance).ToList();
            if (Warhead.IsInProgress)
                Evacuate(ReadyToEvac, Plugin.PluginItem.Config.PlayerWeight, ref ChopperCapacity);
            else
                Evacuate(ReadyToEvac.Where(x => x.Team != Team.MTF || (x.Team == Team.MTF && x.IsCuffed)).ToList(), Plugin.PluginItem.Config.PlayerWeight,ref ChopperCapacity);

            response = "Эвакуация прошла успешно";
            return true;
        }
    }
}
