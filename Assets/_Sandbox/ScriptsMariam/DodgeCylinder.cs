using UnityEngine;

public class DodgeCylinder : MonoBehaviour
{
    public float lifeTime = 4f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
