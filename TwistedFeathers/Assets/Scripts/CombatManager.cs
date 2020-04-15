﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Math;

using Completed;
using TwistedFeathers;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CombatManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks, IPunObservable
{
    #region Fields
    SortedSet<BattleEffect> pq;
    List<TwistedFeathers.Player> battle_players;
    private List<Monster> battle_monsters;
    //Weather weath;
    int currentTurn;
    bool waitingPlayer;
    int protagonistIndex;
    public GameObject ForecastOpener;
    static string forecastText;
    public GameObject textPrefab;

    private GameObject newText;
    public GameObject forecastContent;
    public TwistedFeathers.Environment[,] map;
    int numTexts = 0;
    public int rows = 3;
    public int cols = 4;

    private Vector2 currentLocation;
    private Vector2 changingLocation;
    public TwistedFeathers.Environment CurrentEnvironment;

    public bool selectingMap = false;
    public bool validMove = true;

    public int allowedMoves = 2;

    public static string ForecastText { get => forecastText; set => forecastText = value; }

    public List<GameObject> playerSpawnPoints;
    public List<GameObject> enemySpawnPoints;

    public UIManager UIManager;
    
    public GameObject attackIndicator;
    private Skill chosenSkill;

    public bool selectingEnemy = false;
    private int chosenEnemy = 0;

    private bool resolving_effects = false;
    private bool waiting_effects = false;

    private PunTurnManager turnManager;

    [SerializeField]
    private string localSelection;

    [SerializeField]
    private string remoteSelection;

    public GameObject effectText;

    private int EXPGained = 0;

    private int deadMonsters = 0;
    private int deadPlayers = 0;

    #endregion

    #region Battle Methods
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

    public void renderPlayers(){
        int playerCount = 0;
        int enemyCount = 0;
        foreach(TwistedFeathers.Player player in battle_players){
            GameObject newPlayer = Instantiate(player.myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newPlayer.transform.SetParent(GameObject.Find("Participants").transform);
            newPlayer.transform.position = playerSpawnPoints[playerCount].transform.position;
            player.me = newPlayer;
            playerCount++;
        }

        foreach(Monster monster in battle_monsters){
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
    void queueSkill(Skill skill, Participant user, List<BattleParticipant> target)
    {
        List<BattleParticipant> real_target = new List<BattleParticipant>();
        foreach(BattleEffect effect in skill.Effects.ToArray())
        {
            real_target = new List<BattleParticipant>();
            switch(effect.TargetType)
            {
                case target_type.Self:
                    real_target.Add((BattleParticipant) user);
                    break;
                case target_type.AllEnemies:
                    foreach (TwistedFeathers.Monster monster in battle_monsters)
                    {
                        real_target.Add(monster);
                    }

                    break;
                case target_type.All:
                    foreach (TwistedFeathers.Monster monster in battle_monsters)
                    {
                        real_target.Add(monster);
                    }
                    foreach (TwistedFeathers.Player player in battle_players)
                    {
                        real_target.Add(player);
                    }
                    break;
                case target_type.AllAllies:
                    foreach (TwistedFeathers.Player player in battle_players)
                    {
                        real_target.Add(player);
                    }
                    break;
                case target_type.Ally:
                    Debug.LogError("Error: Ally targeting not supported");
                    break;
                case target_type.None: //TODO Remove this line
                case target_type.Enemy:
                    real_target = target;
                    break;
                default:
                    break;
            }
            BattleEffect battle_effect = new BattleEffect(effect);
            if(battle_effect.select(user, real_target, currentTurn, skill.Name))
            {
                if (!pq.Add(battle_effect))
                {
                    Debug.LogError("Error! " + battle_effect.SkillName + " <- Effect not queued!");
                }
            }
        }
    }

    

    //Method for resolving all effects set to happen this turn in PQ
    void resolveEffects()
    {
        UIManager.animateableButtons[3].GetComponent<Button>().interactable = false;
        UIManager.turnOptions.transform.GetChild(0).transform.GetComponent<Button>().interactable = false;
        UIManager.turnOptions.transform.GetChild(1).transform.GetComponent<Button>().interactable = false;
        Debug.Log("Effects Resolving!");
        resolving_effects = true;
        StartCoroutine(spaceOutEffects());
    }

    IEnumerator spaceOutEffects() {
        while (pq.Count != 0 && pq.Min.Turnstamp <= currentTurn) {
            Debug.Log("Resolving: " + pq.Min.SkillName);
            pq.Min.run();
            effectText.GetComponent<Text>().text = pq.Min.SkillName;
            pq.Remove(pq.Min);
            UIManager.actionOverlay.GetComponent<Animator>().Play("flyIn");
            for(int i = 0; i < battle_players.Count; i++){
                RectTransform healthBar = UIManager.playerHealthBars[i].transform.GetChild(0).GetComponent<RectTransform>();
                healthBar.sizeDelta = new Vector2(getHealthBarLengh((float)battle_players[i].Current_hp, 50f), 100);
            }
            Debug.Log("NumMonsters: " + battle_monsters.Count);
            for(int i = 0; i < battle_monsters.Count; i++){
                RectTransform healthBar = UIManager.enemyHealthBars[i].transform.GetChild(0).GetComponent<RectTransform>();
                Debug.Log("Monster " + i + "health: " + (float)battle_monsters[i].Current_hp);
                healthBar.sizeDelta = new Vector2(getHealthBarLengh((float)battle_monsters[i].Current_hp, 50f), 100);
            }
            yield return new WaitForSeconds(.5f);
            UIManager.actionOverlay.GetComponent<Animator>().Play("flyOut");
            yield return new WaitForSeconds(.5f);
        }
        Debug.Log("Effects Resolved");
        UIManager.animateableButtons[3].GetComponent<Button>().interactable = true;
        UIManager.turnOptions.transform.GetChild(0).transform.GetComponent<Button>().interactable = true;
        UIManager.turnOptions.transform.GetChild(1).transform.GetComponent<Button>().interactable = true;
        resolving_effects = false;
        waiting_effects = false;
        if (!GameManager.singlePlayer)
        {
            this.OnEndTurn();
        }
    }

    void resolveStatuses()
    {
        
        foreach (BattleParticipant bat_part in getBattleParticipants())
        {
            foreach (BattleEffect status in bat_part.Statuses)
            {
                status.run();
                status.Duration -= 1;
            }

            bat_part.Statuses.RemoveAll(status => status.Duration <= 0);

            foreach (BattleEffect buff in bat_part.Buffs.ToArray())
            {
                if (buff.Turnstamp <= currentTurn)
                {
                    buff.run();
                }
            }

            bat_part.Buffs.RemoveAll(buff => buff.Turnstamp <= currentTurn);
        }
        
    }

    void combatEnd(bool isVictory)
    {
        if (isVictory)
        {
            Debug.Log("Victory!!!");
            EXPGained += 5;
        }
        else
        {
            Debug.Log("Defeat...");
            EXPGained = 3;
        }
        GameManager.numWaves+= 1;
        if(GameManager.numWaves == GameManager.wavesRequired){
            passiveReset();
            GameManager.numBattles += 1;
            GameManager.numWaves = 0;
            GameObject.Find("GameManager").GetComponent<GameManager>().finishBattle(EXPGained);
        } else {
            nextWave();
        }
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("TestScene"));
    }
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        EXPGained = 0;
        //Set up UI
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        ForecastText = "";
        //Set up priority queue
        currentTurn = 0;
        pq = new SortedSet<BattleEffect>(new EffectComparator());
        //Set up BattleParticipants
        battle_players = new List<TwistedFeathers.Player>();
        battle_monsters = new List<Monster>();
        protagonistIndex = 0; 
        //Dummy values for testing purposes

        battle_players.Add((TwistedFeathers.Player)GameManager.Player_db["person A"]);
        battle_players.Add((TwistedFeathers.Player)GameManager.Player_db["person B"]);
        battle_monsters.Add((Monster)GameManager.Monster_db["enemy B"]);
        battle_monsters.Add((Monster)GameManager.Monster_db["enemy A"]);



        waitingPlayer = false;
        Debug.Log("TURN BEGIN");
        if (GameManager.singlePlayer)
        {
            buildMap(rows, cols);
        }
        else if (PhotonNetwork.PlayerList.Length == 1)
        {
            buildMap(rows, cols);
        } else
        {
            Debug.Log("Player has joined existing game so we do not build map normally");
        }

        if (!GameObject.Find("GameManager").GetComponent<GameManager>().rotate){
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
            UIManager.playerHealthBars[i].GetComponent<Animator>().SetBool("enter", true);
        }

        renderPlayers();

        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;

    }
    #region Skill Handling

    public void SkillTargetUI(Skill skill)
    {
        bool needUI = false;
        foreach (BattleEffect be in skill.Effects)
        {
            List<BattleParticipant> target = new List<BattleParticipant>();
            switch(be.TargetType)
            {
                case target_type.Self:
                case target_type.AllEnemies:
                case target_type.All:
                case target_type.AllAllies:
                    needUI = false;
                    break;
                case target_type.Ally:
                    Debug.LogError("Error: Don't support ally targeting at this time");
                    break;
                case target_type.Enemy:
                case target_type.None: //TODO Remove this line later
                    needUI = true;
                    break;
                default:
                    Debug.LogError("Error: Invalid Target Type");
                    break;
            }
            if (needUI)
            {
                break;
            }
        }

        if (needUI)
        {
            //TODO Change to support ally targeting as well
            selectingEnemy = true;
            attackIndicator.SetActive(true);
            moveIndicator(0);
            UIManager.animateableButtons[3].GetComponent<Button>().interactable = false;
            UIManager.turnOptions.transform.GetChild(0).transform.GetComponent<Button>().interactable = false;
            UIManager.turnOptions.transform.GetChild(1).transform.GetComponent<Button>().interactable = false;
            chosenSkill = skill;
        }

    }

    public void SelectSkill(Skill skill){
        // we know that this will be the selected skill
        // if one player don't network the move
        UIManager.turnOptions.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Select Skill";
        if (GameManager.singlePlayer)
        {
           if(deadMonsters < 1)
           {
                SkillTargetUI(skill);
                UIManager.SkillInfos.transform.GetChild(0).GetComponent<Animator>().Play("Pop Out");
                GameObject.Find("PlayerSkillInfo").GetComponent<Animator>().SetBool("Open", false);
                // TODO Need some way of seeing if thisd skill needs to be picked between enemies or not.  
           }
           else  
           {
              singlePlayerChooseSkill(skill);
           }
        }
        else if(PhotonNetwork.PlayerList.Length == 1)
        {
            if(deadMonsters < 1){
                SkillTargetUI(skill);
            }
            else  {
              singlePlayerChooseSkill(skill);
            }
        }
        else
        {
            if (deadMonsters < 1)
            {
                SkillTargetUI(skill);
            }
            else
            {
                // we want to select the skill and then send the skill name over the network
                // we can then search for the skill like in FillSkills class for the matching name
                MakeTurn(skill); // sends skill over network
            }
        }

    }

    /// <summary>
    /// Called during single player when they choose a skill.
    /// </summary>
    /// <param name="skill"></param>
    private void singlePlayerChooseSkill(Skill skill)
    {
        Debug.Log("Single Player choose skill");
        chooseRandomSkill(1);
        chooseSkill(0, skill);
        
    }
    
    /// <summary>
    /// Used to get the master client or protagonist's skills.
    /// </summary>
    /// <returns></returns>
    public List<Skill> GetActivePlayerSkills()
    {
        return battle_players[protagonistIndex].Skills;
    }

    /// <summary>
    /// Useed to get the allies or remote player's skill list.
    /// </summary>
    /// <returns></returns>
    public List<Skill> GetRemotePlayerSkills()
    {
        return battle_players[protagonistIndex + 1].Skills;
    }

    /// <summary>
    /// Called throughout the program to determine how many players are in a room.
    /// </summary>
    /// <returns></returns>
    public int getPhotonPlayerListLength()
    {
        return PhotonNetwork.PlayerList.Length;
    }

    
    /// <summary>
    /// Called when the master client updates the room properties with
    /// the chosen environment skill so that the remote client can
    /// choose the same skill.
    /// </summary>
    /// <param name="envSkill">name of the skill to be queued</param>
    public void processNetEnvSkill(string envSkill)
    {
        foreach (KeyValuePair<int, Skill> sk in CurrentEnvironment.Skills)
        {
            if (envSkill.Equals(sk.Value.Name))
            {
                Debug.Log("Queueing environment skill: " + sk.Value.Name);
                queueSkill(sk.Value, CurrentEnvironment, getBattleParticipants());
            }
        }
        
    }

    /// <summary>
    /// Called when the master client pdates the room properties with the
    /// enemy skill information of the current turn for the remote client
    /// to process and queue the same information.
    /// </summary>
    /// <param name="enemySkills"></param>
    public void processNetEnemySkills(string enemySkills)
    {
        // relies on the fact that the enemies are already in the same order within battle_monsters
        int i = 0;
        string[] enemySkillArray = enemySkills.Split(':');
        foreach(Monster mon in battle_monsters)
        {
            // index 0 is the name and index 1 is the target, user is the current monster in the loop
            string[] enemySkillInfo = enemySkillArray[i].Split(',');
            if (enemySkillInfo[0] != "") // if the skill info is not empty
            {
                foreach (Skill sk in mon.Skills)
                {
                    if (enemySkillInfo[0].Equals(sk.Name))
                    {
                        Debug.Log("Queueing enemy skill: " + mon.Name + " - "+ sk.Name);
                        queueSkill(sk, mon, new List<BattleParticipant>() { battle_players[Convert.ToInt32(enemySkillInfo[1])] });
                    }
                }
            }
            i++;
        }
    }

    /// <summary>
    /// Called when the player chooses a skill in single player
    /// </summary>
    /// <param name="index"></param>
    /// <param name="skill"></param>
    public void chooseSkill(int index, Skill skill){
        // set this method up to take a parameter index that specifies
        // whether the player or ally is choosing a skill
        // more work will have to be done with target selection
        attackIndicator.SetActive(false);
        selectingEnemy = false;
        TwistedFeathers.Player protag = (TwistedFeathers.Player) battle_players[index];
        Debug.Log("Queueing player skill: " + protag.Name + " - " + skill.Name);
        if(deadMonsters < 1){
            queueSkill(skill, protag, new List<BattleParticipant>() { battle_monsters[chosenEnemy] });
        } else {
            foreach (TwistedFeathers.Monster mon in battle_monsters.ToArray()){
                if(!mon.isDead){
                    queueSkill(skill, protag, new List<BattleParticipant>() { mon });
                }
            }
        }
        waitingPlayer = false;
        waiting_effects = true;
    }


    /// <summary>
    /// Called when a player chooses a skill in multiplayer.
    /// 
    /// Chooses a skill from the provided skill information
    /// so far skill information is of the format name,chosenEnemyIndex
    /// by accessing the list of skills 
    /// and finding the skill with the matching name
    /// </summary>
    /// <param name="skillInfo"></param>
    public void networkChooseSkill(string skillInfo)
    {
        attackIndicator.SetActive(false);
        selectingEnemy = false;
        Debug.Log(skillInfo);
        // first index of splitinfo should be the skill name
        // second index should be the chosen enemy index
        string[] splitInfo = skillInfo.Split(',');
        // need to find skill
        TwistedFeathers.Skill skill = null;
        TwistedFeathers.Player userP = null;
        if (skill == null && protagAlive)
        {
            foreach (Skill sk in GetActivePlayerSkills())
            {
                if (splitInfo[0].Equals(sk.Name))
                {
                    skill = sk;
                }
            }
            userP = (TwistedFeathers.Player)battle_players[0];
        } 
        if(skill == null && allyAlive)
        {
            foreach (Skill sk in GetRemotePlayerSkills())
            {
                if (splitInfo[0].Equals(sk.Name))
                {
                    skill = sk;
                }
            }
            userP = (TwistedFeathers.Player)battle_players[1];
        }
        //Debug.Log("Chosen Enemy: " + (Convert.ToInt32(splitInfo[1])));
        if (skill != null)
        {
            Debug.Log("Queueing player skill: " + userP.Name + " - " + skill.Name);
            queueSkill(skill, userP, new List<BattleParticipant>() { battle_monsters[Convert.ToInt32(splitInfo[1])] });
        }
        waitingPlayer = false;
        waiting_effects = true;
    }

    public void chooseRandomSkill(int index)
    {
        //attackIndicator.SetActive(false);
        //selectingEnemy = false;
        TwistedFeathers.Player protag = (TwistedFeathers.Player) battle_players[index]; 
        queueSkill(protag.Skills[UnityEngine.Random.Range(0, protag.Skills.Count)], protag, new List<BattleParticipant>() { battle_monsters[UnityEngine.Random.Range(0, battle_monsters.Count)] });
    }
    #endregion

    #region Map Selection
    public void StartSelecting(){
        // We want to begin selecting our environment change, so update the flow
        selectingMap = true;
        // deactivate buttons for the time being
        UIManager.selectionEntity.SetActive(true);
        // Enable the specific indicators for this portion
        UIManager.selectionEntity.transform.SetAsFirstSibling(); //move to the front (on parent)
        UIManager.selectionEntity.transform.SetAsLastSibling();
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
        
        // a turn has been taken
        if (!GameManager.singlePlayer && getPhotonPlayerListLength() == 2) // here we determine if we need to network this change
        {
            Hashtable mapSelectHash = new Hashtable();
            mapSelectHash.Add("mapPosX", (int)currentLocation.x);
            mapSelectHash.Add("mapPosY", (int)currentLocation.y);
            PhotonNetwork.CurrentRoom.SetCustomProperties(mapSelectHash);//update the room properties
            this.turnManager.SendMove((string)(""), true); // send an empty move to end their turn
        } else
        {
            chooseRandomSkill(1); // choose a random skill for the ally
        }
        // this is the usual two lines that end a turn
        waitingPlayer = false;
        waiting_effects = true;

        // TODO at some point a method will be called here to play some animations
    }

    public void changeNetEnvironment(int xPos, int yPos)
    {
        Debug.Log("A player changed the environment");
        currentLocation = new Vector2(xPos, yPos);
        CurrentEnvironment = map[(int)currentLocation.x, (int)currentLocation.y];
        //update the scene to represent the current environment
        GameObject.Find("Main Camera").GetComponent<SceneShift>().scene = CurrentEnvironment.envListIndex;
        //update the currentLocation UI indicator
        GameObject.Find("CurrentLocation").GetComponent<RectTransform>().localPosition = new Vector3(currentLocation.x * 60, currentLocation.y * 60, 0f);

    }

    public void CancelSelecting(){
        // if cancelled, we just go back to normal
        selectingMap = false;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (selectingEnemy){
            if (Input.GetKeyDown("w")) {
                //if up is pressed move the changing location over 1 unless at the edge
                moveIndicator(0);
            }
            //if down is pressed move the changing location over 1 unless at the edge
            if (Input.GetKeyDown("s")) {
                moveIndicator(1);
            }
            if(Input.GetKeyDown("return")){
                if(GameManager.singlePlayer)
                {
                    singlePlayerChooseSkill(chosenSkill);
                } 
                else if(getPhotonPlayerListLength() == 1)
                {
                    singlePlayerChooseSkill(chosenSkill);
                } 
                else if (getPhotonPlayerListLength() == 2)
                {
                    //must have this here to turn off the indicators for networking
                    attackIndicator.SetActive(false);
                    selectingEnemy = false;
                    MakeTurn(chosenSkill);
                }
            }
        }
        // if the UI is in the change environment mode there is a different flow for the update method
        else if(selectingMap){
            // We are now selecting where we want to move to so we need to activate and show it over our current changing position
            UIManager.selectionEntity.GetComponent<RectTransform>().localPosition = new Vector3 (changingLocation.x*60, changingLocation.y*60, -10f);
            //Checks to see if our move is out of scope
            int totalMove = Abs((int)(currentLocation.x - changingLocation.x)) + Abs((int)(currentLocation.y - changingLocation.y));
            if(totalMove <= allowedMoves) {
                // if the move is allowed the selection entity notifies the player by turning yellow and valid move is set to true
                UIManager.selectionEntity.GetComponent<Image>().color = Color.yellow;
                validMove = true;
            } else {
                //otherwise its grey and false
                validMove = false;
                UIManager.selectionEntity.GetComponent<Image>().color = Color.grey;
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
            if ((GameManager.singlePlayer || PhotonNetwork.PlayerList.Length == 1) && !waitingPlayer && !resolving_effects)
            {
                if (waiting_effects)
                {
                    resolveStatuses();
                    resolveEffects();
                }
                else
                {
                    singlePlayerTurn();
                }
            }
        }
        
    }

    public float getHealthBarLengh(float currentHealth, float maxHealth){
        return (currentHealth/maxHealth)*100;
    }
    #region Turn Handling
    // turn method used for single player mode and networked single player
    private void singlePlayerTurn()
    {
        foreach (Transform child in UIManager.popups[2].transform.GetChild(0).transform.GetChild(0).transform) {
            GameObject.Destroy(child.gameObject);
        }
        Debug.Log("TURN END");
        Debug.Log(battle_players[protagonistIndex].displayBuffs());
        //Check for BattleParticipant deaths
        CheckBattleParticipantDeaths();
        
    }

    private bool allyAlive = true;
    private bool protagAlive = true;

    private void CheckBattleParticipantDeaths()
    {
        int playerCount = 0;
        foreach (TwistedFeathers.Player play in battle_players.ToArray())
        {
            if(!play.isDead){
                if (play.Current_hp <= 0)
                {
                    Debug.Log("HE DED");
                    UIManager.playerHealthBars[playerCount].SetActive(false);
                    GameObject.Find("Participants").transform.GetChild(playerCount + 1).gameObject.SetActive(false);
                    if(playerCount == 0)
                    {
                        protagAlive = false;
                    } else
                    {
                        allyAlive = false;
                    }
                    play.isDead = true;
                    deadPlayers++;
                }
            }
            playerCount++;
        }
        int enemyCount = 0;
        foreach (Monster mon in battle_monsters.ToArray())
        {   
            if(!mon.isDead){
                if (mon.Current_hp <= 0)
                {
                    UIManager.enemyHealthBars[enemyCount].SetActive(false);
                    GameObject.Find("Participants").transform.GetChild(3 + enemyCount).gameObject.SetActive(false);
                    mon.isDead = true;
                    deadMonsters++;
                }
            }
            enemyCount++;
        }
        if (battle_players.Count == deadPlayers)
        {
            combatEnd(false);
            waitingPlayer = true;
        }
        else if (battle_monsters.Count == deadMonsters)
        {
            combatEnd(true);
            waitingPlayer = true;
        }
        else if(GameManager.singlePlayer || getPhotonPlayerListLength() == 1)
        {
            TurnBegin();
        }
    }

    private void TurnBegin()
    {
        //Testing HP damage
        // Debug.Log("Adam HP: " + battle_players[protagonistIndex].Current_hp);
        // Debug.Log("Adam Acc: " + battle_players[protagonistIndex].Accuracy);
        // Debug.Log("Ben HP: " + battle_players[protagonistIndex + 1].Current_hp);
        // Debug.Log("Azazel 1 HP: " + battle_monsters[protagonistIndex].Current_hp);
        // Debug.Log("Beelzebub HP: " + battle_monsters[protagonistIndex + 1].Current_hp);
        // Debug.Log("Beelzebub Acc: " + battle_monsters[0].Accuracy);
        //New turn beings here
        Debug.Log("TURN BEGIN");

        // used to network the enemy and environment skills
        Hashtable skillHash = new Hashtable(); 
        //mapHash.Add("map", mapNetRep);
        currentTurn++;

        string enemySkillInfos = "";

        // this string will hold all enemy skill information in  the format
        // "enemy1SkillName,enemy1Target:enemy2SkillName,enemy2Target"
        // we know which monster is using the skill based on the fact that they are added in
        // linear order
        
        //Choose environment skill
        List<int> partial_sums = new List<int>();
        int current_sum = 0;
        foreach (KeyValuePair<int, Skill> env_skill in CurrentEnvironment.Skills)
        {
            current_sum += env_skill.Key;
            partial_sums.Add(current_sum);
        }

        int random_draw = UnityEngine.Random.Range(0, partial_sums[partial_sums.Count - 1]);
        int random_index = 0;
        for (int i = 0; i < partial_sums.Count; i++)
        {
            if (random_draw < partial_sums[i])
            {
                random_index = i;
                break;
            }
        }
        
        Skill chosenEnvSkill = CurrentEnvironment.Skills[random_index].Value;
        queueSkill(chosenEnvSkill, CurrentEnvironment, getBattleParticipants());
        skillHash.Add("envSkill", chosenEnvSkill.Name); //only need the name of the environment skill
        // Choose enemy skills

        foreach (Monster part in battle_monsters)
        {
            Skill chosenEnemySkill = part.Skills[UnityEngine.Random.Range(0, part.Skills.Count)];
            int targetIndex = UnityEngine.Random.Range(0, battle_players.Count);
            queueSkill(chosenEnemySkill, part, new List<BattleParticipant>() { battle_players[targetIndex] });
            enemySkillInfos += (chosenEnemySkill.Name + "," + targetIndex + ":");
            
        }

        skillHash.Add("enemySkills", enemySkillInfos);
        if (!GameManager.singlePlayer && getPhotonPlayerListLength() == 2)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(skillHash);
        }
        //Forecast

        Debug.Log("Forecast Begins!");
        numTexts = 0;
        foreach (BattleEffect eff in pq)
        {
            if (eff.Visible)
            {
                newText = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                newText.transform.SetParent(forecastContent.transform, false);
                newText.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
                newText.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
                newText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0 -(40 * numTexts) - 30, 0);
                newText.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 45);
                string prediction =  eff.Turnstamp - currentTurn + " turns away: " + eff.SkillName + " targeting: --";
                foreach (BattleParticipant tar in eff.Target)
                {
                    prediction += tar.Name + "--";
                }
                newText.GetComponent<Text>().text = prediction;
                newText.GetComponent<Text>().color = Color.white;
                newText.GetComponent<Text>().fontSize = 32;


                numTexts++;
                Debug.Log(prediction);
            }
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
        if (PhotonNetwork.IsMasterClient)
        {
            TurnBegin();
            if (!protagAlive)
            {
                this.turnManager.SendMove((string)(""), true);
            }
        }
        else //not the master client
        {
            if (!allyAlive)
            {
                this.turnManager.SendMove((string)(""), true);
            }
        }
    }

    /// <summary>
    /// Once all players make a selection then the selections are processed.
    /// Effects are resolved and the turn is ended.
    /// </summary>
    public void OnTurnCompleted(int obj)
    {
        Debug.Log("OnTurnCompleted: " + obj);

        if (this.localSelection != null)
        {
            // once we get an actual functioning choose skill
            // we can pass in the selection
            networkChooseSkill(localSelection);
        }
        if (this.remoteSelection != null)
        {
            //attacks.Enqueue(this.remoteSelection);
            networkChooseSkill(remoteSelection);
        }
        Debug.Log("Calling execute skills");

        this.resolveEffects();
    }

    /// <summary>
    /// Called when a when a player moved (but did not finish the turn).
    /// </summary>
    public void OnPlayerMove(Photon.Realtime.Player photonPlayer, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);
    }


    /// <summary>
    /// Called when a player made the last/final move in a turn
    /// </summary>
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


    // Interface method Not Implemented
    public void OnTurnTimeEnds(int obj)
    {
        //do nothing
    }

    #endregion
    /// <summary>
    /// Starts a turn.
    /// </summary>
    public void StartTurn()
    {
        // The master client determines all of the enemy and environment skills chosen
        // these are networked to the other player using room properties
        if (PhotonNetwork.IsMasterClient)
        {
            this.turnManager.BeginTurn();
        }
    }
    /// <summary>
    /// Called when a Photon Player makes a turn selection.
    /// </summary>
    public void MakeTurn(Skill selection)
    {
        this.turnManager.SendMove((string)(selection.Name + "," + chosenEnemy), true);
    }
    /// <summary>
    /// Begins the next turn once all turns are finished.
    /// </summary>
    public void OnEndTurn()
    {
        foreach (Transform child in UIManager.popups[2].transform.GetChild(0).transform.GetChild(0).transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        CheckBattleParticipantDeaths();
        Debug.Log("TURN END");
        //Debug.Log("Calling start turn");
        //this.StartTurn();
        this.StartCoroutine("BeginNextTurnCoroutine");
    }

    /// <summary>
    /// Starts the next turn.
    /// </summary>
    public IEnumerator BeginNextTurnCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("Calling start turn");
        this.StartTurn();
    }
    #endregion

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
            PhotonNetwork.CurrentRoom.SetCustomProperties(mapHash);
            // clear the queue when player joins to avoid duplicate enemy skill selection
            pq.Clear();
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
    /// Called when room properties are updated.
    /// Room properties are currently updated to synchronize skills
    /// and the map for the remote client.
    /// </summary>
    /// <param name="propertiesThatChanged">This parameter is a hashtable of the 
    /// room properties that were updated.</param>
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // Master client logic
        if (!PhotonNetwork.IsMasterClient)
        {
            if (propertiesThatChanged.ContainsKey("map"))
            {
                string mapString = (string)propertiesThatChanged["map"];
                Debug.Log("Received map representation: " + mapString);
                buildMapRemoteClient(mapString);
            }
            if (propertiesThatChanged.ContainsKey("envSkill"))
            {
                processNetEnvSkill((string)propertiesThatChanged["envSkill"]);
            }
            if (propertiesThatChanged.ContainsKey("enemySkills"))
            {
                processNetEnemySkills((string)propertiesThatChanged["enemySkills"]);
            }
        }
        // Both clients logic
        if (propertiesThatChanged.ContainsKey("mapPosX") && propertiesThatChanged.ContainsKey("mapPosY"))
        {
            changeNetEnvironment((int)propertiesThatChanged["mapPosX"], (int)propertiesThatChanged["mapPosY"]);
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

    /// <summary>
    /// Used to synchronize values across network.
    /// Although it looks like it doesn't actually do anything.
    /// </summary>
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
            //stream.SendNext(map);
            stream.SendNext(CurrentEnvironment);
            //stream.SendNext(mapHolder);
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
            //map = (Environment[,])stream.ReceiveNext();
            CurrentEnvironment = (TwistedFeathers.Environment)stream.ReceiveNext();
            //mapHolder = (Transform)stream.ReceiveNext();
        }
    }

    #endregion
    
    private Hashtable mapHash; // used to update the room properties in OnPlayerEnteredRoom
    //Randomly generates the 2D array that represents the UI Map
    public void buildMap(int rows, int cols){
        // hashtable used to synchronize the map
        mapHash = new Hashtable();
        // string representation of the map used as a value in the hashtable to network across
        string mapNetRep = "";
        map = new TwistedFeathers.Environment[cols, rows];
        map[cols-1,rows-1] = GameManager.environments[0];
        for (int i = 0; i < cols; i++){
            for(int j = 0; j < rows; j++){
                float randomInt = UnityEngine.Random.Range(0f, 10.0f);
                if(randomInt >=0 && randomInt <=3){
                    map[i,j] = GameManager.environments[0];
                    mapNetRep += "0";
                } else if(randomInt >=3 && randomInt <=9){
                    map[i,j] = GameManager.environments[1];
                    mapNetRep += "1";
                } else if(randomInt >=9 && randomInt <=10){
                    map[i,j] = GameManager.environments[GameManager.environments.Count-1];
                    mapNetRep += "" + (GameManager.environments.Count - 1);
                }
            }
        }

        // add map array to the hashtable
        mapHash.Add("map", mapNetRep);
        //Debug.Log("Sent Array dimensions: " + cols + ", " + rows);
        //Debug.Log("Sent Map Array Rep: " + mapNetRep);

        for (int i = 0; i < cols; i++){
            for(int j = 0; j < rows; j++){
                
                if (map[i,j] != GameManager.environments[GameManager.environments.Count-1]){
                    currentLocation = new Vector2(i, j);
                    changingLocation = new Vector2(i,j);
                    break;
                }
            }
        }
        MapSetup(rows, cols);
    }

    //Generates a 2D map based on the map data received remotely
    public void buildMapRemoteClient(string mapData)
    {
        // k is a counter used to index into the mapData string
        // the map data string holds elements like this
        // Index | Point on the map
        // ------------------------
        // 0     | (0,0)
        // 1     | (0,1)
        // 2     | (1,0)
        // 3     | (1,1)
        // and so on in this fashion up to 10
        int k = 0;
        int conversionFactor = 48;
        map = new TwistedFeathers.Environment[cols, rows];
        map[cols - 1, rows - 1] = GameManager.environments[0];
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                // could probaly use smaller int
                // convert converts the string "1" to its html value of 49 thus we minus a conversion factor of 48
                map[i, j] = GameManager.environments[Convert.ToInt32(mapData[k]) - conversionFactor];
                k++;
            }
        }
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (map[i, j] != GameManager.environments[GameManager.environments.Count - 1])
                {
                    currentLocation = new Vector2(i, j);
                    changingLocation = new Vector2(i, j);
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
                    UIManager.selectionEntity.transform.SetParent (mapHolder);
                    UIManager.selectionEntity.GetComponent<RectTransform>().localPosition = new Vector3(x*60, y*60, 0f);
                    UIManager.selectionEntity.SetActive(false); //move to the front (on parent)
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

    public void nextWave(){
        UIManager.selectionEntity.transform.SetParent(GameObject.Find("Panel").transform);
        UIManager.locationTracker.transform.SetParent(GameObject.Find("Panel").transform);
        Destroy(GameObject.Find("Board"));
        if (GameManager.singlePlayer)
        {
            buildMap(rows, cols);
        }
        else if (PhotonNetwork.PlayerList.Length == 1)
        {
            buildMap(rows, cols);
        } else
        {
            Debug.Log("Player has joined existing game so we do not build map normally");
        }
        waitingPlayer = true;
        currentTurn = 0;
        deadPlayers = 0;
        deadMonsters = 0;
        UIManager.beginFinish();
        effectText.GetComponent<Text>().text = "Beginning Wave" + (GameManager.numWaves+1);
            
            UIManager.actionOverlay.GetComponent<Animator>().Play("flyIn");
        foreach(TwistedFeathers.Monster mon in battle_monsters.ToArray()){
            mon.isDead = false;
            mon.Current_hp = mon.Max_hp;
        }
        foreach(TwistedFeathers.Player play in battle_players.ToArray()){
            play.isDead = false;
            play.Current_hp += 20;
        }
        for(int i = 1; i <= 4; i++){
            GameObject.Find("Participants").transform.GetChild(i).gameObject.SetActive(true);
        }
        for(int i = 0; i < battle_monsters.Count; i++){
            UIManager.enemyHealthBars[i].GetComponent<Animator>().enabled = true;
            UIManager.enemyHealthBars[i].SetActive(true);
            UIManager.enemyHealthBars[i].GetComponent<Animator>().SetBool("enter", true);
            UIManager.enemyHealthBars[i].GetComponent<Animator>().SetBool("enter", true);
        }
        for(int i = 0; i < battle_players.Count; i++){
            RectTransform healthBar = UIManager.playerHealthBars[i].transform.GetChild(0).GetComponent<RectTransform>();
            healthBar.sizeDelta = new Vector2(getHealthBarLengh((float)battle_players[i].Current_hp, 50f), 100);
        }
        Debug.Log("NumMonsters: " + battle_monsters.Count);
        for(int i = 0; i < battle_monsters.Count; i++){
            RectTransform healthBar = UIManager.enemyHealthBars[i].transform.GetChild(0).GetComponent<RectTransform>();
            Debug.Log("Monster " + i + "health: " + (float)battle_monsters[i].Current_hp);
            healthBar.sizeDelta = new Vector2(getHealthBarLengh((float)battle_monsters[i].Current_hp, 50f), 100);
        }

    }

    public void passiveReset(){
        currentTurn = 0;
        deadPlayers = 0;
        deadMonsters = 0;
        foreach(TwistedFeathers.Monster mon in battle_monsters.ToArray()){
            mon.isDead = false;
            mon.Current_hp = mon.Max_hp;
        }
        foreach(TwistedFeathers.Player play in battle_players.ToArray()){
            play.isDead = false;
            play.Current_hp = play.Max_hp;
        }
    }
}
