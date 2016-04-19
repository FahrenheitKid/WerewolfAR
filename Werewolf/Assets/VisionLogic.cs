using UnityEngine;
using System.Collections;

public class VisionLogic : MonoBehaviour
{


    // Use this for initialization
    public GameObject[] modelArray = new GameObject[3];
    private int modelNumber;
    public string identity = "Werewolf";

    void Start()
    {
        modelNumber = 0;

        for (int x = 0; x < modelArray.Length; x++)
        {
            if (x == 0)
            {
                modelArray[x] = GameObject.Find("Werewolf");
                if (identity == "Werewolf")
                    modelArray[x].SetActive(true);
                else
                    modelArray[x].SetActive(false);
            }

            if (x == 1)
            {
                modelArray[x] = GameObject.Find("Seer");
                if (identity == "Seer")
                    modelArray[x].SetActive(true);
                else
                    modelArray[x].SetActive(false);
            }

            if (x == 2)
            {
                modelArray[x] = GameObject.Find("Villager");
                if (identity == "Villager")
                    modelArray[x].SetActive(true);
                else
                    modelArray[x].SetActive(false); ;
            }


        }
        ModelSwitch();

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
