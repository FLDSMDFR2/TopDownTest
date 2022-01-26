using UnityEngine;
using UnityEngine.UI;

public class UI_Power : MonoBehaviour
{
    public Image PowerBar;

    private void Start()
    {
        UIEvents.PowerUpdate += UIEvents_PowerUpdate;
    }

    private void UIEvents_PowerUpdate(float power, float maxPower)
    {
        PowerBar.fillAmount = power / maxPower;
    }

    private void OnDestroy()
    {
        UIEvents.PowerUpdate -= UIEvents_PowerUpdate;
    }
}
