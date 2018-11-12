using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPoint : MonoBehaviour {
    

	// Use this for initialization
	public virtual void Start ()
    {

	}

    public Vector3 GetPoint()
    {
        return this.transform.position;
    }
}
