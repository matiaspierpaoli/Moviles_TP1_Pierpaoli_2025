using System.Diagnostics.Contracts;
using UnityEngine;

using static GameSignals;

public class ControlDireccion : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    float Giro = 0;

    public bool Habilitado = false;
    CarController carController;

    public int playerId = -1;
    public string inputName = "Horizontal";

    //---------------------------------------------------------//

    private void OnEnable()
    {
        MatchStarted += TurnOnDirection;
    }

    private void OnDisable()
    {
        MatchStarted -= TurnOnDirection;
    }

    // Use this for initialization
    void Start()
    {
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Habilitado) return;

        Giro = inputManager.GetAxis(inputName, playerId.ToString());
        carController.SetGiro(Giro);
    }

    private void TurnOnDirection()
    {
        Habilitado = true;
    }

    public float GetGiro()
    {
        return Giro;
    }

}
