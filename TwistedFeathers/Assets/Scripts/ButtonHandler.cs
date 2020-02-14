using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
     //Without reference variable
    public GameObject objectWithScript;
    public GameObject ScrollView;
    public Text myText;
    public GameObject me;
    private string currentText;
    public GameObject MiniMap;
    public GameObject MegaMap;

    public void Start() {
        currentText = myText.text;
    }
    
    public void SwitchEnvironment()
    {
        if(objectWithScript.GetComponent<SceneShift>().scene == 0){
            objectWithScript.GetComponent<SceneShift>().scene = 1;
        } else {
            objectWithScript.GetComponent<SceneShift>().scene = 0;
        }

    }

    public void OpenSkillSelect(){
        if(!ScrollView.activeSelf){
            ScrollView.SetActive(true);
            myText.text = "Nevermind";
            
        } else {
            ScrollView.SetActive(false);
            myText.text = currentText;
        }
    }

    
    public void OpenForecast(){
        if(!ScrollView.activeSelf){
            ScrollView.SetActive(true);
            myText.text = "Close";
            me.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 251);
            MiniMap.SetActive(false);
        } else {
            ScrollView.SetActive(false);
            myText.text = "";
            me.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
            MiniMap.SetActive(true);
        }
    }
}
