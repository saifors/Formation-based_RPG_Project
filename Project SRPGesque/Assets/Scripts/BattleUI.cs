﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour 
{
	private Vector2 axis;
    public enum SelectingMenu { selectingAction, selectingAttack, selectingTarget, selectingMove};
    public SelectingMenu selecting;
    
    public enum CommandSelection {Attack, Defend, Move, Item, Run};
	public CommandSelection command;

	private float scrollCooldownCounter;
	public float scrollCooldown;
	private GameManager gameManager;

	
    private Color notifTextColor;
	private Color notifBgColor;
	private float textFadeCounter;
    private float notifAlpha;
    public bool notifShown;
    public GameObject notifPanel;
    [Header("Menus")]
    public GameObject battleMenu;
    public GameObject actionMenu;
    public GameObject attackMenu;
    public GameObject partyInfo;
    
    [Header("Everything for tiles with movement")]
    
    public int tileAmount;
    public int selectedTile;
    public Vector2 tileSelection;
    private float tileSelectCooldownCounter;
    public GameObject cursor;
    private Transform selectedTileIndicator;
    [HideInInspector] public int tileCollumnSize;
    [HideInInspector] public int tileRowSize;
    [HideInInspector] public Vector2 startLimit;
    [HideInInspector] public Vector2 endLimit;

    [Header("Attack Menu")]
    public GameObject attackNames;
    public Text[] attackName;
    private Transform[] attackNamePos;
    public Vector2 atkSelVector;
    public int attackSelected;
    public Text SelectedAttackDescription;
    public Text SelectedAttackStats;

    [Header("Images behind the selections")]
	public CanvasGroup selectionImage;
	public Transform selectionImage_trans;
    public Transform attackSelection;
	public Text battleNotificationText;
    public Image battleNotificationBg;

	// Use this for initialization
	void Start () 
	{
		//To test menu stuff.
		selecting = SelectingMenu.selectingAction;
		scrollCooldown = 0.3f ;

		gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		selectionImage_trans = selectionImage.GetComponent<RectTransform>();
        notifTextColor = battleNotificationText.color;
        notifBgColor = battleNotificationBg.color;
        
        
        GameObject obj;
        obj = Instantiate(cursor);
        selectedTileIndicator = obj.GetComponent<Transform>();
        selectedTileIndicator.gameObject.SetActive(false);

        attackName = attackNames.GetComponentsInChildren<Text>();
        attackNamePos = new Transform[attackName.Length];
        for(int i = 0; i < attackNamePos.Length; i++) 
        {
            attackNamePos[i] = attackName[i].GetComponent<Transform>();
        }
        attackSelected = Mathf.FloorToInt(atkSelVector.y + (atkSelVector.x * 2));
        attackSelection.position = attackNamePos[attackSelected].position;
    
        attackMenu.SetActive(false);
        battleMenu.SetActive(false);
        

        SetAttackScroll();

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(scrollCooldownCounter <= scrollCooldown + 0.5f) scrollCooldownCounter += Time.deltaTime;
        //Selecting action Command
        if (selecting == SelectingMenu.selectingAction)
		{
			//Put in a timeCounter so it doesn't read every frame
			if(axis.y < 0 && scrollCooldownCounter >= scrollCooldown)
			{
				SelectNextCommand();
				scrollCooldownCounter = 0;
				
			}
			else if(axis.y > 0 && scrollCooldownCounter >= scrollCooldown)
			{
				SelectPreviousCommand();
				scrollCooldownCounter = 0;
			}
		}
        else if (selecting == SelectingMenu.selectingMove)
        {
            if(tileSelectCooldownCounter < 1) tileSelectCooldownCounter += Time.deltaTime;
            //Controls for moving around the selected tiles.
            if(tileSelectCooldownCounter >= 0.25f) // This section below is utter prefection don't touch
            {
                FormationMovement();
            }
        }
        else if(selecting == SelectingMenu.selectingAttack)
        {
            if(scrollCooldownCounter >= scrollCooldown)
            {
                if(axis.y > 0)
                {
                    atkSelVector.y--; 
                    if(atkSelVector.y < 0) atkSelVector.y++;               
                }
                else if(axis.y < 0)
                {
                    atkSelVector.y++; 
                    if(atkSelVector.y >=3) atkSelVector.y--;
                }
                if(axis.x > 0)
                {
                    atkSelVector.x++;
                    if(atkSelVector.x >= 2) atkSelVector.x--;
                }
                else if(axis.x < 0 )
                {
                    atkSelVector.x--;
                    if(atkSelVector.x < 0) atkSelVector.x++;
                }
                if(axis != Vector2.zero)
                {
                    attackSelected = Mathf.FloorToInt(atkSelVector.x + (atkSelVector.y * 2));
                    attackSelection.position = attackNamePos[attackSelected].position;
                    //UpdateAttackInfo(character.attack[attackSelected]); // Make it get the attackID of the characters selected attack
                    scrollCooldownCounter = 0;
                }
            }
        }

        //Notification fades after a while
		if(notifShown)
		{
			textFadeCounter += Time.deltaTime;
			if(textFadeCounter >= 3)
			{
                notifAlpha -=  Time.deltaTime;
                notifTextColor.a = notifAlpha; 
                notifBgColor.a = notifAlpha/2;
                battleNotificationText.color = notifTextColor;
                battleNotificationBg.color = notifBgColor;
                if (notifAlpha <= 0)
                {
                    notifAlpha = 0;
                    notifShown = false;
                    notifPanel.SetActive(false);
                }
            }
		}
	}

    public void SetAxis(Vector2 inputAxis)
    {
        axis = inputAxis;
    }

    public void UpdateAttackInfo(int attack)
    {
        //Todo: Change Displayed Description to selected attacks description
        /*SelectedAttackDescription.text = gameManager.attackInfo.attackDescriptions[attackSelected];

        //Change displayed Power and MP to that of the attack
        SelectedAttackStats.text = "Power: " + gameManager.attackInfo.attackStrengths[attackSelected] + System.Environment.NewLine + "MP: " + gameManager.attackInfo.attackMpCosts[attackSelected];
*/
        //Hard: Change Displayed range/AoE of attack 
    }

	public void SelectNextCommand()
	{
		if(command <= CommandSelection.Item) command++;
		else command = 0;
		switch (command)
		{
			case CommandSelection.Attack:
				SetAttackScroll();
				break;
			case CommandSelection.Defend:
				SetDefendScroll();
				break;
			case CommandSelection.Move:
				SetMoveScroll();
				break;
			case CommandSelection.Item:
				SetItemScroll();
				break;
			case CommandSelection.Run:
				SetRunScroll();
				break;
			default:
				break;
		}
		
	}

	public void SelectPreviousCommand()
	{
		if(command > 0) command--;
		else command = CommandSelection.Run;
		switch (command)
		{
			case CommandSelection.Attack:
				SetAttackScroll();
				break;
			case CommandSelection.Defend:
				SetDefendScroll();
				break;
			case CommandSelection.Move:
				SetMoveScroll();
				break;
			case CommandSelection.Item:
				SetItemScroll();
				break;
			case CommandSelection.Run:
				SetRunScroll();
				break;
			default:
				break;
		}
		
	}

	public void ConfirmSelectedCommand()
	{
		if(command == CommandSelection.Attack)
		{
			//Go to attack menu
            selecting = SelectingMenu.selectingAttack;
            atkSelVector = Vector2.zero;
            attackSelected = Mathf.FloorToInt(atkSelVector.y + (atkSelVector.x * 2));
            //UpdateAttackInfo(character.attack[attackSelected]);
            partyInfo.SetActive(false);
            attackMenu.SetActive(true);
		}
		else if(command == CommandSelection.Defend)
		{
			//Go to Select defend
		}
		else if(command == CommandSelection.Move)
		{
            //Go to move menu
            selecting = SelectingMenu.selectingMove;
            
            tileSelection = gameManager.charControl[0].tile; //0 is a placeholder for now Will be replaced with an Int indicating the active character when that's a thing            
            selectedTile = Mathf.FloorToInt(tileSelection.y + (tileSelection.x * tileRowSize));
            selectedTileIndicator.gameObject.SetActive(true);
            selectedTileIndicator.position = gameManager.tileScript.tileTransform[selectedTile].position;
            actionMenu.SetActive(false);
            //gameManager.MoveFormation(0,5);
		}
		else if(command == CommandSelection.Item)
		{
			//Go to Item Menu
		}
		else if(command == CommandSelection.Run)
		{
			//Execute Running away
			gameManager.RunFromBattle();
		}
		else 
		{
			Debug.Log("Tried to confirm an Action command selection but nothing was selected");
		}
	}

	public void ReturnToCommandSelection()
	{
        selecting = SelectingMenu.selectingAction;
        selectedTileIndicator.gameObject.SetActive(false);
        attackMenu.SetActive(false);
        partyInfo.SetActive(true);
        actionMenu.SetActive(true);
	}

	public void ChangeNotifText(string notifText)
	{
		notifShown = true;
        notifPanel.SetActive(true);
        battleNotificationText.text = notifText;
        notifTextColor.a = 1;
        notifBgColor.a = 0.5f;
        battleNotificationText.color = notifTextColor;
        battleNotificationBg.color = notifBgColor;
        textFadeCounter = 0;
        notifAlpha = 1;
        
	}
    public void FormationMovement()
    {
        if (axis.x > 0) //Right
        {
            if (axis.y > 0) //Upright
            {
                //tile - ytiles
                tileSelection.x--;

                if (tileSelection.x < startLimit.x) tileSelection.x++;

            }
            else
            if (axis.y < 0) //DownRight
            {
                //tile + 1
                //I don't even know anymore
                tileSelection.y++;

                if (tileSelection.y >= endLimit.y) tileSelection.y--;
            }
            else //Right
            {
                //tile - ytiles + 1
                tileSelection.x--;
                tileSelection.y++;
                if (tileSelection.x < startLimit.x || tileSelection.y >= endLimit.y)
                {
                    tileSelection.x++;
                    tileSelection.y--;
                }

            }
        }
        else if (axis.x < 0) //Left
        {
            if (axis.y > 0) //UpLeft
            {
                //tile - 1
                tileSelection.y--;

                if (tileSelection.y < startLimit.y) tileSelection.y++;

            }
            else if (axis.y < 0) //DownLeft
            {
                // tile + ytiles 
                tileSelection.x++;
                if (tileSelection.x >= endLimit.x) tileSelection.x--;

            }
            else //Left
            {
                //tile + ytiles - 1
                tileSelection.x++;
                tileSelection.y--;
                if (tileSelection.x >= endLimit.x || tileSelection.y < startLimit.y)
                {
                    tileSelection.x--;
                    tileSelection.y++;
                }

            }
        }
        else if (axis.y > 0) //Up
        {
            //tile - ytiles - 1
            tileSelection.x--;
            tileSelection.y--;
            if (tileSelection.x < startLimit.x || tileSelection.y < startLimit.y)
            {
                tileSelection.x++;
                tileSelection.y++;
            }

        }
        else if (axis.y < 0) //Down
        {
            //tile + ytiles + 1
            tileSelection.x++;
            tileSelection.y++;
            if (tileSelection.x >= endLimit.x || tileSelection.y >= endLimit.y)
            {
                tileSelection.x--;
                tileSelection.y--;
            }

        }

        if (axis.x != 0 || axis.y != 0)
        {
            selectedTile = Mathf.FloorToInt(tileSelection.y + (tileSelection.x * tileRowSize));
            selectedTileIndicator.position = gameManager.tileScript.tileTransform[selectedTile].position;

            tileSelectCooldownCounter = 0;
            //gameManager.MoveFormation(0, selectedTile);
        }
    }


	#region Sets

	void SetAttackScroll()
	{
		
		selectionImage_trans.localPosition = new Vector2( 0 , 138.3f );
		
		
	}
	void SetDefendScroll()
	{
		
		selectionImage_trans.localPosition = new Vector2( 0 , 65.3f );
		
	}
	void SetMoveScroll()
	{
		selectionImage_trans.localPosition = new Vector2( 0 , -7.79f );
		
		
	}
	void SetItemScroll()
	{
		
		selectionImage_trans.localPosition = new Vector2( 0 , -80.8f );
		
	}
	void SetRunScroll()
	{
		
		selectionImage_trans.localPosition = new Vector2( 0 , -153.9f );
		
	}

	#endregion
}
