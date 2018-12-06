using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RTS;

public class WorldObject : NetworkBehaviour
{

    protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

    public string objectName;
    public Texture2D objectImage;
    public int hitPoints, maxHitPoints;

    [SerializeField]
    protected Player player;
    protected string[] actions = { };
    protected bool currentlySelected = false;

    public int foodCost, woodCost, goldCost, populationCost;

    protected Bounds selectionBounds;

    // health display
    protected GUIStyle healthStyle = new GUIStyle();
    protected float healthPercentage = 1.0f;


    private List<Material> oldMaterials = new List<Material>();

    //attack
    protected WorldObject target = null;
    [SerializeField]
    protected bool attacking = false;
    public float weaponRange = 1.0f;
    protected bool movingIntoPosition = false;
    protected bool aiming = false;
    public float weaponAimSpeed = 8.0f;
    public float weaponRechargeTime = 2.0f;
    protected float currentWeaponChargeTime;

    public float objectRange = 0.0f;

    protected virtual void Awake()
    {
        selectionBounds = ResourceManager.InvalidBounds;
        CalculateBounds();
    }

    protected virtual void Start()
    {
        SetPlayer();
        if (player) SetTeamColor(); 
    }

    public void SetPlayer()
    {
        player = transform.root.GetComponentInChildren<Player>();
    }

    public Player GetPlayer()
    {
        return player;
    }

    protected virtual void Update()
    {
        currentWeaponChargeTime += Time.deltaTime;
        
    }

    protected virtual void OnGUI()
    {
        if (currentlySelected) DrawSelection();
    }

    public virtual void SetSelection(bool selected, Rect playingArea)
    {
        currentlySelected = selected;
        if (selected) this.playingArea = playingArea;
    }

    public string[] GetActions()
    {
        return actions;
    }

    public virtual void PerformAction(string actionToPerform)
    {

    }

    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller)
    {
        if (currentlySelected && hitObject && hitObject.name != "Ground")
        {
            WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();

            //clicked on another selectable object
            if (worldObject)
            {
                Resource resource = hitObject.transform.parent.GetComponent<Resource>();
                if (resource && resource.isEmpty()) return;
                
                Player owner = hitObject.transform.root.GetComponent<Player>();
                if (owner && this.GetComponentInParent<Player>().IsLocalPlayer())
                {
                    if (player.username != owner.username && CanAttack())
                        player.CmdBeginAttack(worldObject.GetComponent<NetworkIdentity>().netId, this.gameObject.GetComponent<NetworkIdentity>().netId);
                    else ChangeSelectedObject(worldObject, controller);
                }
                else ChangeSelectedObject(worldObject, controller);
            }
        }
    }

    private void ChangeSelectedObject(WorldObject worldObject, Player controller)
    {
        SetSelection(false, playingArea);
        if (controller.SelectedObject)
            controller.SelectedObject.SetSelection(false, playingArea);
        controller.SelectedObject = worldObject;
        worldObject.SetSelection(true, controller.hud.GetPlayingArea());
    }

    private void DrawSelection()
    {
        GUI.skin = ResourceManager.SelectBoxSkin;
        Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);

        GUI.BeginGroup(playingArea);
        DrawSelectionBox(selectBox);
        GUI.EndGroup();
    }

    public void CalculateBounds()
    {
        selectionBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            selectionBounds.Encapsulate(r.bounds);
        }
    }

    // for display health


    protected virtual void DrawSelectionBox(Rect selectBox)
    {
        GUI.Box(selectBox, "");
        CalculateCurrentHealth(0.3f, 0.7f);
        DrawHealthBar(selectBox, "");
    }

    // calculates the selected object health
    protected virtual void CalculateCurrentHealth(float lowSplit, float highSplit)
    {
        healthPercentage = (float)hitPoints / (float)maxHitPoints;
        if (healthPercentage > highSplit) healthStyle.normal.background = ResourceManager.HealthyTexture;
        else if (healthPercentage > lowSplit) healthStyle.normal.background = ResourceManager.DamagedTexture;
        else healthStyle.normal.background = ResourceManager.CriticalTexture;
    }

    protected void DrawHealthBar(Rect selectBox, string label)
    {
        healthStyle.padding.top = -20;
        healthStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(selectBox.x, selectBox.y - 7, selectBox.width * healthPercentage, 5), label, healthStyle);
    }

    public bool IsOwnedBy(Player owner)
    {
        if (player && player.Equals(owner))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void SetHoverState(GameObject hoverObject)
    {
        //only handle input if owned by a human player and currently selected
        if (player && currentlySelected)
        {
            //something other than the ground is being hovered over
            if (hoverObject.name != "Ground")
            {
                Player owner = hoverObject.transform.root.GetComponent<Player>();
                Unit unit = hoverObject.transform.parent.GetComponent<Unit>();
                Building building = hoverObject.transform.parent.GetComponent<Building>();
                if (owner)
                { //the object is owned by a player
                    if (owner.username == player.username) player.hud.SetCursorState(CursorState.Select);
                    else if (CanAttack()) player.hud.SetCursorState(CursorState.Attack);
                    else player.hud.SetCursorState(CursorState.Select);
                }
                else if (unit || building && CanAttack()) player.hud.SetCursorState(CursorState.Attack);
                else player.hud.SetCursorState(CursorState.Select);
            }
        }
    }

    public Bounds GetSelectionBounds()
    {
        return selectionBounds;
    }

    //building 

    public virtual void SetBuilding(Building project)
    {

    }

    public void SetColliders(bool enabled)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders) collider.enabled = enabled;
    }

    public void SetTransparentMaterial(Material material, bool storeExistingMaterial)
    {
        if (storeExistingMaterial) oldMaterials.Clear();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (storeExistingMaterial) oldMaterials.Add(renderer.material);
            renderer.material = material;
        }
    }

    public void RestoreMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (oldMaterials.Count == renderers.Length)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material = oldMaterials[i];
            }
        }
    }

    public void SetPlayingArea(Rect playingArea)
    {
        this.playingArea = playingArea;
    }

    //attack

    public virtual bool CanAttack()
    {
        return false;
    }

    protected void SetTeamColor()
    {
        TeamColor[] teamColors = GetComponentsInChildren<TeamColor>();
        foreach (TeamColor teamColor in teamColors) teamColor.GetComponent<Renderer>().material.color = player.teamColor;
    }

    protected virtual void AimAtTarget()
    {
        //this needs to be specified
        aiming = true;
    }

    protected virtual void UseWeapon()
    {
        //this needs to be specified
        currentWeaponChargeTime = 0.0f;
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0) DestroyObject();
    }

    public virtual void DestroyObject()
    {

    }

    public Dictionary<ResourceType, int> GetCosts()
    {
        Dictionary<ResourceType, int> costList = new Dictionary<ResourceType, int>();
        costList.Add(ResourceType.Food, foodCost);
        costList.Add(ResourceType.Wood, woodCost);
        costList.Add(ResourceType.Gold, goldCost);
        costList.Add(ResourceType.Population, populationCost);

        return costList;
    }
}
