using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : Building {

    protected override void Start()
    {
        base.Start();
        actions = new string[] { "Spearman" };
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
