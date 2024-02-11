using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeudGameManager : MonoBehaviour
{
    public static FeudGameManager Instance;
    public GameObject playerPrefab;
    public GameObject playerInWorld;
    public bool cityVisible;
    public bool arenaVisible; //debug
    public bool arenaOnGoing = false;
    [SerializeField] CivilizationScriptable[] civilizations;
    [SerializeField] CityView cityManager;
  
   
    public UniversalColors colors;
    //public PlayerData playerData = new PlayerData();

    public int startingGold = 100;
    public int playerGold;

    
    public bool randomEncounters;
    public float baseEncounterChance;
    public float encounterChanceDecreaseFactor; //if had an ecnounter, half it. 
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
        if (cityVisible && !arenaVisible)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ViewCity(false);
            }
        }

        if (arenaVisible && !arenaOnGoing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseArena();
            }
        }
    }

    public void CreatePlayer(WorldCity startingCity)
    {
        //playerData.startingCity = startingCity;
        //playerData.ownedCities.Add(startingCity);
        //playerData.startingCity.AssignToCivilization();

        startingCity.AssignToCivilization();
        startingCity.RevealCity();
        playerInWorld = Instantiate(playerPrefab, startingCity.parentTile.transform.position, Quaternion.identity);
       
        WorldUnit unit = playerInWorld.GetComponent<WorldUnit>();
        unit.SpawnSetup(startingCity.parentTile);

        SI_CameraController.Instance.CenterCamera(unit.parentTile);
        playerGold = startingGold;
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

    public void StartArena(bool fromCity, WorldTile origin = null)
    {
        arenaVisible = true;
        SI_CameraController.Instance.ShowAreanView(fromCity, origin);
    }

    public void CloseArena()
    {
        arenaVisible = false;
        UnitManager.Instance.runningArenaCombat = false;
        SI_CameraController.Instance.HideArena(cityVisible);
    }
}
