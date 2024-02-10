using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeudGameManager : MonoBehaviour
{
    public static FeudGameManager Instance;
    public GameObject playerPrefab;
    public bool cityVisible;
    [SerializeField] CivilizationScriptable[] civilizations;
    [SerializeField] CityView cityManager;
    void Awake()
    {
        Instance = this;   
    }

    private void Start()
    {
        MapGenerator.Instance.ClearMap(false);
        MapGenerator.Instance.GenerateMap();
        SI_CameraController.Instance.GameStarted();
        SI_UIManager.Instance.LoadingPanel(false);
    }

    private void Update()
    {
        if (cityVisible)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ViewCity(false);
            }
        }
    }

    public void CreatePlayer(WorldTile startingCity)
    {

    }

    public void ViewCity(bool show, WorldTile city = null)
    {
        cityVisible = show;
        if (cityVisible)
        {
            SI_CameraController.Instance.ShowCity(city.cityObject.GetComponent<WorldCity>());
            cityManager.ShowCity(city.cityObject.GetComponent<WorldCity>());
        }
        else
        {
            SI_CameraController.Instance.HideCity();
            cityManager.HideCity();
        }
    }
}
