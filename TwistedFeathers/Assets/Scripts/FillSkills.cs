using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillSkills : MonoBehaviour
{
    public GameObject Content;
    private Dictionary<string, Skill> skills;
    public GameObject buttonPrefab;
    private GameObject newButton;
    public CombatManager manager;

    void Start(){
        skills = GameManager.Skill_db;
    }
    
    // Update is called once per frame
    public void FillSkillList()
    {
        int numButtons = 0;
        foreach(System.Collections.Generic.KeyValuePair<string, Skill> sk in skills){
            newButton = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newButton.transform.SetParent(Content.transform, false);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newButton.transform.position = new Vector3(Content.transform.position.x + 190, Content.transform.position.y - 40*numButtons - 20 , 0);
            numButtons++;
            newButton.transform.GetChild(0).GetComponent<Text>().text = sk.Value.Name;
            newButton.GetComponent<Button>().onClick.AddListener(() => {
                manager.GetComponent<CombatManager>().SelectSkill(sk.Value.Name);
            });
            newButton.GetComponent<Button>().onClick.AddListener(() => {
                Close();
            });
        }
    }

    public void Close(){
        Content.transform.parent.parent.gameObject.SetActive(false);
    }
}
