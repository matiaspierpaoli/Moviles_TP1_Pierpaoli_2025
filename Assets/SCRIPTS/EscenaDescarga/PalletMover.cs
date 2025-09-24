using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletMover : ManejoPallets
{
    [SerializeField] private InputManager inputManager;
    public int playerID = -1;
    public string horizontalInputName = "Horizontal";
    public string verticalInputName = "Vertical";

    public ManejoPallets Desde, Hasta;
    private enum EstadoPallet { Default, FirstStep, SecondStep }
    private EstadoPallet currentState = EstadoPallet.Default;

    private void Update()
    {
        float horizontalInput = InputManager.Instance.GetAxis(horizontalInputName, playerID.ToString());
        float verticalInput = InputManager.Instance.GetAxis(verticalInputName, playerID.ToString());

        switch (currentState)
        {
            case EstadoPallet.Default:
                if (!Tenencia() && Desde.Tenencia() && horizontalInput < -InputManager.Instance.GetMinAxisValue())
                {
                    PrimerPaso();
                }
                break;

            case EstadoPallet.FirstStep:
                if (Tenencia() && verticalInput < -InputManager.Instance.GetMinAxisValue())
                {
                    SegundoPaso();
                }
                break;

            case EstadoPallet.SecondStep:
                if (horizontalInput > InputManager.Instance.GetMinAxisValue())
                {
                    TercerPaso();
                }
                break;
        }
    }

    void PrimerPaso()
    {
        Desde.Dar(this);
        currentState = EstadoPallet.FirstStep;
    }
    void SegundoPaso()
    {
        base.Pallets[0].transform.position = transform.position;
        currentState = EstadoPallet.SecondStep;
    }
    void TercerPaso()
    {
        Dar(Hasta);
        currentState = EstadoPallet.Default;
    }

    public override void Dar(ManejoPallets receptor)
    {
        if (Tenencia())
        {
            if (receptor.Recibir(Pallets[0]))
            {
                Pallets.RemoveAt(0);
            }
        }
    }
    public override bool Recibir(Pallet pallet)
    {
        if (!Tenencia())
        {
            pallet.Portador = this.gameObject;
            base.Recibir(pallet);
            return true;
        }
        else
            return false;
    }
}
