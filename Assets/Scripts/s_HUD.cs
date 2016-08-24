using UnityEngine;
using System.Collections;

public class s_HUD : MonoBehaviour {

	public Texture2D texture_Crosshair;
	public float crosshairSize;

    Player playerScript;
    public GUIStyle ammoGUIStyle;

	// Use this for initialization
	void Start () 
	{
        playerScript = gameObject.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		 
	}

	// GUI
	void OnGUI()
	{
		GUI.DrawTexture(new Rect((Screen.width / 2) - (crosshairSize / 2), (Screen.height / 2) - (crosshairSize / 2), 
		                         crosshairSize,crosshairSize), texture_Crosshair);

        if (!playerScript.isReloading)
        {
            if (playerScript.weapon1 == Player.WeaponType.pistol)
            {
                GUI.Label(new Rect(Screen.width * .9f, Screen.height * .95f, 100, 100), playerScript.pistolAmmo.ToString(), ammoGUIStyle);
            }
            if (playerScript.weapon1 == Player.WeaponType.shotgun)
            {
                GUI.Label(new Rect(Screen.width * .9f, Screen.height * .95f, 100, 100), playerScript.shotgunAmmo.ToString(), ammoGUIStyle);
            }
            if (playerScript.weapon1 == Player.WeaponType.machinegun)
            {
                GUI.Label(new Rect(Screen.width * .9f, Screen.height * .95f, 100, 100), playerScript.machinegunAmmo.ToString(), ammoGUIStyle);
            }
        }
        else { GUI.Label(new Rect(Screen.width * .9f, Screen.height * .95f, 100, 100), "Reloading!", ammoGUIStyle); }

        if (playerScript.hasGrenadeReady)
        {
            GUI.Label(new Rect(Screen.width * .9f, Screen.height * .85f, 100, 100), "Grenade HYPE", ammoGUIStyle);
        }

        GUI.Label(new Rect(Screen.width * .9f, Screen.height * .75f, 100, 100), "G: " + GameManager.heldGold.ToString(), ammoGUIStyle);
    }
}
