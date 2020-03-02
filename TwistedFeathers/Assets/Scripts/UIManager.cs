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
    public GameObject miniMapButtons;
    public GameObject turnOptions;
    public GameObject takeTurnButton;

    public List<GameObject> playerHealthBars;
    public List<GameObject> enemyHealthBars;

    public GameObject forecastButton;


    public void Start(){
        GameObject newUIElement = Instantiate(playerHealthBar);
        newUIElement = Instantiate(playerHealthBar, new Vector3(0f, 0f, 0f), Quaternion.identity);
        newUIElement.transform.SetParent(GameObject.Find("Canvas").transform);
        newUIElement.GetComponent<RectTransform>().anchoredPosition = new Vector3(25f, -25f, 0f);
        playerHealthBars.Add(newUIElement);
        newUIElement = Instantiate(enemyHealthBar, new Vector3(0f,0f,0f), Quaternion.identity);
        newUIElement.transform.SetParent(GameObject.Find("Canvas").transform);
        newUIElement.GetComponent<RectTransform>().anchoredPosition = new Vector3(-515f, -25f, 0f);
        enemyHealthBars.Add(newUIElement);
    }

    // deactivates all buttons on page  
    public void toggleButtons(bool show){
        foreach(GameObject button in buttons){
            button.gameObject.SetActive(show);
        }
    }

    public void toggleMiniMapButtons(bool show){
        miniMapButtons.gameObject.SetActive(show);
    }

    //shirnks the miniMap after confirming selection
    public void shrinkMiniMap(){
        RectTransform mapTransform = miniMap.GetComponent<RectTransform>();
        mapTransform.localScale = new Vector3(0.2067623f,0.1873575f,0.2265888f);
        mapTransform.anchorMin = new Vector2(1f, 0f);
        mapTransform.anchorMax = new Vector2(1f, 0f);
        mapTransform.anchoredPosition = new Vector3(-42f,119f,0);
        mapTransform.pivot = new Vector2(1f, 0f);
        toggleButtons(true);
        toggleMiniMapButtons(false);
        if(popups[2].activeSelf){
            toggleForecast();
        }
        closePopUps();
    }

    
    public void growMiniMap(){
        miniMapButtons.SetActive(true);
        miniMap.SetActive(true);
        RectTransform mapTransform = miniMap.GetComponent<RectTransform>();
        mapTransform.anchorMin = new Vector2(0.9f, 0.1f);
        mapTransform.anchorMax = new Vector2(0.9f, 0.1f);
        mapTransform.pivot = new Vector2(1f, 0f);
        mapTransform.localScale = new Vector3(0.75f,0.75f,0);
        mapTransform.anchoredPosition = new Vector3(0,0,0);
        toggleButtons(false);
        toggleMiniMapButtons(true);
    }

    public void togglePlayerSkills(){
        popups[0].SetActive(!popups[0].gameObject.activeSelf);
    }

    public void toggleEnemySkills(){
        popups[1].SetActive(!popups[1].gameObject.activeSelf);
    }

    public void toggleTakeTurn(){
        turnOptions.SetActive(!turnOptions.gameObject.activeSelf);
        takeTurnButton.SetActive(!takeTurnButton.gameObject.activeSelf);

    }

    public void toggleForecast(){
        GameObject forecastList = popups[2];
        if(!forecastList.activeSelf){
            forecastList.SetActive(true);
            forecastButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
            miniMap.SetActive(false);
        } else {
            forecastList.SetActive(false);
            forecastButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
            miniMap.SetActive(true);
        }
        forecastList.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = CombatManager.ForecastText;
    }

    public void addForecast(bool extendedFore){
        if(!extendedFore){
            forecastButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
        }
    }

    public void closePopUps(){
        foreach(GameObject popup in popups){
            popup.SetActive(false);
        }
    }
}
