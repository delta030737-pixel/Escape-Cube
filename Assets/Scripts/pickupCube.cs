using UnityEngine;

public class pickupCube : MonoBehaviour
{
    private bool interactable;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.CompareTag("Player"))
        {
            interactable = true;
            GameManager.Instance?.ShowInteractUI(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.CompareTag("Player"))
        {
            interactable = false;
            GameManager.Instance?.ShowInteractUI(false);
        }
    }

    void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            interactable = false;
            GameManager.Instance?.ShowInteractUI(false);
            GameManager.Instance?.AddCube(); 
            Destroy(gameObject);
        }
    }
}