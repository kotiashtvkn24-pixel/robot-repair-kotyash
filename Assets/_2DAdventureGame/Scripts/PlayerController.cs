using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    public float speed = 3.0f;

    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip healSound;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    public InputAction LaunchAction;

    public InputAction TalkAction;
    private NonPlayerCharacter lastNonPlayerCharacter;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void OnEnable()
    {
        SetInputEnabled(true);
    }

    void OnDisable()
    {
        SetInputEnabled(false);
    }

    public void SetInputEnabled(bool active)
    {
        if (active)
        {
            MoveAction.Enable();
            LaunchAction.Enable();
            TalkAction.Enable();
        }
        else
        {
            MoveAction.Disable();
            LaunchAction.Disable();
            TalkAction.Disable();
            move = Vector2.zero;
        }
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        //Debug.Log(move);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (LaunchAction.WasPressedThisFrame())
        {
            Launch();
        }

        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f,
            moveDirection, 1.5f, LayerMask.GetMask("NPC"));
        if (hit.collider != null)
        {
            NonPlayerCharacter npc = hit.collider.GetComponent<NonPlayerCharacter>();
            npc.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = npc;
            FindFriend();
        }
        else
        {
            if (lastNonPlayerCharacter != null)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                lastNonPlayerCharacter = null;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position
            + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public bool ChangeHealth(int amount)
    {
        if (currentHealth == 0 || (amount < 0 && isInvincible))
        {
            return false;
        }

        int nextHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (nextHealth == currentHealth)
        {
            return false;
        }

        if (amount < 0)
        {
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }

        currentHealth = nextHealth;
        UIHandler.instance.SetHealthValue(currentHealth, maxHealth, amount < 0);
        AudioClip clip = amount < 0 ? hitSound : healSound;
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        return true;
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);

        animator.SetTrigger("Launch");
    }

    void FindFriend()
    {
        if (TalkAction.WasPressedThisFrame())
        {
            UIHandler.instance.DisplayDialogue();
        }
    }
}
