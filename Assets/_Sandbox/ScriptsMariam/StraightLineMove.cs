using UnityEngine;

public class StraightLineMove : MonoBehaviour
{
    public float speed = 6f;

    private Vector3 direction;
    [SerializeField] private GameObject shield;

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        if (Mathf.Abs(direction.y) > 0) {
            Quaternion xOffset = Quaternion.Euler(0, 0, 90);
            shield.transform.localRotation = shield.transform.localRotation * xOffset;
        }
    }

    void Update()
    {
        transform.position += direction * (speed * Time.deltaTime);
    }
}
