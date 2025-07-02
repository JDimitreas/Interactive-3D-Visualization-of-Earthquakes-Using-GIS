using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.GameEngine.Map;
using Esri.ArcGISMapsSDK.Components;

public class ArcGISMapSwitcher : MonoBehaviour {
    public ArcGISMapComponent basemapComponent;

    public void SwitchToSatellite() {
        basemapComponent.BasemapType = BasemapTypes.ImageLayer;
        basemapComponent.Basemap = "https://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer";
    }

    public void SwitchToCustom() {
        basemapComponent.BasemapType = BasemapTypes.VectorTileLayer;
        basemapComponent.Basemap = "https://raw.githubusercontent.com/JDimitreas/Interactive-3D-Visualization-of-Earthquakes-Using-GIS/refs/heads/main/World%20Simple%20Map%20(with%20Contours%20and%20Hillshade).json";
    }

    public void SwitchToCustomTransparent()
    {
        basemapComponent.BasemapType = BasemapTypes.VectorTileLayer;
        basemapComponent.Basemap = "https://raw.githubusercontent.com/JDimitreas/Interactive-3D-Visualization-of-Earthquakes-Using-GIS/refs/heads/main/WorldSimpleMap_transparent.json";

    }

    public void ToggleBasemap()
    {
        if (basemapComponent == null)
        {
            Debug.LogError("BasemapComponent is not assigned!");
            return;
        }

        string currentBasemap = basemapComponent.Basemap;

        if (currentBasemap.Contains("World_Imagery"))
        {
            SwitchToCustom();
        }
        else if (currentBasemap.Contains("Hillshade"))
        {
            SwitchToCustomTransparent();
        }
        else if (currentBasemap.Contains("transparent"))
        {
            SwitchToSatellite();
        }
    }
}

