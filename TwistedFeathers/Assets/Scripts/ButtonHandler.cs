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
        MiniMap.SetActive(true);
        ScrollView.SetActive(false);
        GameObject.Find("Forecast").transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        GameObject.Find("Forecast").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
        smallMapTransform = MiniMap.GetComponent<RectTransform>();
        smallMapTransform.anchorMin = new Vector2(0.9f, 0.1f);
        smallMapTransform.anchorMax = new Vector2(0.9f, 0.1f);
        smallMapTransform.pivot = new Vector2(1f, 0f);
        smallMapTransform.localScale = new Vector3(0.75f,0.75f,0);
        smallMapTransform.anchoredPosition = new Vector3(0,0,0);
        GameObject.Find("CheckEnemy").SetActive(false);
        GameObject.Find("Forecast").SetActive(false);
        GameObject.Find("SwitchEnvironment").SetActive(false);
        GameObject.Find("OpenSkills").SetActive(false);
        GameObject.Find("CombatManager").GetComponent<CombatManager>().StartSelecting();

    }

    public void ConfirmMapSelect(){
        if(GameObject.Find("CombatManager").GetComponent<CombatManager>().validMove){
            smallMapTransform = MiniMap.GetComponent<RectTransform>();
            smallMapTransform.localScale = new Vector3(0.2067623f,0.1873575f,0.2265888f);
            smallMapTransform.anchorMin = new Vector2(1f, 0f);
            smallMapTransform.anchorMax = new Vector2(1f, 0f);
            smallMapTransform.anchoredPosition = new Vector3(-40f,75f,0);
            smallMapTransform.pivot = new Vector2(1f, 0f);
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
        smallMapTransform = MiniMap.GetComponent<RectTransform>();
        smallMapTransform.anchorMin = new Vector2(1f, 0f);
        smallMapTransform.anchorMax = new Vector2(1f, 0f);
        smallMapTransform.pivot = new Vector2(1f, 0f);
        smallMapTransform.anchoredPosition = new Vector3(-40f,75,0);
        smallMapTransform.localScale = new Vector3(0.2067623f,0.1873575f,0.2265888f);
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

    public void toggleSettings(){
        GameObject.Find("Settings").transform.GetChild(0).gameObject.SetActive(!GameObject.Find("Settings").transform.GetChild(0).gameObject.activeSelf);
    }


    public void toggleRotation(){
        GameObject.Find("GameManager").GetComponent<GameManager>().rotate = !GameObject.Find("GameManager").GetComponent<GameManager>().rotate;
    }
}
