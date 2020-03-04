using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.Math;

using Completed;
using TwistedFeathers;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class CombatManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks, IPunObservable
{
    SortedSet<BattleEffect> pq;
    List<TwistedFeathers.Player> battle_players;
    private List<Monster> battle_monsters;
    Environment env;
    //Weather weath;
    int currentTurn;
    bool waitingPlayer;
    int protagonistIndex;
    public GameObject ForecastOpener;
    static string forecastText;
    public GameObject textPrefab;

    private GameObject newText;
    public GameObject forecastContent;
    public Environment[,] map;
    int numTexts = 0;
    public int rows = 3;
    public int cols = 4;

    private Vector2 currentLocation;
    private Vector2 changingLocation;
    public Environment CurrentEnvironment;

    public bool selectingMap = false;
    public bool validMove = true;

    public int allowedMoves = 2;

    public static string ForecastText { get => forecastText; set => forecastText = value; }


    public UIManager UIManager;

    private PunTurnManager turnManager;

    [SerializeField]
    private string localSelection;

    [SerializeField]
    private string remoteSelection;

    List<BattleParticipant> getBattleParticipants()
    {
        List<BattleParticipant> list = new List<BattleParticipant>();
        foreach (Monster mon in battle_monsters)
        {
            list.Add(mon);
        }

        foreach (TwistedFeathers.Player play in battle_players)
        {
            list.Add(play);
        }

        return list;

    }


    //Method for taking a skill and queueing the effect into the PQ
    void queueSkill(Skill skill, BattleParticipant user, List<BattleParticipant> target)
    {
        foreach(BattleEffect effect in skill.Effect)
        {
            BattleEffect battle_effect = effect;
            battle_effect.select(user, target, currentTurn);
            pq.Add(battle_effect);
        }
    }

    //Method for resolving all effects set to happen this turn in PQ
    void resolveEffects()
    {
        Debug.Log("Effects Resolving!");
        while (pq.Count != 0 && pq.Min.Turnstamp <= currentTurn)
        {
            pq.Min.run();
            pq.Remove(pq.Min);
        }
        Debug.Log("Effects Resolved!");
    }

    void resolveStatuses()
    {
        
        foreach (BattleParticipant bat_part in getBattleParticipants())
        {
            foreach (KeyValuePair<string, BattleEffect> status in bat_part.Statuses.ToArray())
            {
                // This is where we code in what each status effect actually does
                switch (status.Key)
                {
                    case "Poison":

                        break;
                    case "Burn":

                        break;
                    default:
                        status.Value.Duration -= 1;
                        if (status.Value.Duration <= 0)
                        {
                            bat_part.Statuses.Remove(status);
                        }
                        break;
                }
            }
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        ForecastText = "";
        currentTurn = 0;
        pq = new SortedSet<BattleEffect>(new EffectComparator());
        battle_players = new List<TwistedFeathers.Player>();
        battle_monsters = new List<Monster>();
        protagonistIndex = 0; 
        //Dummy values for testing purposes
        battle_players.Add((TwistedFeathers.Player)GameManager.Participant_db["person A"]);
        battle_players.Add((TwistedFeathers.Player)GameManager.Participant_db["person B"]);
        battle_monsters.Add((Monster)GameManager.Participant_db["enemy B"]);
        battle_monsters.Add((Monster)GameManager.Participant_db["enemy A"]);
        waitingPlayer = false;
        Debug.Log("TURN BEGIN");
        buildMap(rows, cols);
        if(!GameObject.Find("GameManager").GetComponent<GameManager>().rotate){
            // Checks to see if the player has indicated that they want environmental rotation animations
            Animator animator = GameObject.Find("DesertAnimation").GetComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("Animations/Empty") as RuntimeAnimatorController;
            animator = GameObject.Find("SwampAnimation").GetComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("Animations/Empty") as RuntimeAnimatorController;
        }

        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;

    }
    
    public void SelectSkill(string skill){
        // we know that this will be the selected skill
        // if one player don't network the move
        if (GameManager.singlePlayer)
        {
            singlePlayerChooseSkill();
        }
        else if(PhotonNetwork.PlayerList.Length == 1)
        {
            singlePlayerChooseSkill();
        }
        else
        {
            MakeTurn(skill);
        }
        Debug.Log(skill); // change this to actually do what the skill does
    }

    private void singlePlayerChooseSkill()
    {
        Debug.Log("Single Player choose skill");
        chooseSkill(0);
        chooseSkill(1);
    }

    public void chooseSkill(int index){
        // set this method up to take a parameter index that specifies
        // whether the player or ally is choosing a skill
        // more work will have to be done with target selection
        TwistedFeathers.Player protag = (TwistedFeathers.Player) battle_players[index]; 
        queueSkill( protag.Skills[Random.Range(0, protag.Skills.Count)], protag, new List<BattleParticipant>(){battle_monsters[index]});
        waitingPlayer = false;
    }

    public void StartSelecting(){
        // We want to begin selecting our environment change, so update the flow
        selectingMap = true;
        // deactivate buttons for the time being
        GameObject.Find("Board").transform.Find("SelectionEntity").gameObject.SetActive(true);
        UIManager.toggleMiniMapButtons(true);
        // Enable the specific indicators for this portion
        GameObject.Find("SelectionEntity").transform.SetAsFirstSibling(); //move to the front (on parent)
        GameObject.Find("CurrentLocation").transform.SetAsLastSibling();
    }

    public void ConfirmSelecting(){
        //If the movement is valid, this is called and go back to normal flow
        selectingMap = false;
        // update the current location
        currentLocation = changingLocation;
        // Update the currentEnvironment
        CurrentEnvironment = map[(int)currentLocation.x, (int)currentLocation.y];
        //update the scene to represent the current environment
        GameObject.Find("Main Camera").GetComponent<SceneShift>().scene = CurrentEnvironment.envListIndex;
        //update the currentLocation UI indicator
        GameObject.Find("CurrentLocation").GetComponent<RectTransform>().localPosition = new Vector3(currentLocation.x*60, currentLocation.y*60, 0f);
        // a turn ahas been taken
        waitingPlayer = false;
        // TODO at some point a method will be called here to play some animations
    }

    public void CancelSelecting(){
        // if cancelled, we just go back to normal
        selectingMap = false;
    }
    // Update is called once per frame
    void Update()
    {
        // if the UI is in the change environment mode there is a different flow for the update method
        if(selectingMap){
            // We are now selecting where we want to move to so we need to activate and show it over our current changing position
            GameObject.Find("SelectionEntity").GetComponent<RectTransform>().localPosition = new Vector3 (changingLocation.x*60, changingLocation.y*60, -10f);
            //Checks to see if our move is out of scope
            int totalMove = Abs((int)(currentLocation.x - changingLocation.x)) + Abs((int)(currentLocation.y - changingLocation.y));
            if(totalMove <= allowedMoves) {
                // if the move is allowed the selection entity notifies the player by turning yellow and valid move is set to true
                GameObject.Find("SelectionEntity").GetComponent<Image>().color = Color.yellow;
                validMove = true;
            } else {
                //otherwise its grey and false
                validMove = false;
                GameObject.Find("SelectionEntity").GetComponent<Image>().color = Color.grey;
            }
            if (Input.GetKeyDown("a")) {
                //if left is pressed move the changing location over 1 unless at the edge
                if(changingLocation.x != 0 && map[(int)changingLocation.x-1, (int)changingLocation.y] != GameManager.environments[GameManager.environments.Count-1]){
                    changingLocation.x = changingLocation.x-1;
                }
            }
            if (Input.GetKeyDown("w")) {
                //if left up is pressed move the changing location over 1 unless at the edge
                if(changingLocation.y != rows-1 && map[(int)changingLocation.x, (int)changingLocation.y+1] != GameManager.environments[GameManager.environments.Count-1]){
                    changingLocation.y = changingLocation.y+1;
                }
            }
            //if left right is pressed move the changing location over 1 unless at the edge
            if (Input.GetKeyDown("d")) {
                if(changingLocation.x != cols-1 && map[(int)changingLocation.x+1, (int)changingLocation.y] != GameManager.environments[GameManager.environments.Count-1]){
                    changingLocation.x = changingLocation.x+1;
                }
            }
            //if down is pressed move the changing location over 1 unless at the edge
            if (Input.GetKeyDown("s")) {
                if(changingLocation.y != 0 && map[(int)changingLocation.x, (int)changingLocation.y-1] != GameManager.environments[GameManager.environments.Count-1]){
                    changingLocation.y = changingLocation.y-1;
                }
            }
        } else {
            if (GameManager.singlePlayer && !waitingPlayer)
            {
                singlePlayerTurn();
            } else if(PhotonNetwork.PlayerList.Length == 1 && !waitingPlayer)
            {
                singlePlayerTurn();
            }

        }
        
    }

    private void singlePlayerTurn()
    {
        //Effects are resolved and turn ends
        resolveEffects();
        Debug.Log("TURN END");
        //resolveStatuses();
        TurnBegin();
    }

    private void TurnBegin()
    {
        //Testing HP damage
        Debug.Log("Adam HP: " + battle_players[protagonistIndex].Current_hp);
        Debug.Log("Ben HP: " + battle_players[protagonistIndex + 1].Current_hp);
        Debug.Log("Enemy 1 HP: " + battle_monsters[protagonistIndex].Current_hp);
        Debug.Log("Enemy 2 HP: " + battle_monsters[protagonistIndex + 1].Current_hp);
        //New turn beings here
        Debug.Log("TURN BEGIN");
        currentTurn++;
        foreach (Monster part in battle_monsters)
        {
            queueSkill((Skill)part.Skills[Random.Range(0, part.Skills.Count)], part, new List<BattleParticipant>() { battle_players[protagonistIndex] });
        }

        //Forecast

        Debug.Log("Forecast Begins!");

        foreach (BattleEffect eff in pq)
        {
            newText = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newText.transform.SetParent(forecastContent.transform, false);
            newText.GetComponent<RectTransform>().localScale = new Vector3(0.6968032f, 1.7355f, 1.7355f);
            newText.transform.position = new Vector3(forecastContent.transform.position.x + 275, forecastContent.transform.position.y - 40 * numTexts - 30);
            newText.GetComponent<Text>().text = eff.User.Name;
            newText.GetComponent<Text>().color = Color.white;

            numTexts++;
            Debug.Log(eff.User.Name);

        }
        ForecastOpener.GetComponent<ButtonHandler>().newForecast();
        Debug.Log("Forecast Over!");

        waitingPlayer = true;
    }

    #region TurnManager Callbacks
    /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins() turn: " + turn);
        this.localSelection = null;
        this.remoteSelection = null;

        TurnBegin();
    }

    public void OnTurnCompleted(int obj)
    {
        Debug.Log("OnTurnCompleted: " + obj);

        if (this.localSelection != null)
        {
            // once we get an actual functioning choose skill
            // we can pass in the selection
            chooseSkill(0);
        }
        if (this.remoteSelection != null)
        {
            //attacks.Enqueue(this.remoteSelection);
            chooseSkill(1);
        }
        Debug.Log("Calling execute skills");

        this.resolveEffects();
        Debug.Log("TURN END");
        Debug.Log("Calling on end turn");
        this.OnEndTurn();
    }

    // when a player moved (but did not finish the turn)
    public void OnPlayerMove(Photon.Realtime.Player photonPlayer, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);
    }


    // when a player made the last/final move in a turn
    public void OnPlayerFinished(Photon.Realtime.Player photonPlayer, int turn, object move)
    {
        Debug.Log("OnTurnFinished: " + photonPlayer + " turn: " + turn + " action: " + move);

        if (photonPlayer.IsLocal)
        {
            this.localSelection = (string)move;
        }
        else
        {
            this.remoteSelection = (string)move;
        }
    }



    public void OnTurnTimeEnds(int obj)
    {
        //do nothing
    }


    #endregion

    public void StartTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.turnManager.BeginTurn();
        }
    }

    public void MakeTurn(string selection)
    {
        this.turnManager.SendMove((string)selection, true);
    }

    public void OnEndTurn()
    {
        this.StartCoroutine("BeginNextTurnCoroutine");
    }

    public IEnumerator BeginNextTurnCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("Calling start turn");
        this.StartTurn();
    }

    #region Photon Callbacks

    /// <summary>
    /// Called when a Photon Player got connected.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            Debug.Log("Other player arrived");
            if (this.turnManager.Turn == 0)
            {
                // when the room has a player, make sure that the player can play without needing to wait
                this.StartTurn();
            }
        }
        else
        {
            Debug.Log("Waiting for another player");
        }
    }

    /// <summary>
    /// Called when a Photon Player got disconnected.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

        }
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("TestScene");
    }

    #endregion

    #region IPunObservable implementation

    // This is how we send and receive data
    // JACOB WAS HERE
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            for (int i = 0; i < battle_players.Count; i++)
            {
                // We own this player: send the others our data
                stream.SendNext(battle_players[i].Current_hp);
                stream.SendNext(battle_players[i].Attack);
                stream.SendNext(battle_players[i].Defense);
                stream.SendNext(battle_players[i].Dodge);
            }
            for (int i = 0; i < battle_monsters.Count; i++)
            {
                // We own this player: send the others our data
                stream.SendNext(battle_monsters[i].Current_hp);
                stream.SendNext(battle_monsters[i].Attack);
                stream.SendNext(battle_monsters[i].Defense);
                stream.SendNext(battle_monsters[i].Dodge);
            }
        }
        else
        {
            // Network player, receive data
            for (int i = 0; i < battle_players.Count; i++)
            {
                battle_players[i].Current_hp = (int)stream.ReceiveNext();
                battle_players[i].Attack = (float)stream.ReceiveNext();
                battle_players[i].Defense = (float)stream.ReceiveNext();
                battle_players[i].Dodge = (float)stream.ReceiveNext();
            }
            for (int i = 0; i < battle_monsters.Count; i++)
            {
                battle_monsters[i].Current_hp = (int)stream.ReceiveNext();
                battle_monsters[i].Attack = (float)stream.ReceiveNext();
                battle_monsters[i].Defense = (float)stream.ReceiveNext();
                battle_monsters[i].Dodge = (float)stream.ReceiveNext();
            }
        }
    }

    #endregion

    //Randomly generates the 2D array that represents the UI Map
    public void buildMap(int rows, int cols){
        map = new Environment[cols, rows];
        map[cols-1,rows-1] = GameManager.environments[0];
        for(int i = 0; i < cols; i++){
            for(int j = 0; j < rows; j++){
                float randomInt = Random.Range(0f, 10.0f);
                if(randomInt >=0 && randomInt <=3){
                    map[i,j] = GameManager.environments[0];
                } else if(randomInt >=3 && randomInt <=9){
                    map[i,j] = GameManager.environments[1];
                } else if(randomInt >=9 && randomInt <=10){
                    map[i,j] = GameManager.environments[GameManager.environments.Count-1];
                }
            }
        }
        for(int i = 0; i < cols; i++){
            for(int j = 0; j < rows; j++){
                if(map[i,j] != GameManager.environments[GameManager.environments.Count-1]){
                    currentLocation = new Vector2(i, j);
                    changingLocation = new Vector2(i,j);
                    break;
                }
            }
        }
        MapSetup(rows, cols);
    }

    // the board object within the canvas that holds all of the environment
    private Transform mapHolder; 

    // Method that sets up the map and assigns map icons to the correct position in the UI
    void MapSetup (int rows, int cols)
    {
        //Instantiate Board and set boardHolder to its transform.
        mapHolder = new GameObject ("Board", typeof(RectTransform)).transform;
        mapHolder.SetParent(GameObject.Find("Panel").transform);
        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for(int x = 0; x < cols; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for(int y = 0; y < rows; y++)
            {
                GameObject toInstantiate;
                toInstantiate = map[x,y].miniIcon;
                GameObject instance = Instantiate (toInstantiate, new Vector3 (x*60, y*60, 0f), Quaternion.identity) as GameObject;
                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent (mapHolder);
                if(currentLocation.x == x && currentLocation.y ==y){
                    GameObject.Find("CurrentLocation").transform.SetParent (mapHolder);
                    GameObject.Find("CurrentLocation").GetComponent<RectTransform>().localPosition = new Vector3(x*60, y*60, 0f);
                    GameObject.Find("SelectionEntity").transform.SetParent (mapHolder);
                    GameObject.Find("SelectionEntity").GetComponent<RectTransform>().localPosition = new Vector3(x*60, y*60, 0f);
                    GameObject.Find("SelectionEntity").SetActive(false); //move to the front (on parent)
                    CurrentEnvironment = map[x,y];
                }
            }
        }
        mapHolder.SetParent(GameObject.Find("Panel").transform);
        mapHolder.GetComponent<RectTransform>().anchorMin = new Vector2(0f,0f);
        mapHolder.GetComponent<RectTransform>().anchorMax = new Vector2(0f,0f);
        mapHolder.GetComponent<RectTransform>().anchoredPosition = new Vector3(170, 230, 0f);
        GameObject.Find("Main Camera").GetComponent<SceneShift>().scene = CurrentEnvironment.envListIndex;

    }
}
