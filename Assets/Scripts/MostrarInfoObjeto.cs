using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class MostrarInfoObjeto : MonoBehaviour
{
    [Header("Datos del objeto")]
    public ObjetoInfoSO infoObjeto;

    [Header("UI del panel")]
    public GameObject panelInfo;
    public Text textoNombre;
    public Text textoCategoria;
    public Text textoDescripcion;

    [Header("Objetos que se desactivan al mostrar panel")]
    public GameObject[] objetosADesactivar;

    private bool primerClickRegistrado = false;

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(ManejarSeleccionXR);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(ManejarSeleccionXR);
    }

    /// <summary>
    /// Manejador de selección desde XR (mandos de Oculus)
    /// </summary>
    /// <param name="args"></param>
    private void ManejarSeleccionXR(SelectEnterEventArgs args)
    {
        // Si estamos en fase de exploración, mostrar panel
        if (GeneradorFallas.faseActual == FaseSimulacion.Exploracion)
        {
            MostrarInformacion();
        }
        // Si estamos en fase de evaluación, registrar selección
        else if (GeneradorFallas.faseActual == FaseSimulacion.Evaluacion)
        {
            Debug.Log($"[Evaluación] Objeto {gameObject.name} seleccionado por el usuario.");
            GestorEvaluacion.instancia?.RegistrarSeleccion(gameObject);
        }
    }

    /// <summary>
    /// Muestra la información del objeto en el panel.
    /// Solo se activa en fase de Exploración.
    /// </summary>
    public void MostrarInformacion()
    {
        if (GeneradorFallas.faseActual != FaseSimulacion.Exploracion)
            return;

        if (infoObjeto == null || textoNombre == null || textoCategoria == null ||
            textoDescripcion == null || panelInfo == null)
        {
            Debug.LogWarning($"⚠ Faltan referencias en {gameObject.name}");
            return;
        }

        textoNombre.text = infoObjeto.nombreObjeto;
        textoCategoria.text = infoObjeto.categoria;
        textoDescripcion.text = infoObjeto.descripcion;

        panelInfo.SetActive(true);

        if (objetosADesactivar != null)
        {
            foreach (var obj in objetosADesactivar)
                if (obj != null) obj.SetActive(false);
        }

        // Solo se registra el primer click (para iniciar progreso)
        if (!primerClickRegistrado)
        {
            ProgresoInspeccion progreso = FindObjectOfType<ProgresoInspeccion>();
            if (progreso != null) progreso.IniciarConteo();
            primerClickRegistrado = true;
        }
    }

    public void RestaurarObjetos()
    {
        if (objetosADesactivar != null)
        {
            foreach (var obj in objetosADesactivar)
                if (obj != null) obj.SetActive(true);
        }
    }

    public void CerrarPanel()
    {
        if (panelInfo != null)
            panelInfo.SetActive(false);

        RestaurarObjetos();
    }
}
