using UnityEngine;

public class SI_UIManager : MonoBehaviour
{
    public static SI_UIManager Instance;
    [SerializeField] GameObject loadingPanel;

    void Awake()
    {
        Instance = this;
        loadingPanel.SetActive(true);
    }

    public void LoadingPanel(bool isOpen)
    {
        loadingPanel.SetActive(isOpen);
    }
   
}
