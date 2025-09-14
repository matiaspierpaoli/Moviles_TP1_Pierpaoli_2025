using UnityEngine;
using static GameSignals;

/// <summary>
/// Activa/desactiva los paneles de cada jugador cuando el GM lo “avisa”.
/// Cada panel contiene su VirtualJoystick con playerID configurado (“1” o “2”).
/// </summary>
public class PlayerUICanvasManager : MonoBehaviour
{
    [Header("Asignar en el Canvas principal (Overlay)")]
    [SerializeField] private GameObject leftPanel;   // mitad izquierda
    [SerializeField] private GameObject rightPanel;  // mitad derecha

    private void OnEnable()
    {
        TogglePlayerUI += OnTogglePlayerUI;
        PlayerSideAssigned += OnPlayerSideAssigned;
    }

    private void OnDisable()
    {
        TogglePlayerUI -= OnTogglePlayerUI;
        PlayerSideAssigned -= OnPlayerSideAssigned;
    }

    private void Start()
    {
    }

    private void OnPlayerSideAssigned(int playerId, PlayerSide side)
    {
    }

    private void OnTogglePlayerUI(int playerId, bool visible)
    {
        if (playerId == 0 && leftPanel) leftPanel.SetActive(visible);
        if (playerId == 1 && rightPanel) rightPanel.SetActive(visible);
    }
}
