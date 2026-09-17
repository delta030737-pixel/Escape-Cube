using UnityEngine;

public class EnemyVisualEffect : MonoBehaviour
{
    [Header("Idle Wobble (Scale)")]
    public float wobbleSpeed = 2f;
    public float wobbleAmount = 0.05f;

    [Header("Y-Axis Bobbing (Local Position)")]
    public float bobSpeed = 4f;
    public float bobHeight = 0.15f;

    [Header("Chase (เร็ว/แรงขึ้นตอนไล่ล่า)")]
    public bool isChasing = false;
    public float chaseWobbleSpeed = 8f;
    public float chaseWobbleAmount = 0.15f;
    public float chaseBobSpeed = 10f;
    public float chaseBobHeight = 0.3f;

    private Vector3 baseScale;
    private Vector3 baseLocalPos;
    private float timer;

    void Start()
    {
        baseScale = transform.localScale;
        baseLocalPos = transform.localPosition; 
    }

    void Update()
    {
        float wobbleSpd = isChasing ? chaseWobbleSpeed : wobbleSpeed;
        float wobbleAmt = isChasing ? chaseWobbleAmount : wobbleAmount;
        float bobSpd = isChasing ? chaseBobSpeed : bobSpeed;
        float bobH = isChasing ? chaseBobHeight : bobHeight;

        timer += Time.deltaTime;

        float noiseX = Mathf.PerlinNoise(timer * wobbleSpd, 0f) - 0.5f;
        float noiseY = Mathf.PerlinNoise(0f, timer * wobbleSpd) - 0.5f;
        float noiseZ = Mathf.PerlinNoise(timer * wobbleSpd, timer * wobbleSpd) - 0.5f;
        transform.localScale = baseScale + new Vector3(noiseX, noiseY, noiseZ) * wobbleAmt;

        float yOffset = Mathf.Sin(timer * bobSpd) * bobH;
        transform.localPosition = baseLocalPos + new Vector3(0f, yOffset, 0f);
    }

    public void SetChasing(bool chasing)
    {
        isChasing = chasing;
    }
}