using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class pickupCube : MonoBehaviour
{
    public GameObject collectTextObj, intText;
    public bool interactable;
    public static int cubeCollected;
    public TMP_Text collectText;

    private void Start()
    {
        cubeCollected = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                cubeCollected = cubeCollected + 1;
                collectText.text = cubeCollected + "/8 cubes";
                collectTextObj.SetActive(true);

                intText.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}