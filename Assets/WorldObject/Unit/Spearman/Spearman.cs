using UnityEngine;
using RTS;

public class Spearman : Unit
{
    protected override void Start()
    {
        base.Start();
        actions = new string[] { "Attack" };
    }

    protected override void Update()
    {
        base.Update();
        if (!rotating && !moving && !idle)
        {
            
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
                //Resource resource = hoverObject.transform.parent.GetComponent<Resource>();
                //if (resource && !resource.isEmpty()) player.hud.SetCursorState(CursorState.Harvest);
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
                idle = false;
                
            }
            
            if (doBase) base.MouseClick(hitObject, hitPoint, controller);
        }
    }

    /* Private Methods */

    protected override void DrawSelectionBox(Rect selectBox)
    {
        base.DrawSelectionBox(selectBox);
    }

    public override void StartMove(Vector3 destination)
    {
        base.StartMove(destination);
    }
}