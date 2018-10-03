using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Castle : Building
{

    protected override void Start()
    {
        base.Start();
        actions = new string[] { "Worker" };
    }

    public override void PerformAction(string actionToPerform)
    {
        base.PerformAction(actionToPerform);
        if (!UnderConstruction())
        {
            CreateUnit(actionToPerform);
        }
    }

}
