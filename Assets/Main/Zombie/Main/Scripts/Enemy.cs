using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Enemy : MonoBehaviour
{
    
    private float health;
    [Header("Stats")]
    public float maxHealth;
    public float damage;
    public float targetRadius = 20f;
    public int attackChance;
    public int speeds;
    public GameObject aggressive;

    private bool attacking;
    private bool alive = true;

    private GameObject targetPos;
    private Player player;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioResource attackSound;
    public AudioResource loopSound;

    private NavMeshAgent nav;
    private Animator animator;
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = targetPos.GetComponent<Player>();

        nav.speed = speeds;
        health = maxHealth;
    }

    void Update()
    {
        aggressive.SetActive(attacking);
        if (attacking)
        {
            nav.SetDestination(targetPos.transform.position);
        }
        if (targetPos != null && !nav.pathPending && nav.remainingDistance <= 0.1f && alive)
        {
            FindPath();
        }
        if (!nav.hasPath)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Walking", false);
        } else
        {
            if (nav.speed < 3)
            {
                animator.SetBool("Walking", true);
                animator.SetBool("Running", false);
            }
            else
            {
                animator.SetBool("Running", true);
                animator.SetBool("Walking", false);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health = health - damage;
        if (health < 0)
        {
            alive = false;
            attacking = false;
            animator.SetBool("Die", true);
            nav.ResetPath();
            Destroy(this.gameObject, 5f);
        }
    }

    public void FindPath()
    {
        int attack = Random.Range(0, attackChance);
        if (attack == 0)
        {
            attacking = true;
            audioSource.resource = attackSound;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
        else
        {
            Vector2 randomOffset = Random.insideUnitCircle * targetRadius;
            Vector3 pathPos = new Vector3(
                targetPos.transform.position.x + randomOffset.x,
                targetPos.transform.position.y,
                targetPos.transform.position.z + randomOffset.y);

            nav.SetDestination(pathPos);
        }
    }

    public void SetTargetPos(GameObject Pos)
    {
        targetPos = Pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            attacking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            attacking = false;
        }
    }
}
