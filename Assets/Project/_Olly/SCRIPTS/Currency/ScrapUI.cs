using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class ScrapUI : MonoBehaviour
{
    public static ScrapUI Instance { get; private set;}

    [SerializeField] private TextMeshProUGUI scrapText;
    [SerializeField] private RectTransform scrapTextTransform;

    #region Getters And Setters
    public RectTransform ScrapIconTransform => scrapTextTransform;
    #endregion


    private void Start()
    {
        Instance = this;

        ScrapManager.Instance.OnScrapChanged.AddListener(UpdateScrapUI);
        ScrapManager.Instance.OnScrapGained.AddListener(PlayGainFeedback);
        ScrapManager.Instance.OnScrapSpent.AddListener(PlaySpendFeedback);

        UpdateScrapUI(ScrapManager.Instance.Scrap);
    }

    private void UpdateScrapUI(int newAmount)
    {
        scrapText.text = newAmount.ToString();
    }

    private void PlayGainFeedback(int amount)
    {
        scrapTextTransform.DOKill();
        scrapTextTransform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 10, 1).SetEase(Ease.OutBack);
        scrapText.DOColor(Color.yellow, 0.15f).OnComplete(() =>
            scrapText.DOColor(Color.white, 0.2f)
        );
    }

    private void PlaySpendFeedback(int amount)
    {
        scrapTextTransform.DOKill();
        scrapTextTransform.DOShakeScale(0.3f, 0.2f).SetEase(Ease.OutElastic);
        scrapText.DOColor(Color.red, 0.15f).OnComplete(() =>
            scrapText.DOColor(Color.white, 0.2f)
        );
    }
}
