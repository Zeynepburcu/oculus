using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpatialAnchorManager : MonoBehaviour
{
    public OVRSpatialAnchor anchorPrefab;
    public GameObject followSphere; // Top Prefab
    public float followSpeed = 5.0f;
    
    // NumUuidsPlayerPref sabiti, UUID'leri kaydetmek ve yüklemek için kullanılacak
    public const string NumUuidsPlayerPref = "numUuids";

    private List<Vector3> anchorPositions = new List<Vector3>(); // Anchor pozisyonlarını saklar
    private List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>(); // Tüm anchorları saklar
    private OVRSpatialAnchor lastCreatedAnchor;
    private AnchorLoader anchorLoader;

    private List<string> colorNames = new List<string> { "Red", "Blue", "Green", "Yellow", "Purple", "Orange", "Pink", "Cyan", "Magenta", "Brown" };
    private List<string> usedColorNames = new List<string>();

    private int currentTargetIndex = 0;
    private List<Vector3> shortestPath; // En kısa yol noktalarını saklar

    private void Awake()
    {
        anchorLoader = GetComponent<AnchorLoader>();
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            CreateSpatialAnchor();
        }

        // Topun yolu takip etmesi
        FollowPath();
    }

    // Anchor oluşturma fonksiyonu
    public void CreateSpatialAnchor()
    {
        OVRSpatialAnchor workingAnchor = Instantiate(anchorPrefab, OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch));

        // Anchor pozisyonunu kaydet
        anchorPositions.Add(workingAnchor.transform.position);

        // Anchorları listeye ekle
        anchors.Add(workingAnchor);
        lastCreatedAnchor = workingAnchor;

        // Rastgele bir renk ismi seçiyoruz
        string randomColorName = GetRandomColorName();

        // Prefab içindeki AnchorDisplay script'ini alıp bilgileri güncelliyoruz
        AnchorDisplay anchorDisplay = workingAnchor.GetComponentInChildren<AnchorDisplay>();
        anchorDisplay.SetTexts(workingAnchor.Uuid.ToString(), "Not Saved", randomColorName);

        // En kısa yolu hesapla
        shortestPath = CalculateShortestPath();
    }

    // Rastgele renk ismi seç
    private string GetRandomColorName()
    {
        List<string> availableColors = new List<string>(colorNames);
        availableColors.RemoveAll(color => usedColorNames.Contains(color));

        if (availableColors.Count == 0)
        {
            usedColorNames.Clear();
            availableColors = new List<string>(colorNames);
        }

        int randomIndex = UnityEngine.Random.Range(0, availableColors.Count);
        string selectedColor = availableColors[randomIndex];
        usedColorNames.Add(selectedColor);

        return selectedColor;
    }

    // En kısa yol hesaplaması (basit sıralı yol)
    private List<Vector3> CalculateShortestPath()
    {
        List<Vector3> path = new List<Vector3>();

        if (anchorPositions.Count > 1)
        {
            // Şu an basit bir sıralı ziyaret
            path.AddRange(anchorPositions);
        }

        return path;
    }

    // Topun en kısa yolu takip etmesi
    private void FollowPath()
    {
        if (shortestPath != null && currentTargetIndex < shortestPath.Count)
        {
            Vector3 targetPosition = shortestPath[currentTargetIndex];

            // Topu hedef pozisyona doğru hareket ettir
            followSphere.transform.position = Vector3.MoveTowards(followSphere.transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Top hedef pozisyona ulaştıysa bir sonraki hedefe geç
            if (Vector3.Distance(followSphere.transform.position, targetPosition) < 0.1f)
            {
                currentTargetIndex++;
            }
        }
    }
}
