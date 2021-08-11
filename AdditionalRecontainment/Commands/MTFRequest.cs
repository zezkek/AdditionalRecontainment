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
namespace AdditionalRecontainment.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class MTFRequest : ICommand
    {
        public string Command { get; } = "Request";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "request mtf helicopter";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out RespawnEffectType pls)
        {
            if (sender is PlayerCommandSender)
            {
                RespawnEffectType.SummonNtfChopper
                return true;
            }
            else
            {
                pls = RespawnEffectType.
                return false;
            }
        }
    }
}
