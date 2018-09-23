using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;


public class Wood : Resource
{
    
    protected override void Start ()
    {
        base.Start();
        resourceType = ResourceType.Wood;
        amountLeft = capacity;
    }
    
    protected override void Update () {
        base.Update();
        if (isEmpty())
        {
            Destroy(gameObject);
        }
    }
}
