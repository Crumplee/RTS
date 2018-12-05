using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RTS;

public class Building : WorldObject
{
    public float maxTrainProgress;
    protected Queue<string> trainQueue;
    private float currentTrainProgress = 0.0f;

    public Vector3 spawnPoint;
    protected Vector3 rallyPoint;
    public Vector3 buildingPoint;
    public Texture2D rallyPointImage;

    private bool needsBuilding = false;

    public int population;

    protected override void Awake()
    {
        base.Awake();
        trainQueue = new Queue<string>();
    }

    protected override void Start()
    {
        base.Start();
        spawnPoint = GetComponentInChildren<SpawnPoint>().GetPoint();
        buildingPoint = GetComponentInChildren<BuildingPoint>().GetPoint();
        rallyPoint = spawnPoint;
    }

    protected override void Update()
    {
        base.Update();
        ProcessTrainQueue();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
        if (needsBuilding) DrawBuildProgress();
    }

    public override void DestroyObject()
    {
        base.DestroyObject();
        Dictionary<ResourceType, int> resourceCosts = new Dictionary<ResourceType, int>();
        resourceCosts.Add(ResourceType.Population, population);
        player.ReduceResources(resourceCosts);
        Destroy(gameObject);
    }

    protected void CreateUnit(string unitName)
    {
        player.ReduceResources(ResourceManager.GetUnit(unitName).GetComponent<WorldObject>().GetCosts());
        trainQueue.Enqueue(unitName);
    }

    protected void ProcessTrainQueue()
    {
        if (trainQueue.Count > 0)
        {
            currentTrainProgress += Time.deltaTime * ResourceManager.BuildSpeed;
            if (currentTrainProgress > maxTrainProgress)
            {
                if (player) player.CmdAddUnit(trainQueue.Dequeue(), spawnPoint, rallyPoint, transform.rotation/*, this*/);
                currentTrainProgress = 0.0f;
            }
        }
    }

    public string[] getTrainQueueValues()
    {
        string[] values = new string[trainQueue.Count];
        int pos = 0;
        foreach (string unit in trainQueue) values[pos++] = unit;
        return values;
    }

    public float getTrainPercentage()
    {
        return currentTrainProgress / maxTrainProgress;
    }

    public override void SetSelection(bool selected, Rect playingArea)
    {
        base.SetSelection(selected, playingArea);
        if (player)
        {
            RallyPoint flag = player.GetComponentInChildren<RallyPoint>();
            if (selected)
            {
                if (flag && spawnPoint != ResourceManager.InvalidPosition && rallyPoint != ResourceManager.InvalidPosition)
                {
                    flag.transform.localPosition = rallyPoint;
                    flag.transform.forward = transform.forward;
                    flag.Enable();
                }
            }
            else
            {
                if (flag) flag.Disable();
            }
        }
    }

    public bool hasSpawnPoint()
    {
        return spawnPoint != ResourceManager.InvalidPosition && rallyPoint != ResourceManager.InvalidPosition;
    }

    public override void SetHoverState(GameObject hoverObject)
    {
        base.SetHoverState(hoverObject);
        //only handle input if owned by a human player and currently selected
        if (player && currentlySelected)
        {
            if (hoverObject.name == "Ground")
            {
                if (player.hud.GetPreviousCursorState() == CursorState.RallyPoint) player.hud.SetCursorState(CursorState.RallyPoint);
            }
        }
    }

    public void SetRallyPoint(Vector3 position)
    {
        rallyPoint = position;
        if (player && currentlySelected)
        {
            RallyPoint flag = player.GetComponentInChildren<RallyPoint>();
            if (flag) flag.transform.localPosition = rallyPoint;
        }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClick(hitObject, hitPoint, controller);
        //only handle iput if owned by a human player and currently selected
        if (player && currentlySelected)
        {
            if (hitObject.name == "Ground")
            {
                if ((player.hud.GetCursorState() == CursorState.RallyPoint || player.hud.GetPreviousCursorState() == CursorState.RallyPoint) && hitPoint != ResourceManager.InvalidPosition)
                {
                    SetRallyPoint(hitPoint);
                }
            }
        }
    }

    //building
    public void StartConstruction()
    {
        CalculateBounds();
        needsBuilding = true;
        this.GetComponentInParent<Player>().tempBuilding = null;
        hitPoints = 0;
        NavMeshObstacle nmo = GetComponent<NavMeshObstacle>();
        if (nmo) nmo.enabled = true;
    }

    private void DrawBuildProgress()
    {
        GUI.skin = ResourceManager.SelectBoxSkin;
        Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
        //Draw the selection box around the currently selected object, within the bounds of the main draw area
        GUI.BeginGroup(playingArea);
        CalculateCurrentHealth(0.5f, 0.99f);
        DrawHealthBar(selectBox, "Building ...");
        GUI.EndGroup();
    }

    public bool UnderConstruction()
    {
        return needsBuilding;
    }

    private void SetSpawnandRallyPoint()
    {
        spawnPoint = GetComponentInChildren<SpawnPoint>().GetPoint();
        rallyPoint = spawnPoint;
    }

    public void Construct(int amount)
    {
        hitPoints += amount;
        if (hitPoints >= maxHitPoints)
        {
            hitPoints = maxHitPoints;
            needsBuilding = false;
            RestoreMaterials();
            SetTeamColor();
            SetSpawnandRallyPoint();
            player.AddResource(ResourceType.Population, population);
            if (gameObject.tag == "Farm") gameObject.GetComponent<FarmResource>().enabled = true;
        }
    }
}
