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
        MostrarInformacion();

        // 🔹 Reiniciar el conteo SIEMPRE que el usuario interactúe con un objeto
        ProgresoInspeccion progreso = FindObjectOfType<ProgresoInspeccion>();
        if (progreso != null)
        {
            progreso.IniciarConteo();
        }
    }

    public void MostrarInformacion()
    {
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
        MostrarInformacion();
    }
}
