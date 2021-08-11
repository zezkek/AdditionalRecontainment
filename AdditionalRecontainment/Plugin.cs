using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalRecontainment
{
    public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = "AdditionalRecontainment";
        public override string Author { get; } = ".fkn_goose";
        public override Version Version => new Version(0, 1, 0);
        public static readonly Lazy<Plugin> LazyInstance = new Lazy<Plugin>(valueFactory: () => new Plugin());
        public static Plugin PluginItem => LazyInstance.Value;
    }
}
