using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverHandler : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private UnityEngine.UI.Image buttonIcon;

    [SerializeField] private Color normalTextColor = new Color(0.945f, 0.961f, 0.984f);
    [SerializeField] private Color normalIconColor = new Color(0.945f, 0.961f, 0.984f);

    [SerializeField] private Color hoverTextColor = new Color(0.133f, 0.129f, 0.063f);
    [SerializeField] private Color hoverIconColor = new Color(0.133f, 0.129f, 0.063f);

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetColors(hoverTextColor, hoverIconColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetColors(normalTextColor, normalIconColor);
    }

    private void SetColors(Color textCol, Color iconCol)
    {
        if (buttonText != null) buttonText.color = textCol;
        if (buttonIcon != null) buttonIcon.color = iconCol;
    }
}
