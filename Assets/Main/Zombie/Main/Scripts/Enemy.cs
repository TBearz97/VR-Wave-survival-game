using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class Enemy : MonoBehaviour
{
    
    private float health;
    [Header("Stats")]
    public float maxHealth;
    public float damage;
    private float targetRadius = 30f;
    [Range(0, 100)]
    public int attackChance;
    public int speed;
    public GameObject aggressive;
    public int maxActive;

    private bool attacking;
    private bool alive = true;

    public GameObject player;
    public GameObject graveyard;
    private Player playerScript;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioResource attackSound;
    public AudioResource loopSound;
    public AudioClip hitSound;

    private NavMeshAgent nav;
    private Animator animator;
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerScript = player.GetComponent<Player>();

        health = maxHealth;
    }

    void Update()
    {
        aggressive.SetActive(attacking);
        if (attacking)
        {
            nav.SetDestination(player.transform.position);
        }
        if (player != null && !nav.pathPending && nav.remainingDistance <= 0.1f && alive)
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

    public bool TakeDamage(float damage)
    {
        health = health - damage;
        audioSource.PlayOneShot(hitSound, 1);
        if (health < 0)
        {
            alive = false;
            attacking = false;
            animator.SetBool("Die", true);
            nav.ResetPath();
            audioSource.Stop();
            playerScript.GainExperience((int)maxHealth);
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(DisableAfterSeconds(5));
            return true;
        }
        return true;
    }

    public void FindPath()
    {
        attacking = Random.Range(0, 99) < attackChance;
        if (attacking)
        {
            audioSource.resource = loopSound;
            audioSource.spatialBlend = 1;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Vector2 randomOffset = Random.insideUnitCircle * targetRadius;
            Vector3 pathPos = new Vector3(
                player.transform.position.x + randomOffset.x,
                player.transform.position.y,
                player.transform.position.z + randomOffset.y);

            nav.SetDestination(pathPos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null && attacking)
        {
            player.TakeDamage(damage);
            attacking = false;
            audioSource.loop = false;
            audioSource.Stop();
        }
    }

    private IEnumerator DisableAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
        transform.SetParent(graveyard.transform, false);
        animator.SetBool("Die", false);
    }
}
