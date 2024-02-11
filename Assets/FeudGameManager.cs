using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeudGameManager : MonoBehaviour
{
    public static FeudGameManager Instance;
    public GameObject playerPrefab;
    public GameObject playerInWorld;
    public bool cityVisible;
    [SerializeField] CivilizationScriptable[] civilizations;
    [SerializeField] CityView cityManager;
  

    public UniversalColors colors;
    //public PlayerData playerData = new PlayerData();
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

    public void CreatePlayer(WorldCity startingCity)
    {
        //playerData.startingCity = startingCity;
        //playerData.ownedCities.Add(startingCity);
        //playerData.startingCity.AssignToCivilization();

        startingCity.AssignToCivilization();
        playerInWorld = Instantiate(playerPrefab, startingCity.parentTile.transform.position, Quaternion.identity);
       
        WorldUnit unit = playerInWorld.GetComponent<WorldUnit>();
        unit.SpawnSetup(startingCity.parentTile);
        //probably effect or whatever 
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
