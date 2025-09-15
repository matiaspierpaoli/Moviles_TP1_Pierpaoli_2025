using UnityEngine;
using System.Collections;

using static GameSignals;

public class Player : MonoBehaviour 
{
	public int Dinero = 0;
	public int IdPlayer = 0;
	
	public Bolsa[] Bolasas;
	int CantBolsAct = 0;
	public string TagBolsas = "";
	
	public enum Estados{EnDescarga, EnConduccion, EnCalibracion, Default }
	public Estados EstAct = Estados.EnConduccion;
	
	public bool EnConduccion = true;
	public bool EnDescarga = false;
	
	public ControladorDeDescarga ContrDesc;
	public ContrCalibracion ContrCalib;
	
	Visualizacion MiVisualizacion;

    public bool selected = false;
    public bool FinCalibrado = false;
    public bool FinTuto = false;

    //------------------------------------------------------------------//

    private void OnEnable()
    {
		GameStateChanged += OnGameStateChanged;
		PlayerSelected += OnPlayerSelected;
    }

    private void OnDisable()
    {
        GameStateChanged -= OnGameStateChanged;
        PlayerSelected -= OnPlayerSelected;
    }

    // Use this for initialization
    void Start () 
	{
		for(int i = 0; i< Bolasas.Length;i++)
			Bolasas[i] = null;
		
		MiVisualizacion = GetComponent<Visualizacion>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	//------------------------------------------------------------------//
	
	private void OnPlayerSelected(int id)
	{
		if (id == IdPlayer)
			selected = true;
	}


    public bool AgregarBolsa(Bolsa b)
	{
		if(CantBolsAct + 1 <= Bolasas.Length)
		{
			Bolasas[CantBolsAct] = b;
			CantBolsAct++;
			Dinero += (int)b.Monto;
			b.Desaparecer();
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public void VaciarInv()
	{
		for(int i = 0; i< Bolasas.Length;i++)
			Bolasas[i] = null;
		
		CantBolsAct = 0;
	}
	
	public bool ConBolasas()
	{
		for(int i = 0; i< Bolasas.Length;i++)
		{
			if(Bolasas[i] != null)
			{
				return true;
			}
		}
		return false;
	}
	
	public void SetContrDesc(ControladorDeDescarga contr)
	{
		ContrDesc = contr;
	}
	
	public ControladorDeDescarga GetContr()
	{
		return ContrDesc;
	}
	
	private void OnGameStateChanged(GameState state)
	{
		switch (state)	
		{

			case GameState.Calibrating:
				CambiarACalibracion();
                break;
			case GameState.Playing:
				CambiarAConduccion();
                break;
			case GameState.Paused:
				break;
			case GameState.Finished:
				break;
			default:
				break;
		}
	}


    public void CambiarACalibracion()
	{
		EstAct = Player.Estados.EnCalibracion;
		MiVisualizacion.CambiarACalibracion();
	}
	
	public void CambiarAConduccion()
	{
		EstAct = Player.Estados.EnConduccion;
		MiVisualizacion.CambiarAConduccion();
	}
	
	public void CambiarADescarga()
	{
		EstAct = Player.Estados.EnDescarga;
		MiVisualizacion.CambiarADescarga();
	}
	
	public void SacarBolasa()
	{
		for(int i = 0; i < Bolasas.Length; i++)
		{
			if(Bolasas[i] != null)
			{
				Bolasas[i] = null;
				return;
			}				
		}
	}
	
	
}
