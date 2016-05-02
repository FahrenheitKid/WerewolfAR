
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

    public static string whoWon = "Villagers win";

    public Font font;
    private float startTime = 0;
    private float ellapsedTime = 0;
    private string textTime;

    private bool isSeer = false;
    private string textTimeSeer;
    private string whoSeerSaw = ""; //guarda quem a vidende viu
    private float startTimeSeer = 0;
    private float ellapsedTimeSeer = 0;

    private bool isNextPlayer = false;
    private float startTimeNextPlayer = 0;
    private float ellapsedTimeNextPlayer = 0;

    public struct counts // counts é uma struct pra usar num for na hora de carregar os players, pra saber quantos de cada classe ainda precisa adicinoar
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
            return 3; // essa função n faz nada Q
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

    public bool night = true; // controla os ciclos de dia e noite
    public bool day = false;


    public int day_count = 0; // cont ao numero de dias e noite
    public int night_count = 0;


    public int player_turn = 0; // qual o player da rodada, passa-se essa variavel no startTurn();
    public bool playerOk = true; // checka se o player deu ready
    public string whoWasKilledLastNight = ""; // guarda quem foi morto na ultima noite
    public string whoWasLynched = ""; // guarda quem foi linchaod no ultimo dia

    List<GameObject> players = new List<GameObject>(); // lista dos Players (formados pelos modelos e o script Citizen)
    List<Dropdown.OptionData> playlist = new List<Dropdown.OptionData>(); // lista do dropdown
    public GameObject currentPlayerTargetAtNight = new GameObject(); // guarda o current player que foi alvo. Ex: seer escolheu player 1, ai guarda o player 1, ou se o werewolf votar player 2, guarda p2
    public GameObject currentPlayerAtNight = new GameObject(); // mesma coisa que antes só que guarda o jogador que está jogando no turno atual, e não seu alvo

    // Use this for initialization
    void Start()
    {
        startTime = Time.time;

        // define numero de players iniciais e werewolfs
        n_players1 = 6;
        n_werewolfs1 = n_players1 / 3;
        Mathf.Floor(n_werewolfs1);
        n_werewolfs = (int)n_werewolfs1;
        n_players = (int)n_players1;
        n_villagers = n_players - n_werewolfs;

        // define quais classes vão estar no jogo
        seer = true;

        initGame(); // embaralha os players
        initTargets(); // carrega os imagetargets e atribui pra acda um o player respectivo
        // startNight();

        startTurn(player_turn); // começa o turno (default é pela noite)
    }

    // Update is called once per frame
    void Update()
    {
        // night = false;
        int teamV = 0;
        int teamW = 0;

        for (int i = 0; i < players.Count; i++)
        {
            //checka quantos jogadores tem em ada time
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
            whoWon = "VILLAGERS WIN";
            Application.LoadLevel(2);
        }
        else if (teamV < teamW)
        {
            // werewolves outnumbered villagers, werewolves vecem!
            Debug.Log("WEREWOLVES WIN!");
            whoWon = "WEREWOLVES WIN";
            Application.LoadLevel(2);
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
        n_players_alive = tempi; // atualiza qtd de jogadores vivos
    }

    public void nextPlayer() // essa função é chamada quando o jogador aperta ready
    {
        playerOk = false;

        if (night == true)
        {
            // antes do proximo turno, efetivar as ações do player atual baseado na sua classe
            // aqui adicinoamos mais cases e logica conforme a classe
            switch (currentPlayerAtNight.GetComponent<Citizen>().identity)
            {
                case "Werewolf":

                    currentPlayerTargetAtNight.GetComponent<Citizen>().votes_werewolf++;
                    break;

                case "Seer":

                    string trueident;

                    Citizen s = currentPlayerAtNight.GetComponent<Citizen>();


                    
                    trueident = currentPlayerTargetAtNight.GetComponent<Citizen>().identity;
                    // mostrar na tela trueident

                    currentPlayerTargetAtNight.GetComponent<Citizen>().ModelSwitch(trueident);

                    Debug.Log("Seer descobriu que" + currentPlayerTargetAtNight.name + " eh um " + currentPlayerTargetAtNight.GetComponent<Citizen>().identity);
                    // setar timer pro seer conseguir ver quem ele é dps retornar o modelo
                    //Timer Seer
                    isSeer = true;
                    whoSeerSaw = "Seer descobriu que" + currentPlayerTargetAtNight.name + " eh um " + currentPlayerTargetAtNight.GetComponent<Citizen>().identity;
                    startTimeSeer = Time.time;

                    currentPlayerTargetAtNight.GetComponent<Citizen>().ModelSwitch("Villager");

                    for (int k = 0; k < s.players_info.Count; k++)
                    {
                        //Debug.Log(" sao iguais? " + s.players_info[k].player_name + "|" + pl.gameObject.name);
                        if (s.players_info[k].player_name == currentPlayerTargetAtNight.gameObject.name)
                        {
                            Debug.Log(currentPlayerTargetAtNight.GetComponent<Citizen>().identity);
                           Citizen.player_info tempInfo = new Citizen.player_info();
                            tempInfo = s.players_info[k];

                            tempInfo.setIden(currentPlayerTargetAtNight.GetComponent<Citizen>().identity); // atualizamos a info da seer, agora ela sabe a identidade desse player

                            s.players_info[k] = tempInfo;

                            Debug.Log("<color=blue>Setando seer info:</color> " + s.players_info[k].player_name + " agora eh= " + s.players_info[k].player_identity);
                        }
                    }

                    break;

                case "Villager":
                    // n faz nada hu3
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

        if (player_turn < n_players - 1)
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
                   // player_turn++;
                   // break;
                }
            }

            // aqui vamos pro proximo player vivo a jogar
            isNextPlayer = true;
            startTimeNextPlayer = Time.time;
            //startTurn(player_turn);
        }
        else // se todos ja foram, vira dia ou noite
        {
            Debug.Log("Player turno " + (player_turn + 1) + " apertou enter |" + n_players_alive);
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

    void setTargetToDropdownDefault(Dropdown dr) // funçãozinha pra dropmenu n começar vazio o valor
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
                //Debug.Log(" guardei como alvo:" + p.name);
                currentPlayerTargetAtNight = p.gameObject; // guarda o Citizen marcado
                break;
            }
            //s.ModelSwitch("Villager");
        }

    }

    void initMenu(string ident) // essa função é chamada sempre que é a vez de alguém, ela cria o dropdown de acordo com a classe
    {

        if (night == true)
        {
            dropmenu = GameObject.Find("Dropdown").GetComponent<Dropdown>();
            playlist = new List<Dropdown.OptionData>();
            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
            Dropdown.OptionData opcao = new Dropdown.OptionData();

            // quando adicinoar classes, adicinoar um case aqu também pra serem criados opções certas no dropdown
            // (ex: outra classe em grupo n deve ter como eliminar seus aliados)
            switch (ident)
            {
                case "Villager":

                    dropmenu.ClearOptions();
                    break;

                case "Werewolf":

                    dropmenu.ClearOptions();
                    // aqui temos que mostrar só não-werewolves
                    for (int i = 0; i < plist.transform.childCount; i++)
                    {

                        opcao = new Dropdown.OptionData();
                        GameObject p;
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
                    // aqui precisamos ignorar só a propria seer
                    for (int i = 0; i < plist.transform.childCount; i++)
                    {

                        opcao = new Dropdown.OptionData();
                        GameObject p;
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
            // se for dia, temos que msotrar todo mundo vivo, e ignorar a si mesmo.

            dropmenu = GameObject.Find("Dropdown").GetComponent<Dropdown>();

            playlist = new List<Dropdown.OptionData>();
            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
            Dropdown.OptionData opcao = new Dropdown.OptionData();

            dropmenu.ClearOptions();

            for (int i = 0; i < plist.transform.childCount; i++)
            {
                opcao = new Dropdown.OptionData();
                GameObject p;
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                                               //s.resetInfo();

                // n mostre outros werewolfs
                //s.ModelSwitch("Villager");

                if (currentPlayerAtNight.name == p.name)
                {
                    Debug.Log("atual:" + currentPlayerAtNight.name + " n adicionou opcao" + p.name);
                    continue;
                }
                
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

    public void getDropmenuSelected(int selec) // função que pega o player selecinoado no dropdown e guarda no CurrentPlayerTargetAtNight
    {
        int res = playlist.Count;
        Debug.Log("valor do botao:" + selec + " | Size da playlist: " + res);

        GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player

        for (int i = 0; i < plist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
            s = p.GetComponent<Citizen>(); // player X's script
                                           //s.resetInfo();

            if (playlist[selec].text == p.name)
            {
                //Debug.Log(" guardei como alvo:" + p.name);
                currentPlayerTargetAtNight = p.gameObject; // guarda o Citizen marcado
                break;
            }
            //s.ModelSwitch("Villager");
        }
    }

    void startTurn(int trn) // começa o turno do player trn, aí dentro da função tem uma parte pra acso seja dia, e outra pra acso seja noite
    {
        if (night == true) // se for noite
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
                        Citizen s;
                        p = plistt.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                        s = p.GetComponent<Citizen>(); // player X's script
                        s.resetInfo();
                    }

                    
                }

                //night = true;
                GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player
                for (int i = 0; i < plist.transform.childCount; i++) // começa a iterar pelos jogadores
                {
                    GameObject p;
                    Citizen s;
                    p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                    s = p.GetComponent<Citizen>(); // player X's script

                    // Debug.Log(p.gameObject.name + s.identity + " info count: " + s.players_info.Count);
                    //jojo????????????????????????????????????????????????????????????????????????????
                    //Citizen shold = new Citizen();

                   // if (!s.alive) continue;

                    Debug.Log(p.name + "|" + s.identity);

                    string nomee = "Player " + (player_turn + 1);

                    if(p.name == nomee && s.alive == false)
                    {
                        player_turn++;
                        i = 0;
                        continue;
                    }

                    if (p.name == nomee && s.alive == true) // se o jogador do for ali de cima, for == ao jogador da vez, é esse msm, continua lek!
                    {
                        whosturn = p.name + " Turn | " + s.identity;
                        Debug.Log("SETEI CURRENT PLAYER = " + p.name);
                        currentPlayerAtNight = p.gameObject; // atualiza o jogador da vez
                        initMenu(s.identity); // cria menu pro turno desse player
                        s.ModelSwitch(s.identity);

                        for (int j = 0; j < plist.transform.childCount; j++) // nesse for passamos por todos os outros jogadores e atualizamos o que o jogador da vez deve enxergar de acordo com as informações q ele tem
                        {
                            if (i == j) continue; // se for ele msm pula

                            GameObject pl;
                            pl = plist.transform.GetChild(j).gameObject.transform.GetChild(0).gameObject; // current player Y
                            Citizen sl;
                            sl = pl.GetComponent<Citizen>(); // player y's script

                            //if (pl.name == p.name) continue; // se for ele msm pula

                           // if (!sl.alive) continue; // se n tiver vivo pula

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
                                   
                                    string test = s.players_info[k].player_identity; // aqui guardamos como o jogador da vez deve enxergar o outro jogador
                                                                                     // Debug.Log(pl.gameObject.name + " deveria parecer um: " + s.players_info[k].player_identity);
                                    if (s.identity == "Seer")
                                    {
                                        Debug.Log("<color=pink>Seer ta vendo </color>" + s.players_info[k].player_name + "como " + test);
                                    }

                                    sl.ModelSwitch(test); // aqui que mudamos os modelos dos outros jogadores
                                    //  sl.canchange = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (day == true) // se for dia
        {
            // fazemos basicamento o msm ciclo da noite.

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
                    Citizen s;
                    p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                    s = p.GetComponent<Citizen>(); // player X's script

                    // Debug.Log(p.gameObject.name + s.identity + " info count: " + s.players_info.Count);
                    //jojo????????????????????????????????????????????????????????????????????????????
                    //Citizen shold = new Citizen();

                    //if (s.alive == false) continue;

                    Debug.Log(p.name + "|" + s.identity);

                    string nomee = "Player " + (player_turn + 1);
                    if (p.name == nomee && s.alive == false)
                    {
                        player_turn++;
                        i = 0;
                        continue;
                    }

                    if (p.name == nomee && s.alive == true) // se o jogador do for ali de cima, for == ao jogador da vez, é esse msm, continua lek!
                    {
                        whosturn = p.name + " Turn | " + s.identity;
                        Debug.Log("SETEI CURRENT PLAYER = " + p.name);
                        currentPlayerAtNight = p.gameObject;
                        initMenu(s.identity); // cria menu pro turno desse player
                        s.ModelSwitch(s.identity);

                        for (int j = 0; j < plist.transform.childCount; j++)
                        {
                            if (i == j) continue;
                            GameObject pl;
                            pl = plist.transform.GetChild(j).gameObject.transform.GetChild(0).gameObject; // current player Y
                            Citizen sl;
                            sl = pl.GetComponent<Citizen>(); // player y's script

                           // if (pl.name == p.name) continue;

                            //if (!sl.alive) continue;

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
                                    //Debug.Log(pl.gameObject.name + " deveria parecer um: " + s.players_info[k].player_identity);

                                    sl.ModelSwitch(test); // mudamos os modelos de acordo com cada um novamente
                                    //  sl.canchange = false;
                                }
                            }
                        }
                    }
                }
            }
            // votacao do dia aqui ???????? (n lembro o pq dessa anotação, ignorem até eu lembrar)
        }
    }

    void startNight() // função que começa a noite e atualiza as coisas de acordo com as mudanças do dia
    {
        Debug.Log("<color=orange>Comecou a noite </color>");

        night = true;
        day = false;
        GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player

        string killedByPeople = "";
        int votestemp = 0;

        //nesse for vemos quem teve mais votos das pessoas pra ver quem foi linchado
        for (int i = 0; i < plist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
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
        // nesse for verificamos se houve empate na votação
        for (int i = 0; i < plist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
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

        // se teve empate, resetamos os votos e fazemos dnv isso aqui
        if (tie > 1)
        {
            // empate recomeca o dia
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                Dropdown.OptionData opcao1 = new Dropdown.OptionData();
                GameObject p;
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                                               //s.resetInfo();
                s.resetVotes(true, false);
                //s.ModelSwitch("Villager");
            }
            //aqui mandamos um startDay passando true, pq o TIE = true (houve empate)
            startDay(true);
            return;
        }

        // aqui vemos quem foi morto e marcamos devidamente
        for (int i = 0; i < plist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
            Citizen.player_info temp = new Citizen.player_info();
            Citizen s;
            p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
            s = p.GetComponent<Citizen>(); // player X's script
                                           //s.resetInfo();
            s.resetVotes(false, true);
            if (p.name == killedByPeople)
            {
                s.alive = false;
                s.doomed = true;

            }
            //s.ModelSwitch("Villager");

        }
        //mostramos a mensagem de quem foi linchado
        whoWasLynched = killedByPeople + " foi linchado aos " + textTime + " min";
        //começamos o prox turno
        startTurn(player_turn);
    }

    void startDay(bool tie)
    {
        

        night = false;
        day = true;

        if (tie == false) // se n houve empate faz o ciclo "normal"
        {
            night_count++;
            Debug.Log(" <color=orange>Comecou dia normal</color>");
            // TIMER: mostra na tela quem foi assassinado

            GameObject plist = GameObject.Find("PlayersList"); // lista de (parents) player

            string killedbywolf = "";
            int votestemp = 0;


            Debug.Log("<color=green>Checando votos wolf</color>");
            //nesse for verificamos quem teve a maioria dos votos dos lobos para mata-lo
            // outras classes que matam jogador (em conjunto ou sozinho) verificam as coisas aqui tb (por enquanto só os lobos)
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                Dropdown.OptionData opcao1 = new Dropdown.OptionData();
                GameObject p;
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

            // aqui setamos como morto tal jogador
            for (int i = 0; i < plist.transform.childCount; i++)
            {
                Dropdown.OptionData opcao1 = new Dropdown.OptionData();
                GameObject p;
                Citizen s;
                p = plist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
                s = p.GetComponent<Citizen>(); // player X's script
                                               //s.resetInfo();

                if (p.name == killedbywolf)
                {
                    Debug.Log(p.name + " FOI MORTO COM " + votestemp + " votos");
                    s.alive = false;
                    s.doomed = true;

                }
                //s.ModelSwitch("Villager");
            }

            //mostramos quem foi morto pelo lobo
            whoWasKilledLastNight = killedbywolf + " foi assassinado pelos wolf aos " + textTime + " min";
        }

        Debug.Log(" tie =" + tie);

        GameObject pplist = GameObject.Find("PlayersList"); // lista de (parents) player
        for (int i = 0; i < pplist.transform.childCount; i++)
        {
            Dropdown.OptionData opcao1 = new Dropdown.OptionData();
            GameObject p;
            Citizen s;
            p = pplist.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject; // current player X
            s = p.GetComponent<Citizen>(); // player X's script
                                           //s.resetInfo();

            s.resetVotes(true, true);
            //s.ModelSwitch("Villager");
        }


        //começa o proximo turno
        startTurn(player_turn);
    }

    void initGame() // aqui começamos as regras do jogo: criamos todas as classes necessarias e randomizamos players
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
        

        //nesse for que usamos a struct counts, cada vez que criamos classe x, incrementamos a classe x no counts. Quando a classe x atingir a quantidade necessaria, paramos de criar classe x e começamos
        // a criar classe y, e assim por diante
        for (int i = 0; i < listap.Count; i++)
        {
            GameObject go = Instantiate(Resources.Load("Player")) as GameObject;

            // seta posição pro player
          //  go = Instantiate(go, new Vector3(0, 0, 0), go.transform.rotation) as GameObject;
            
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

    void initTargets() // aqui é a parte onde o vuria carrega as database e os image target, agora tão com as cartas certas e os players respectivos certos
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
                char num = name[name.Length - 1];

                int numero = int.Parse(num.ToString());
                if (numero <= players.Count)
                {

                    trackableBehaviour.gameObject.transform.name = "ImageTarget " + (numero);

                    trackableBehaviour.gameObject.transform.SetParent(GameObject.Find("PlayersList").transform);
                    trackableBehaviour.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    for (int j = 0; j < players.Count; j++)
                    {

                        //  Debug.Log("<color=yellow>player name: " + players[i].name + "</color>");
                        //Debug.Log("<color=blue>player da vez: Player " + (j + 1)  + "</color>");
                        //Debug.Log("Trackable name: " + name);
                        string nametemp = "Player " + (numero);
                        if (players[j].name == nametemp)
                        {
                            Debug.Log("<color=yellow> criando player: " + (numero) + "</color>");
                            players[j].transform.SetParent(trackableBehaviour.gameObject.transform);
                           // players[j].transform.position.Set(0.0f, 0.0f, 0.0f);

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

    void OnGUI() // mostramos mensagens na tela
    {
        GUI.skin.font = font;
        GUI.skin.label.fontSize = 16;

        //Timer
        ellapsedTime = Time.time - startTime;
        int minutes = (int)ellapsedTime / 60;
        int seconds = (int)ellapsedTime % 60;

        textTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 40), textTime);

        //Other stuff
        GUI.Label(new Rect(10, 10, 500, 40), whosturn);

        GUI.Label(new Rect(10, 40, 900, 40), whoWasKilledLastNight);

        GUI.Label(new Rect(10, 80, 500, 40), whoWasLynched);

        if(isNextPlayer)
        {
            ellapsedTimeNextPlayer = Time.time - startTimeNextPlayer;
            minutes = (int)ellapsedTimeNextPlayer / 60;
            seconds = (int)ellapsedTimeNextPlayer % 60;

            textTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            GUI.Label(new Rect(500, 10, 100, 40), textTime);

            if (ellapsedTimeNextPlayer > 0)
            {
                startTurn(player_turn);
                isNextPlayer = false;
            }
        }
        else if(GUI.Button(new Rect(Screen.width - 250, Screen.height - 85, 160, 30), "Ready"))
        {
            nextPlayer();
        }

        if (isSeer)
        {
            GUI.Label(new Rect(10, 120, 900, 40), whoSeerSaw);
            ellapsedTimeSeer = Time.time - startTimeSeer;

          //  Debug.Log(ellapsedTimeSeer);
            if (ellapsedTimeSeer > 5)
            {
                isSeer = false;
            }
        }

        if (night)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, 50, 100, 40), "Night");
        }
        else if (day)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, 50, 100, 40), "Day");
        }
    }
}
