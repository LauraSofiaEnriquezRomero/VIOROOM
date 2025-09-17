using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
 // Para XR grab interactable

public class Recorrido : MonoBehaviour
{
    public static Recorrido instancia;

    [Header("Lista (en orden) de objetos a guiar")]
    public List<GameObject> objetosImportantes = new List<GameObject>();

    [Header("Lista de puntos de luz (mismo orden que objetosImportantes)")]
    public List<GameObject> puntosDeLuz = new List<GameObject>();

    [Header("UI de Progreso")]
    public GameObject panelProgreso;
    public Text textoProgreso;

    [Header("Eventos")]
    public UnityEvent OnRecorridoCompleto;

    [Header("Configuración de validación de clic")]
    [Tooltip("Permite que el clic sobre hijos del objeto objetivo sea válido.")]
    public bool permitirClickEnHijos = true;

    private int indiceActual = 0;
    private GameObject objetivoActual;

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        if (panelProgreso != null)
            panelProgreso.SetActive(false);

        if (objetosImportantes == null || objetosImportantes.Count == 0)
        {
            Debug.LogWarning("[RecorridoGuiado] No hay objetos en 'objetosImportantes'.");
            return;
        }

        if (puntosDeLuz == null || puntosDeLuz.Count == 0)
            Debug.LogWarning("[RecorridoGuiado] 'puntosDeLuz' vacío. Asigna indicadores en el mismo orden que 'objetosImportantes'.");
        else if (puntosDeLuz.Count != objetosImportantes.Count)
            Debug.LogWarning($"[RecorridoGuiado] Tamaños distintos: objetos={objetosImportantes.Count}, puntosDeLuz={puntosDeLuz.Count}.");

        // Apagar todas las luces
        foreach (var luz in puntosDeLuz)
            if (luz != null) luz.SetActive(false);

        // Registrar eventos grab XR
        RegistrarEventosGrab();

        // Iniciar el recorrido
        MostrarSiguiente();
    }

    private void RegistrarEventosGrab()
    {
        foreach (var obj in objetosImportantes)
        {
            if (obj == null) continue;

            UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable = obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.AddListener((args) => { RegistrarClick(obj); });
            }
            else
            {
                Debug.LogWarning($"[RecorridoGuiado] Objeto {obj.name} no tiene XRBaseInteractable.");
            }
        }
    }

    public void RegistrarClick(GameObject objClicado)
    {
        if (objetivoActual == null)
        {
            Debug.Log("[RecorridoGuiado] No hay objetivo actual.");
            return;
        }

        if (EsObjetivoValido(objClicado, objetivoActual))
        {
            Debug.Log($"[RecorridoGuiado] ✔ Correcto: {objClicado.name}");
            Avanzar();
        }
        else
        {
            Debug.Log($"[RecorridoGuiado] ✖ {objClicado.name} no era el objetivo (esperado: {objetivoActual.name}).");
        }
    }

    private bool EsObjetivoValido(GameObject clicado, GameObject objetivo)
    {
        if (clicado == objetivo) return true;
        if (!permitirClickEnHijos) return false;

        return clicado.transform.IsChildOf(objetivo.transform) || objetivo.transform.IsChildOf(clicado.transform);
    }

    private void MostrarSiguiente()
    {
        // Verificar si terminó el recorrido
        if (indiceActual >= objetosImportantes.Count)
        {
            // Apagar todas las luces
            foreach (var luz in puntosDeLuz)
                if (luz != null) luz.SetActive(false);

            Debug.Log("[RecorridoGuiado] Recorrido terminado ✅");

            // Activar panel solo al terminar
            if (panelProgreso != null)
                panelProgreso.SetActive(true);

            OnRecorridoCompleto?.Invoke();
            return;
        }

        // Definir el objetivo actual
        objetivoActual = objetosImportantes[indiceActual];
        if (objetivoActual == null)
        {
            Debug.LogWarning($"[RecorridoGuiado] Objeto en índice {indiceActual} es NULL. Saltando.");
            indiceActual++;
            MostrarSiguiente();
            return;
        }

        // Encender únicamente la luz del índice actual
        for (int i = 0; i < puntosDeLuz.Count; i++)
            if (puntosDeLuz[i] != null)
                puntosDeLuz[i].SetActive(i == indiceActual);

        ActualizarUI();
    }

    private void Avanzar()
    {
        // Apagar luz actual
        if (indiceActual < puntosDeLuz.Count && puntosDeLuz[indiceActual] != null)
            puntosDeLuz[indiceActual].SetActive(false);

        indiceActual++;
        MostrarSiguiente();
    }

    public void ReiniciarRecorrido()
    {
        foreach (var luz in puntosDeLuz)
            if (luz != null) luz.SetActive(false);

        indiceActual = 0;
        if (panelProgreso != null)
            panelProgreso.SetActive(false);

        MostrarSiguiente();
    }

    private void ActualizarUI()
    {
        if (textoProgreso != null)
            textoProgreso.text = $"Seleccionaste {Mathf.Min(indiceActual, objetosImportantes.Count)}/{objetosImportantes.Count} objetos.";
    }
}
