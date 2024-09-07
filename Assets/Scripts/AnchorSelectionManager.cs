using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnchorSelectionManager : MonoBehaviour
{
    public List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>(); // Sahnedeki tüm anchor'lar burada olacak
    public Dropdown firstAnchorDropdown; // İlk dropdown
    public Dropdown secondAnchorDropdown; // İkinci dropdown
    public Button confirmButton; // Seçimleri onaylamak için kullanılacak buton

    private OVRSpatialAnchor selectedFirstAnchor;
    private OVRSpatialAnchor selectedSecondAnchor;

    private void Start()
    {
        // Onay butonuna tıklanınca işlemi yap
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        // Dropdown seçimi değiştiğinde anchor'ı güncelle
        firstAnchorDropdown.onValueChanged.AddListener(delegate { FirstAnchorSelected(); });
        secondAnchorDropdown.onValueChanged.AddListener(delegate { SecondAnchorSelected(); });
    }

    // Dropdown menülerine anchor isimlerini ekleme
    public void PopulateDropdowns()
    {
        // Dropdown menülerini sıfırla
        firstAnchorDropdown.ClearOptions();
        secondAnchorDropdown.ClearOptions();

        List<string> anchorNames = new List<string>();

        // Tüm anchor'ların isimlerini topla (isim yoksa UUID kullanıyoruz)
        foreach (var anchor in anchors)
        {
            string anchorName = anchor.name; // Anchor ismi, yoksa UUID
            if (string.IsNullOrEmpty(anchorName))
            {
                anchorName = anchor.Uuid.ToString(); // UUID'yi isim olarak kullan
            }
            anchorNames.Add(anchorName);
        }

        // Dropdown'lara isimleri ekle
        firstAnchorDropdown.AddOptions(anchorNames);
        secondAnchorDropdown.AddOptions(anchorNames);
    }

    // İlk anchor seçimi yapıldığında çağrılır
    private void FirstAnchorSelected()
    {
        int index = firstAnchorDropdown.value; // Seçilen dropdown değeri
        selectedFirstAnchor = anchors[index]; // Listeden anchor'ı alıyoruz
        Debug.Log("First anchor selected: " + selectedFirstAnchor.name);
    }

    // İkinci anchor seçimi yapıldığında çağrılır
    private void SecondAnchorSelected()
    {
        int index = secondAnchorDropdown.value; // Seçilen dropdown değeri
        selectedSecondAnchor = anchors[index]; // Listeden anchor'ı alıyoruz
        Debug.Log("Second anchor selected: " + selectedSecondAnchor.name);
    }

    // Onay butonuna tıklandığında çalışacak fonksiyon
    private void OnConfirmButtonClicked()
    {
        if (selectedFirstAnchor != null && selectedSecondAnchor != null)
        {
            // İki anchor seçildiğinde işlem yapabilirsiniz, örneğin aralarındaki mesafeyi bulabilirsiniz
            float distance = Vector3.Distance(selectedFirstAnchor.transform.position, selectedSecondAnchor.transform.position);
            Debug.Log("Distance between anchors: " + distance);
            
            // Burada iki anchor arasında istediğiniz işlemi yapabilirsiniz
        }
        else
        {
            Debug.LogWarning("Please select both anchors!");
        }
    }
}
