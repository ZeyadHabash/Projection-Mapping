using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A D
        float v = Input.GetAxis("Vertical");   // W S

        Vector3 move = new Vector3(h, v, 0);
        transform.position += move * speed * Time.deltaTime;
    }


    public void TakeDamage()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRed());
    }

    System.Collections.IEnumerator FlashRed()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        rend.material.color = originalColor;
    }
}
