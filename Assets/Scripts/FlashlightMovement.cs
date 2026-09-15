using UnityEngine;

public class FlashlightMovement : MonoBehaviour
{
    public Animator flashlightAnim;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                flashlightAnim.ResetTrigger("walk");
                flashlightAnim.SetTrigger("sprint");
            }
            else
            {
                flashlightAnim.ResetTrigger("sprint");
                flashlightAnim.SetTrigger("walk");
            }
        }
    }
}
