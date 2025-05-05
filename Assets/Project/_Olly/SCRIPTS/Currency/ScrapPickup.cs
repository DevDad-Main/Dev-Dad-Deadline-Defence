using UnityEngine;
using DG.Tweening;

public class ScrapPickup : MonoBehaviour
{
 [Header("Scrap Settings")]
    [SerializeField] private int scrapAmount = 1;
    [SerializeField] private float travelTime = 0.6f;
    [SerializeField] private float sizeIncreaseFactor = 1.2f;
    [SerializeField] private GameObject shadowGO;
    private bool isCollecting = false;

    public void Collect(Transform uiTarget)
    {
        if (isCollecting || uiTarget == null) return;
        isCollecting = true;

        // Disable collider so it doesn't trigger again
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        shadowGO.SetActive(false);

        // Convert UI position to world position in canvas space
        Vector3 worldPos = uiTarget.position;

        // Grow the object as it moves
        transform.DOScale(sizeIncreaseFactor, travelTime).SetEase(Ease.OutBack);

        // Fly toward the UI
        transform.DOMove(worldPos, travelTime)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                ScrapManager.Instance.AddScrap(scrapAmount);
                Destroy(gameObject);
            });

        // Shrink as it travels
        transform.DOScale(0.1f, travelTime).SetEase(Ease.InOutQuad);

        // Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCollecting && collision.CompareTag("Player"))
        {
            // Find the UI target
            ScrapUI ui = FindObjectOfType<ScrapUI>();
            if (ui != null)
            {
                Collect(ui.ScrapIconTransform);
            }
        }
    }
}
