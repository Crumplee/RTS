using UnityEngine;
using RTS;

public class Archer : Unit
{
    protected override void Start()
    {
        base.Start();
        actions = new string[] { "Attack" };
    }

    protected override void Update()
    {
        base.Update();
    }


    /* Public Methods */

    public override void SetHoverState(GameObject hoverObject)
    {
        base.SetHoverState(hoverObject);

        if (player && currentlySelected)
        {
            if (hoverObject.name != "Ground")
            {
            }
        }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClick(hitObject, hitPoint, controller);
        bool doBase = true;

        if (player)
        {
            if (hitObject && hitObject.name != "Ground")
            {

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

    //attack
    public override bool CanAttack()
    {
        return true;
    }

    protected override void UseWeapon()
    {
        base.UseWeapon();

        Vector3 spawnPoint = GetComponentInChildren<Crossbow>().transform.position;
        GameObject gameObject = (GameObject)Instantiate(ResourceManager.GetWorldObject("Bolt"), spawnPoint, transform.rotation);
        Bolt projectile = gameObject.GetComponentInChildren<Bolt>();
        projectile.SetRange(0.9f * weaponRange);
        projectile.SetTarget(target);
    }
}