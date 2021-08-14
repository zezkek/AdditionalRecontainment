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

        [Description("Ignored roles")]
        public List<RoleType> Roles { get; set; } = new List<RoleType> { RoleType.Scp079, RoleType.Scp106 };
        [Description("Cassie after scp evacuation")]
        public string ScpEvacuate { get; set; } = "{scpnumber} Evacuate Successfully";
        [Description("Items that mtf can evacuate")]
        public List<ItemType> MTFEvacItems { get; set; } = new List<ItemType> { 
            ItemType.KeycardChaosInsurgency, ItemType.KeycardContainmentEngineer,
            ItemType.KeycardFacilityManager, ItemType.KeycardNTFCommander,
            ItemType.KeycardO5, ItemType.MicroHID,
            ItemType.SCP018, ItemType.SCP207,
            ItemType.SCP207, ItemType.SCP500 };
        [Description("Items that chaos can evacuate")]
        public List<int> ChaosEvacItems { get; set; } = new List<int> { (int)ItemType.KeycardFacilityManager, (int)ItemType.KeycardO5,
            (int)ItemType.SCP018, (int)ItemType.SCP207, 
            (int)ItemType.SCP268, (int)ItemType.SCP500, 
            (int)ItemType.MicroHID };
    }
}
