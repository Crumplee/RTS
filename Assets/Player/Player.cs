﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour
{

    public string username;
    public bool human;
    public HUD hud;

    public WorldObject SelectedObject { get; set; }

    private Dictionary<ResourceType, int> resources;
    private int currentPopulation;

    //for building
    public Material notAllowedMaterial, allowedMaterial;


    public Building tempBuilding;
    private Unit tempCreator;
    private bool findingPlacement = false;

    public Color teamColor;
    // Use this for initialization
    void Start()
    {
        hud = GetComponentInChildren<HUD>();
    }

    void Awake()
    {
        resources = InitResourceList();
        currentPopulation = 0;
        AddResource(ResourceType.Population, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (human)
        {
            hud.SetResourceValues(resources);
            if (findingPlacement)
            {
                tempBuilding.CalculateBounds();
                if (CanPlaceBuilding()) tempBuilding.SetTransparentMaterial(allowedMaterial, false);
                else tempBuilding.SetTransparentMaterial(notAllowedMaterial, false);
            }
        }
    }


    private Dictionary<ResourceType, int> InitResourceList()
    {
        Dictionary<ResourceType, int> list = new Dictionary<ResourceType, int>();
        list.Add(ResourceType.Food, 1000);
        list.Add(ResourceType.Wood, 1000);
        list.Add(ResourceType.Gold, 1000);
        list.Add(ResourceType.Population, 0);
        return list;
    }

    public void AddResource(ResourceType type, int amount)
    {
        resources[type] += amount;
    }

    public void ReduceResources(Dictionary<ResourceType, int> resourceCosts)
    {
        foreach (KeyValuePair<ResourceType, int> resourceCost in resourceCosts)
        {
            resources[resourceCost.Key] -= resourceCost.Value;
        }
    }
    
    public int GetResource(ResourceType type)
    {
        return resources[type];
    }

    public void AddUnit(string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation, Building creator)
    {
        Units units = GetComponentInChildren<Units>();
        GameObject newUnit = (GameObject)Instantiate(ResourceManager.GetUnit(unitName), spawnPoint, rotation);
        newUnit.transform.parent = units.transform;
        Unit unitObject = newUnit.GetComponent<Unit>();
        if (unitObject)
        {
            unitObject.Init(creator);
            if (spawnPoint != rallyPoint) unitObject.StartMove(rallyPoint);
        }
    }

    //building
    public void CreateBuilding(string buildingName, Vector3 buildPoint, Unit creator, Rect playingArea)
    {
        GameObject newBuilding = (GameObject)Instantiate(ResourceManager.GetBuilding(buildingName), buildPoint, new Quaternion());
        tempBuilding = newBuilding.GetComponent<Building>();
        if (tempBuilding)
        {
            tempCreator = creator;
            findingPlacement = true;
            tempBuilding.SetTransparentMaterial(notAllowedMaterial, true);
            tempBuilding.SetColliders(false);
            tempBuilding.SetPlayingArea(playingArea);
        }
        else Destroy(newBuilding);
    }

    public bool IsFindingBuildingLocation()
    {
        return findingPlacement;
    }

    public void FindBuildingLocation()
    {
        Vector3 newLocation = WorkManager.FindHitPoint(Input.mousePosition);
        newLocation.y = 0;
        tempBuilding.transform.position = newLocation;
    }

    public bool CanPlaceBuilding()
    {
        bool canPlace = true;

        Bounds placeBounds = tempBuilding.GetSelectionBounds();
        //shorthand for the coordinates of the center of the selection bounds
        float cx = placeBounds.center.x;
        float cy = placeBounds.center.y;
        float cz = placeBounds.center.z;
        //shorthand for the coordinates of the extents of the selection box
        float ex = placeBounds.extents.x;
        float ey = placeBounds.extents.y;
        float ez = placeBounds.extents.z;

        //Determine the screen coordinates for the corners of the selection bounds
        List<Vector3> corners = new List<Vector3>();
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz + ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz - ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz + ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz + ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz - ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz + ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz - ez)));
        corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz - ez)));

        foreach (Vector3 corner in corners)
        {
            GameObject hitObject = WorkManager.FindHitObject(corner);
            if (hitObject && hitObject.name != "Ground")
            {
                WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
                if (worldObject && placeBounds.Intersects(worldObject.GetSelectionBounds())) canPlace = false;
            }
        }
        return canPlace;
    }

    public void StartConstruction()
    {
        findingPlacement = false;
        Buildings buildings = GetComponentInChildren<Buildings>();
        if (buildings) tempBuilding.transform.parent = buildings.transform;
        tempBuilding.SetPlayer();
        tempBuilding.SetColliders(true);
        ReduceResources(tempBuilding.GetCosts());
        tempCreator.SetBuilding(tempBuilding);
        tempBuilding.StartConstruction();
    }

    public void CancelBuildingPlacement()
    {
        findingPlacement = false;
        Destroy(tempBuilding.gameObject);
        tempBuilding = null;
        tempCreator = null;
    }

    public void ModifycurrentPopulation(int amount)
    {
        currentPopulation += amount;
    }

    public int GetcurrentPopulation()
    {
        return currentPopulation;
    }

}
