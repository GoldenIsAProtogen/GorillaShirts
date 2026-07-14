using UnityEngine;

namespace GorillaShirts.Models.Locations;

internal class Location_GTFC : Location_Base
{
    public override GTZone[] Zones => [GTZone.GTFC];
    public override Vector3  Position => new(232.2535f, 159.4769f, -790.4909f);
    public override Vector3  EulerAngles => Vector3.up * 302.3933f;
}