using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    [SerializeField] GameObject[] stats;
    [SerializeField] StatType type;
    public void UpdateVisuals(int amount)
    {

        for (int i = 0; i < stats.Length; i++)
        {
            if (amount > i)
            {
                switch (type)
                {
                    case StatType.STR:
                        stats[i].GetComponent<Image>().color = FeudGameManager.Instance.colors.strColor;
                        break;
                    case StatType.DEX:
                        stats[i].GetComponent<Image>().color = FeudGameManager.Instance.colors.dexColor;
                        break;
                    case StatType.CON:
                        stats[i].GetComponent<Image>().color = FeudGameManager.Instance.colors.conColor;
                        break; 
                }
               
            }
            else
            {
                stats[i].GetComponent<Image>().color = FeudGameManager.Instance.colors.disableStatColor;
            }
           
        }
    }
}
