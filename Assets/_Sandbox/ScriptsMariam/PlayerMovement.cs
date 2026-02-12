using UnityEngine;
using _Sandbox.Scripts.Managers;


public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Renderer rend;
    private Color originalColor;
    public bool IsMoving;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    void Update()
    {
        if (!IsMoving )
        {
            return;
        }
        float h = Input.GetAxis("Horizontal"); // A D
        float v = Input.GetAxis("Vertical");   // W S

        Vector3 move = new Vector3(h, v, 0);
        transform.position += move * speed * Time.deltaTime;
    }


    public void TakeDamage()
    {
        if (WordManager.Instance != null &&
            WordManager.Instance.CollectedWords.Count > 0)
        {
            WordManager.Instance.RemoveRandomCollectedWord();
        }

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
