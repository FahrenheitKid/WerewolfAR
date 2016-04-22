
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;


public class GameManager : MonoBehaviour
{

    public static void Swap(IList<int> list, int indexA, int indexB)
    {
        int tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }
    /*
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    */

        public struct counts
    {

        public int villagers;
        public int werewolfs;
        public int seer; // seleciona um jogador e descobre sua identidade
        public int hunter; // quando linchado, o jogador que ele votou morre também
        public int shaman; // pode descobrir a identidade de um jogador morto
        public int cupid; // seleciona dois players para serem "amantes" quando um morre, o outro morre junto
        public int little_girl; // no começo do jogo, sabe a identidade de todos os werewolfs
        public int witch;

       

        public  void init(int vil, int wer)
        {

            villagers = 0;
            werewolfs = 0;
            seer = 0;
            hunter = 0;
            shaman = 0;
            cupid = 0;
            little_girl = 0;
            witch = 0;
                
        }

        public int getVillagers()
        {
            return 3;

        }


    }

    public float n_players1;
    public float n_werewolfs1; // um terço do numero de players
    public int n_villagers;
    public int n_players;
    public int n_players_alive;
    public int n_werewolfs; // um terço do numero de players
    public counts qts;
    Citizen script;

    public string whosturn;
    public bool seer = false; // seleciona um jogador e descobre sua identidade
    public bool hunter = false; // quando linchado, o jogador que ele votou morre também
    public bool shaman = false; // pode descobrir a identidade de um jogador morto
    public bool cupid = false; // seleciona dois players para serem "amantes" quando um morre, o outro morre junto
    public bool little_girl = false; // no começo do jogo, sabe a identidade de todos os werewolfs
    public bool witch = false;


    public bool night = true;
    public int night_count = 0;
    public int player_turn = 0;
    public bool playerOk = true;
    
    List<GameObject> players = new List<GameObject>();

    // Use this for initialization
    void Start()
    {

        // script = GetComponent<VisionLogic>();

        n_players1 = 6;
        n_werewolfs1 = n_players1 / 3;
        Mathf.Floor(n_werewolfs1);
        n_werewolfs = (int)n_werewolfs1;
        n_players = (int)n_players1;
        n_villagers = n_players - n_werewolfs;
        seer = true;

        initGame();
        initTargets();
        // startNight();

        startTurn(player_turn);

    }

    // Update is called once per frame
    void Update()
    {
       // night = false;
        int teamV = 0;
        int teamW = 0;
        
        for (int i = 0; i < players.Count; i++)
        {
            script = players[i].GetComponent<Citizen>();
            if(script.team == "Villagers")
            {
                teamV++;
                continue;
            }

            if (script.team == "Werewolves")
            {
                teamV++;
                continue;
            }

         
        }

        if(teamW == 0)
        {
            // sem werewolves, a vila venceu!
        }
        else
        {
            if(teamV < teamW)
            {
                // werewolves outnumbered villagers, werewolves vecem!
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                GameObject p;
                Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                //s.resetInfo();

                //s.ModelSwitch("Villager");
            }

            playerOk = false;

            if (player_turn < n_players_alive)
            {
                player_turn++;
            }
            else
            {
                player_turn = 0;
            }
            startTurn(player_turn);
            


        }

    }

