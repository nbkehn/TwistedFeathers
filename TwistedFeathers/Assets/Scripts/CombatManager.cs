using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;

using Completed;

public class CombatManager : MonoBehaviour
{
    SortedSet<BattleEffect> pq;
    List<Participant> battle_participants;
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

    //Method for taking a skill and queueing the effect into the PQ
    void queueSkill(Skill skill, Participant user, Participant target)
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

    // Start is called before the first frame update
    void Start()
    {

        ForecastText = "";
        currentTurn = 0;
        pq = new SortedSet<BattleEffect>(new EffectComparator());
        battle_participants = new List<Participant>();
        protagonistIndex = 0; 
        //Dummy values for testing purposes
        battle_participants.Add(GameManager.Participant_db["person A"]);
        battle_participants.Add(GameManager.Participant_db["enemy B"]);

        waitingPlayer = false;
        Debug.Log("TURN BEGIN");
        buildMap(rows, cols);
    }
    
    public void SelectSkill(string skill){
        Debug.Log(skill); // change this to actually do what the skill does
        chooseSkill();
    }

    public void chooseSkill(){
        Player protag = (Player) battle_participants[protagonistIndex]; 
        queueSkill( protag.Skills[Random.Range(0, protag.Skills.Count)], protag, null);
        waitingPlayer = false;
    }

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
    // Update is called once per frame
    void Update()
    {
        if(selectingMap){
            GameObject.Find("SelectionEntity").GetComponent<RectTransform>().localPosition = new Vector3 (changingLocation.x*60, changingLocation.y*60, -10f);
            int totalMove = Abs((int)(currentLocation.x - changingLocation.x)) + Abs((int)(currentLocation.y - changingLocation.y));
            if(totalMove <= allowedMoves) {
                GameObject.Find("SelectionEntity").GetComponent<Image>().color = Color.yellow;
                    validMove = true;
            } else {
                validMove = false;
                GameObject.Find("SelectionEntity").GetComponent<Image>().color = Color.grey;
            }
            if (Input.GetKeyDown("left")) {
                if(changingLocation.x != 0 && map[(int)changingLocation.x-1, (int)changingLocation.y] != GameManager.environments[GameManager.environments.Count-1]){
                    changingLocation.x = changingLocation.x-1;
                }
            }
            if (Input.GetKeyDown("up")) {
                if(changingLocation.y != rows-1 && map[(int)changingLocation.x, (int)changingLocation.y+1] != GameManager.environments[GameManager.environments.Count-1]){
                    changingLocation.y = changingLocation.y+1;
                }
            }
            if (Input.GetKeyDown("right")) {
                if(changingLocation.x != cols-1 && map[(int)changingLocation.x+1, (int)changingLocation.y] != GameManager.environments[GameManager.environments.Count-1]){
                    changingLocation.x = changingLocation.x+1;
                }
            }
            if (Input.GetKeyDown("down")) {
                if(changingLocation.y != 0 && map[(int)changingLocation.x, (int)changingLocation.y-1] != GameManager.environments[GameManager.environments.Count-1]){
                    changingLocation.y = changingLocation.y-1;
                }
            }
        } else {
            if (!waitingPlayer)
            {
                //Effects are resolved and turn ends
                resolveEffects();
                //Testing HP damage
                Debug.Log("Adam HP: " + battle_participants[protagonistIndex].Current_hp);
                Debug.Log("TURN END");
                //New turn beings here
                Debug.Log("TURN BEGIN");
                currentTurn++;
                foreach (Participant part in battle_participants)
                {
                    if (part.Type != p_type.player)
                    {
                        queueSkill((Skill) part.Skills[Random.Range(0, part.Skills.Count)], part, battle_participants[protagonistIndex]);
                    }
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

    private List <Vector3> gridPositions = new List <Vector3> ();

    private Transform mapHolder; 

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
}
