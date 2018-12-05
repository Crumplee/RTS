﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using RTS;

public class Unit : WorldObject
{
    public NavMeshAgent agent;

    [SerializeField]
    protected bool moving;

    [SerializeField]
    private Vector3 destination;

    //attack
    private Quaternion aimRotation;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        //player.ModifycurrentPopulation(populationCost);
    }

    protected override void Update()
    {
        base.Update();
        
        if (moving)
            Move();
        else if (aiming)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, aimRotation, weaponAimSpeed);
            CalculateBounds();
            //sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
            Quaternion inverseAimRotation = new Quaternion(-aimRotation.x, -aimRotation.y, -aimRotation.z, -aimRotation.w);
            if (transform.rotation == aimRotation || transform.rotation == inverseAimRotation)
            {
                aiming = false;
            }
        }
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }

    public override void DestroyObject()
    {
        base.DestroyObject();
        player.ModifycurrentPopulation(-populationCost);
        Destroy(gameObject);
    }

    public virtual void Init()
    {
        
    }


    public override void SetHoverState(GameObject hoverObject)
    {
        base.SetHoverState(hoverObject);

        if (player && currentlySelected)
        {
            bool moveHover = false;
            if (hoverObject.name == "Ground")
            {
                moveHover = true;
            }
            else
            {
                Resource resource = hoverObject.transform.parent.GetComponent<Resource>();
                if (resource && resource.isEmpty()) moveHover = true;
            }
            if (moveHover) player.hud.SetCursorState(CursorState.Move);
        }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        base.MouseClick(hitObject, hitPoint, controller);
        
        if (!this.GetComponentInParent<Player>().IsLocalPlayer())
        {
            return;
        }

        if (player && currentlySelected)
        {
            bool clickedOnEmptyResource = false;
            if (hitObject.transform.parent)
            {
                Resource resource = hitObject.transform.parent.GetComponent<Resource>();
                if (resource && resource.isEmpty()) clickedOnEmptyResource = true;
            }
            if ((hitObject.name == "Ground" || clickedOnEmptyResource) && hitPoint != ResourceManager.InvalidPosition)
            {
                float x = hitPoint.x;
                float y = hitPoint.y + player.SelectedObject.transform.position.y;
                float z = hitPoint.z;
                Vector3 destination = new Vector3(x, y, z);
                player.CmdStopAttack(this.gameObject.GetComponent<NetworkIdentity>().netId);

                player.CmdStartMove(destination, this.gameObject.GetComponent<NetworkIdentity>().netId);
            }
        }
    }
    

    public virtual void StartMove(Vector3 destination)
    {
        this.destination = destination;

        gameObject.GetComponent<NavMeshObstacle>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
        agent.SetDestination(destination);
        moving = true;
    }

    private void Move()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    moving = false;
                    gameObject.GetComponent<NavMeshAgent>().enabled = false;
                    gameObject.GetComponent<NavMeshObstacle>().enabled = true;
                    movingIntoPosition = false; // for attack
                }
            }
        }
        CalculateBounds();
    }

    protected bool DestinationTargetInRange(WorldObject target)
    {
        Vector3 targetLocation = target.transform.position;
        Vector3 direction = targetLocation - transform.position;
        if (direction.sqrMagnitude < 100)
        {
            return true;
        }
        return false;
    }
   

    //attack
    protected override void AimAtTarget()
    {
        base.AimAtTarget();
        aimRotation = Quaternion.LookRotation(target.transform.position - transform.position);
    }

}
