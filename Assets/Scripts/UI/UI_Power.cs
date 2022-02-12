using UnityEngine;
using UnityEngine.UI;

public class UI_Power : MonoBehaviour
{
    public Image PowerBar;

    public Color Green;
    public Color Orange;
    public Color Red;

    private void Start()
    {
        UIEvents.PowerUpdate += UIEvents_PowerUpdate;
    }

    private void UIEvents_PowerUpdate(float power, float maxPower)
    {
        var powerLevel = power / maxPower;
        if (powerLevel < .3f)
            PowerBar.color = Red;
        else if (powerLevel < .6f)
            PowerBar.color = Orange;
        else
            PowerBar.color = Green;

        PowerBar.fillAmount = powerLevel;
    }

    private void OnDestroy()
    {
        UIEvents.PowerUpdate -= UIEvents_PowerUpdate;
    }
}
