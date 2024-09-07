using UnityEngine;
using TMPro;

public class AnchorDisplay : MonoBehaviour
{
    public TextMeshProUGUI uuidText;
    public TextMeshProUGUI savedStatusText;
    public TextMeshProUGUI colorNameText;

    
    public void SetTexts(string uuid, string savedStatus, string colorName)
    {
        if (uuidText != null)
        {
            uuidText.text = "UUID: " + uuid;
        }

        if (savedStatusText != null)
        {
            savedStatusText.text = savedStatus;
        }

        if (colorNameText != null)
        {
            colorNameText.text = "Color: " + colorName;
        }
    }
}