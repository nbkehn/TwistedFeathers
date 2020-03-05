using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TwistedFeathers;

public class FillSkills : MonoBehaviour
{
    public GameObject Content;
    private Dictionary<string, Skill> skills;
    public GameObject buttonPrefab;
    private GameObject newButton;
    public CombatManager manager;

    void Start(){
        skills = new Dictionary<string, Skill>();
        foreach (Skill sk in manager.GetComponent<CombatManager>().GetActivePlayerSkills())
        {
            skills.Add(sk.Name, sk);
        }
    }
    
    // Update is called once per frame
    public void FillSkillList()
    {
        int numButtons = 0;
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
        }
    }

        public void FillEnemyList(){
        int numButtons = 0;
        foreach(System.Collections.Generic.KeyValuePair<string, Skill> sk in skills){
            newButton = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newButton.transform.SetParent(Content.transform, false);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newButton.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1.0f);
            newButton.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1.0f);
            newButton.GetComponent<RectTransform>().pivot = new Vector2(0f, 1.0f);
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(-290, -1 * (40*numButtons + 20) , 0);
            numButtons++;
            newButton.transform.GetChild(0).GetComponent<Text>().text = sk.Value.Name;
        }
    }

    public void Close(){
        Content.transform.parent.parent.gameObject.SetActive(false);
    }
}
