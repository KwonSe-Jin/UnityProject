using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMP¿ë

public class GameRoomManager : MonoBehaviour
{
    
    [Header("Buttons")]
    public Button buttonShop;
    public Button buttonRanking;
    public Button buttonMatching;

    private void Start()
    {
        buttonShop.onClick.AddListener(OnShopnClicked);
        buttonRanking.onClick.AddListener(OnRankingClicked);
        buttonMatching.onClick.AddListener(OnMatchingClicked);
    }

    void OnShopnClicked()
    {
        UIManager.Instance.ShowPanel(UIPanelType.Shop);
    }

    void OnRankingClicked() 
    {
        UIManager.Instance.ShowPanel(UIPanelType.Ranking);
    }
    void OnMatchingClicked()
    {
        UIManager.Instance.ShowPanel(UIPanelType.Matching);
    }
}
