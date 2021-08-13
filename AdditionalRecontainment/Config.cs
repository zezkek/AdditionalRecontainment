using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditionalRecontainment
{
    public class Config:IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("Max distance between SCP and player while containming")]
        public float Distance { get; set; } = 10f;
        [Description("Min amount of players to contain SCP")]
        public sbyte PersonCount { get; set; } = 2;
        [Description("Time to wait for transport(will be x3)")]
        public List<RoleType> Roles { get; set; } = new List<RoleType> { RoleType.Scp079, RoleType.Scp106 };
        [Description("Priority of every RoleType")]
        public int[,] RoleTypeArray = new int[,]{{(int)RoleType.Scp173,0 }, {(int)RoleType.Scp049,0 }, {(int)RoleType.Scp096,0 },
            {(int)RoleType.Scientist,0 }, {(int)RoleType.NtfCommander,0 }, {(int)RoleType.NtfScientist,0 },{(int)RoleType.NtfLieutenant,0 },{(int)RoleType.NtfCadet,0 },
            {(int)RoleType.Scientist,1 }, {(int)RoleType.NtfCommander,1 }, {(int)RoleType.NtfScientist,1 },{(int)RoleType.NtfLieutenant,1 },{(int)RoleType.NtfCadet,1 },
            {(int)RoleType.ChaosInsurgency,1 },{(int)RoleType.ClassD,1 },{(int)RoleType.Scp93989,1 }, {(int)RoleType.Scp93953,1 }, {(int)RoleType.Scp0492,1 } };
        [Description("Weight of every RoleType")]
        public Dictionary<RoleType, sbyte> PlayerWeight = new Dictionary<RoleType, sbyte>
        {
            {RoleType.Scp173, 3 },
            {RoleType.Scp049, 2 },
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
    }
}
