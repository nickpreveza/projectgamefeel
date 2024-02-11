using UnityEngine;

public class WorldButton : MonoBehaviour
{
    void OnMouseDown()
    {
        FeudGameManager.Instance.ViewCity(false);
    }
}
