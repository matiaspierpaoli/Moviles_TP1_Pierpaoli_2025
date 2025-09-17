using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static DatosPartida;
using static GameSignals;

/// <summary>
/// clase encargada de TODA la visualizacion
/// de cada player, todo aquello que corresconda a 
/// cada seccion de la pantalla independientemente
/// </summary>
public class Visualizacion : MonoBehaviour 
{
    [Header("General")]
    [SerializeField] private GameObject calibratingScene;

    [Header("UI")]
    [SerializeField] private GameObject UIRoot;
    [SerializeField] GameObject waitStateObject;
    [SerializeField] GameObject virtualJoystickObject;

    [Header("Deposit")]
    public GameObject BonusRoot;
    public Image BonusFill;
    public Text BonusText;

    [Header("Cameras")]
    public Camera CamCalibracion;
	public Camera CamConduccion;
	public Camera CamDescarga;

    [Header("Inventory")]
    public Image Inventario;
    public float Parpadeo = 0.8f;
    public float TempParp = 0;
    public bool PrimIma = true;
    public Sprite[] InvSprites;
    public Text Dinero;


    [Header("Tutorial Steps")]
    public GameObject TutoCalibrando;
    public GameObject TutoDescargando;
    public GameObject TutoFinalizado;

    private EnableInPlayerState[] enableInPlayerStates;
    private PlayerSide LadoAct = PlayerSide.Default;

	ControlDireccion Direccion;
	Player Pj;
    
    //------------------------------------------------------------------//

    private void OnEnable()
    {
		PlayerSideAssigned += OnPlayerSideAssigned;
        PlayerFinished += OnPlayerFinished;
        MatchStarted += OnMatchStarted;
    }

    private void OnDisable()
    {
        PlayerSideAssigned -= OnPlayerSideAssigned;
        PlayerFinished -= OnPlayerFinished;
        MatchStarted -= OnMatchStarted;
    }

    private void Awake()
    {
		Pj = GetComponent<Player>();
        enableInPlayerStates = UIRoot.GetComponentsInChildren<EnableInPlayerState>(includeInactive: true);

        foreach (var component in enableInPlayerStates)
        {
            component.gameObject.SetActive(false);
        }

        CamCalibracion.enabled = false;
        CamConduccion.enabled = false;
        CamDescarga.enabled = false;
    }

    void Start () 
	{
		Direccion = GetComponent<ControlDireccion>();

        virtualJoystickObject.SetActive(true);
        UIRoot.SetActive(true);
        waitStateObject.SetActive(false);
        calibratingScene.SetActive(true);
    }
	
	void Update () 
	{
        switch (Pj.EstAct)
        {
            case Player.Estados.EnCalibracion:
                SetCalibr();
                break;
            case Player.Estados.EnConduccion:
                //inventario
                SetInv();
                //contador de dinero
                SetDinero();
                break;
            case Player.Estados.EnDescarga:
                //inventario
                SetInv();
                //el bonus
                SetBonus();
                //contador de dinero
                SetDinero();
                break;
        }
    }		
	
	//--------------------------------------------------------//
	
	public void CambiarACalibracion()
	{
		CamCalibracion.enabled = true;
		CamConduccion.enabled = false;
		CamDescarga.enabled = false;

		foreach (var item in enableInPlayerStates)
			item.SetPlayerState(Pj.EstAct);

    }
	
	public void CambiarAConduccion()
	{
		CamCalibracion.enabled = false;
		CamConduccion.enabled = true;
		CamDescarga.enabled = false;

        foreach (var item in enableInPlayerStates)
            item.SetPlayerState(Pj.EstAct);
    }
	
	public void CambiarADescarga()
	{
		CamCalibracion.enabled = false;
		CamConduccion.enabled = false;
		CamDescarga.enabled = true;

        foreach (var item in enableInPlayerStates)
            item.SetPlayerState(Pj.EstAct);
    }
	
	//---------//
	
