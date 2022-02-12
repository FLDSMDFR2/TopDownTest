using System;
using Unity.Mathematics;

[Serializable]
public class RoomItemCharger : RoomItem
{
    #region Config
    protected override void ConfigureSelf()
    {
        Type = RoomItemType.Charger;
        maxItemUsesInRoom = 1;
    }
    #endregion

    #region Place Location Checks
    protected override bool CheckLocation(Room room, int2 locationKey)
    {
        // this must be next to a wall
        return room.HasTypeAdjacent(locationKey, RoomLocationEnvironmentTypes.Wall);
    }
    #endregion
}
