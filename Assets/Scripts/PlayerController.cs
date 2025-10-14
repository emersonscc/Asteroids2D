/*
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;         //Floating point variable to store the player's movement speed.

    private Rigidbody2D rb2d;   //Store a reference to the Rigidbody2D component required to use 2D Physics.
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
    // FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        //Store the current horizontal input in the float moveHorizontal.
        float moveHorizontal = Input.GetAxis ("Horizontal");

        //Store the current vertical input in the float moveVertical.
        float moveVertical = Input.GetAxis ("Vertical");

        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 movement = new Vector2 (moveHorizontal, moveVertical);

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        rb2d.AddForce (movement * speed);
    }
	
    //OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    void OnTriggerEnter2D(Collider2D other) 
    {
        //Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }
    }	
}

//teste
*/
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float thrust = 7f;            // ↑ acelera
    public float reverseThrust = 4f;     // ↓ ré/freio
    public float rotationSpeed = 180f;   // ←/→ giram
    public float maxSpeed = 10f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 18f;
    public float fireCooldown = 0.15f;

    Rigidbody2D rb;     // RB do corpo (raiz)
    Transform body;     // Transform que realmente move/rota (o do RB)
    float fireTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
        if (rb == null) { Debug.LogError("PlayerController: Rigidbody2D não encontrado no Player."); enabled = false; return; }
        body = rb.transform;

        if (firePoint == null)
        {
            var fp = body.Find("FirePoint");
            if (fp != null) firePoint = fp;
        }
    }

    void Update()
    {
        // Rotação com setas
        float rot = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))  rot += 1f;
        if (Input.GetKey(KeyCode.RightArrow)) rot -= 1f;
        body.Rotate(0f, 0f, rot * rotationSpeed * Time.deltaTime);

        // Tiro com Espaço
        fireTimer -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && fireTimer <= 0f)
        {
            fireTimer = fireCooldown;
            if (bulletPrefab != null && firePoint != null)
            {
                var b = Instantiate(bulletPrefab, firePoint.position, body.rotation);
                var brb = b.GetComponent<Rigidbody2D>();
                Vector2 forward = (Vector2)body.up;
                brb.linearVelocity = forward * bulletSpeed + rb.linearVelocity * 0.3f;
            }
            else
            {
                Debug.LogWarning("PlayerController: bulletPrefab ou firePoint não atribuídos.");
            }
        }

        ScreenWrap();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            rb.AddForce((Vector2)body.up * thrust);

        if (Input.GetKey(KeyCode.DownArrow))
            rb.AddForce((Vector2)(-body.up) * reverseThrust);

        if (rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void ScreenWrap()
    {
        var cam = Camera.main;
        var v = cam.WorldToViewportPoint(body.position);
        if (v.x > 1f) v.x = 0f; else if (v.x < 0f) v.x = 1f;
        if (v.y > 1f) v.y = 0f; else if (v.y < 0f) v.y = 1f;
        var w = cam.ViewportToWorldPoint(v);
        body.position = new Vector3(w.x, w.y, 0f);
    }
}