    private void OnPlayerFinished(int id)
    {
        if (id == Pj.IdPlayer)
        {
            waitStateObject.SetActive(true);
            virtualJoystickObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void OnPlayerSideAssigned(int id, PlayerSide side)
	{
        if (id == Pj.IdPlayer)
            SetLado(side);
    }

    public void SetLado(PlayerSide lado)
	{
		if (LadoAct != PlayerSide.Default) return;
		LadoAct = lado;

        Rect r = new Rect();
        r.width = CamConduccion.rect.width;
        r.height = CamConduccion.rect.height;
        r.y = CamConduccion.rect.y;

        switch (lado)
        {
            case PlayerSide.Middle:
                r.width = 1.0f;

                RectTransform rectTransform = UIRoot.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0.5f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 1f);

                rectTransform.offsetMin = new Vector2(0.0f, rectTransform.offsetMin.y);
                break;
            case PlayerSide.Right:
                r.x = 0.5f;
                break;
            case PlayerSide.Left:
                r.x = 0;
                break;
        }

        CamCalibracion.rect = r;
        CamConduccion.rect = r;
        CamDescarga.rect = r;
    }
	
    private void OnMatchStarted()
    {
        calibratingScene.SetActive(false);
    }


    void SetBonus()
	{
        if (Pj.ContrDesc.PEnMov != null)
        {
            BonusRoot.SetActive(true);

            //el fondo
            float bonus = Pj.ContrDesc.Bonus;
            float max = (float)(int)Pallet.Valores.Valor1;
            float t = bonus / max;
            BonusFill.fillAmount = t;
            //la bolsa
            BonusText.text = "$" + Pj.ContrDesc.Bonus.ToString("0");
        }
        else
        {
            BonusRoot.SetActive(false);
        }
    }
	
	void SetDinero()
	{
        Dinero.text = PrepararNumeros(Pj.Dinero);
    }
	
	void SetCalibr()
	{
        if (!Pj.ContrCalib) return;

        switch (Pj.ContrCalib.EstAct)
        {
            case ContrCalibracion.Estados.Calibrating:
                TutoCalibrando.SetActive(true);
                TutoDescargando.SetActive(false);
                TutoFinalizado.SetActive(false);
                break;

            case ContrCalibracion.Estados.Tutorial:
                TutoCalibrando.SetActive(false);
                TutoDescargando.SetActive(true);
                TutoFinalizado.SetActive(false);
                break;

            case ContrCalibracion.Estados.Finished:
                TutoCalibrando.SetActive(false);
                TutoDescargando.SetActive(false);
                TutoFinalizado.SetActive(true);
                break;
        }
    }

    void SetInv()
    {
        int contador = 0;
        for (int i = 0; i < 3; i++)
        {
            if (Pj.Bolasas[i] != null)
                contador++;
        }

        if (contador >= 3)
        {
            TempParp += T.GetDT();

            if (TempParp >= Parpadeo)
            {
                TempParp = 0;
                if (PrimIma)
                    PrimIma = false;
                else
                    PrimIma = true;


                if (PrimIma)
                {
                    Inventario.sprite = InvSprites[3];
                }
                else
                {
                    Inventario.sprite = InvSprites[4];
                }
            }
        }
        else
        {
            Inventario.sprite = InvSprites[contador];
        }
    }

	public string PrepararNumeros(int dinero)
	{
        string strDinero = dinero.ToString();
        string res = "";

        if (dinero < 1)//sin ditero
        {
            res = "";
        }
        else if (strDinero.Length == 6)//cientos de miles
        {
            for (int i = 0; i < strDinero.Length; i++)
            {
                res += strDinero[i];

                if (i == 2)
                {
                    res += ".";
                }
            }
        }
        else if (strDinero.Length == 7)//millones
        {
            for (int i = 0; i < strDinero.Length; i++)
            {
                res += strDinero[i];

                if (i == 0 || i == 3)
                {
                    res += ".";
                }
            }
        }

        return res;
    }
	
	public PlayerSide GetLadoActual() => LadoAct;
}
