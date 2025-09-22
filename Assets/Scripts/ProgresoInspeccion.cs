using UnityEngine;

public class ProgresoInspeccion : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject panelPopup; // Panel que aparece al terminar el recorrido
    public bool desactivarInteractors = true;
    public GeneradorFallas generadorFallas;

    private bool yaUsoSeguirExplorando = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] cachedInteractors;

    private void Start()
    {
        if (panelPopup != null)
            panelPopup.SetActive(false);

        if (desactivarInteractors)
            cachedInteractors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
    }

    /// <summary>
    /// Llamar este método al finalizar el recorrido de los puntos.
    /// </summary>
    public void MostrarPopup()
    {
        if (desactivarInteractors && cachedInteractors != null)
        {
            foreach (var it in cachedInteractors)
                if (it != null) it.enabled = false;
        }

        if (panelPopup != null)
            panelPopup.SetActive(true);

        Debug.Log("[Progreso] Popup mostrado tras finalizar recorrido");
    }

    // Botón "Seguir Explorando"
    public void SeguirExplorando()
    {
        if (yaUsoSeguirExplorando)
        {
            IniciarFase2();
            return;
        }

        yaUsoSeguirExplorando = true;
        panelPopup.SetActive(false);

        if (desactivarInteractors && cachedInteractors != null)
        {
            foreach (var it in cachedInteractors)
                if (it != null) it.enabled = true;
        }

        Debug.Log("[Progreso] Usuario eligió seguir explorando");
    }

    // Botón "Iniciar Fase 2"
    public void IniciarFase2()
    {
        panelPopup.SetActive(false);

        if (desactivarInteractors && cachedInteractors != null)
        {
            foreach (var it in cachedInteractors)
                if (it != null) it.enabled = true;
        }

        if (generadorFallas != null)
            generadorFallas.IniciarFase2();
        else
            Debug.LogWarning("[Progreso] No se encontró GeneradorFallas en escena.");
    }
}
