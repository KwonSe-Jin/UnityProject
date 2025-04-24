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
        // �̱��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // �ϳ��� ����
        Instance = this;

        panelDict = new Dictionary<UIPanelType, GameObject>
        {
            { UIPanelType.Login, Panel_Login },
            { UIPanelType.GameRoom, Panel_GameRoom },
            { UIPanelType.Shop, Panel_Shop },
            { UIPanelType.Ranking, Panel_Ranking },
            { UIPanelType.Matching, Panel_Matching },
        };


        // ���� �� �⺻ UI ����
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
