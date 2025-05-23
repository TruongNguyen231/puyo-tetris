using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using Assets.Scripts;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    // Create -> Fall -> Arrange -> Link -> Calculate combo
    public enum GameStatus
    {
        GameInitializing, PuyoCreating, PuyoFalling, PuyoArranging, PuyoLinking, ComboCalculating, GamePause, GameOver
    }
    public static GameMaster Instance { get; private set; }
    public GameObject puyoGroup;
    public float fallingSpeed;
    public GameObject mainPuyoShiny;
    public GameObject gameOver;
    public Text scoresText;
    public static GameStatus gameStatus;
    public static GameObject puyoGroupObj;
    public static GameObject mainPuyoShinyObj;
    public static GameObject gameOverObj;
    public static Puyo[,] puyoArr = new Puyo[6, 13];
    public static Puyo controlMainPuyo;
    public static Puyo controlSubPuyo;
    public static Queue<Puyo> puyoInventory;
    //0=top, 1=right, 2=down, 3=left
    public static int subPuyoDirection = 2;
    public static int comboNumber = 0;
    public long scorces = 0;
    public int destroyPuyoNumber = 0;
    public static int bottomPosition = -176;
    public static int leftPosition = -96;
    public static int rightPosition = 64;

    private bool falling = true;
    [SerializeField]
    public static List<Rank> ranksList;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start() {
        puyoGroupObj = puyoGroup;
        gameOverObj = gameOver;
        puyoInventory = new Queue<Puyo>();
        gameStatus = GameStatus.GameInitializing;
        mainPuyoShinyObj = mainPuyoShiny;
        Instance.scorces = 0;
        Instance.destroyPuyoNumber = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameStatus == GameStatus.GameInitializing)
        {
            puyoInventory.Enqueue(PuyoCreater.PuyoCreate(100, 175));
            puyoInventory.Enqueue(PuyoCreater.PuyoCreate(100, 143));
            puyoInventory.Enqueue(PuyoCreater.PuyoCreate(100, 50));
            puyoInventory.Enqueue(PuyoCreater.PuyoCreate(100, 18));

            gameStatus = GameStatus.PuyoCreating;
        }

        if (gameStatus == GameStatus.PuyoCreating)
        {
            PuyoController.puyoCreate();
            gameStatus = GameStatus.PuyoFalling;
        }

        if (gameStatus == GameStatus.PuyoFalling)
        {
            if (falling)
            {
                StartCoroutine("fallingGap");
                falling = false;
            }
        }

        if (gameStatus == GameStatus.PuyoArranging)
        {
            PuyoController.puyoArrange();
            gameStatus = GameStatus.PuyoLinking;
        }

        if (gameStatus == GameStatus.PuyoLinking)
        {
            PuyoController.resetPuyoStatusAndLinkPuyoList();
            PuyoController.linkSamePuyo();
            gameStatus = GameStatus.ComboCalculating;
        }

        if (gameStatus == GameStatus.ComboCalculating)
        {
            if (PuyoController.readyToEliminatePuyo())
            {
                StartCoroutine("statusChangingGap");
                gameStatus = GameStatus.GamePause;
            }
            else
            {
                gameStatus = GameStatus.PuyoCreating;
            }
        }
        
    }

    IEnumerator fallingGap() {
        yield return new WaitForSeconds(fallingSpeed);
        //If reach the bottom, create new puyo
        if (PuyoController.reachBottom((int)controlMainPuyo.getPosition().x, (int)controlMainPuyo.getPosition().y) || 
            PuyoController.reachBottom((int)controlSubPuyo.getPosition().x, (int)controlSubPuyo.getPosition().y))
        {
            if (PuyoController.isGameOver())
            {
                gameOverObj.SetActive(true);
                if(scorces > 0)
                {
                    SaveScore();
                }
                gameStatus = GameStatus.GamePause;
            }
            else
            {
                int mainX = (int)controlMainPuyo.getPosition().x;
                int mainY = (int)controlMainPuyo.getPosition().y;
                int subX = (int)controlSubPuyo.getPosition().x;
                int subY = (int)controlSubPuyo.getPosition().y;
                puyoArr[mainX, mainY] = controlMainPuyo;
                puyoArr[subX, subY] = controlSubPuyo;

                gameStatus = GameStatus.PuyoArranging;
            }
        }
        else
        {
            PuyoController.puyoDown(true);
        }
        falling = true;
    }
    public void UpdateScores()
    {
        scoresText.text = "Scores: " + Instance.scorces;
    }
    private void SaveScore()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            if (PlayerPrefs.GetInt("HighScore") < scorces)
            {
                PlayerPrefs.SetInt("HighScore", (int)scorces);
            }
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", (int)scorces);
        }

        SaveScoreToRankFile();
    }

    private void SaveScoreToRankFile()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "rank.json");
        List<Rank> scoresList = new List<Rank>();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            scoresList = JsonUtility.FromJson<RankList>(json).scores;
        }
        else
        {
            File.Create(filePath).Dispose();
            Debug.Log("Đã tạo file!");
        }

        var newScores = new Rank { scores = scorces, destroyedPuyo = destroyPuyoNumber };
        scoresList.Add(newScores);
        scoresList.Sort((x, y) =>
        {
            int scoreComparison = y.scores.CompareTo(x.scores);
            if (scoreComparison == 0)
            {
                return y.destroyedPuyo.CompareTo(x.destroyedPuyo);
            }
            return scoreComparison;
        });

        RankList rankList = new RankList { scores = scoresList };
        string newJson = JsonUtility.ToJson(rankList, true);
        File.WriteAllText(filePath, newJson);
        Debug.Log("Đã lưu điểm!");
    }

    public void BackToMain()
    {
        Time.timeScale = 1;
        gameStatus = GameStatus.GameInitializing; // Reset trạng thái game
        puyoArr = new Puyo[6, 13]; // Reset mảng puyo
        puyoInventory.Clear(); // Xóa queue puyo
        comboNumber = 0;
        scorces = 0;
        destroyPuyoNumber = 0;
        SceneManager.LoadScene("Start Menu", LoadSceneMode.Single);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    //Before eliminated puyo, wait a while.
    IEnumerator statusChangingGap()
    {
        yield return new WaitForSeconds(0.8f);
        StartCoroutine("showComboImg");
        ImageController.setComboNumber(++comboNumber);
        PuyoController.eliminatePuyo();
        gameStatus = GameStatus.PuyoArranging;
    }

    IEnumerator showComboImg()
    {
        ImageController.Instance.comboGameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        ImageController.Instance.comboGameObject.SetActive(false);
    }
}

