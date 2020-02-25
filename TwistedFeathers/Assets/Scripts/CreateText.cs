using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateText : MonoBehaviour
{
    private GameObject newText;
    public GameObject Content;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addLine(string value, int numTexts, GameObject text){
        newText = text;
        newText.transform.SetParent(Content.transform, false);
        newText.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        newText.transform.position = new Vector3(Content.transform.position.x + 190, Content.transform.position.y - 40*numTexts - 20 );
        newText.GetComponent<Text>().text = value;
    }
}
