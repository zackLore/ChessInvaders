using UnityEngine;
using System.Collections;

public class ClearMenu : MonoBehaviour {

    public GameObject RollMenu;
	// Use this for initialization
	void Start () {
        //RollMenu = GameObject.Find("RollMenu");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("MouseClick on background.");
            if (RollMenu != null)
            {
                //RollMenu.SetActive(false);
            }
            else
            {
                Debug.Log("Roll Menu object was null.");
            }
        }
	}
}
