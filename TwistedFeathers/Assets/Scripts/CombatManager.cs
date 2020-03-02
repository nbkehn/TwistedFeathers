using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using Completed;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class CombatManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks, IPunObservable
{
    #region Public Fields

    //static public CombatManager Instance;

    SortedSet<BattleEffect> pq;
    List<PlayerScriptableObject> battle_players;
    List<MonsterScriptableObject> battle_monsters;
    Environment env;
    //Weather weath;
    int currentTurn;
    bool waitingPlayer;
    int protagonistIndex;
    public GameObject ForecastOpener;
    static string forecastText;
    public GameObject textPrefab;

    public GameObject forecastContent;
    public Environment[,] map;
    int numTexts = 0;
    public int rows = 3;
    public int cols = 4;

    public Environment CurrentEnvironment;

    public bool selectingMap = false;
    public bool validMove = true;

    public int allowedMoves = 2;

    #endregion

    #region Private Fields

    private GameObject newText;

    private Vector2 currentLocation;

    private Vector2 changingLocation;

    private List<Vector3> gridPositions = new List<Vector3>();

    private Transform mapHolder;

    //private GameObject instance;

    private PunTurnManager turnManager;

    /*
    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerPrefab;
    */

    // will have to replace this with the game
    // ui element
    //[SerializeField]
    //private CanvasGroup ButtonCanvasGroup;

    [SerializeField]
    private string localSelection;

    [SerializeField]
    private string remoteSelection;

    #endregion

    #region MonoBehaviour CallBacks
    // Start is called before the first frame update
    void Start()
    {

        ForecastText = "";
        currentTurn = 0;
        pq = new SortedSet<BattleEffect>(new EffectComparator());
        battle_players = new List<PlayerScriptableObject>();
        battle_monsters = new List<MonsterScriptableObject>();
        protagonistIndex = 0; 
        //Dummy values for testing purposes
        battle_players.Add((PlayerScriptableObject) GameManager.Participant_db["person A"]);
        battle_players.Add((PlayerScriptableObject)GameManager.Participant_db["person B"]);
        battle_monsters.Add((MonsterScriptableObject) GameManager.Participant_db["enemy B"]);
        battle_monsters.Add((MonsterScriptableObject)GameManager.Participant_db["enemy A"]);
        Debug.Log("Players in battle: " + battle_players.Count);
        Debug.Log("Monsters in battle: " + battle_monsters.Count);

        waitingPlayer = false;
        Debug.Log("TURN BEGIN");
        buildMap(rows, cols);

        //Instance = this;

        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("TestScene");

            return;
        }
        /*
        if (playerPrefab == null)
        { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {

            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                //PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {

                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }


        }
        */
        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;
        this.turnManager.TurnDuration = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectingMap)
        {
            GameObject.Find("SelectionEntity").GetComponent<RectTransform>().localPosition = new Vector3(changingLocation.x * 60, changingLocation.y * 60, -10f);
            int totalMove = Abs((int)(currentLocation.x - changingLocation.x)) + Abs((int)(currentLocation.y - changingLocation.y));
            if (totalMove <= allowedMoves)
            {
                GameObject.Find("SelectionEntity").GetComponent<Image>().color = Color.yellow;
                validMove = true;
            }
            else
            {
                validMove = false;
                GameObject.Find("SelectionEntity").GetComponent<Image>().color = Color.grey;
            }
            if (Input.GetKeyDown("left"))
            {
                if (changingLocation.x != 0 && map[(int)changingLocation.x - 1, (int)changingLocation.y] != GameManager.environments[GameManager.environments.Count - 1])
                {
                    changingLocation.x = changingLocation.x - 1;
                }
            }
            if (Input.GetKeyDown("up"))
            {
                if (changingLocation.y != rows - 1 && map[(int)changingLocation.x, (int)changingLocation.y + 1] != GameManager.environments[GameManager.environments.Count - 1])
                {
                    changingLocation.y = changingLocation.y + 1;
                }
            }
            if (Input.GetKeyDown("right"))
            {
                if (changingLocation.x != cols - 1 && map[(int)changingLocation.x + 1, (int)changingLocation.y] != GameManager.environments[GameManager.environments.Count - 1])
                {
                    changingLocation.x = changingLocation.x + 1;
                }
            }
            if (Input.GetKeyDown("down"))
            {
                if (changingLocation.y != 0 && map[(int)changingLocation.x, (int)changingLocation.y - 1] != GameManager.environments[GameManager.environments.Count - 1])
                {
                    changingLocation.y = changingLocation.y - 1;
                }
            }
        }
        else
        { // if there is one player handle single player
            if (PhotonNetwork.PlayerList.Length == 1 && !waitingPlayer)
            {
                //Effects are resolved and turn ends
                resolveEffects();
                //Testing HP damage
                Debug.Log("Adam HP: " + battle_players[protagonistIndex].Current_hp);
                Debug.Log("Ben HP: " + battle_players[protagonistIndex + 1].Current_hp);
                Debug.Log("Beelzebub 1 HP: " + battle_monsters[protagonistIndex].Current_hp);
                Debug.Log("Beelzebub 2 HP: " + battle_monsters[protagonistIndex + 1].Current_hp);
                Debug.Log("TURN END");
                //New turn beings here
                currentTurn++;
                Debug.Log("TURN BEGIN " + currentTurn);
                
                foreach (Participant part in battle_monsters)
                {
                    Debug.Log("Monster skill chosen");
                    queueSkill((Skill)part.Skills[Random.Range(0, part.Skills.Count)], part, battle_players[protagonistIndex]);
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

        }

    }
    #endregion

    #region TurnManager Callbacks
    /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins() turn: " + turn);
        this.localSelection = null;
        this.remoteSelection = null;

        Debug.Log("Adam HP: " + battle_players[protagonistIndex].Current_hp);
        Debug.Log("Ben HP: " + battle_players[protagonistIndex + 1].Current_hp);
        Debug.Log("Beelzebub 1 HP: " + battle_monsters[protagonistIndex].Current_hp);
        Debug.Log("Beelzebub 2 HP: " + battle_monsters[protagonistIndex + 1].Current_hp);

        // ButtonCanvasGroup.interactable = true;

        currentTurn++;
        foreach (Participant part in battle_monsters)
        {
            queueSkill((Skill)part.Skills[Random.Range(0, part.Skills.Count)], part, battle_players[protagonistIndex]);
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
    public void OnPlayerMove(Player photonPlayer, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);
    }


    // when a player made the last/final move in a turn
    public void OnPlayerFinished(Player photonPlayer, int turn, object move)
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

    #region Core Gameplay Methods
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
        //ButtonCanvasGroup.interactable = false;

        yield return new WaitForSeconds(2.0f);
        Debug.Log("Calling start turn");
        this.StartTurn();
    }

    //Method for taking a skill and queueing the effect into the PQ
    void queueSkill(Skill skill, Participant user, Participant target)
    {
        foreach (BattleEffect effect in skill.Effect)
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

    public static string ForecastText { get => forecastText; set => forecastText = value; }

    public void SelectSkill(string skill)
    {
        // we know that this will be the selected skill
        // if one player don't network the move
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            Debug.Log("Single Player choose skill");
            chooseSkill(0);
            chooseSkill(1);
        }
        else
        {
            MakeTurn(skill);
        }
        Debug.Log(skill); // change this to actually do what the skill does
        
    }

    public void chooseSkill(int index)
    {
        PlayerScriptableObject protag = (PlayerScriptableObject)battle_players[index];
        queueSkill(protag.Skills[Random.Range(0, protag.Skills.Count)], protag, (MonsterScriptableObject)battle_monsters[index]);
        waitingPlayer = false;
    }
    #endregion

    #region Photon Callbacks

    /// <summary>
    /// Called when a Photon Player got connected. We need to then load a bigger scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            //LoadArena();
        }

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            Debug.Log("Other player arrived");
            if (this.turnManager.Turn == 0)
            {
                pq.Clear();
                // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                this.StartTurn();
            }
        }
        else
        {
            Debug.Log("Waiting for another player");
        }
    }

    /// <summary>
    /// Called when a Photon Player got disconnected. We need to load a smaller scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            //LoadArena();
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
            for (int i = 0; i < battle_players.Count; i++) {
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

    #region Map Selection Methods

    public void StartSelecting(){
        selectingMap = true;
        GameObject.Find("Board").transform.Find("SelectionEntity").gameObject.SetActive(true);
        GameObject.Find("Panel").transform.Find("Confirmation").gameObject.SetActive(true);
        GameObject.Find("Panel").transform.Find("Cancel").gameObject.SetActive(true);
        GameObject.Find("SelectionEntity").transform.SetAsFirstSibling(); //move to the front (on parent)
        GameObject.Find("CurrentLocation").transform.SetAsLastSibling();
    }

    public void ConfirmSelecting(){
        selectingMap = false;
        currentLocation = changingLocation;
        CurrentEnvironment = map[(int)currentLocation.x, (int)currentLocation.y];
        GameObject.Find("Main Camera").GetComponent<SceneShift>().scene = CurrentEnvironment.envListIndex;
        GameObject.Find("CurrentLocation").GetComponent<RectTransform>().localPosition = new Vector3(currentLocation.x*60, currentLocation.y*60, 0f);
        waitingPlayer = false;
    }

    public void CancelSelecting(){
        selectingMap = false;
    }
    #endregion

    #region Map Generation Methods
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
        InitialiseList(rows, cols);
    }

    void InitialiseList (int rows, int cols)
    {
        //Clear our list gridPositions.
        gridPositions.Clear ();

        //Loop through x axis (columns).
        for(int x = 1; x < cols; x++)
        {
            //Within each column, loop through y axis (rows).
            for(int y = 1; y < rows; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add (new Vector3(x, y, 0f));
            }
        }
        MapSetup(rows, cols);
    }

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
                    Debug.Log($"setting CurrentLocation {x} {y}");
                    GameObject.Find("CurrentLocation").transform.SetParent (mapHolder);
                    GameObject.Find("CurrentLocation").GetComponent<RectTransform>().localPosition = new Vector3(x*60, y*60, 0f);
                    GameObject.Find("SelectionEntity").transform.SetParent (mapHolder);
                    GameObject.Find("SelectionEntity").GetComponent<RectTransform>().localPosition = new Vector3(x*60, y*60, 0f);
                    GameObject.Find("SelectionEntity").SetActive(false);
                    GameObject.Find("Confirmation").SetActive(false);
                    GameObject.Find("Cancel").SetActive(false);
                    CurrentEnvironment = map[x,y];
                }
            }
        }
        mapHolder.SetParent(GameObject.Find("Panel").transform);

        GameObject go = GameObject.Find("Panel");
        float width = go.GetComponent<RectTransform>().rect.width;
        float height = go.GetComponent<RectTransform>().rect.height;
        GameObject child = mapHolder.GetChild(0).gameObject;
        mapHolder.gameObject.GetComponent<RectTransform>().localPosition = new Vector3((-1*(width/2))+ (child.GetComponent<RectTransform>().rect.width/2)*2 + 100 , (-1*(height/2))+ (child.GetComponent<RectTransform>().rect.height/2)*5 + 100 , 0f);
        GameObject.Find("Main Camera").GetComponent<SceneShift>().scene = CurrentEnvironment.envListIndex;

    }
    #endregion
}
