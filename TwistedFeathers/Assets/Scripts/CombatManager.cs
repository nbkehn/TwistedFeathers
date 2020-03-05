using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;

using Completed;
using TwistedFeathers;

public class CombatManager : MonoBehaviour
{
    SortedSet<BattleEffect> pq;
    List<Player> battle_players;
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

    public List<GameObject> playerSpawnPoints;
    public List<GameObject> enemySpawnPoints;

    public UIManager UIManager;

    public GameObject attackIndicator;

    public bool selectingEnemy = false;
    private int chosenEnemy = 0;

    List<BattleParticipant> getBattleParticipants()
    {
        List<BattleParticipant> list = new List<BattleParticipant>();
        foreach (Monster mon in battle_monsters)
        {
            list.Add(mon);
        }

        foreach (Player play in battle_players)
        {
            list.Add(play);
        }

        return list;

    }

    public void renderPlayers(){
        int playerCount = 0;
        int enemyCount = 0;
        foreach(Participant player in battle_players){
            GameObject newPlayer = Instantiate(player.myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newPlayer.transform.SetParent(GameObject.Find("Participants").transform);
            newPlayer.transform.position = playerSpawnPoints[playerCount].transform.position;
            player.me = newPlayer;
            playerCount++;
        }

        foreach(Participant monster in battle_monsters){
            GameObject newEnemy = Instantiate(monster.myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newEnemy.transform.SetParent(GameObject.Find("Participants").transform);
            newEnemy.transform.position = enemySpawnPoints[enemyCount].transform.position;
            monster.me = newEnemy;
            enemyCount++;
        }
    }

    public void moveIndicator(int enemy){
        attackIndicator.transform.localPosition = enemySpawnPoints[enemy].transform.localPosition;
        attackIndicator.transform.localPosition = attackIndicator.transform.localPosition + new Vector3(0, 8, 0);
        chosenEnemy = enemy;
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
        battle_players = new List<Player>();
        battle_monsters = new List<Monster>();
        protagonistIndex = 0; 
        //Dummy values for testing purposes
        battle_players.Add((Player) GameManager.Participant_db["person A"]);
        battle_monsters.Add((Monster) GameManager.Participant_db["enemy B"]);
        battle_monsters.Add((Monster) GameManager.Participant_db["enemy A"]);

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
        for(int i = 0; i < battle_monsters.Count; i++){
            UIManager.enemyHealthBars[i].SetActive(true);
            UIManager.enemyHealthBars[i].GetComponent<Animator>().SetBool("enter", true);
        }
        for(int i = 0; i < battle_players.Count; i++){
            UIManager.playerHealthBars[i].SetActive(true);
            UIManager.enemyHealthBars[i].GetComponent<Animator>().SetBool("enter", true);
        }

        renderPlayers();

    }
    
    public void SelectSkill(string skill){
        if(battle_monsters.Count > 1){
            selectingEnemy = true;
            attackIndicator.SetActive(true);
            moveIndicator(0);
        }
        else  {
            chooseSkill();
        }
        Debug.Log(skill); // change this to actually do what the skill does
    }

    public void chooseSkill(){
        attackIndicator.SetActive(false);
        selectingEnemy = false;
        Player protag = (Player) battle_players[protagonistIndex]; 
        queueSkill( protag.Skills[Random.Range(0, protag.Skills.Count)], protag, new List<BattleParticipant>(){battle_monsters[chosenEnemy]});
        waitingPlayer = false;
    }

    public void StartSelecting(){
        // We want to begin selecting our environment change, so update the flow
        selectingMap = true;
        // deactivate buttons for the time being
        GameObject.Find("Board").transform.Find("SelectionEntity").gameObject.SetActive(true);
        // Enable the specific indicators for this portion
        GameObject.Find("SelectionEntity").transform.SetAsFirstSibling(); //move to the front (on parent)
        GameObject.Find("CurrentLocation").transform.SetAsLastSibling();
    }

    public void ConfirmSelecting(){
        Debug.Log("CONFIRMED");
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
        if(selectingEnemy){
            if (Input.GetKeyDown("w")) {
                //if up is pressed move the changing location over 1 unless at the edge
                moveIndicator(0);
            }
            //if down is pressed move the changing location over 1 unless at the edge
            if (Input.GetKeyDown("s")) {
                moveIndicator(1);
            }
            if(Input.GetKeyDown("return")){
                chooseSkill();
            }
        }
        // if the UI is in the change environment mode there is a different flow for the update method
        else if(selectingMap){
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
            if (!waitingPlayer)
            {
                //Effects are resolved and turn ends
                resolveEffects();
                //resolveStatuses();
                //Testing HP damage
                for(int i = 0; i < battle_players.Count; i++){
                    RectTransform healthBar = UIManager.playerHealthBars[i].transform.GetChild(0).GetComponent<RectTransform>();
                    healthBar.sizeDelta = new Vector2(getHealthBarLengh((float)battle_players[i].Current_hp, 50f), 100);
                }
                for(int i = 0; i < battle_players.Count; i++){
                    RectTransform healthBar = UIManager.enemyHealthBars[i].transform.GetChild(0).GetComponent<RectTransform>();
                    healthBar.sizeDelta = new Vector2(getHealthBarLengh((float)battle_monsters[i].Current_hp, 50f), 100);
                }
                Debug.Log("Adam HP: " + battle_players[protagonistIndex].Current_hp);
                Debug.Log("TURN END");
                //New turn beings here
                Debug.Log("TURN BEGIN");
                currentTurn++;
                foreach (Monster part in battle_monsters)
                {
                    queueSkill((Skill) part.Skills[Random.Range(0, part.Skills.Count)], part, new List<BattleParticipant>() { battle_players[protagonistIndex]});
                }

                //Forecast

                Debug.Log("Forecast Begins!");
                
                foreach (BattleEffect eff in pq)
                {
                    newText = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    newText.transform.SetParent(forecastContent.transform, false);
                    newText.GetComponent<RectTransform>().localScale = new Vector3(0.6968032f, 1.7355f, 1.7355f);
                    newText.transform.position = new Vector3(forecastContent.transform.position.x + 275, forecastContent.transform.position.y - 40*numTexts -30);
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

    public float getHealthBarLengh(float currentHealth, float maxHealth){
        return (currentHealth/maxHealth)*100;
    }

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
