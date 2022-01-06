using UnityEngine;
using UnityEngine.UI;

public class FPSCounterDisplay : MonoBehaviour
{
    protected float avgFramerate;
    protected string display = "{0} FPS";
    protected Text m_Text;

    private void Start()
    {
        m_Text = GetComponent<Text>();
        InvokeRepeating(nameof(GetFPS), 1, 1);
    }

    private void GetFPS()
    {
        avgFramerate = (int)(1f / Time.unscaledDeltaTime);
        m_Text.text = string.Format(display, avgFramerate.ToString());
    }
}
