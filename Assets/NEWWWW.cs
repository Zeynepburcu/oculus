using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpatialAnchorManager : MonoBehaviour
{
    public OVRSpatialAnchor anchorPrefab;
    public const string NumUuidsPlayerPref = "numUuids";

    private Canvas canvas;
    private TextMeshProUGUI uuidText;
    private TextMeshProUGUI savedStatusText;
    private TextMeshProUGUI colorNameText;
    private List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>();
    private OVRSpatialAnchor lastCreatedAnchor;
    private AnchorLoader anchorLoader;

    // Renk isimleri listesi
    private List<string> colorNames = new List<string> { "Red", "Blue", "Green", "Yellow", "Purple", "Orange", "Pink", "Cyan", "Magenta", "Brown" };

    // Kullanılan renklerin listesi
    private List<string> usedColorNames = new List<string>();

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

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            SaveLastCreatedAnchor();
        }

        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            UnsaveLastCreatedAnchor();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            UnsaveAllAnchors();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            LoadSavedAnchors();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            DeleteLastCreatedAnchor();
        }


    }

    public void CreateSpatialAnchor()
    {
        OVRSpatialAnchor workingAnchor = Instantiate(anchorPrefab, OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch));
        canvas = workingAnchor.gameObject.GetComponentInChildren<Canvas>();
        uuidText = canvas.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        savedStatusText = canvas.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        colorNameText = canvas.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        // Rastgele bir renk ismi seç ve renk ismi kullanılmamışsa ata
        string randomColorName = GetRandomColorName();
        colorNameText.text = "Color: " + randomColorName;

        StartCoroutine(AnchorCreated(workingAnchor, randomColorName));
    }

    private IEnumerator AnchorCreated(OVRSpatialAnchor workingAnchor, string colorName)
    {
        while (!workingAnchor.Created && !workingAnchor.Localized)
        {
            yield return new WaitForEndOfFrame();
        }

        Guid anchorGuid = workingAnchor.Uuid;
        anchors.Add(workingAnchor);
        lastCreatedAnchor = workingAnchor;

        uuidText.text = "UUID: " + anchorGuid.ToString();
        savedStatusText.text = "Not Saved";
        colorNameText.text = "Room: " + colorName;
    }

    private void SaveLastCreatedAnchor()
    {
        lastCreatedAnchor.Save((lastCreatedAnchor, success) =>
        {
            if (success)
            {
                savedStatusText.text = "Saved";
                SaveUuidToPlayerPrefs(lastCreatedAnchor.Uuid);
            }
        });
    }

    void SaveUuidToPlayerPrefs(Guid uuid)
    {
        if (!PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(NumUuidsPlayerPref, 0);
        }

        int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);
        PlayerPrefs.SetString("uuid" + playerNumUuids, uuid.ToString());
        PlayerPrefs.SetInt(NumUuidsPlayerPref, ++playerNumUuids);
    }

    private void UnsaveLastCreatedAnchor()
    {
        lastCreatedAnchor.Erase((lastCreatedAnchor, success) =>
        {
            if (success)
            {
                savedStatusText.text = "Not Saved";
            }
        });
    }

    private void UnsaveAllAnchors()
    {
        foreach (var anchor in anchors)
        {
            UnsaveAnchor(anchor);
        }

        anchors.Clear();
        ClearAllUuidsFromPlayerPrefs();
    }

    private void UnsaveAnchor(OVRSpatialAnchor anchor)
    {
        anchor.Erase((erasedAnchor, success) =>
        {
            if (success)
            {
                var textComponents = erasedAnchor.GetComponentsInChildren<TextMeshProUGUI>();
                if (textComponents.Length > 1)
                {
                    var savedStatusText = textComponents[1];
                    savedStatusText.text = "Not Saved";
                }
            }
        });
    }

    private void ClearAllUuidsFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);
            for (int i = 0; i < playerNumUuids; i++)
            {
                PlayerPrefs.DeleteKey("uuid" + i);
            }

            PlayerPrefs.DeleteKey(NumUuidsPlayerPref);
            PlayerPrefs.Save();
        }
    }

    public void LoadSavedAnchors()
    {
        anchorLoader.LoadAnchorsByUuid();
    }

    private void DeleteLastCreatedAnchor()
    {
        if (lastCreatedAnchor != null)
        {
            Destroy(lastCreatedAnchor.gameObject);
            anchors.Remove(lastCreatedAnchor);
            lastCreatedAnchor = null;
            savedStatusText.text = "Deleted";
        }
    }


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
}
