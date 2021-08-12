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
        public int PersonCount { get; set; } = 2;
        [Description("Time to wait for transport(will be x3)")]
        public int TimeToWait { get; set; } = 10;
    }
}
