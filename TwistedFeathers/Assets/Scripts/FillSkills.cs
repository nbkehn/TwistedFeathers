using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TwistedFeathers;

public class FillSkills : MonoBehaviour
{
    public GameObject Content;
    private Dictionary<string, Skill> skills;
    Dictionary<string, Skill> checkESkills;
    public GameObject buttonPrefab;
    private GameObject newButton;
    public CombatManager manager;
    private string clickedSkill = "";
    private string clickedEnemySkill = "";
    // used for the necromancer skill fear curse
    private int disableSkill;

    public int DisableSkill
    {
        get => disableSkill;
        set => disableSkill = value;
    }

    void Start(){
        skills = new Dictionary<string, Skill>();
        checkESkills = new Dictionary<string, Skill>();
        if (GameManager.singlePlayer)
        {
            foreach (Skill sk in manager.GetComponent<CombatManager>().GetActivePlayerSkills())
            {
                skills.Add(sk.Name, sk);
            }
        } else if(manager.GetComponent<CombatManager>().getPhotonPlayerListLength() == 1)
        {
            foreach (Skill sk in manager.GetComponent<CombatManager>().GetActivePlayerSkills())
            {
                skills.Add(sk.Name, sk);
            }
        }

        disableSkill = int.MinValue;
    }
    
    // Update is called once per frame
    public void FillSkillList()
    {
        // this only needs to run when a remote player chooses select skill
        // A remote player is not a master client and the photon player list would be two
        // should not mess with single player as photonplayerlistlength returns 0 on single player
        if (!manager.GetComponent<CombatManager>().isMasterClient() && manager.GetComponent<CombatManager>().getPhotonPlayerListLength() == 2)
        {
            foreach (Skill sk in manager.GetComponent<CombatManager>().GetRemotePlayerSkills())
            {
                if(skills.TryGetValue(sk.Name, out Skill value))
                {
                    // do nothing
                } else
                {
                    skills.Add(sk.Name, sk);
                }
                
            }
        }
        int numButtons = 0;
        
        int disableCounter = 0;

        foreach(System.Collections.Generic.KeyValuePair<string, Skill> sk in skills){
            newButton = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newButton.transform.SetParent(Content.transform, false);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newButton.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1.0f);
            newButton.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1.0f);
            newButton.GetComponent<RectTransform>().pivot = new Vector2(0f, 1.0f);
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(20, -1 * (40*numButtons + 20) , 0);
            numButtons++;
            newButton.transform.GetChild(0).GetComponent<Text>().text = sk.Value.Name;
            newButton.GetComponent<Button>().onClick.AddListener(() => {
                manager.GetComponent<CombatManager>().SelectSkill(sk.Value);
            });
            newButton.GetComponent<Button>().onClick.AddListener(() => {
                Close();
            });
            newButton.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
                GameObject.Find("PlayerSkillInfo").transform.GetChild(0).GetComponent<Text>().text =sk.Value.Description;
                string clicked = sk.Value.Name;

                Animator skillInfoAnimator = GameObject.Find("PlayerSkillInfo").GetComponent<Animator>();
            
                if(!skillInfoAnimator.GetBool("Open"))
                {
                    
                    skillInfoAnimator.Play("Click");
                    skillInfoAnimator.SetBool("Open", true);
                }
                else if(skillInfoAnimator.GetBool("Open") && clicked == clickedSkill){
                    
                    skillInfoAnimator.Play("Pop Out");
                    skillInfoAnimator.SetBool("Open", false);
                }
                clickedSkill = sk.Value.Name;
            });
            if(disableSkill == disableCounter)
            {
                newButton.GetComponent<Button>().interactable = false; // disable the button if the necromancer skill is used
                Debug.Log("Disabled skill: " + sk.Value.Name);
            }
            disableCounter++;
        }
    }

    public void FillEnemyList(){
        foreach (Skill sk in manager.GetComponent<CombatManager>().GetEnemySkills())
        {
            if (checkESkills.TryGetValue(sk.Name, out Skill value))
            {
                // do nothing
            }
            else
            {
                checkESkills.Add(sk.Name, sk);
            }

        }
        int numButtons = 0;
        foreach(System.Collections.Generic.KeyValuePair<string, Skill> sk in checkESkills)
        {
            Debug.Log(sk.Value.Name);
            newButton = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newButton.transform.SetParent(Content.transform, false);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newButton.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1.0f);
            newButton.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1.0f);
            newButton.GetComponent<RectTransform>().pivot = new Vector2(0f, 1.0f);
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(-290, -1 * (40*numButtons + 20) , 0);
            numButtons++;
            newButton.transform.GetChild(0).GetComponent<Text>().text = sk.Value.Name;
            newButton.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
                GameObject.Find("EnemySkillInfo").transform.GetChild(0).GetComponent<Text>().text =sk.Value.Description;
                string clicked = sk.Value.Name;

                Animator enemyInfoAnimator = GameObject.Find("EnemySkillInfo").GetComponent<Animator>();
            
                if(!enemyInfoAnimator.GetBool("Open"))
                {
                    
                    enemyInfoAnimator.Play("Click");
                    enemyInfoAnimator.SetBool("Open", true);
                }
                else if(enemyInfoAnimator.GetBool("Open") && clicked == clickedEnemySkill){
                    
                    enemyInfoAnimator.Play("Pop Out");
                    enemyInfoAnimator.SetBool("Open", false);
                }
                clickedEnemySkill = sk.Value.Name;
            });
        }
    }

    public void Close(){
        Content.transform.parent.parent.gameObject.SetActive(false);
        if(GameObject.Find("PlayerSkillInfo").GetComponent<Animator>().GetBool("Open")){
            GameObject.Find("PlayerSkillInfo").GetComponent<Animator>().Play("Pop Out");
            GameObject.Find("PlayerSkillInfo").GetComponent<Animator>().SetBool("Open", false);
        }
    }
}
