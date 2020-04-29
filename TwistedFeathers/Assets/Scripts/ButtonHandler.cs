using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

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

    public void goToUpgrade(){
        SceneManager.LoadScene("UpgradeScreen");
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
            UIManager.playTransition();
            StartCoroutine(transitionOnTime());
        } else {
            Debug.Log("Invalid move");
        }
    }

    IEnumerator transitionOnTime(){
        yield return new WaitForSeconds(.7f);
        GameObject.Find("CombatManager").GetComponent<CombatManager>().ConfirmSelecting();
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
        GameObject animation = GameObject.Find("Canvas").transform.GetChild(7).gameObject;
        animation.SetActive(true);
        animation.gameObject.GetComponent<Animator>().Play("TransitionAnimation2");
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle(){
        yield return new WaitForSeconds(.7f);
        SceneManager.LoadScene("EnvironmentSwitching");
    }

    public void toggleSettings(){
        GameObject.Find("Settings").transform.GetChild(0).gameObject.SetActive(!GameObject.Find("Settings").transform.GetChild(0).gameObject.activeSelf);
        GameObject.Find("Settings").transform.GetChild(1).gameObject.SetActive(!GameObject.Find("Settings").transform.GetChild(1).gameObject.activeSelf);
    }


    public void toggleRotation(){
        GameObject.Find("GameManager").GetComponent<GameManager>().rotate = !GameObject.Find("GameManager").GetComponent<GameManager>().rotate;
    }

    public void quitGame() 
    {
        Application.Quit();
    }

    public void startFromSplash(){
        GameObject animation = GameObject.Find("Canvas").transform.GetChild(4).gameObject;
        animation.SetActive(true);
        animation.gameObject.GetComponent<Animator>().Play("TransitionAnimation");
        StartCoroutine(newGame());
    }

    IEnumerator newGame(){
        yield return new WaitForSeconds(.7f);
        SceneManager.LoadScene("StartGame");

    }

    public void leaveRoom()
    {
        UIManager.actionOverlay.GetComponent<Animator>().Play("flyOut");
        GameObject.Find("GameManager").GetComponent<GameManager>().finishBattle(0);
    }
}
