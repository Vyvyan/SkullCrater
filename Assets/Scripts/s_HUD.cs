using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class s_HUD : MonoBehaviour {

	public Texture2D texture_Crosshair;
	public float crosshairSize;

    Player playerScript;
    public GUIStyle ammoGUIStyle;

    public Text gameTimer, ammo, money, droppedMoney, killedBy, enemiesKilled, goldDepositted, endTime, ENDGAME_enemiesKilled, ENDGAME_MissionTime, ENDGAME_golddeposited;
    public Image grenadeImage;
    public GameObject playingGroup, deadGroup, endGameGroup;
    bool hasSwitchedToDeadGroup, hasSwitchedToEndGameGroup;

    public Sprite hud_shotgun, hud_pistol, hud_machinegun, hud_rocket, hud_none;
    public Image secondaryWeaponImage;

    public GameObject moneyPickupParticles;

	// Use this for initialization
	void Start () 
	{
        // resets the text on screen when the game starts
        gameTimer.text = ammo.text = money.text = "";
        // makes sure the dead hud is disabled when we start
        deadGroup.SetActive(false);
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hasSwitchedToDeadGroup = false;
        hasSwitchedToEndGameGroup = false;

        crosshairSize = Screen.width / 160;
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (GameManager.gameState == GameManager.GameState.Playing || GameManager.gameState == GameManager.GameState.EndGame)
        {
            // display the game timer
            //gameTimer.text = "Expedition Time: " + GameManager.gameTimer.ToString("F1");
            gameTimer.text = "Expedition Time: " + FormatSeconds(GameManager.gameTimer);

            // displays the gold
            money.text = GameManager.heldGold.ToString() + "g";

            // updates the grenade radial effect
            grenadeImage.fillAmount = playerScript.GrenadeJuiceCurrent / 100;
        }
        if (GameManager.gameState != GameManager.GameState.Dead)
        {
            // display the ammo
            if (!playerScript.isReloading)
            {
                if (playerScript.weapon1 == Player.WeaponType.pistol)
                {
                    ammo.text = playerScript.pistolAmmo.ToString();
                }
                if (playerScript.weapon1 == Player.WeaponType.shotgun)
                {
                    ammo.text = playerScript.shotgunAmmo.ToString();
                }
                if (playerScript.weapon1 == Player.WeaponType.machinegun)
                {
                    ammo.text = playerScript.machinegunAmmo.ToString();
                }
                if (playerScript.weapon1 == Player.WeaponType.rocket)
                {
                    ammo.text = playerScript.rocketAmmo.ToString();
                }
            }
            else
            {
                ammo.text = "reloading";
            }

            if (playerScript.weapon2 == Player.WeaponType.pistol)
            {
                secondaryWeaponImage.sprite = hud_pistol;
            }
            else if (playerScript.weapon2 == Player.WeaponType.shotgun)
            {
                secondaryWeaponImage.sprite = hud_shotgun;
            }
            else if (playerScript.weapon2 == Player.WeaponType.machinegun)
            {
                secondaryWeaponImage.sprite = hud_machinegun;
            }
            else if (playerScript.weapon2 == Player.WeaponType.rocket)
            {
                secondaryWeaponImage.sprite = hud_rocket;
            }
            else if (playerScript.weapon2 == Player.WeaponType.none)
            {
                secondaryWeaponImage.sprite = hud_none;
            }
        }

        // switch to dead hud if we die
        if (GameManager.gameState == GameManager.GameState.Dead)
        {
            if (!hasSwitchedToDeadGroup)
            {
                SwitchToDeadHUD();
                hasSwitchedToDeadGroup = true;
            }
        }

        // switch to end game hud if we die
        if (GameManager.gameState == GameManager.GameState.EndGame)
        {
            if (!hasSwitchedToEndGameGroup)
            {
                SwitchToEndGameHUD();
                hasSwitchedToEndGameGroup = true;
            }

            // update the endgame gold text 
            ENDGAME_golddeposited.text = GameManager.thisSessionGoldGained.ToString() + "g" + " + " + GameManager.heldGold.ToString() + " Held G";
        }
    }

	// GUI
	void OnGUI()
	{
        // cross hair cause it works and I'm lazy
        if (GameManager.gameState != GameManager.GameState.Dead)
        {
            GUI.DrawTexture(new Rect((Screen.width / 2) - (crosshairSize / 2), (Screen.height / 2) - (crosshairSize / 2),
                                     crosshairSize, crosshairSize), texture_Crosshair);
        }
    }

    void SwitchToDeadHUD()
    {
        droppedMoney.text = money.text;
        killedBy.text = GameManager.enemyThatKilledPlayer;
        goldDepositted.text = GameManager.thisSessionGoldGained.ToString() + "g";
        enemiesKilled.text = GameManager.enemiesKilledThisSession.ToString();
        //endTime.text = GameManager.gameTimer.ToString("F1");
        endTime.text = FormatSeconds(GameManager.gameTimer);
        playingGroup.SetActive(false);
        deadGroup.SetActive(true);
    }

    void SwitchToEndGameHUD()
    {
        ENDGAME_golddeposited.text = GameManager.thisSessionGoldGained.ToString() + "g" + " + " + GameManager.heldGold.ToString() + " Held Gold";
        ENDGAME_enemiesKilled.text = GameManager.enemiesKilledThisSession.ToString();
        //endTime.text = GameManager.gameTimer.ToString("F1");
        ENDGAME_MissionTime.text = FormatSeconds(GameManager.gameTimer);
        playingGroup.SetActive(true);
        deadGroup.SetActive(false);
        endGameGroup.SetActive(true);
    }

    void DisplayPlayingHud()
    {
        playingGroup.SetActive(true);
        deadGroup.SetActive(false);
        endGameGroup.SetActive(false);
    }

    string FormatSeconds(float elapsed)
    {
        int d = (int)(elapsed * 100.0f);
        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;

        // WE ONLY WANT ONE DIGIT SO WE DIVIDE IT BY TEN, REMOVE THE TEN FOR 2 DIGITS
        int hundredths = (d % 100) / 10;
        if (minutes > 0)
        {
            return String.Format("{0:00}:{1:00}.{2:0}", minutes, seconds, hundredths);
        }
        else
        {
            return String.Format("{0:00}.{1:0}", seconds, hundredths);
        }
    }
}
