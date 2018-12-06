using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public float velocity;
    public int damage;
    
    protected WorldObject target;

    void Update()
    {
        
    }
    

    public void SetTarget(WorldObject target)
    {
        this.target = target;
    }

    protected bool HitSomething()
    {
        if (target && target.GetSelectionBounds().Contains(transform.position)) return true;
        return false;
    }

    public void InflictDamage()
    {
        if (target) target.TakeDamage(damage);
    }
}