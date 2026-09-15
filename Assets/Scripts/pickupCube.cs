using UnityEngine;
using UnityEngine.UI;

public class pickupCube : MonoBehaviour
{
    [Header("UI References")]
    public GameObject collectTextObj;
    public GameObject intText;
    public Text collectText;

    [Header("Settings")]
    public int totalCubes = 8;

    private static int cubeCollected;
    private bool interactable;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            Collect();
        }
    }

    void Collect()
    {
        cubeCollected++;
        collectText.text = cubeCollected + "/" + totalCubes + " cubes";
        collectTextObj.SetActive(true);

        intText.SetActive(false); 
        interactable = false;

        if (cubeCollected >= totalCubes)
        {
            GameManager.Instance?.WinGame();
        }

        Destroy(gameObject); 
    }
}