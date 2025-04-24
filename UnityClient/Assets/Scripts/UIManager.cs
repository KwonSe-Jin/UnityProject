using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UIPanelType // Panel
{
    Login,
    Matching,
    GameRoom,
    Shop,
    Ranking
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Dictionary<UIPanelType, GameObject> panelDict;

    [Header("UI Panels")]
    public GameObject Panel_Login;
    public GameObject Panel_GameRoom;
    public GameObject Panel_Shop;
    public GameObject Panel_Ranking;
    public GameObject Panel_Matching;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // 하나만 생성
        Instance = this;

        panelDict = new Dictionary<UIPanelType, GameObject>
        {
            { UIPanelType.Login, Panel_Login },
            { UIPanelType.GameRoom, Panel_GameRoom },
            { UIPanelType.Shop, Panel_Shop },
            { UIPanelType.Ranking, Panel_Ranking },
            { UIPanelType.Matching, Panel_Matching },
        };


        // 시작 시 기본 UI 설정
        ShowPanel(UIPanelType.Login);
    }

    public void ShowPanel(UIPanelType panelName)
    {
        HideAllPanels();

        switch (panelName)
        {
            case UIPanelType.Login:
                Panel_Login.SetActive(true);
                break;

            case UIPanelType.Matching:
                Panel_Matching.SetActive(true);
                break;

            case UIPanelType.GameRoom:
                Panel_GameRoom.SetActive(true);
                break;
            case UIPanelType.Shop:
                Panel_Shop.SetActive(true);
                break;
            case UIPanelType.Ranking:
                Panel_Ranking.SetActive(true);
                break;
            default:
                Debug.LogWarning($"[UIManager] Unknown panel name: {panelName}");
                break;
        }
    }

    private void HideAllPanels()
    {
        foreach (var key in panelDict)
        {
            if (key.Value != null)
                key.Value.SetActive(false);
        }
    }
}
