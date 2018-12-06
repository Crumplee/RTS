using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Barrack : Building
{

    protected override void Start()
    {
        base.Start();
        actions = new string[] { "Spearman", "Swordsman", "Archer" };
    }

    public override void PerformAction(string actionToPerform)
    {
        base.PerformAction(actionToPerform);
        if (!UnderConstruction())
        {
            if (ResourceManager.CanCreateObject(player, "Unit", actionToPerform)) CreateUnit(actionToPerform);
        }
    }
}
