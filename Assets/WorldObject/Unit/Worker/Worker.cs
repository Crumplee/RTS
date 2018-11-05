using UnityEngine;
using RTS;

public class Worker : Unit
{

    public float capacity;

    private bool harvesting = false, emptying = false;
    private float currentLoad = 0.0f;
    private ResourceType harvestType;

    private Resource resourceDeposit;
    public Building resourceStore;

    public float collectionAmount, depositAmount;
    private float currentDeposit = 0.0f;

    //building
    public int buildSpeed;
    [SerializeField]
    private Building currentProject;
    [SerializeField]
    private bool building = false;
    private float amountBuilt = 0.0f;


    /*** Game Engine methods, all can be overridden by subclass ***/

    protected override void Start()
    {
        base.Start();
        harvestType = ResourceType.Unknown;
        actions = new string[] { "Castle", "Barrack", "House", "Farm" };
    }

    protected override void Update()
    {
        base.Update();
        if (!rotating && !moving)// && !idle)
        {
            if (harvesting || emptying)
            {
                if (harvesting)
                {
                    Collect();
                    if (currentLoad >= capacity || resourceDeposit.isEmpty())
                    {
                        //make sure that we have a whole number to avoid bugs
                        //caused by floating point numbers
                        currentLoad = Mathf.Floor(currentLoad);
                        harvesting = false;
                        emptying = true;
                        //foreach (Arms arm in arms) arm.renderer.enabled = false;
                        //StartMove(resourceStore.transform.position, resourceStore.gameObject);
                        StartMove(resourceStore.spawnPoint, resourceStore.gameObject);
                    }
                }
                else
                {
                    Deposit();
                    if (currentLoad <= 0)
                    {
                        emptying = false;
                        //foreach (Arms arm in arms) arm.renderer.enabled = false;
                        if (!resourceDeposit.isEmpty())
                        {
                            harvesting = true;
                            StartMove(resourceDeposit.transform.position, resourceDeposit.gameObject);
                        }
                    }
                }
            }
            
            if (building && currentProject && currentProject.UnderConstruction())
            {
                amountBuilt += buildSpeed * Time.deltaTime;
                int amount = Mathf.FloorToInt(amountBuilt);
                if (amount > 0)
                {
                    amountBuilt -= amount;
                    currentProject.Construct(amount);
                    if (!currentProject.UnderConstruction())
                    {
                        building = false;
                        //idle = true;
                    }
                }
            }
        }
    }

    public override void Init(Building creator)
    {
        base.Init(creator);
        resourceStore = creator;
    }


    private void Collect()
    {
        float collect = collectionAmount * Time.deltaTime;
        //make sure that the harvester cannot collect more than it can carry
        if (currentLoad + collect > capacity) collect = capacity - currentLoad;
        resourceDeposit.Remove(collect);
        currentLoad += collect;
    }

    private void Deposit()
    {
        currentDeposit += depositAmount * Time.deltaTime;
        int deposit = Mathf.FloorToInt(currentDeposit);
        if (deposit >= 1)
        {
            if (deposit > currentLoad) deposit = Mathf.FloorToInt(currentLoad);
            currentDeposit -= deposit;
            currentLoad -= deposit;
            ResourceType depositType = harvestType;
            //if (harvestType == ResourceType.Ore) depositType = ResourceType.Money;
            player.AddResource(depositType, deposit);
        }
    }


    /* Public Methods */

    public override void SetHoverState(GameObject hoverObject)
    {
        base.SetHoverState(hoverObject);
        //only handle input if owned by a human player and currently selected
        if (player && player.human && currentlySelected)
        {
            if (hoverObject.name != "Ground")
            {
                Resource resource = hoverObject.transform.parent.GetComponent<Resource>();
                if (resource && !resource.isEmpty()) player.hud.SetCursorState(CursorState.Harvest);
            }
        }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClick(hitObject, hitPoint, controller);
        bool doBase = true;
        //only handle input if owned by a human player
        if (player && player.human)
        {
            if (hitObject && hitObject.name != "Ground")
            {
                //idle = false;
                Resource resource = hitObject.transform.parent.GetComponent<Resource>();
                Building building = hitObject.transform.parent.GetComponent<Building>();

                if (resource && !resource.isEmpty())
                {
                    //make sure that we select harvester remains selected
                    if (resource.GetResourceType() == ResourceType.Food && !resource.CanHarvest)
                    {

                    }
                    else
                    {
                        if (player.SelectedObject) player.SelectedObject.SetSelection(false, playingArea);
                        SetSelection(true, playingArea);
                        player.SelectedObject = this;
                        StartHarvest(resource);
                    }

                }
                if (building)
                {
                    if (building.UnderConstruction())
                    {
                        SetBuilding(building);
                        doBase = false;
                    }
                }
            }
            else StopHarvest();
            if (doBase) base.MouseClick(hitObject, hitPoint, controller);
        }
    }
    

   
    private void StartHarvest(Resource resource)
    {
        resourceDeposit = resource;

        if (resource.GetResourceType() == ResourceType.Food) resource.CanHarvest = false;

        StartMove(resource.transform.position, resource.gameObject);
        //we can only collect one resource at a time, other resources are lost
        if (harvestType == ResourceType.Unknown || harvestType != resource.GetResourceType())
        {
            harvestType = resource.GetResourceType();
            currentLoad = 0.0f;
        }
        harvesting = true;
        emptying = false;
    }

    private void StopHarvest()
    {
        //idle = true;
        harvesting = false;
        emptying = false;
        if (resourceDeposit && resourceDeposit.GetResourceType() == ResourceType.Food) resourceDeposit.CanHarvest = true;
    }

    protected override void DrawSelectionBox(Rect selectBox)
    {
        base.DrawSelectionBox(selectBox);
        float percentFull = currentLoad / capacity;
        float maxHeight = selectBox.height - 4;
        float height = maxHeight * percentFull;
        float leftPos = selectBox.x + selectBox.width - 7;
        float topPos = selectBox.y + 2 + (maxHeight - height);
        float width = 5;
        Texture2D resourceBar = ResourceManager.GetResourceHealthBar(harvestType);

        if (resourceBar) GUI.DrawTexture(new Rect(leftPos, topPos, width, height), resourceBar);
    }

    public override void StartMove(Vector3 destination)
    {
        base.StartMove(destination);
        amountBuilt = 0.0f;
        building = false;
    }

    //building
    public override void SetBuilding(Building project)
    {
        currentProject = project;
        StartMove(currentProject.transform.position, currentProject.gameObject);
        building = true;
        //idle = false;
    }

    public override void PerformAction(string actionToPerform)
    {
        base.PerformAction(actionToPerform);
        Player player = this.GetComponentInParent<Player>();
        if (player && player.human)
        {
            if (player.tempBuilding) player.CancelBuildingPlacement();
        }
        if (ResourceManager.CanCreateObject(player, "Building", actionToPerform))
            CreateBuilding(actionToPerform);
    }

    private void CreateBuilding(string buildingName)
    {
        Vector3 buildPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z + 10);
        if (player) player.CreateBuilding(buildingName, buildPoint, this, playingArea);
    }
}