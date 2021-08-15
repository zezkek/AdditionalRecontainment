using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace AdditionalRecontainment.Handlers
{
    public class Support
    {
        public Support NameHandler;

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            ev.NextKnownTeam = Respawning.SpawnableTeamType.NineTailedFox;
        }
    }
}
