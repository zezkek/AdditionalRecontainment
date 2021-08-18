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
    class MTFRequest : ICommand
    {
        public string Command { get; } = "mtfchop";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Вызов эвакуационного вертолёта МОГ";
        Vector3 MTFPoint = new Vector3(178, 992.5f, -60);
        bool OnEvacuateCooldown = false;
        bool OnSupportCooldown = false;
        public int[,] RoleTypeArray = new int[,]{{(int)RoleType.Scp173,0 }, {(int)RoleType.Scp049,0 }, {(int)RoleType.Scp096,0 },
            {(int)RoleType.Scientist,0 }, {(int)RoleType.NtfCommander,0 }, {(int)RoleType.NtfScientist,0 },{(int)RoleType.NtfLieutenant,0 },{(int)RoleType.NtfCadet,0 },
            {(int)RoleType.Scientist,1 }, {(int)RoleType.NtfCommander,1 }, {(int)RoleType.NtfScientist,1 },{(int)RoleType.NtfLieutenant,1 },{(int)RoleType.NtfCadet,1 },
            {(int)RoleType.ChaosInsurgency,1 },{(int)RoleType.ClassD,1 },{(int)RoleType.Scp93989,1 }, {(int)RoleType.Scp93953,1 }, {(int)RoleType.Scp0492,1 } };
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
            sbyte ChopperCapacity = 10;
            List<Pickup> ItemsToEvac = Pickup.Instances.Where(x => Vector3.Distance(x.position, MTFPoint) <= Plugin.PluginItem.Config.Distance && Plugin.PluginItem.Config.MTFEvacItems.Contains(x.ItemId)).ToList();
            foreach (var Item in ItemsToEvac)
                Item.Delete();
            for (int role = 0; role < RoleTypeArray.GetLength(0); role++)
            {
                foreach (Player ply in ReadyToEvacuate.Where(x => x.Role == (RoleType)RoleTypeArray[role, 0] && x.IsCuffed == Convert.ToBoolean(RoleTypeArray[role, 1])))
                {
                    if (PlayerWeight[ply.Role] > ChopperCapacity)
                    {
                        if(ply.Team == Team.MTF)
                            ply.ShowHint("\"Говорит Пилот Эпсилон-11. Погрузка окончена. Улетаем\"\n<i>Вам не хватило места</i>");
                        else
                            ply.ShowHint("<i>Вам не хватило места</i>");
                        continue;
                        //huita.
                    }
                    ChopperCapacity -= PlayerWeight[ply.Role];
                    if(ply.Team==Team.SCP)
                        switch (ply.Role)
                        {
                            case RoleType.Scp049:
                                Cassie.Message(Plugin.PluginItem.Config.ScpEvacuate.Replace("{scpnumber}", "scp 0 4 9"));
                                break;
                            case RoleType.Scp096:
                                Cassie.Message(Plugin.PluginItem.Config.ScpEvacuate.Replace("{scpnumber}", "scp 0 9 6"));
                                break;
                            case RoleType.Scp173:
                                Cassie.Message(Plugin.PluginItem.Config.ScpEvacuate.Replace("{scpnumber}", "scp 1 7 3"));
                                break;
                            case RoleType.Scp93953:
                                Cassie.Message(Plugin.PluginItem.Config.ScpEvacuate.Replace("{scpnumber}", "scp 9 3 9"));
                                break;
                            case RoleType.Scp93989:
                                Cassie.Message(Plugin.PluginItem.Config.ScpEvacuate.Replace("{scpnumber}", "scp 9 3 9"));
                                break;
                        }
                    if (ply.Team == Team.MTF)
                        ply.ShowHint("\"Говорит Пилот Эпсилон-11. Погрузка окончена. Улетаем\"\n<i>Вы успешно эвакуировались из Комплекса</i>");
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
            Respawn.PlayEffect(RespawnEffectType.SummonNtfChopper);
            yield return Timing.WaitForSeconds(18f);
            List<Player> ReadyToEvac = Player.List.Where(x => Vector3.Distance(x.Position, MTFPoint) <= Plugin.PluginItem.Config.Distance).ToList();
            if (Warhead.IsInProgress)
                Evacuate(ReadyToEvac, PlayerWeight);
            else
                Evacuate(ReadyToEvac.Where(x => x.Team != Team.MTF || (x.Team == Team.MTF && x.IsCuffed)).ToList(), PlayerWeight);
        }
        private IEnumerator<float> WaitForSupport()
        {
            Timing.RunCoroutine(CooldownSupport());
            yield return Timing.WaitForSeconds(Plugin.PluginItem.Config.WaitForSupport);
            Respawn.PlayEffect(RespawnEffectType.SummonNtfChopper);
            yield return Timing.WaitForSeconds(18f);
            Respawn.ForceWave(Respawning.SpawnableTeamType.NineTailedFox, false);
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
                response = "Аргументы: \n .mtfchop evac - эвакуация объектов, людей и предметов \n.mtfchop support - вызов подкрепления";
                return false;
            }
            if (!player_requester.IsHuman)
            {
                player_requester.ShowHint("<i>Вы не человек</i>", 5);
                response = "Вы не человек";
                return true;
            }
            if (!player_requester.IsNTF)
            {
                player_requester.ShowHint("<i>Вы не член отряда МОГ</i>", 5);
                response = "Вы не член отряда МОГ";
                return true;
            }
            if ((int)player_requester.CurrentItem.id != 12)
            {
                player_requester.ShowHint("<i>Нужно достать рацию</i>", 5);
                response = "Нужно достать рацию";
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
                        response = "Перезарядка";
                        return true;
                    }
                    if (Warhead.IsInProgress && Warhead.DetonationTimer < 36)
                    {
                        //TODO: Передать группу вызывающего
                        player_requester.ShowHint("\"Говорит Пилот Эпсилон-11. Вылет невозможен, буду в зоне поражения Альфа-боеголовки\"", 5);
                        response = "Говорит Пилот Эпсилон-11. Вылет невозможен, буду в зоне поражения Альфа-боеголовки";
                        return true;
                    }
                    player_requester.ShowHint("\"Говорит Пилот Эпсилон-11. Буду на месте через 30 секунд\"", 5);
                    Timing.RunCoroutine(WaitingRoom());
                    response = "\"Говорит Пилот Эпсилон-11. Буду на месте через 30 секунд\"";
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
                    response = "\"Говорит Пилот Эпсилон-11. Буду на месте через {time} секунд\"".Replace("{time}",Plugin.PluginItem.Config.WaitForSupport.ToString());
                    return true;
                }
            }
            response = "Аргументы: \n .mtfchop evac - эвакуация объектов, людей и предметов \n.mtfchop support - вызов подкрепления";
            return false;
        }
    }
}
