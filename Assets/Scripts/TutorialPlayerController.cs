using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    private Rigidbody2D playerRigidbody;
    public bool isGrounded;
    public LayerMask whatIsGroundLayer;
    private Collider2D playerCollider;
    private Color[] colorOrder = { Color.red, Color.green, Color.yellow };
    private int currentColorIndex = 0;
    private GameObject currentPlatform;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.IsTouchingLayers(playerCollider, whatIsGroundLayer);
        
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        playerRigidbody.velocity = new Vector2(horizontalInput * moveSpeed, playerRigidbody.velocity.y);

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeColorAscending();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeColorDescending();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
        }

        if (currentPlatform != null)
        {
            Color platformColor = currentPlatform.GetComponent<SpriteRenderer>().color;
            if (spriteRenderer.color != platformColor && platformColor != Color.white)
            {
                RestartGame();
                return;
            }
        }
    }

    void ChangeColorAscending()
    {
        currentColorIndex = (currentColorIndex + 1) % colorOrder.Length;

        Color newColor = colorOrder[currentColorIndex];

        spriteRenderer.color = newColor; // Apply color change
    }

    void ChangeColorDescending()
    {
        currentColorIndex = (currentColorIndex - 1);
        if (currentColorIndex < 0)
        {
            currentColorIndex = colorOrder.Length - 1;
        }

        Color newColor = colorOrder[currentColorIndex];

        spriteRenderer.color = newColor; // Apply color change
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = collision.gameObject;
            Color platformColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
            Debug.Log("Platform color: " + platformColor);
            Debug.Log("Player color: " + spriteRenderer.color);
            if (spriteRenderer.color != platformColor && platformColor != Color.white)
            {
                RestartGame();
                Debug.Log("Game Over! Player landed on a different color platform.");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = null;
            transform.SetParent(null);
        }
    }
}
