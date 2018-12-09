using UnityEngine;
using RTS;

public class Resource : WorldObject
{

    private bool canHarvest;


    public float capacity;


    [SerializeField]
    protected float amountLeft;
    protected ResourceType resourceType;

  
    

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
