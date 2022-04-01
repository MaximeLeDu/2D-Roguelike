using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    private BoardManager boardManager;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playerTurn = true;

    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    private int level = 1;

    private List<Enemy> enemies;
    private bool enemiesMoving;

    private Text levelText;
    private GameObject levelImage;
    private bool doingSetup;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);

        enemies = new List<Enemy>();
        boardManager = GetComponent<BoardManager>();
        InitGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("Registered");
    }

    static private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Heya");
        Instance.level++;
        Instance.InitGame();
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("Level Image");
        levelText = GameObject.Find("Level Text").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke(nameof(HideLevelImage), levelStartDelay);

        enemies.Clear();    
        boardManager.SetupScene(level);
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    private void Update()
    {
        if (playerTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemiesToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i= 0;i< enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playerTurn = true;
        enemiesMoving = false;
    }
}
