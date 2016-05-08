using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

    public Vector2 speed = Vector2.zero;
    public Vector2 offset = Vector2.zero;

    public Material material;
    
	// Use this for initialization
	void Start () {
        material = GetComponent<Renderer>().material;
        offset = material.GetTextureOffset("_MainTex");
	}
	
	// Update is called once per frame
	void Update () {
        offset += speed * Time.deltaTime;

        material.SetTextureOffset("_MainTex", offset);
	}
}
