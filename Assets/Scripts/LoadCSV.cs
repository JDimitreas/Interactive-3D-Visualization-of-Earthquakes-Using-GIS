using System;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.Map;
using Esri.GameEngine.View;
using Esri.HPFramework;
using Esri.Unity;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoadCSV : MonoBehaviour {
    public GameObject markerPrefab;
    public ArcGISMapComponent arcGISMap;
    private List<GameObject> markers = new List<GameObject>();
    public float positionJitter = 40f;

    void Start()
    {
        if (!markerPrefab) {
            Debug.LogError("markerPrefab is not assigned in the inspector!");
            return;
        }

        markerPrefab.SetActive(false);

        TextAsset data = Resources.Load<TextAsset>("all_month");
        if (!data) {
            Debug.LogError("CSV file not found at Resources/all_month");
            return;
        }

        ArcGISView view = arcGISMap.View;
        if (view == null) {
            Debug.LogError("ArcGISMapComponent.View is null!");
            return;
        }

        string[] lines = data.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) {
            var cols = lines[i].Split(',');
            if (cols.Length < 5) continue;

            if (!float.TryParse(cols[1], out float lat)) continue;
            if (!float.TryParse(cols[2], out float lon)) continue;
            if (!float.TryParse(cols[4], out float mag)) continue;

            // Create geographic point with altitude 0 (or set altitude here if needed)
            ArcGISPoint geoPoint = new ArcGISPoint(lon, lat, 0, ArcGISSpatialReference.WGS84());

            Debug.Log($"Spawning marker at lat={lat}, lon={lon}, mag={mag}, location={(cols.Length > 13 ? cols[13] : "N/A")}");

            // Instantiate marker at origin; positioning handled by ArcGISLocation
            GameObject m = Instantiate(markerPrefab, Vector3.zero, Quaternion.identity);
            m.transform.SetParent(arcGISMap.gameObject.transform);

            ArcGISLocationComponent arcGISLocationComponent = markerPrefab.GetComponent<ArcGISLocationComponent>();
            arcGISLocationComponent.Position = geoPoint;

            // Scale by magnitude
            float baseScale = Mathf.Clamp(0.3f + mag * 0.4f, 0.5f, 5f);
            m.transform.localScale = Vector3.one * baseScale * Random.Range(100f, 200f);

            m.SetActive(true);

            // Set color by magnitude
            Renderer r = m.GetComponent<Renderer>();
            if (r) {
                r.material = new Material(r.material);
                // if (mag >= 5f)
                //     r.material.color = Color.red;
                // else if (mag >= 3f)
                //     r.material.color = Color.yellow;
                // else
                //     r.material.color = Color.blue;
                float t = Mathf.InverseLerp(0f, 5f, mag);
                r.material.color = Color.Lerp(Color.blue, Color.red, t);
            }

            // Add vertical line to ground
            LineRenderer line = m.AddComponent<LineRenderer>();
            line.positionCount = 2;
            Vector3 top = m.transform.position;
            Vector3 bottom = new Vector3(top.x, 0f, top.z);
            line.SetPosition(0, top);
            line.SetPosition(1, bottom);
            line.startWidth = 0.1f;
            line.endWidth = 0.02f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.white;
            line.endColor = Color.white;
            line.enabled = false;

            markers.Add(m);
        }
        Debug.Log($"{lines.Length} lines extracted from CSV file.");
    }

    void Update() {
        Camera cam = Camera.main;
        if (!cam) return;

        GameObject closestMarker = null;
        float closestDist = float.MaxValue;

        foreach (GameObject marker in markers) {
            float dist = Vector3.Distance(cam.transform.position, marker.transform.position);
            if (dist < closestDist) {
                closestDist = dist;
                closestMarker = marker;
            }
        }

        foreach (GameObject marker in markers) {
            LineRenderer lr = marker.GetComponent<LineRenderer>();
            if (lr) lr.enabled = false;
        }

        if (closestMarker) {
            LineRenderer lr = closestMarker.GetComponent<LineRenderer>();
            if (lr) lr.enabled = true;
        }
    }
}
