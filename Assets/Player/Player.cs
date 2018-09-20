using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour {

	public string username;
	public bool human;
	public HUD hud;
	
	public WorldObject SelectedObject { get; set; }
	
	private Dictionary< ResourceType, int > resources;

    //for building
    public Material notAllowedMaterial, allowedMaterial;

    private Building tempBuilding;
    private Unit tempCreator;
    private bool findingPlacement = false;

    // Use this for initialization
    void Start () {
		hud = GetComponentInChildren< HUD >();
	}
	
	void Awake () {
		resources = InitResourceList();
	}
	
	// Update is called once per frame
	void Update () {
		if(human) {
			hud.SetResourceValues(resources);
		}
	}
	
	
	private Dictionary< ResourceType, int > InitResourceList() {
		Dictionary< ResourceType, int > list = new Dictionary< ResourceType, int >();
		list.Add(ResourceType.Food, 100);
		list.Add(ResourceType.Wood, 100);
		list.Add(ResourceType.Gold, 100);
		return list;
	}
	
	public void AddResource(ResourceType type, int amount) {
		resources[type] += amount;
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
}
