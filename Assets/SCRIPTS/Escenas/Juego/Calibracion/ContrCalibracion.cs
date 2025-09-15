using UnityEngine;
using System.Collections;

using static GameSignals;

public class ContrCalibracion : MonoBehaviour
{
	public Player Pj;

	public float TiempEspCalib = 3;
	float Tempo2 = 0;
	
	public enum Estados{Calibrating, Tutorial, Finished}
	public Estados EstAct = Estados.Calibrating;
	
	public ManejoPallets Partida;
	public ManejoPallets Llegada;
	public Pallet P;
    public ManejoPallets palletsMover;

    //----------------------------------------------------//

    void Start () 
	{
        palletsMover.enabled = false;
        Pj.ContrCalib = this;		
		
		P.CintaReceptora = Llegada.gameObject;
		Partida.Recibir(P);
		
		SetActivComp(false);
	}
	
	void Update ()
	{
        if (EstAct == ContrCalibracion.Estados.Calibrating && Pj.selected)
        {
            IniciarTesteo();
        }
        if (EstAct == ContrCalibracion.Estados.Tutorial)
        {
            if (Tempo2 < TiempEspCalib)
            {
                Tempo2 += T.GetDT();
                if (Tempo2 > TiempEspCalib)
                {
                    SetActivComp(true);
                }
            }
        }
    }

    void IniciarTesteo()
    {
        EstAct = Estados.Tutorial;
        palletsMover.enabled = true;
    }
	
	public void FinTutorial()
	{
		EstAct = ContrCalibracion.Estados.Finished;
        palletsMover.enabled = false;
        Pj.FinCalibrado = true;
        RaiseCalibrationDone(Pj.IdPlayer);
    }
	
	void SetActivComp(bool estado)
	{
		if(Partida.GetComponent<Renderer>() != null)
			Partida.GetComponent<Renderer>().enabled = estado;
		Partida.GetComponent<Collider>().enabled = estado;
		if(Llegada.GetComponent<Renderer>() != null)
			Llegada.GetComponent<Renderer>().enabled = estado;
		Llegada.GetComponent<Collider>().enabled = estado;
		P.GetComponent<Renderer>().enabled = estado;
	}
}
