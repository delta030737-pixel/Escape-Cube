using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class FNAFButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Text Settings")]
    public TMP_Text buttonText;
    public string hoverPrefix = ">> ";            
    public Color normalColor = Color.white;       
    public Color hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f); 

    [Header("Audio Settings (Optional)")]
    public AudioSource audioSource;
    public AudioClip hoverSound;                 
    public AudioClip clickSound;                 

    private string originalText;

    void Awake()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
        {
            originalText = buttonText.text;
            buttonText.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.text = hoverPrefix + originalText;
            buttonText.color = hoverColor;
        }

        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetButtonText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    void OnDisable()
    {
        ResetButtonText();
    }

    private void ResetButtonText()
    {
        if (buttonText != null && !string.IsNullOrEmpty(originalText))
        {
            buttonText.text = originalText;
            buttonText.color = normalColor;
        }
    }
}