using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Completed;

public class UIManager : MonoBehaviour
{
    //Store references to various UI elements that we will need later
    public List<GameObject> popups;
    public GameObject miniMap;
    public GameObject playerHealthBar;
    public GameObject enemyHealthBar;
    public List<GameObject> buttons;
    public GameObject turnOptions;
    public List<GameObject> playerHealthBars;
    public List<GameObject> enemyHealthBars;

    public GameObject forecastButton;
    public GameObject transitionAnimation;

    public bool starting = true;

    public List<GameObject> animateableButtons;

    public ButtonHandler buttonHandler;

    public GameObject actionOverlay;

    public GameObject SkillInfos;
    public GameObject selectionEntity;
    public GameObject locationTracker;

    public void Start(){
        foreach(GameObject button in animateableButtons){
            if(button.activeSelf){
                button.GetComponent<Animator>().SetBool("enter", true);
            }    
        }
        transitionAnimation.SetActive(true);
        transitionAnimation.GetComponent<Animator>().Play("TransitionAnimation2",0,0.7f);
        beginFinish();

        turnOptions.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(togglePlayerSkills);
        turnOptions.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(buttonHandler.SwitchEnvironment);
    }

    public void beginFinish(){
        StartCoroutine(finishStart());
    }

    IEnumerator finishStart (){
        yield return new WaitForSeconds(.3f);
        starting = false;
        StartCoroutine(stopHealthBars());
    }

    IEnumerator stopHealthBars() {
        yield return new WaitForSeconds(2f);
        foreach(GameObject healthBar in playerHealthBars){
            healthBar.GetComponent<Animator>().enabled = false;
        }
        foreach(GameObject healthBar in enemyHealthBars){
            healthBar.GetComponent<Animator>().enabled = false;
        }
    }

    public void Update(){
        if(!starting){
            if(transitionAnimation.activeSelf){
                if(!(transitionAnimation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TransitionAnimation2"))){
                    transitionAnimation.SetActive(false);
                }
            }
        }
    }

    // deactivates all buttons on page  
    public void toggleButtons(bool show){
        foreach(GameObject button in buttons){
            button.gameObject.SetActive(show);
        }
    }

    //shirnks the miniMap after confirming selection
    public void shrinkMiniMap(){
        miniMap.GetComponent<Animator>().Play("ShrinkPanel");
        forecastButton.SetActive(true);
        // RectTransform mapTransform = miniMap.GetComponent<RectTransform>();
        // mapTransform.localScale = new Vector3(0.2067623f,0.1873575f,0.2265888f);
        // mapTransform.anchorMin = new Vector2(1f, 0f);
        // mapTransform.anchorMax = new Vector2(1f, 0f);
        // mapTransform.anchoredPosition = new Vector3(-42f,119f,0);
        // mapTransform.pivot = new Vector2(1f, 0f);
        toggleTurnOptions(false);
        animateableButtons[3].GetComponent<Button>().interactable = true;
        if(popups[2].activeSelf){
            toggleForecast();
        }
        foreach(GameObject button in animateableButtons){
            if(button.activeSelf){
                button.GetComponent<Animator>().SetBool("enter", true);
            }
        }
        closePopUps();
        
    }

    public void toggleTurnOptions(bool bigMap){
        if(bigMap){
            turnOptions.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Cancel";
            turnOptions.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(buttonHandler.CancelMapSelect);
            turnOptions.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener (togglePlayerSkills);

            turnOptions.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Confirm";
            turnOptions.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(buttonHandler.ConfirmMapSelect);
            turnOptions.transform.GetChild(1).GetComponent<Button>().onClick.RemoveListener (buttonHandler.SwitchEnvironment);
        } else {
            turnOptions.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Select Skill";
            turnOptions.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Change Environment";

            turnOptions.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(buttonHandler.CancelMapSelect);
            turnOptions.transform.GetChild(0).GetComponent<Button>().onClick.AddListener (togglePlayerSkills);

            turnOptions.transform.GetChild(1).GetComponent<Button>().onClick.RemoveListener(buttonHandler.ConfirmMapSelect);
            turnOptions.transform.GetChild(1).GetComponent<Button>().onClick.AddListener (buttonHandler.SwitchEnvironment);
        }
    }

    public void playTransition() {
        transitionAnimation.SetActive(true);
        starting = true;
        transitionAnimation.GetComponent<Animator>().Play("TransitionAnimation2");
        StartCoroutine(finishStart());

    }

    
    public void growMiniMap(){
        miniMap.SetActive(true);
        forecastButton.SetActive(false);
        animateableButtons[3].GetComponent<Button>().interactable = false;
        miniMap.GetComponent<Animator>().Play("GrowPanel");
        // RectTransform mapTransform = miniMap.GetComponent<RectTransform>();
        // mapTransform.anchorMin = new Vector2(0.9f, 0.1f);
        // mapTransform.anchorMax = new Vector2(0.9f, 0.1f);
        // mapTransform.pivot = new Vector2(1f, 0f);
        // mapTransform.localScale = new Vector3(0.75f,0.75f,0);
        // mapTransform.anchoredPosition = new Vector3(0,0,0);
        toggleTurnOptions(true);
    }

    public void togglePlayerSkills(){
        popups[0].SetActive(!popups[0].gameObject.activeSelf);
        if(popups[0].activeSelf){
            turnOptions.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Cancel";
            turnOptions.transform.GetChild(1).GetComponent<Button>().interactable = false;
        } else {
            turnOptions.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Select Skill";
            turnOptions.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }
        if(GameObject.Find("PlayerSkillInfo").GetComponent<Animator>().GetBool("Open")){
            GameObject.Find("PlayerSkillInfo").GetComponent<Animator>().Play("Pop Out");
            GameObject.Find("PlayerSkillInfo").GetComponent<Animator>().SetBool("Open", false);
        } 
    }

    public void toggleEnemySkills(){
        popups[1].SetActive(!popups[1].gameObject.activeSelf);
        if(GameObject.Find("EnemySkillInfo").GetComponent<Animator>().GetBool("Open")){
            GameObject.Find("EnemySkillInfo").GetComponent<Animator>().Play("Pop Out");
            GameObject.Find("EnemySkillInfo").GetComponent<Animator>().SetBool("Open", false);
        } 
    }

    public void toggleForecast(){
        GameObject forecastList = popups[2];
        if(!forecastList.activeSelf){
            forecastList.SetActive(true);

            miniMap.SetActive(false);
        } else {
            forecastList.SetActive(false);
            miniMap.SetActive(true);
        }
        if(forecastButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("bigForecast")){
            forecastButton.GetComponent<Animator>().Play("shrinkForecast");
        }
        forecastList.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = CombatManager.ForecastText;
    }

    public void addForecast(bool extendedFore){
        if(!extendedFore){
            forecastButton.GetComponent<Animator>().Play("growForecast");
        }
    }

    public void closePopUps(){
        foreach(GameObject popup in popups){
            popup.SetActive(false);
        }
    }
}
