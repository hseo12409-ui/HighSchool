using UnityEngine;

public class dolpalme : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] Vector2 vec;

    Rigidbody2D rb;
    SpriteRenderer spr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            spr.enabled = true;
            rb.gravityScale = 1;
            rb.AddForce(vec * speed, ForceMode2D.Impulse);
        }
    }
}
