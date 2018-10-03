using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class FarmResource : Resource
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        resourceType = ResourceType.Food;
        amountLeft = capacity;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isEmpty())
        {
            Destroy(gameObject);
        }
    }
}
