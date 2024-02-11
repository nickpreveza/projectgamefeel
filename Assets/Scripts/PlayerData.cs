using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData 
{
    public WorldCity startingCity;
    public List<WorldCity> ownedCities = new List<WorldCity>();
    public CivilizationScriptable playerCiv;
}
