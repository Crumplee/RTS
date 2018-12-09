using UnityEngine;
using RTS;

public class Swordsman : Unit
{
    protected override void Start()
    {
        base.Start();
        actions = new string[] { "Attack" };
    }

    protected override void Update()
    {
        base.Update();
        if (attacking && !movingIntoPosition && !aiming) AnimateAttack();
    }

    private void AnimateAttack()
    {
        int frame = (int)Time.time % 2;
        GameObject att1 = null;
        GameObject att2 = null;
        foreach (Transform child in transform)
        {
            if (child.name == "Attack1") att1 = child.gameObject;
            if (child.name == "Attack2") att2 = child.gameObject;
        }
        if (frame == 0)
        {
            att1.SetActive(true);
            att2.SetActive(false);
        }
        else
        {
            att2.SetActive(true);
            att1.SetActive(false);
        }
            
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
        Weapon sword = this.GetComponentInChildren<Weapon>();
        sword.SetTarget(target);
        sword.InflictDamage();
    }
}
