using System;
using TMPro;
using UnityEngine;

public class AnchorLoader : MonoBehaviour
{
    private OVRSpatialAnchor anchorPrefab;
    private SpatialAnchorManager spatialAnchorManager;
    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;

    private void Awake()
    {
        spatialAnchorManager = GetComponent<SpatialAnchorManager>();
        anchorPrefab = spatialAnchorManager?.anchorPrefab;
        _onLoadAnchor = OnLocalized;
    }

    public void LoadAnchorsByUuid()
    {
        if (!PlayerPrefs.HasKey(SpatialAnchorManager.NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(SpatialAnchorManager.NumUuidsPlayerPref, 0);
        }

        var playerUuidCount = PlayerPrefs.GetInt(SpatialAnchorManager.NumUuidsPlayerPref);

        if (playerUuidCount == 0)
            return;

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
        OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
        {
            if (anchors == null)
            {
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
        if (!success) return;

        // Bind the unbound anchor to a new spatial anchor instance
  

        // Use transform properties to get position and rotation
        var pose = unboundAnchor.Pose;
        var instantiatedAnchor = Instantiate(anchorPrefab, pose.position, pose.rotation);
        
        // Now you can display the UUID and status
        if (instantiatedAnchor.TryGetComponent<OVRSpatialAnchor>(out var foundAnchor))
        {
            var uuidText = instantiatedAnchor.GetComponentInChildren<TextMeshProUGUI>();
            var savedStatusText = instantiatedAnchor.GetComponentsInChildren<TextMeshProUGUI>()[1];

            uuidText.text = "UUID: " + foundAnchor.Uuid.ToString();
            savedStatusText.text = "Loaded from Device";
        }
    }
}
