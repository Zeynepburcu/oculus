using System;
using TMPro;
using UnityEngine;

public class AnchorLoader : MonoBehaviour
{
    private OVRSpatialAnchor anchorPrefab;
    private SpatialAnchorManager spatialAnchorManager;
    private Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;

    private void Awake()
    {
        // SpatialAnchorManager bileşenini alıyoruz
        spatialAnchorManager = GetComponent<SpatialAnchorManager>();
        // SpatialAnchorManager'dan anchorPrefab'i alıyoruz
        anchorPrefab = spatialAnchorManager?.anchorPrefab;
        // Yerelleştirme için callback fonksiyonu oluşturuyoruz
        _onLoadAnchor = OnLocalized;
    }

    public void LoadAnchorsByUuid()
    {
        // `SpatialAnchorManager.NumUuidsPlayerPref` ile PlayerPrefs'e erişiyoruz
        if (!PlayerPrefs.HasKey(SpatialAnchorManager.NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(SpatialAnchorManager.NumUuidsPlayerPref, 0);
        }

        int playerUuidCount = PlayerPrefs.GetInt(SpatialAnchorManager.NumUuidsPlayerPref);

        if (playerUuidCount == 0)
        {
            Debug.Log("No saved UUIDs found.");
            return;
        }

        var uuids = new Guid[playerUuidCount];
        for (int i = 0; i < playerUuidCount; i++)
        {
            var uuidKey = "uuid" + i;
            var currentUuid = PlayerPrefs.GetString(uuidKey);
            uuids[i] = new Guid(currentUuid);
        }

        Load(new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = uuids
        });
    }

    private void Load(OVRSpatialAnchor.LoadOptions options)
    {
        // UnboundAnchors'ı yüklüyoruz
        OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
        {
            if (anchors == null || anchors.Length == 0)
            {
                Debug.Log("No anchors found.");
                return;
            }

            foreach (var unboundAnchor in anchors)
            {
                if (unboundAnchor.Localized)
                {
                    _onLoadAnchor(unboundAnchor, true);
                }
                else if (!unboundAnchor.Localizing)
                {
                    unboundAnchor.Localize(_onLoadAnchor);
                }
            }
        });
    }

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        if (!success)
        {
            Debug.LogError("Failed to localize anchor.");
            return;
        }

        // Unbound anchor'ı yeni bir spatial anchor'a bağlıyoruz
        var pose = unboundAnchor.Pose; // Konum ve rotasyon al
        var instantiatedAnchor = Instantiate(anchorPrefab, pose.position, pose.rotation);

        // UUID ve durumu gösteriyoruz
        if (instantiatedAnchor.TryGetComponent<OVRSpatialAnchor>(out var foundAnchor))
        {
            var uuidText = instantiatedAnchor.GetComponentInChildren<TextMeshProUGUI>();
            var savedStatusText = instantiatedAnchor.GetComponentsInChildren<TextMeshProUGUI>()[1];

            uuidText.text = "UUID: " + foundAnchor.Uuid.ToString();
            savedStatusText.text = "Loaded from Device";
        }
    }
}
