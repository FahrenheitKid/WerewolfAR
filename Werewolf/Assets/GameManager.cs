
using UnityEngine;
using UnityEngine.UI;
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

        public void init(int vil, int wer)
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
    Dropdown dropmenu; // referencia dropdown

    public string whosturn;
    public bool seer = false; // seleciona um jogador e descobre sua identidade
    public bool hunter = false; // quando linchado, o jogador que ele votou morre também
    public bool shaman = false; // pode descobrir a identidade de um jogador morto
    public bool cupid = false; // seleciona dois players para serem "amantes" quando um morre, o outro morre junto
    public bool little_girl = false; // no começo do jogo, sabe a identidade de todos os werewolfs
    public bool witch = false;

    public bool night = true;
    public bool day = false;
    public int day_count = 0;
    public int night_count = 0;
    public int player_turn = 0;
    public bool playerOk = true;
    public string whoWasKilledLastNight = "";
    public string whoWasLynched = "";

    List<GameObject> players = new List<GameObject>();
    List<Dropdown.OptionData> playlist = new List<Dropdown.OptionData>(); // lista do dropdown
    public GameObject currentPlayerTargetAtNight = new GameObject();
    public GameObject currentPlayerAtNight = new GameObject();

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
            if (script.team == "Villagers" && script.alive == true)
            {
                teamV++;
                continue;
            }

            if (script.team == "Werewolves" && script.alive == true)
            {
                teamW++;
                continue;
            }
        }

        if (teamW == 0)
        {
            // sem werewolves, a vila venceu!
            Debug.Log("VILLAGERS WIN!");
        }
        else if (teamV < teamW)
        {
            // werewolves outnumbered villagers, werewolves vecem!
            Debug.Log("WEREWOLVES WIN!");
        }

        int tempi = 0;
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

            if (s.alive == true) tempi++;

        }

        n_players_alive = tempi;
    }

    public void nextPlayer()
    {
        playerOk = false;

        if (night == true)
        {
            // antes do proximo turno, efetivar as ações do player atual
            switch (currentPlayerAtNight.GetComponent<Citizen>().identity)
            {
                case "Werewolf":

                    currentPlayerTargetAtNight.GetComponent<Citizen>().votes_werewolf++;
                    break;

                case "Seer":

                    string trueident;

                    trueident = currentPlayerTargetAtNight.GetComponent<Citizen>().identity;
                    // mostrar na tela trueident

                    currentPlayerTargetAtNight.GetComponent<Citizen>().ModelSwitch(trueident);

                    Debug.Log("Seer descobriu que" + currentPlayerTargetAtNight.name + " eh um" + currentPlayerTargetAtNight.GetComponent<Citizen>().identity);
                    // setar timer pro seer conseguir ver quem ele é dps retornar o modelo

                    currentPlayerTargetAtNight.GetComponent<Citizen>().ModelSwitch("Villager");

                    break;

                case "Villager":

                    break;
            }
        }

        if (day == true)
        {

            // votacao do dia incrementa aqui
            // antes do proximo turno, efetivar as ações do player atual
            switch (currentPlayerAtNight.GetComponent<Citizen>().identity)
            {
                default:

                    currentPlayerTargetAtNight.GetComponent<Citizen>().votes_people++;
                    break;
            }
        }

        if (player_turn < n_players_alive - 1)
        {

            Debug.Log("Player turno " + (player_turn + 1) + " apertou enter |" + n_players_alive);
            player_turn++;

            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player

            // for(int j = 0; j < playlist.Count; j++)
            //  {

            for (int i = 0; i < plist.transform.childCount; i++)
            {
                Dropdown.OptionData opcao1 = new Dropdown.OptionData();
                GameObject p;
                // Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                                               //s.resetInfo();
                string temp = "Player " + player_turn;

                if (p.name == temp && s.alive == false)
                {
                    player_turn++;
                    break;
                }
            }

            startTurn(player_turn);
        }
        else
        {
            player_turn = 0;
            if (day == true && night == false)
            {
                Debug.Log("Acabou o dia ");
                startNight();
            }
            else
            {
                Debug.Log("Acabou a noite ");
                startDay(false);
            }
        }
    }

    void setTargetToDropdownDefault(Dropdown dr)
    {
        GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player

        // for(int j = 0; j < playlist.Count; j++)
        //  {

        for (int i = 0; i < plist.transform.childCount; i++)
        {
            // Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
            Citizen.player_info temp = new Citizen.player_info();
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
            s = p.GetComponent<Citizen>(); // player X's script
                                           //s.resetInfo();

            if (playlist[dr.value].text == p.name)
            {
                Debug.Log(" guardei como alvo:" + p.name);
                currentPlayerTargetAtNight = p.gameObject; // guarda o Citizen marcado
                break;
            }
            //s.ModelSwitch("Villager");
        }

    }

    void initMenu(string ident)
    {

        if (night == true)
        {
            dropmenu = GameObject.Find("Dropdown").GetComponent<Dropdown>();

            playlist = new List<Dropdown.OptionData>();
            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
            Dropdown.OptionData opcao = new Dropdown.OptionData();

            switch (ident)
            {
                case "Villager":

                    dropmenu.ClearOptions();



                    break;

                case "Werewolf":

                    dropmenu.ClearOptions();



                    for (int i = 0; i < plist.transform.childCount; i++)
                    {

                        opcao = new Dropdown.OptionData();
                        GameObject p;
                        Citizen.player_info temp = new Citizen.player_info();
                        Citizen s;
                        p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                        s = p.GetComponent<Citizen>(); // player X's script
                                                       //s.resetInfo();

                        if (s.identity == "Werewolf") continue; // n mostre outros werewolfs
                                                                //s.ModelSwitch("Villager");
                        if (s.alive == false) continue;

                        // Debug.Log("WOLF add opcao" + p.name);
                        opcao.text = p.name;
                        playlist.Add(opcao);

                        if (playlist.Count == 1) currentPlayerTargetAtNight = p;
                    }

                    dropmenu.AddOptions(playlist);
                    setTargetToDropdownDefault(dropmenu);


                    break;

                case "Seer":

                    dropmenu.ClearOptions();



                    for (int i = 0; i < plist.transform.childCount; i++)
                    {

                        opcao = new Dropdown.OptionData();
                        GameObject p;
                        Citizen.player_info temp = new Citizen.player_info();
                        Citizen s;
                        p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                        s = p.GetComponent<Citizen>(); // player X's script
                                                       //s.resetInfo();

                        // n mostre outros werewolfs
                        //s.ModelSwitch("Villager");

                        if (s.identity == "Seer") continue;

                        if (s.alive == false) continue;
                        //  Debug.Log("SEER add opcao" + p.name);
                        opcao.text = p.name;
                        playlist.Add(opcao);

                        if (playlist.Count == 1) currentPlayerTargetAtNight = p;
                    }

                    /*
                    for (int i = 0; i < playlist.Count; i++)
                    {
                        Debug.Log("opcao" + i + "= " + playlist[i].text);
                    }

                    */
                    dropmenu.AddOptions(playlist);
                    setTargetToDropdownDefault(dropmenu);

                    break;

            }

        }

        if (day == true)
        {

            dropmenu = GameObject.Find("Dropdown").GetComponent<Dropdown>();

            playlist = new List<Dropdown.OptionData>();
            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
            Dropdown.OptionData opcao = new Dropdown.OptionData();

            dropmenu.ClearOptions();



            for (int i = 0; i < plist.transform.childCount; i++)
            {

                opcao = new Dropdown.OptionData();
                GameObject p;
                Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                                               //s.resetInfo();

                // n mostre outros werewolfs
                //s.ModelSwitch("Villager");

                if (currentPlayerAtNight.name == p.name) continue;

                if (s.alive == false) continue;
                //Debug.Log("SEER add opcao" + p.name);
                opcao.text = p.name;
                playlist.Add(opcao);

                if (playlist.Count == 1) currentPlayerTargetAtNight = p;
            }

            /*
            for (int i = 0; i < playlist.Count; i++)
            {
                Debug.Log("opcao" + i + "= " + playlist[i].text);
            }

            */
            dropmenu.AddOptions(playlist);
            setTargetToDropdownDefault(dropmenu);


        }

    }

    public void getDropmenuSelected(int selec)
    {
        int res = playlist.Count;
        Debug.Log("valor do botao:" + selec + " | Size da playlist: " + res);

        GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player

        // for(int j = 0; j < playlist.Count; j++)
        //  {

        for (int i = 0; i < plist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
            Citizen.player_info temp = new Citizen.player_info();
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
            s = p.GetComponent<Citizen>(); // player X's script
                                           //s.resetInfo();

            if (playlist[selec].text == p.name)
            {
                Debug.Log(" guardei como alvo:" + p.name);
                currentPlayerTargetAtNight = p.gameObject; // guarda o Citizen marcado
                break;
            }
            //s.ModelSwitch("Villager");
        }

        // }



    }

    void startTurn(int trn)
    {
        if (night == true)
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
                    //jojo????????????????????????????????????????????????????????????????????????????
                    //Citizen shold = new Citizen();

                    if (!s.alive) continue;

                    Debug.Log(p.name + "|" + s.identity);

                    string nomee = "Player " + (player_turn + 1);
                    if (p.name == nomee)
                    {
                        initMenu(s.identity); // cria menu pro turno desse player


                        whosturn = p.name + " Turn | " + s.identity;
                        Debug.Log("SETEI CURRENT PLAYER");
                        currentPlayerAtNight = p.gameObject;

                        for (int j = 0; j < plist.transform.childCount; j++)
                        {
                            if (i == j) continue;

                            GameObject pl;
                            pl = plist.transform.GetChild(j).gameObject.transform.GetChild(0).gameObject; // current player Y
                            Citizen sl;
                            sl = pl.GetComponent<Citizen>(); // player y's script

                            if (pl.name == p.name) continue;

                            if (!sl.alive) continue;

                            // Debug.Log(" b4 " + p.gameObject.name + " info count" + s.players_info.Count);
                            for (int z = 0; z < s.players_info.Count; z++)
                            {
                                // Debug.Log(p.gameObject.name + " info " + z + ": Nome= " + s.players_info[z].player_name + " id=" + s.players_info[z].player_identity);

                            }

                            for (int k = 0; k < s.players_info.Count; k++)
                            {
                                //Debug.Log(" sao iguais? " + s.players_info[k].player_name + "|" + pl.gameObject.name);
                                if (s.players_info[k].player_name == pl.gameObject.name)
                                {

                                    string test = s.players_info[k].player_identity;
                                    // Debug.Log(pl.gameObject.name + " deveria parecer um: " + s.players_info[k].player_identity);


                                    sl.ModelSwitch(test);
                                    //  sl.canchange = false;
                                }
                            }

                        }

                    }

                }

            }

        }

        if (day == true)
        {

            if (playerOk == true)
            {
                Debug.Log("ERREI turn");
            }
            else
            {
                //Debug.Log("Entrei turn");
                playerOk = false;


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
                    //jojo????????????????????????????????????????????????????????????????????????????
                    //Citizen shold = new Citizen();


                    if (s.alive == false) continue;

                    Debug.Log(p.name + "|" + s.identity);

                    string nomee = "Player " + (player_turn + 1);
                    if (p.name == nomee)
                    {
                        initMenu(s.identity); // cria menu pro turno desse player

                        whosturn = p.name + " Turn | " + s.identity;
                        Debug.Log("SETEI CURRENT PLAYER");
                        currentPlayerAtNight = p.gameObject;

                        for (int j = 0; j < plist.transform.childCount; j++)
                        {
                            if (i == j) continue;

                            GameObject pl;
                            pl = plist.transform.GetChild(j).gameObject.transform.GetChild(0).gameObject; // current player Y
                            Citizen sl;
                            sl = pl.GetComponent<Citizen>(); // player y's script

                            if (pl.name == p.name) continue;

                            if (!sl.alive) continue;

                            // Debug.Log(" b4 " + p.gameObject.name + " info count" + s.players_info.Count);
                            /*
                            for (int z = 0; z < s.players_info.Count; z++)
                            {
                                Debug.Log(p.gameObject.name + " info " + z + ": Nome= " + s.players_info[z].player_name + " id=" + s.players_info[z].player_identity);

                            }

                            */
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

            // votacao do dia aqui
        }
    }

    void startNight()
    {
        night = true;
        day = false;

        // TIMER: mostra na tela quem foi assassinado

        GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player

        // for(int j = 0; j < playlist.Count; j++)
        //  {

        string killedByPeople = "";
        int votestemp = 0;
        for (int i = 0; i < plist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
            Citizen.player_info temp = new Citizen.player_info();
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
            s = p.GetComponent<Citizen>(); // player X's script
                                           //s.resetInfo();

            if (s.votes_people >= votestemp)
            {
                killedByPeople = p.name;
                votestemp = s.votes_people;
            }
            //s.ModelSwitch("Villager");
        }

        int tie = 0;
        for (int i = 0; i < plist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
            Citizen.player_info temp = new Citizen.player_info();
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
            s = p.GetComponent<Citizen>(); // player X's script
                                           //s.resetInfo();

            if (s.votes_people == votestemp)
            {
                tie++;
            }
            //s.ModelSwitch("Villager");
        }


        if (tie > 1)
        {
            // empate recomeca o dia
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                Dropdown.OptionData opcao1 = new Dropdown.OptionData();
                GameObject p;
                Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                                               //s.resetInfo();
                bool tru = true;
                bool fal = false;
                s.resetVotes(true, false);
                //s.ModelSwitch("Villager");
            }

            startDay(true);

            return;
        }


        for (int i = 0; i < plist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
            Citizen.player_info temp = new Citizen.player_info();
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
            s = p.GetComponent<Citizen>(); // player X's script
                                           //s.resetInfo();

            if (p.name == killedByPeople)
            {
                s.alive = false;
                s.doomed = true;

            }
            //s.ModelSwitch("Villager");
        }

        whoWasLynched = killedByPeople + " foi linchado";

        startTurn(player_turn);
    }

    void startDay(bool tie)
    {
        Debug.Log(" Comecou dia");

        night = false;
        day = true;

        if (tie == false)
        {
            // TIMER: mostra na tela quem foi assassinado

            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player

            // for(int j = 0; j < playlist.Count; j++)
            //  {

            string killedbywolf = "";
            int votestemp = 0;
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                Dropdown.OptionData opcao1 = new Dropdown.OptionData();
                GameObject p;
                Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                                               //s.resetInfo();

                if (s.votes_werewolf >= votestemp)
                {
                    killedbywolf = p.name;

                    votestemp = s.votes_werewolf;
                }
                //s.ModelSwitch("Villager");
            }

            for (int i = 0; i < plist.transform.childCount; i++)
            {
                Dropdown.OptionData opcao1 = new Dropdown.OptionData();
                GameObject p;
                Citizen.player_info temp = new Citizen.player_info();
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                                               //s.resetInfo();

                if (p.name == killedbywolf)
                {
                    s.alive = false;
                    s.doomed = true;

                }
                //s.ModelSwitch("Villager");
            }

            whoWasKilledLastNight = killedbywolf + " foi assassinado pelos lobisomens";

        }
        startTurn(player_turn);
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
            int r = k + Random.Range(1, 100) % (listap.Count - k); // careful here!

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

            if (qts.villagers < n_villagers)
            {
                script.identity = "Villager";
                for (int j = 0; j < go.transform.childCount; j++)
                {
                    if (go.transform.GetChild(j).gameObject.name == script.identity)
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

            if (seer == true && qts.seer < 1)
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
                            Debug.Log("<color=yellow> criando player: " + (i + 1) + "</color>");
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
        GUI.Label(new Rect(10, 10, 300, 30), whosturn);

        GUI.Label(new Rect(10, 40, 300, 30), whoWasKilledLastNight);

        GUI.Label(new Rect(10, 80, 300, 30), whoWasLynched);

        if(night)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 30), "Night");
        }
        else if(day)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, 30, 100, 30), "Day");
        }
    }
}

