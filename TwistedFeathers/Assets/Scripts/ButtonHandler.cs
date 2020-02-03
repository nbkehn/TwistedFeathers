using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Completed;

public class ButtonHandler : MonoBehaviour
{
     //Without reference variable
    public GameObject objectWithScript;

    public void SwitchEnvironment()
    {
        if(objectWithScript.GetComponent<SceneShift>().scene == 0){
            objectWithScript.GetComponent<SceneShift>().scene = 1;
        } else {
            objectWithScript.GetComponent<SceneShift>().scene = 0;
        }

    }
}
