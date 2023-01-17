using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents : MonoBehaviour
{
    protected static int PlayerId;
    public static void SetPlayerId(int id)
    {
        PlayerId = id;
    }

    public delegate void PowerUpdateEvent(BatteryLevel power, float currentPower, float maxPower);
    public static event PowerUpdateEvent PowerUpdate;
    public static void RaisePowerUpdateEvent(int playerId, BatteryLevel power, float currentPower, float maxPower)
    {
        if (PlayerId == playerId) PowerUpdate?.Invoke(power, currentPower, maxPower);
    }
}
