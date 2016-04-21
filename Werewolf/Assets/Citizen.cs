using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public struct player_info{

       public string player_name;
       public string player_identity;

        public void set(string name, string iden) {
            player_name = name;
            player_identity = iden;
        }


        };

    public List<player_info> players_info = new List<player_info>();

    void Start()
    {
        modelNumber = 0;

        for (int x = 0; x < modelArray.Length; x++)
        {
            //Transform temp;
            if (x == 0)
            {
                
                //temp =
                modelArray[x] = this.gameObject.transform.Find("Werewolf").gameObject;
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
                modelArray[x] = this.gameObject.transform.Find("Seer").gameObject;
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
                modelArray[x] = this.gameObject.transform.Find("Villager").gameObject;
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

        GameObject cam = GameObject.Find("ARCamera");
        GameManager script = cam.GetComponent<GameManager>();

        GameObject plist = GameObject.Find("PlayersList");

      //  Debug.Log("Sou " + identity + "iplist count: " + plist.transform.childCount);
        for (int i = 0; i < plist.transform.childCount; i++)
        {
            GameObject p;
            player_info temp = new player_info();
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject;
            s = p.GetComponent<Citizen>();

           
            if (identity == "Werewolf")
            {
                if(s.identity == "Werewolf")
                {

                    temp.set(p.name, "Werewolf");
                    players_info.Add(temp);
                }
                else
                {
                    temp.set(p.name, "Villager");
                    players_info.Add(temp);
                }
               
                continue;
            }

            if (identity == "Seer" || identity == "Villager")
            {
                
                 temp.set(p.name, "Villager");
                players_info.Add(temp);

                continue;
            }

         
        }

        /*
        Debug.Log(" Sou " + identity);
        for(int i = 0; i < players_info.Count; i ++)
        {
            Debug.Log("Vejo " + players_info[i].player_name + " como: " + players_info[i].player_identity);

        }
        */

        //Debug.Log("Sou " + this.gameObject.name + "info count: " + players_info.Count);
    }

    public void resetInfo()
    {

        GameObject cam = GameObject.Find("ARCamera");
        GameManager script = cam.GetComponent<GameManager>();

        GameObject plist = GameObject.Find("PlayersList");

        //  Debug.Log("Sou " + identity + "iplist count: " + plist.transform.childCount);
        for (int i = 0; i < plist.transform.childCount; i++)
        {
            GameObject p;
            player_info temp = new player_info();
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject;
            s = p.GetComponent<Citizen>();


            if (identity == "Werewolf")
            {
                if (s.identity == "Werewolf")
                {

                    temp.set(p.gameObject.name, "Werewolf");
                    players_info.Add(temp);
                }
                else
                {
                    temp.set(p.gameObject.name, "Villager");
                    players_info.Add(temp);
                }

                continue;
            }

            if (identity == "Seer" || identity == "Villager")
            {

                temp.set(p.gameObject.name, "Villager");
                players_info.Add(temp);

                continue;
            }

        }
    }
   public void ModelSwitch(string which) // escreva o modelo que quer deixar ativo
    {
       

        for (int j = 0; j < this.transform.childCount; j++)
        {
            if (this.transform.GetChild(j).gameObject.name == which)
            {
                Debug.Log(this.gameObject.name + " setando " + this.transform.GetChild(j).gameObject.name + "true");
                this.transform.GetChild(j).gameObject.SetActive(true);
            }
            else
            {
                this.transform.GetChild(j).gameObject.SetActive(false);
            }
        }

        if(which == "Dead")
        {

            for (int j = 0; j < this.transform.childCount; j++)
            {
               
                    this.transform.GetChild(j).gameObject.SetActive(false);
                
            }
        }

    }

    void Update()
    {
       
    }
}
