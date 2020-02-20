using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    SortedSet<BattleEffect> pq;
    ArrayList battle_participants;
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

    public static string ForecastText { get => forecastText; set => forecastText = value; }

    //Method for taking a skill and queueing the effect into the PQ
    void queueSkill(Skill skill, Participant user, Participant target)
    {
        BattleEffect effect = skill.Effect;
        effect.User = user;
        effect.Target = target;
        effect.Turnstamp = currentTurn;
        pq.Add(effect);
    }

    //Method for resolving all effects set to happen this turn in PQ
    void resolveEffects()
    {
        Debug.Log("Effects Resolving!");
        while (pq.Count != 0 && pq.Min.Turnstamp <= currentTurn)
        {
            Debug.Log(pq.Min.Message);
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
        battle_participants = new ArrayList();
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
        queueSkill((Skill) protag.Skills[Random.Range(0, protag.Skills.Count)], protag, null);
        waitingPlayer = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!waitingPlayer)
        {
            //Effects are resolved and turn ends
            resolveEffects();
            Debug.Log("TURN END");
            //New turn beings here
            Debug.Log("TURN BEGIN");
            currentTurn++;
            foreach (Participant part in battle_participants)
            {
                if (part.Type != p_type.player)
                {
                    queueSkill((Skill) part.Skills[Random.Range(0, part.Skills.Count)], part, null);
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

    public void buildMap(int rows, int cols){
        map = new Environment[cols, rows];
        for(int i = 0; i < cols; i++){
            for(int j = 0; j < rows; j++){
                map[i,j] = GameManager.environments[0];
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

        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for(int x = 0; x < cols; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for(int y = 0; y < rows; y++)
            {
                GameObject toInstantiate = map[x,y].miniIcon;
                GameObject instance = Instantiate (toInstantiate, new Vector3 (x*60, y*60, 0f), Quaternion.identity) as GameObject;
                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent (mapHolder);

            }
        }
        mapHolder.SetParent(GameObject.Find("Panel").transform);

        GameObject go = GameObject.Find("Panel");
        float width = go.GetComponent<RectTransform>().rect.width;
        float height = go.GetComponent<RectTransform>().rect.height;
        Debug.Log(width);
        GameObject child = mapHolder.GetChild(0).gameObject;
        mapHolder.gameObject.GetComponent<RectTransform>().localPosition = new Vector3((-1*(width/2))+ (child.GetComponent<RectTransform>().rect.width/2)*2 + 100 , (-1*(height/2))+ (child.GetComponent<RectTransform>().rect.height/2)*5 + 100 , 0f);
    }
}
