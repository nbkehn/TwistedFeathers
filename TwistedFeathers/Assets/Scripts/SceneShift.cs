using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Completed
{
    public class SceneShift : MonoBehaviour
    {
        
        
        public int scene = 0;
        public GameObject[] environments;
        private GameObject currentEnvironment;
        public GameObject lightSource;

        // Start is called before the first frame update
        void Start()
        {
            currentEnvironment = environments[0];
        }

        // Update is called once per frame
        void Update()
        {
            if(scene == 0){
                lightSource.GetComponent<Light>().color = Color.red;
                currentEnvironment = environments[0];
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Desert"))
                {
                    go.GetComponent<MeshRenderer>().enabled = true;
                }

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Swamp"))
                {
                    go.GetComponent<Renderer>().enabled = false;
                }
            }
            if(scene == 1){
                currentEnvironment = environments[1];
                lightSource.GetComponent<Light>().color = Color.blue;
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Desert"))
                {
                    go.GetComponent<MeshRenderer>().enabled = false;
                }

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Swamp"))
                {
                    go.GetComponent<Renderer>().enabled = true;
                }
            }
        }
    }
}
