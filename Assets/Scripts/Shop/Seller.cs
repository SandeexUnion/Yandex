using UnityEngine;

public class Seller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject DialogWindow;
    [SerializeField] Sprite sideSprite;
    [SerializeField] Sprite frontSprite;
    SpriteRenderer spriteRenderer;
    PlayerController playerController;
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        Flip();
    }

    void Flip()
    {
        if (playerController.transform.position.x > transform.position.x-3 && playerController.transform.position.x < transform.position.x + 3)
        {
            spriteRenderer.sprite = frontSprite;
        }
        else
        {
            spriteRenderer.sprite = sideSprite;
            if(playerController.transform.position.x < transform.position.x - 3)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}
