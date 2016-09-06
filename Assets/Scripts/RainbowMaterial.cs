using UnityEngine;
using System.Collections;

public class RainbowMaterial : MonoBehaviour {

    Renderer rend;
    public float scrollSpeed;
    float offset;

    // Use this for initialization
    void Start ()
    {
        rend = gameObject.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        offset -= scrollSpeed * Time.deltaTime;
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, -0.05f));

        if (rend.material.GetTextureOffset("_MainTex").x < -1f)
        {
            offset = -.05f;
        }
    }
}
