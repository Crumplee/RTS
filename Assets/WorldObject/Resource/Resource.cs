using UnityEngine;
using RTS;

public class Resource : WorldObject
{

    private bool canHarvest;

    //Public variables
    public float capacity;

    //Variables accessible by subclass
    [SerializeField]
    protected float amountLeft;
    protected ResourceType resourceType;

    /*** Game Engine methods, all can be overridden by subclass ***/    

    protected override void Start()
    {
        base.Start();
        amountLeft = capacity;
        resourceType = ResourceType.Unknown;
        CanHarvest = true;
    }

    /*** Public methods ***/

    public void Remove(float amount)
    {
        //need fix
        amountLeft -= amount;
        if (amountLeft <= 0) amountLeft = 0;
    }

    public bool isEmpty()
    {
        return amountLeft <= 0;
    }

    public ResourceType GetResourceType()
    {
        return resourceType;
    }

    protected override void CalculateCurrentHealth(float lowSplit, float highSplit)
    {
        healthPercentage = amountLeft / capacity;
        healthStyle.normal.background = ResourceManager.GetResourceHealthBar(resourceType);
    }
    
    public bool CanHarvest
    {
        get
        {
            return canHarvest;
        }

        set
        {
            canHarvest = value;
        }
    }
}
