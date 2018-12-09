using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class UserInput : MonoBehaviour
{

    private Player player;
    
    void Start()
    {
        player = transform.root.GetComponent<Player>();
    }
    
    void Update()
    {
        if (player.IsLocalPlayer())
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OpenPauseMenu();
            MoveCamera();
            RotateCamera();
            MouseActivity();
        }
    }

    private void OpenPauseMenu()
    {
        GetComponentInChildren<PauseMenu>().enabled = true;
        GetComponent<UserInput>().enabled = false;
        Cursor.visible = true;
        ResourceManager.MenuOpen = true;
    }

    private void MoveCamera()
    {
        float positionX = Input.mousePosition.x;
        float positionY = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);
        bool mouseScroll = false;

        //horizontal
        if (positionX >= 0 && positionX < ResourceManager.ScrollWidth)
        {
            movement.x -= ResourceManager.ScrollSpeed;
            player.hud.SetCursorState(CursorState.PanLeft);
            mouseScroll = true;
        }
        else if (positionX <= Screen.width && positionX > Screen.width - ResourceManager.ScrollWidth)
        {
            movement.x += ResourceManager.ScrollSpeed;
            player.hud.SetCursorState(CursorState.PanRight);
            mouseScroll = true;
        }

        //vertical
        if (positionY >= 0 && positionY < ResourceManager.ScrollWidth)
        {
            movement.z -= ResourceManager.ScrollSpeed;
            player.hud.SetCursorState(CursorState.PanDown);
            mouseScroll = true;
        }
        else if (positionY <= Screen.height && positionY > Screen.height - ResourceManager.ScrollWidth)
        {
            movement.z += ResourceManager.ScrollSpeed;
            player.hud.SetCursorState(CursorState.PanUp);
            mouseScroll = true;
        }
        
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        
        movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        //calculate camera position
        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        //limit min max camera height
        if (destination.y > ResourceManager.MaxCameraHeight)
        {
            destination.y = ResourceManager.MaxCameraHeight;
        }
        else if (destination.y < ResourceManager.MinCameraHeight)
        {
            destination.y = ResourceManager.MinCameraHeight;
        }

        //update camera
        if (destination != origin)
        {
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
        }

        if (!mouseScroll)
        {
            player.hud.SetCursorState(CursorState.Select);
        }

        if (Input.GetKeyDown("space"))
        {
            if (player.isLocalPlayer) Camera.main.transform.position = new Vector3(player.transform.position.x+30, Camera.main.transform.position.y, player.transform.position.z-20);
        }
    }

    private void RotateCamera()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        //alt + right mouse
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(1))
        {
            destination.x -= Input.GetAxis("Mouse Y") * ResourceManager.RotateAmount;
            destination.y += Input.GetAxis("Mouse X") * ResourceManager.RotateAmount;
        }

        //update camera
        if (destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.RotateSpeed);
        }
    }

    private void MouseActivity()
    {
        if (Input.GetMouseButtonDown(0)) LeftMouseClick();
        else if (Input.GetMouseButtonDown(1)) RightMouseClick();
        MouseHover();
    }

    private void LeftMouseClick()
    {
        if (player.hud.MouseInBounds())
        {
            if (player.IsFindingBuildingLocation())
            {
                if (player.CanPlaceBuilding()) player.ConfirmConstruction();
            }
            else
            {
                GameObject hitObject = WorkManager.GetHitObject(Input.mousePosition);
                Vector3 hitPoint = WorkManager.GetHitPoint(Input.mousePosition);

                if (hitObject && hitPoint != ResourceManager.InvalidPosition)
                {
                    if (player.SelectedObject) player.SelectedObject.MouseClick(hitObject, hitPoint, player);
                    else if (hitObject.name != "Ground")
                    {
                        WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
                        if (worldObject)
                        {
                            //set that object to the player
                            player.SelectedObject = worldObject;
                            worldObject.SetSelection(true, player.hud.GetPlayingArea());
                        }
                    }
                }
            }

        }
    }

    private void RightMouseClick()
    {
        if (player.hud.MouseInBounds() && !Input.GetKey(KeyCode.LeftAlt) && player.SelectedObject)
        {
            if (player.IsFindingBuildingLocation())
            {
                player.CancelBuildingPlacement();
            }
            else
            {
                player.SelectedObject.SetSelection(false, player.hud.GetPlayingArea());
                player.SelectedObject = null;
            }
        }
    }

    
    private void MouseHover()
    {
        if (player.hud.MouseInBounds())
        {
            if (player.IsFindingBuildingLocation())
            {
                player.FindBuildingLocation();
            }
            else
            {
                GameObject hoverObject = WorkManager.GetHitObject(Input.mousePosition);
                if (hoverObject)
                {
                    if (player.SelectedObject) player.SelectedObject.SetHoverState(hoverObject);
                    else if (hoverObject.name != "Ground")
                    {
                        Player owner = hoverObject.transform.root.GetComponent<Player>();
                        if (owner)
                        {
                            Unit unit = hoverObject.transform.parent.GetComponent<Unit>();
                            Building building = hoverObject.transform.parent.GetComponent<Building>();
                            if (owner.username == player.username && (unit || building)) player.hud.SetCursorState(CursorState.Select);
                        }
                    }
                }
            }
        }
    }


}
