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
using UnityEngine;

namespace AdditionalRecontainment.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class MTFRequest : ICommand
    {
        public string Command { get; } = "request";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "request mtf helicopter";
        Vector3 mtfposition = new Vector3(163, 990, -52);
        Vector3 mtfposition2 = new Vector3(193,995,-68);
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player_requester = Player.Get((sender as CommandSender)?.SenderId);
            if (player_requester.IsHuman)
            {
                if (player_requester.IsNTF)
                {
                    if (player_requester.Position.x > mtfposition.x && player_requester.Position.x < mtfposition2.x
                        && player_requester.Position.y > mtfposition.y && player_requester.Position.y < mtfposition2.y
                        && player_requester.Position.z < mtfposition.z && player_requester.Position.z > mtfposition2.z)
                    {
                        
                        Respawn.PlayEffect(RespawnEffectType.SummonNtfChopper);
                        response = "Success";
                        return true;
                    }
                }
                if (player_requester.Team == Team.CHI)
                {
                    response = "Success";
                    return true;
                }
                response = "failed";
                return false;
            }
            else
            {
                response = "failed";
                return false;
            }
        }
    }
}
