using UnityEngine;

namespace GorillaShirts.Models.Locations
{
    internal class Location_Mines : Location_Base
    {
        public override GTZone[] Zones => [GTZone.mines];
        public override Vector3 Position => new(-45.7829f, -7.323f, -76.7468f);
        public override Vector3 EulerAngles => Vector3.up * 53.7666f;
    }
}
