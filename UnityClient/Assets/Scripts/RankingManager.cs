using UnityEngine;
using UnityEngine.UI;
using TMPro; // TMP¿ë

public class RankingManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button buttonExit;
    private void Start()
    {
        buttonExit.onClick.AddListener(OnExitClicked);
    }

    void OnExitClicked()
    {
        UIManager.Instance.ShowPanel(UIPanelType.GameRoom);
    }
}
