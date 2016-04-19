using UnityEngine;
using System.Collections;

public class Citizen : MonoBehaviour {

    // Use this for initialization
    public GameObject[] modelArray = new GameObject[3];
    private int modelNumber;
    public string identity = "";
    public string team = "";
    public bool alive = true;
    public bool night_action = false;
    public int night_action_cooldown = 0;
    public bool doomed = false; // marcado para morrer

    void Start()
    {
        modelNumber = 0;

        for (int x = 0; x < modelArray.Length; x++)
        {
            if (x == 0)
            {
                modelArray[x] = GameObject.Find("Werewolf");
                if (identity == "Werewolf")
                {
                    team = "Werewolves";
                    modelArray[x].SetActive(true);
                }
                else
                    modelArray[x].SetActive(false);
            }

            if (x == 1)
            {
                modelArray[x] = GameObject.Find("Seer");
                if (identity == "Seer")
                {
                    team = "Villagers";
                modelArray[x].SetActive(true);
            }
            else
                modelArray[x].SetActive(false);
            }

            if (x == 2)
            {
                modelArray[x] = GameObject.Find("Villager");
                if (identity == "Villager")
                {
                    team = "Villagers";
                    modelArray[x].SetActive(true);
                }
                else
                    modelArray[x].SetActive(false); ;
            }


        }
        //ModelSwitch();

        /*
        for (int x = 0; x < modelArray.Length; x++)
        {
            if (modelArray[x].name == "Seer" && identity == "Seer")
            {
                modelArray[x].SetActive(true);
            }
            else { 
            modelArray[x].SetActive(false);
        }

            if (modelArray[x].name == "Villager" && identity == "Villager")
            {
                modelArray[x].SetActive(true);
            }
            else {
                modelArray[x].SetActive(false);
            }

            if (modelArray[x].name == "Werewolf" && identity == "Werewolf")
            {
                modelArray[x].SetActive(true);
            }
            else {
                modelArray[x].SetActive(false);
            }


        } */

    }

    void ModelSwitch()
    {
        for (int x = 0; x < modelArray.Length; x++)
        {
            if (x == modelNumber)
            {
                modelArray[x].SetActive(true);
            }
            else
            {
                modelArray[x].SetActive(false);
            }
        }
        modelNumber += 1;
        if (modelNumber > modelArray.Length - 1)
        {
            modelNumber = 0;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ModelSwitch();
        }
    }
}
