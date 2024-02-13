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
    [SerializeField] public Civilization[] gameCivilizations;
    public int playerCivInde = 0; //always 0

    public CityView cityManager;
  
   
    public UniversalColors colors;
    //public PlayerData playerData = new PlayerData();

    public int startingGold = 100;
    
    public bool randomEncounters;
    public float baseEncounterChance;
    public float encounterChanceDecreaseFactor; //if had an ecnounter, half it. 

 
    public Civilization GetCiv(int playerIndex)
    {
        return gameCivilizations[playerIndex];
    }

    public Civilization Player()
    {
        return gameCivilizations[0];
    }
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

        startingCity.AssignToCivilization(0); //0 is playerIndex
        startingCity.RevealCity();
        playerInWorld = Instantiate(playerPrefab, startingCity.parentTile.transform.position, Quaternion.identity);
       
        WorldUnit unit = playerInWorld.GetComponent<WorldUnit>();
        unit.SpawnSetup(startingCity.parentTile);

        SI_CameraController.Instance.CenterCamera(unit.parentTile);
        Player().gold = startingGold;

        for(int i = 0; i < gameCivilizations.Length; i++)
        {
            gameCivilizations[i].knownCivs.Add(i);

            foreach(ItemScriptable itemScriptable in gameCivilizations[i].storeItemsBase)
            {
                Item item = new Item();
                item.SetData(itemScriptable.item);
                gameCivilizations[i].storeItemsPool.Add(item);
            }

            foreach (ItemScriptable itemScriptable in gameCivilizations[i].startingItemsBase)
            {
                Item item = new Item();
                item.SetData(itemScriptable.item);
                gameCivilizations[i].ownedItems.Add(item);
            }

        }
        //probably effect or whatever 
    }

    public void RefreshStoreItems(int civIndex)
    {
        List<Item> itemPool = new List<Item>(gameCivilizations[civIndex].storeItemsPool);
        List<Item> selectedStoreItems = new List<Item>();

        for (int i = 0; i < 4; i++)
        {
            int selectedIndex = Random.Range(0, itemPool.Count);
            selectedStoreItems.Add(itemPool[selectedIndex]);
            itemPool.Remove(itemPool[selectedIndex]);
        }

        gameCivilizations[civIndex].selectedStoreItems = selectedStoreItems;
    }

    public void ViewCity(bool show, WorldTile city = null)
    {
        cityVisible = show;
   
        if (cityVisible)
        {
            SI_CameraController.Instance.ShowCity(city.cityObject.GetComponent<WorldCity>());
            if (Player().lastVisitedCity != city.cityObject.GetComponent<WorldCity>())
            {
                RefreshStoreItems(city.cityObject.GetComponent<WorldCity>().civIndex);
            }
            Player().lastVisitedCity = city.cityObject.GetComponent<WorldCity>();

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
