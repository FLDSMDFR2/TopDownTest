using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomItemIteraction : MonoBehaviour
{
    [SerializeField]
    public Canvas InteractVisualCanvas;
    [SerializeField]
    public TextMeshProUGUI interactDisplayText;

    protected virtual void Awake()
    {
        interactDisplayText = InteractVisualCanvas.GetComponentInChildren<TextMeshProUGUI>();
        interactDisplayText.text = "E";//TODO: Set based on hot key

        InteractVisualCanvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the interaction Icon
    /// </summary>
    public virtual void Show()
    {
        InteractVisualCanvas.gameObject.transform.LookAt(Camera.main.transform.position);
        InteractVisualCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide the interaction icon
    /// </summary>
    public virtual void Hide()
    {
        InteractVisualCanvas.gameObject.SetActive(false);
    }
}
