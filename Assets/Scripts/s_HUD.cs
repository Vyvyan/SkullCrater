using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class s_HUD : MonoBehaviour {

	public Texture2D texture_Crosshair;
	public float crosshairSize;

    Player playerScript;
    public GUIStyle ammoGUIStyle;

    public Text gameTimer, ammo, money, droppedMoney, killedBy, enemiesKilled, goldDepositted, endTime;
    public Image grenadeImage;
    public GameObject playingGroup, deadGroup;
    bool hasSwitchedToDeadGroup;

	// Use this for initialization
	void Start () 
	{
        // resets the text on screen when the game starts
        gameTimer.text = ammo.text = money.text = "";
        // makes sure the dead hud is disabled when we start
        deadGroup.SetActive(false);
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hasSwitchedToDeadGroup = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (GameManager.gameState == GameManager.GameState.Playing)
        {
            // display the game timer
            gameTimer.text = "Expedition Time: " + GameManager.gameTimer.ToString("F2");

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
                ammo.text = "Reloading!";
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
        endTime.text = GameManager.gameTimer.ToString("F2");
        playingGroup.SetActive(false);
        deadGroup.SetActive(true);
    }

    void DisplayPlayingHud()
    {
        playingGroup.SetActive(true);
        deadGroup.SetActive(false);
    }
}
