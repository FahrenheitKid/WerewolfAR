using UnityEngine;
using System.Collections;

public class EndManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnGUI()
    {
        GUI.skin.label.fontSize = 40;
        //GUI.skin.label.alignment = TextAnchor.UpperCenter;
        GUI.Label(new Rect(Screen.width /2 - 450, 300, 900, 80), GameManager.whoWon);

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50, 200, 50), "Restart"))
        {
            Application.LoadLevel(1);
        }
    }
}
