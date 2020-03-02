using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
     //Without reference variable
    private bool extendedFore = false;
    private RectTransform smallMapTransform;

    private UIManager UIManager;
    

    public void Start() {
        if(GameObject.Find("UIManager")){
            UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        }
    }
    
        // This is called when the change environment button is clicked
    public void SwitchEnvironment()
    {
        //deactivate all other buttons
        UIManager.growMiniMap();
        //Start the updated flow in the combat manager
        GameObject.Find("CombatManager").GetComponent<CombatManager>().StartSelecting();

    }

    public void ConfirmMapSelect(){
        if(GameObject.Find("CombatManager").GetComponent<CombatManager>().validMove){
            UIManager.shrinkMiniMap();   
            GameObject.Find("CombatManager").GetComponent<CombatManager>().ConfirmSelecting();         
        } else {
            Debug.Log("Invalid move");
        }
    }
    public void CancelMapSelect(){
        UIManager.shrinkMiniMap();
        GameObject.Find("CombatManager").GetComponent<CombatManager>().CancelSelecting(); 
    }

    public void OpenSkillSelect(){
        UIManager.togglePlayerSkills();
    }
    public void OpenEnemySkills(){
        UIManager.toggleEnemySkills();
    }

    public void newForecast(){
        UIManager.addForecast(extendedFore);
    }
    
    public void toggleForecast(){
        UIManager.toggleForecast();
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

    public void takeTurn(){
        UIManager.toggleTakeTurn();
    }
}
