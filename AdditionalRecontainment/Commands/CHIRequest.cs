using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdditionalRecontainment.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class CHIRequest:ICommand
    {
        public string Command { get; } = "chicar";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Вызов грузовика CHI";
        Vector3 CHIPoint = new Vector3(5, 988.5f, -58);
        bool OnEvacuateCooldown = false;
        bool OnSupportCooldown = false;
        public int[,] RoleTypeArray = new int[,]{{(int)RoleType.Scp173,0 }, {(int)RoleType.Scp049,0 },
            {(int)RoleType.Scp096,0 },{(int)RoleType.ChaosInsurgency,0 },
            {(int)RoleType.Scientist,1 }, {(int)RoleType.NtfCommander,1 },
            {(int)RoleType.NtfScientist,1 },{(int)RoleType.NtfLieutenant,1 },
            {(int)RoleType.NtfCadet,1 },{(int)RoleType.ClassD,0 },
            {(int)RoleType.Scp93989,0 }, {(int)RoleType.Scp93953,0 },
            {(int)RoleType.Scp0492,0 } };
        public Dictionary<RoleType, sbyte> PlayerWeight = new Dictionary<RoleType, sbyte>
        {
            {RoleType.Scp173, 3 },
            {RoleType.Scp049, 10 },
            {RoleType.Scp0492, 1 },
            {RoleType.Scp096, 3},
            {RoleType.Scp93953, 3 },
            {RoleType.Scp93989, 3 },
            {RoleType.Scientist, 1 },
            {RoleType.NtfCommander, 1 },
            {RoleType.NtfScientist, 1 },
            {RoleType.NtfLieutenant, 1 },
            {RoleType.NtfCadet, 1 },
            {RoleType.ChaosInsurgency, 1 },
            {RoleType.ClassD, 1 }
        };

        public void Evacuate(List<Player> ReadyToEvacuate, Dictionary<RoleType, sbyte> PlayerWeight)
        {
            sbyte CarCapacity = 10;
            List<Pickup> ItemsToEvac = Pickup.Instances.Where(x => Vector3.Distance(x.position, CHIPoint) <= Plugin.PluginItem.Config.Distance && Plugin.PluginItem.Config.CHIEvacItems.Contains(x.ItemId)).ToList();
            foreach (var Item in ItemsToEvac)
                Item.Delete();
            for (int role = 0; role < RoleTypeArray.GetLength(0); role++)
            {
                foreach (Player ply in ReadyToEvacuate.Where(x => x.Role == (RoleType)RoleTypeArray[role, 0] && x.IsCuffed == Convert.ToBoolean(RoleTypeArray[role, 1])))
                {
                    if (PlayerWeight[ply.Role] > CarCapacity)
                    {
                        if (ply.Team == Team.CHI)
                            ply.ShowHint("\"Больше не влезет, валим отсюда\"\n<i>Вам не хватило места</i>");
                        else
                            ply.ShowHint("<i>Вам не хватило места</i>");
                        continue;
                    }
                    CarCapacity -= PlayerWeight[ply.Role];
                    if (ply.Team == Team.SCP)
                        switch (ply.Role)
                        {
                            case RoleType.Scp049:
                                Cassie.Message(Plugin.PluginItem.Config.ScpSteal.Replace("{scpnumber}", "scp 0 4 9"));
                                break;
                            case RoleType.Scp096:
                                Cassie.Message(Plugin.PluginItem.Config.ScpSteal.Replace("{scpnumber}", "scp 0 9 6"));
                                break;
                            case RoleType.Scp173:
                                Cassie.Message(Plugin.PluginItem.Config.ScpSteal.Replace("{scpnumber}", "scp 1 7 3"));
                                break;
                            case RoleType.Scp93953:
                                Cassie.Message(Plugin.PluginItem.Config.ScpSteal.Replace("{scpnumber}", "scp 9 3 9"));
                                break;
                            case RoleType.Scp93989:
                                Cassie.Message(Plugin.PluginItem.Config.ScpSteal.Replace("{scpnumber}", "scp 9 3 9"));
                                break;
                        }
                    if (ply.Team == Team.CHI)
                        ply.ShowHint("\"Больше не влезет, валим отсюда\"\n<i>Вы успешно эвакуировались из Комплекса</i>");
                    else
                        ply.ShowHint("<i>Вы успешно эвакуировались из Комплекса</i>");
                    ply.Inventory.Clear();
                    ply.SetRole(RoleType.Spectator);
                }
            }
        }
        private IEnumerator<float> WaitingRoom()
        {
            Timing.RunCoroutine(CooldownEvacuate());
            yield return Timing.WaitForSeconds(12f);
            Respawn.PlayEffect(RespawnEffectType.SummonChaosInsurgencyVan);
            yield return Timing.WaitForSeconds(13f);
            List<Player> ReadyToEvac = Player.List.Where(x => Vector3.Distance(x.Position, CHIPoint) <= Plugin.PluginItem.Config.Distance).ToList();
            if (Warhead.IsInProgress)
                Evacuate(ReadyToEvac, PlayerWeight);
            else
                Evacuate(ReadyToEvac.Where(x => x.Team != Team.CHI || (x.Team == Team.CHI && x.IsCuffed)).ToList(), PlayerWeight);
        }
        private IEnumerator<float> WaitForSupport()
        {
            Timing.RunCoroutine(CooldownSupport());
            yield return Timing.WaitForSeconds(Plugin.PluginItem.Config.WaitForSupport);
            Respawn.PlayEffect(RespawnEffectType.SummonChaosInsurgencyVan);
            yield return Timing.WaitForSeconds(13f);
            Respawn.ForceWave(Respawning.SpawnableTeamType.ChaosInsurgency, false);
        }
        private IEnumerator<float> CooldownEvacuate()
        {
            int Cooldown = Plugin.PluginItem.Config.CooldownEv;
            while (Cooldown > 0)
            {
                Cooldown--;
                OnEvacuateCooldown = true;
                yield return Timing.WaitForSeconds(1f);
            }
            OnEvacuateCooldown = false;
        }
        private IEnumerator<float> CooldownSupport()
        {
            int Cooldown = Plugin.PluginItem.Config.CooldownSup;
            while (Cooldown > 0)
            {
                Cooldown--;
                OnSupportCooldown = true;
                yield return Timing.WaitForSeconds(1f);
            }
            OnSupportCooldown = false;
        }
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player_requester = Player.Get((sender as CommandSender)?.SenderId);
            if (arguments.Count != 1)
            {
                response = "Аргументы: \n .chicar evac - эвакуация объектов, людей и предметов \n.chicar support - вызов подкрепления";
                return false;
            }
            if (!player_requester.IsHuman)
            {
                player_requester.ShowHint("<i>Вы не человек</i>", 5);
                response = "Вы не человек";
                return true;
            }
            if (player_requester.Team != Team.CHI)
            {
                player_requester.ShowHint("<i>Вы не агент Хаос инсерженси</i>", 5);
                response = "Вы не агент Хаос инсерженси";
                return true;
            }
            if (player_requester.Position.y <= 900)
            {
                player_requester.ShowHint("[Нет сигнала]", 5);
                response = "Нет сигнала";
                return true;
            }
            var args = arguments.Array;
            {
                if (args[1].ToLower().Equals("evac"))
                {
                    if (OnEvacuateCooldown)
                    {
                        player_requester.ShowHint("<i>Перезарядка</i>", 5);
                        response = "Перезарядка";
                        return true;
                    }
                    if (Warhead.IsInProgress && Warhead.DetonationTimer < 36)
                    {
                        //TODO: Передать группу вызывающего
                        player_requester.ShowHint("\"Я буду в зоне поражения Альфа-боеголовки, эвакуация невозможна\"", 5);
                        response = "Я буду в зоне поражения Альфа-боеголовки, эвакуация невозможна";
                        return true;
                    }
                    player_requester.ShowHint("\"Прибытие к точке эвакуации через 30 секунд, держитесь\"", 5);
                    Timing.RunCoroutine(WaitingRoom());
                    response = "\"Прибытие к точке эвакуации через 30 секунд, держитесь\"";
                    return true;
                }
                if (args[1].ToLower().Equals("support"))
                {
                    if (OnSupportCooldown)
                    {
                        response = "Перезарядка";
                        return true;
                    }
                    Timing.RunCoroutine(WaitForSupport());
                    response = "\"Прибуду к точке через {time} секунд, держитесь\"".Replace("{time}", Plugin.PluginItem.Config.WaitForSupport.ToString());
                    return true;
                }
            }
            response = "\nАргументы: \n.chicar evac - эвакуация объектов, людей и предметов \n.chicar support - вызов подкрепления";
            return false;
        }
    }
}
