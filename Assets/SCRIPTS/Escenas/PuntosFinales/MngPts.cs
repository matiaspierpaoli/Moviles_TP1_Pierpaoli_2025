using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MngPts : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public string creditsSceneName = "Credits";

    public float TiempEmpAnims = 2.5f;
    public float TiempEspReiniciar = 10f;
    public float TiempParpadeo = 0.7f;

    public float sceneTransitionDuration = 10f;

    private float tempo = 0;
    private float tempoParpadeo = 0;
    private bool primerImaParp = true;
    private bool activadoAnims = false;

    public Text leftMoneyText;
    public Text rightMoneyText;
    public Image winningPlayerImage;

    public Sprite leftWinningPlayerSprite;
    public Sprite rightWinningPlayerSprite;

    private Visualizacion viz = new Visualizacion();

    GameMode gameMode;
    int player1Money;
    int player2Money;

    void Start()
    {
        gameMode = GameContext.Instance.Current.mode;
        player1Money = GameContext.Instance.Current.player1Money;
        player2Money = GameContext.Instance.Current.player2Money;

        if (gameMode == GameMode.SinglePlayer)
            DisableSecondPlayerUI();

        SetGanador();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(0);
        }

        TiempEspReiniciar -= Time.deltaTime;
        if (TiempEspReiniciar <= 0)
        {
            SceneManager.LoadScene(0);
        }

        if (activadoAnims)
        {
            tempoParpadeo += Time.deltaTime;

            if (tempoParpadeo >= TiempParpadeo)
            {
                tempoParpadeo = 0;
                primerImaParp = !primerImaParp;
            }
        }

        if (!activadoAnims)
        {
            tempo += Time.deltaTime;
            if (tempo >= TiempEmpAnims)
            {
                tempo = 0;
                activadoAnims = true;
                SetDinero();
            }
        }
    }

    private void DisableSecondPlayerUI()
    {
        rightMoneyText.gameObject.SetActive(false);
    }

    void SetGanador()
    {
        if (gameMode == GameMode.Multiplayer)
        {
            if (player1Money > player2Money)
                winningPlayerImage.sprite = leftWinningPlayerSprite;
            else
                winningPlayerImage.sprite = rightWinningPlayerSprite;
        }
        else
        {
            winningPlayerImage.gameObject.SetActive(false);
        }
    }

    void SetDinero()
    {
        if (gameMode == GameMode.Multiplayer)
        {
            leftMoneyText.text = "$" + viz.PrepararNumeros(player1Money);
            rightMoneyText.text = "$" + viz.PrepararNumeros(player2Money);
        }
        else
        {
            leftMoneyText.text = "$" + viz.PrepararNumeros(player1Money);
        }
    }
}
