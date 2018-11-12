using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPoint : MonoBehaviour {

    Vector3 point;

	// Use this for initialization
	public virtual void Start ()
    {
        point = this.transform.position;
	}
}
