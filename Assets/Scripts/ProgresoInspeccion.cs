using UnityEngine;

public class ProgresoInspeccion : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject panelPopup; // Panel que aparece al pasar el tiempo
    public bool desactivarInteractors = true;
    public float tiempoEspera = 30f; // Segundos antes de mostrar panel
    public GeneradorFallas generadorFallas;

    private bool yaUsoSeguirExplorando = false;
    private float tiempoRestante;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] cachedInteractors;

    private void Start()
    {
        if (panelPopup != null)
            panelPopup.SetActive(false);

        if (desactivarInteractors)
            cachedInteractors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
    }

    public void IniciarConteo()
    {
        tiempoRestante = tiempoEspera;
        CancelInvoke(nameof(ContarTiempo)); // Reinicia si ya estaba contando
        InvokeRepeating(nameof(ContarTiempo), 1f, 1f);
        Debug.Log("[Progreso] Conteo iniciado/reiniciado");
    }

    private void ContarTiempo()
    {
        tiempoRestante -= 1f;

        if (tiempoRestante <= 0)
        {
            CancelInvoke(nameof(ContarTiempo));
            MostrarPopup();
        }
    }

    private void MostrarPopup()
    {
        if (desactivarInteractors && cachedInteractors != null)
        {
            foreach (var it in cachedInteractors)
                if (it != null) it.enabled = false;
        }

        if (panelPopup != null)
            panelPopup.SetActive(true);
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

        // Reinicia el conteo por última vez
        tiempoRestante = tiempoEspera;
        CancelInvoke(nameof(ContarTiempo));
        InvokeRepeating(nameof(ContarTiempo), 1f, 1f);
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
