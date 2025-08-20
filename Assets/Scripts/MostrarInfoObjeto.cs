using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class MostrarInfoObjeto : MonoBehaviour
{
    public ObjetoInfoSO infoObjeto;

    public GameObject panelInfo;
    public Text textoNombre;
    public Text textoCategoria;
    public Text textoDescripcion;
    public GameObject[] objetosADesactivar;

    private bool primerClickRegistrado = false;

    private void Start()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(ManejarSeleccionXR);
        }
    }

    private void ManejarSeleccionXR(SelectEnterEventArgs args)
    {
        if (GeneradorFallas.faseActual == FaseSimulacion.Exploracion)
        {
            // ✅ Solo en exploración se muestra el panel
            MostrarInformacion();

            if (!primerClickRegistrado)
            {
                ProgresoInspeccion progreso = FindObjectOfType<ProgresoInspeccion>();
                if (progreso != null)
                {
                    progreso.IniciarConteo();
                    primerClickRegistrado = true;
                }
            }
        }
        else if (GeneradorFallas.faseActual == FaseSimulacion.Evaluacion)
        {
            Debug.Log($"[Evaluación] Objeto {gameObject.name} seleccionado por el usuario.");

            // ✅ Registrar selección en el gestor
            if (GestorEvaluacion.instancia != null)
                GestorEvaluacion.instancia.RegistrarSeleccion(gameObject);
        }

    }

    public void MostrarInformacion()
    {
        // 🔒 Seguridad extra: nunca mostrar info en Evaluación
        if (GeneradorFallas.faseActual != FaseSimulacion.Exploracion)
            return;

        if (infoObjeto == null || textoNombre == null || textoCategoria == null || textoDescripcion == null || panelInfo == null)
        {
            Debug.LogWarning($"Faltan referencias en {gameObject.name}");
            return;
        }

        textoNombre.text = infoObjeto.nombreObjeto;
        textoCategoria.text = infoObjeto.categoria;
        textoDescripcion.text = infoObjeto.descripcion;

        panelInfo.SetActive(true);

        if (objetosADesactivar != null)
        {
            foreach (var obj in objetosADesactivar)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    public void EjecutarDesdeInspector()
    {
        //  Si quieres probar desde inspector, ignora fase
        MostrarInformacion();
    }
}
