using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public InputField iField;
    public static int GlobalNumberOfPlayer;

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
        if(GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 + 50, 200, 50), "Start"))
        {
            GlobalNumberOfPlayer = int.Parse(iField.text);
            
            Debug.Log(GlobalNumberOfPlayer);

            Application.LoadLevel(1);
        }
    }   
}
