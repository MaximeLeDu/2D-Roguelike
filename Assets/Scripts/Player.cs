using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;

    public Text foodText;

    public float restartLevelDelay = 1.0f;

    private Animator animator;
    private int food;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;    
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.Instance.playerFoodPoints;

        foodText.text = "Food " + food;

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.Instance.playerFoodPoints = food;
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.Instance.GameOver();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.playerTurn || isMoving) return;

        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");
        if (horizontal != 0) 
            vertical = 0;

        if(horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        GameManager.Instance.playerTurn = false;
        food--;
        foodText.text = "Food " + food;

        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if (hit.transform == null)
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            return;
        }


        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);

        CheckIfGameOver();
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("PlayerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("PlayerHit");
        food -= loss;
        foodText.text = "Food " + food;
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exit"))
        {
            Invoke(nameof(Restart), restartLevelDelay);
            enabled = false;
        }
        else if (collision.CompareTag("Food"))
        {
            food += pointsPerFood;
            foodText.text = "Food " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Soda"))
        {
            food += pointsPerSoda;
            foodText.text = "Food " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            collision.gameObject.SetActive(false);
        }
    }
}
