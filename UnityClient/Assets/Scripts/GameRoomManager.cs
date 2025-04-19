using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMP¿ë

public class GameRoomManager : MonoBehaviour
{

    [Header("Buttons")]
    public Button buttonShop;
    public Button buttonRanking;

    private void Start()
    {
        buttonShop.onClick.AddListener(OnShopnClicked);
        buttonRanking.onClick.AddListener(OnRankingClicked);
    }

    void OnShopnClicked()
    {
        UIManager.Instance.ShowPanel(UIPanelType.Shop);
    }

    void OnRankingClicked() 
    {
        UIManager.Instance.ShowPanel(UIPanelType.Ranking);
    }

}
