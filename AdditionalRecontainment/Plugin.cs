using Exiled.API.Features;
using System;
using MEC;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerEv = Exiled.Events.Handlers.Player;
using ServerEv = Exiled.Events.Handlers.Server;

namespace AdditionalRecontainment
{
    public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = "AdditionalRecontainment";
        public override string Author { get; } = ".fkn_goose & Mydak";
        public override Version Version => new Version(0, 5, 0);
        public static readonly Lazy<Plugin> LazyInstance = new Lazy<Plugin>(valueFactory: () => new Plugin());
        public static Plugin PluginItem => LazyInstance.Value;
    }
}
