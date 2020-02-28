using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
     //Without reference variable
    public GameObject objectWithScript;
    public GameObject ScrollView;
    public Text myText;
    public GameObject me;
    private string currentText;
    public GameObject MiniMap;
    private bool extendedFore = false;
    private RectTransform smallMapTransform;
    

    public void Start() {
        currentText = myText.text;
    }
    
    public void SwitchEnvironment()
    {
        MiniMap = GameObject.Find("Panel");
        smallMapTransform = MiniMap.GetComponent<RectTransform>();
        MiniMap.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
        MiniMap.GetComponent<RectTransform>().localScale = new Vector3(0.75f,0.75f,0);
        GameObject.Find("CheckEnemy").SetActive(false);
        GameObject.Find("Forecast").SetActive(false);
        GameObject.Find("SwitchEnvironment").SetActive(false);
        GameObject.Find("OpenSkills").SetActive(false);
        GameObject.Find("CombatManager").GetComponent<CombatManager>().StartSelecting();

    }

    public void ConfirmMapSelect(){
        if(GameObject.Find("CombatManager").GetComponent<CombatManager>().validMove){
            MiniMap = GameObject.Find("Panel");
            MiniMap.GetComponent<RectTransform>().localPosition = new Vector3(533,-263,0);
            MiniMap.GetComponent<RectTransform>().localScale = new Vector3(0.2067623f,0.1873575f,0.2265888f);
            GameObject.Find("Canvas").transform.Find("CheckEnemy").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Forecast").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("SwitchEnvironment").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("OpenSkills").gameObject.SetActive(true);
            GameObject.Find("Confirmation").SetActive(false);
            GameObject.Find("Cancel").SetActive(false);
            GameObject.Find("SelectionEntity").SetActive(false);
            GameObject.Find("CombatManager").GetComponent<CombatManager>().ConfirmSelecting(); 
            
        } else {
            Debug.Log("Invalid move");
        }
    }
    public void CancelMapSelect(){
        MiniMap = GameObject.Find("Panel");
        MiniMap.GetComponent<RectTransform>().localPosition = new Vector3(533,-263,0);
        MiniMap.GetComponent<RectTransform>().localScale = new Vector3(0.2067623f,0.1873575f,0.2265888f);
        GameObject.Find("Canvas").transform.Find("CheckEnemy").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("SwitchEnvironment").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Forecast").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("OpenSkills").gameObject.SetActive(true);
        GameObject.Find("Confirmation").SetActive(false);
        GameObject.Find("Cancel").SetActive(false);
        GameObject.Find("SelectionEntity").SetActive(false);
        GameObject.Find("CombatManager").GetComponent<CombatManager>().CancelSelecting(); 
    }

    public void OpenSkillSelect(){
        if(!ScrollView.activeSelf){
            ScrollView.SetActive(true);
            
        } else {
            ScrollView.SetActive(false);
        }
    }

    public void newForecast(){
        if(!extendedFore){
            me.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
        }
    }
    
    public void OpenForecast(){
        if(!ScrollView.activeSelf){
            ScrollView.SetActive(true);
            myText.text = "Close";
            me.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
            MiniMap.SetActive(false);
        } else {
            ScrollView.SetActive(false);
            myText.text = "";
            me.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
            MiniMap.SetActive(true);
        }
        ScrollView.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = CombatManager.ForecastText;
    }

    public void SwitchScene(){
        SceneManager.LoadScene("EnvironmentSwitching");
    }
}
