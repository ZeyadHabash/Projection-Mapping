using UnityEngine;

public class ClickToDestroy : MonoBehaviour
{
    void OnMouseDown()
    {
        Destroy(gameObject);
    }
}