    void startTurn(int trn)
    {



        if (playerOk == true)
        {
            Debug.Log("ERREI turn");
        }
        else
        {



            //Debug.Log("Entrei turn");
            playerOk = false;

            if (night_count == 0)
            {

                GameObject plistt = GameObject.Find("PlayersList"); // lista de (parents) player
                for (int i = 0; i < plistt.transform.childCount; i++)
                {
                    GameObject p;
                    Citizen.player_info temp = new Citizen.player_info();
                    Citizen s;
                    p = plistt.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                    s = p.GetComponent<Citizen>(); // player X's script
                    s.resetInfo();

                }

            }

            bool keepGo = false;



            //night = true;
            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                GameObject p;
                Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script

                // Debug.Log(p.gameObject.name + s.identity + " info count: " + s.players_info.Count);

                if (!s.alive) continue;


                

                Debug.Log(p.name + "|" + s.identity);

                string nomee = "Player " + (player_turn + 1);
                if (p.name == nomee)
                {

                    Debug.Log("Entrei turn dentrao");
                    whosturn = p.name + " Turn";

                    for (int j = 0; j < plist.transform.childCount; j++)
                    {
                        if (i == j) continue;

                        GameObject pl;
                        pl = plist.transform.GetChild(j).gameObject.transform.GetChild(0).gameObject; // current player Y
                        Citizen sl;
                        sl = pl.GetComponent<Citizen>(); // player y's script

                        if (pl.name == p.name) continue;


                        // Debug.Log(" b4 " + p.gameObject.name + " info count" + s.players_info.Count);
                        for (int z = 0; z < s.players_info.Count; z++)
                        {
                            Debug.Log(p.gameObject.name + " info " + z + ": Nome= " + s.players_info[z].player_name + " id=" + s.players_info[z].player_identity);

                        }

                        for (int k = 0; k < s.players_info.Count; k++)
                        {
                            //Debug.Log(" sao iguais? " + s.players_info[k].player_name + "|" + pl.gameObject.name);
                            if (s.players_info[k].player_name == pl.gameObject.name)
                            {

                                string test = s.players_info[k].player_identity;
                                Debug.Log(pl.gameObject.name + " deveria parecer um: " + s.players_info[k].player_identity);


                                sl.ModelSwitch(test);
                                //  sl.canchange = false;
                            }
                        }


                    }



                }






            }


        }
    

}

    void startNight()
    {
        int turn = 1;
        if (night_count == 0)
        {

            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                GameObject p;
                Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                s.resetInfo();

            }

         }

        bool keepGo = false;
        
        if (night == false) // ja ta acontecendo
        {

            Debug.Log("NÃO entrei noite");
            return;
        }
        else
        {
            
            night = true;
            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                GameObject p;
                Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script

               // Debug.Log(p.gameObject.name + s.identity + " info count: " + s.players_info.Count);

                if (!s.alive) continue;

                whosturn = p.name + " Turn";

                Debug.Log(whosturn + "|" + s.identity);
                if (turn == 1)
                {

                    
                    for (int j = 0; j < plist.transform.childCount; j++)
                    {
                        if (i == j) continue;

                        GameObject pl;
                        pl = plist.transform.GetChild(j).gameObject.transform.GetChild(0).gameObject; // current player Y
                        Citizen sl;
                        sl = pl.GetComponent<Citizen>(); // player y's script

                        if (pl.name == p.name) continue;


                        // Debug.Log(" b4 " + p.gameObject.name + " info count" + s.players_info.Count);
                        for(int z = 0; z  < s.players_info.Count; z ++)
                        {
                            Debug.Log( p.gameObject.name + " info " + z +": Nome= " + s.players_info[z].player_name + " id=" + s.players_info[z].player_identity );

                        }
                       
                        for (int k = 0; k < s.players_info.Count; k++)
                        {
                            //Debug.Log(" sao iguais? " + s.players_info[k].player_name + "|" + pl.gameObject.name);
                            if (s.players_info[k].player_name == pl.gameObject.name)
                            {

                                string test = s.players_info[k].player_identity;
                                Debug.Log(pl.gameObject.name + " deveria parecer um: " + s.players_info[k].player_identity);

                               
                                sl.ModelSwitch(test);
                                sl.canchange = false;
                            }
                        }


                    }

                    turn = 2;

                }


                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //ModelSwitch("Villager");

                    turn++;
                }



            }

        }
        night_count++;
    }


    void startDay()
    {


    }
    void initGame()
    {

        n_players_alive = n_players;

        qts.init(n_villagers, n_werewolfs);
        n_villagers = qts.getVillagers();

        List<int> listap = new List<int>();


        for (int i = 0; i < n_players; i++)
        {
            listap.Add(i);
        }

        // first shuffle
        for (int k = 0; k < listap.Count; k++)
        {
            int r = k + Random.Range(1,100) % (listap.Count - k); // careful here!


            //listap.Swap(elements[k], elements[r]);
            //Swap.(listap, k, r);

            int temp = listap[k];
            listap[k] = listap[r];
            listap[r] = temp;
        }

        

            for (int i = 0; i < listap.Count; i++)
        {
            GameObject go = Instantiate(Resources.Load("Player")) as GameObject;
            go.name = "Player " + (listap[i] + 1);
            script = go.GetComponent<Citizen>();

           // Debug.Log("qtd were= " + qts.werewolfs + " | n_were = " + n_werewolfs);
            if (qts.werewolfs < n_werewolfs)
            {

                
                script.identity = "Werewolf";
                
                players.Add(go);
               // Debug.Log("entrei lobo");
                qts.werewolfs++;
                continue;

            }

            if(qts.villagers < n_villagers)
            {
                script.identity = "Villager";
                for(int j = 0; j < go.transform.childCount; j++)
                {
                    if(go.transform.GetChild(j).gameObject.name == script.identity)
                    {
                       // Debug.Log("Entrei no player right");
                        go.transform.GetChild(j).gameObject.SetActive(true);
                    }
                    else
                    {
                        go.transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
                players.Add(go);
               // Debug.Log("entrei vila");
                qts.villagers++;
                continue;
            }

            if(seer == true && qts.seer < 1)
            {
                script.identity = "Seer";
                players.Add(go);
               // Debug.Log("entrei seer");
                qts.seer++;
                continue;
            }

        }

    }

    void initTargets()
    {

        {

            if (DataSet.Exists("WerewolfAR"))
            {
                Debug.Log("dataset werewlf exists ");
            }
            else Debug.Log("dataset werewlf dont exist ");

            IEnumerable<TrackableBehaviour> trackableBehaviours = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
            Debug.Log("number of trackables: " + System.Linq.Enumerable.Count(trackableBehaviours));

            int i = 0;
            // Loop over all TrackableBehaviours.
            foreach (TrackableBehaviour trackableBehaviour in trackableBehaviours)
            {
                string name = trackableBehaviour.TrackableName;
                Debug.Log("loading Trackable: " + name);


                if (i < players.Count)
                {

                    trackableBehaviour.gameObject.transform.name = "ImageTarget " + (i + 1);
                    
                    trackableBehaviour.gameObject.transform.SetParent(GameObject.Find("PlayersList").transform);
                    trackableBehaviour.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    for (int j = 0; j < players.Count; j++)
                    {

                        //  Debug.Log("<color=yellow>player name: " + players[i].name + "</color>");
                        //Debug.Log("<color=blue>player da vez: Player " + (j + 1)  + "</color>");
                        //Debug.Log("Trackable name: " + name);
                        string nametemp = "Player " + (i + 1);
                        if (players[j].name == nametemp)
                        {
                            Debug.Log("<color=yellow> criando player: " + (i +1) + "</color>");
                            players[j].transform.SetParent(trackableBehaviour.gameObject.transform);

                        }


                    }

                    //trackableBehaviour.gameObject.transform.parent = players[i].transform;
                    /*
                    // chips target detected for the first time
                    // augmentation object has not yet been created for this target
                    // let's create it
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // attach cube under target
                    cube.transform.parent = trackableBehaviour.transform;

                    // Add a Trackable event handler to the Trackable.
                    // This Behaviour handles Trackable lost/found callbacks.
                    trackableBehaviour.gameObject.AddComponent<DefaultTrackableEventHandler>();

                    // set local transformation (i.e. relative to the parent target)
                    cube.transform.localPosition = new Vector3(0, 0.2f, 0);
                    cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    cube.transform.localRotation = Quaternion.identity;
                    cube.gameObject.SetActive(true);

                    // mChipsObjectCreated = true;
                    */
                }
                i++;

            }
        }
    }


    void OnGUI()
    {
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        GUI.Label(rect, whosturn);
    }
}

