using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WASDMovement : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Vector3 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // evita tombar com colisões
    }

    void Update()
    {
        // Captura do input (WASD) – mapeado por padrão em Horizontal/Vertical
        float x = Input.GetAxisRaw("Horizontal"); // A/D
        float y = Input.GetAxisRaw("Vertical");   // W/S
        input = new Vector3(x, y, 0).normalized;
    }

    void FixedUpdate()
    {
        // Movimento baseado em física
        Vector3 velocity = input * speed;
        Vector3 move = velocity * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + (Vector2)move);
    }
}