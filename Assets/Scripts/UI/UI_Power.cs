using UnityEngine;
using UnityEngine.UI;

public class UI_Power : MonoBehaviour
{
    public Image PowerBar;

    private void Start()
    {
        UIEvents.PowerUpdate += UIEvents_PowerUpdate;
    }

    private void UIEvents_PowerUpdate(BatteryLevel power, float currentPower, float maxPower)
    {
        PowerBar.color = power.DisplayColor;
        PowerBar.fillAmount = currentPower / maxPower;
    }

    private void OnDestroy()
    {
        UIEvents.PowerUpdate -= UIEvents_PowerUpdate;
    }
}
