using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoadCSV : MonoBehaviour {
    public GameObject markerPrefab;
    private List<GameObject> markers = new List<GameObject>();
    public float positionJitter = 40f;

    void Start() {
        // Check if prefab is assigned
        if (!markerPrefab) {
            Debug.LogError("markerPrefab is not assigned in the inspector!");
            return;
        }

        // Load CSV file
        TextAsset data = Resources.Load<TextAsset>("all_month");
        if (!data) {
            Debug.LogError("CSV file not found at Resources/all_month");
            return;
        }

        string[] lines = data.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) {
            var cols = lines[i].Split(',');
            if (cols.Length < 5) continue;

            if (!float.TryParse(cols[1], out float lat)) continue;
            if (!float.TryParse(cols[2], out float lon)) continue;
            if (!float.TryParse(cols[4], out float mag)) continue;

            // Slight jitter to avoid overlapping markers
            float offsetX = Random.Range(-positionJitter, positionJitter);
            float offsetY = Random.Range(-positionJitter * 0.3f, positionJitter * 0.3f);
            float offsetZ = Random.Range(-positionJitter, positionJitter);

            Vector3 pos = new Vector3(lon + offsetX, -650.5f, lat + offsetZ);

            Debug.Log($"Spawning marker at {pos}, mag={mag}");

            GameObject m = Instantiate(markerPrefab, pos, Quaternion.identity);
            float scale = Mathf.Clamp(0.3f + mag * 0.4f, 0.5f, 5f);
            m.transform.localScale = Vector3.one * scale;

            // Per-instance material so colors are independent
            Renderer r = m.GetComponent<Renderer>();
            if (r) {
                r.material = new Material(r.material);
                if (mag >= 5f)
                    r.material.color = Color.red;
                else if (mag >= 3f)
                    r.material.color = Color.yellow;
                else
                    r.material.color = Color.blue;
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
            line.enabled = false; // hidden until nearby

            markers.Add(m);
        }
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
