using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : Weapon {

    float range;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (HitSomething())
        {
            InflictDamage();
            Destroy(gameObject);
        }
        if (range > 0)
        {
            float positionChange = Time.deltaTime * velocity;
            range -= positionChange;
            transform.position += (positionChange * transform.forward);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetRange(float range)
    {
        this.range = range;
    }
}
