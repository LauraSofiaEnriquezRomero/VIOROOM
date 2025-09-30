using UnityEngine;

public class ProgresoInspeccion : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject panelPopup; // Panel que aparece al pasar el tiempo
    public float tiempoEspera = 30f; // Segundos antes de mostrar panel
    public GeneradorFallas generadorFallas;

    private float tiempoRestante;

    private void Start()
    {
        IniciarConteo();
        if (panelPopup != null)
            panelPopup.SetActive(false);
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
        if (panelPopup != null)
            panelPopup.SetActive(true);
    }

    // Botón "Iniciar Fase 2"
    public void IniciarFase2()
    {
        if (panelPopup != null)
            panelPopup.SetActive(false);

        if (generadorFallas != null)
            generadorFallas.IniciarFase2();
        else
            Debug.LogWarning("[Progreso] No se encontró GeneradorFallas en escena.");
    }
}
