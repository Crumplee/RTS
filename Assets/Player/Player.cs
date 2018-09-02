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
	
	public void AddUnit(string unitName, Vector3 spawnPoint, Quaternion rotation) {
		Debug.Log ("add " + unitName + " to player");
		Units units = GetComponentInChildren< Units >();
		GameObject newUnit = (GameObject)Instantiate(ResourceManager.GetUnit(unitName), spawnPoint, rotation);
		newUnit.transform.parent = units.transform;
	}
}
