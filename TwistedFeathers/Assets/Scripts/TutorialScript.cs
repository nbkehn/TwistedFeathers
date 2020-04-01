using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    private int segment = 0;
    public GameObject tutorialCanvas;
    public Animator animator;

    public void StartTutorial(){
        segment = 1;
        animator.Play("HealthBars");
    }

    public void CancelTutorial(){
        tutorialCanvas.SetActive(false);
    }

    public void next(){      
        switch (segment)
        {
            case 1:
                segment = 2;
                animator.Play("miniMap");
                break;
            case 2:
                animator.Play("TakeTurn");
                segment = 3;
                break;
            case 3:
                animator.Play("Close");
                break;
            default:
                Debug.Log("InvalidAnimationState");
                break;
        }
    }
}
