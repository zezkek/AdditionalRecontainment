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
        [Description("Cooldown of comands")]
        public int Cooldown { get; set; } = 20;
        [Description("Max distance between SCP and player while containming")]
        public float Distance { get; set; } = 10f;

        [Description("Ignored roles")]
        public List<RoleType> Roles { get; set; } = new List<RoleType> { RoleType.Scp079, RoleType.Scp106 };
        [Description("Cassie after scp evacuation by MTF")]
        public string ScpEvacuate { get; set; } = "{scpnumber} Evacuate Successfully";
        [Description("Cassie after scp evacuation by CHI")]
        public string ScpSteal { get; set; } = "{scpnumber} is lost from possible scan zone";
        [Description("Items that mtf can evacuate")]
        public List<ItemType> MTFEvacItems { get; set; } = new List<ItemType> { 
            ItemType.KeycardChaosInsurgency, ItemType.KeycardContainmentEngineer,
            ItemType.KeycardFacilityManager, ItemType.KeycardNTFCommander,
            ItemType.KeycardO5, ItemType.MicroHID,
            ItemType.SCP018, ItemType.SCP207,
            ItemType.SCP207, ItemType.SCP500 };
        [Description("Items that chaos can evacuate")]
        public List<ItemType> CHIEvacItems { get; set; } = new List<ItemType> { 
            ItemType.KeycardFacilityManager, ItemType.KeycardO5,
            ItemType.SCP018, ItemType.SCP207, 
            ItemType.SCP268, ItemType.SCP500, 
            ItemType.MicroHID };
    }
}
