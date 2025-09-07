using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Recorrido : MonoBehaviour
{
    public static Recorrido instancia;

    [Header("Prefab de Partículas (Highlight) - opcional, se mantiene por compatibilidad")]
    public ParticleSystem particlePrefab;
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public bool pegarAlObjeto = true;

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
    private ParticleSystem psActual;
    private GameObject objetivoActual;

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        if (panelProgreso != null) panelProgreso.SetActive(false);

        // Validaciones de listas
        if (objetosImportantes == null || objetosImportantes.Count == 0)
        {
            Debug.LogWarning("[RecorridoGuiado] No hay objetos en 'objetosImportantes'.");
            return;
        }
        if (puntosDeLuz == null || puntosDeLuz.Count == 0)
        {
            Debug.LogWarning("[RecorridoGuiado] 'puntosDeLuz' vacío. Asigna indicadores en el mismo orden que 'objetosImportantes'.");
        }
        else if (puntosDeLuz.Count != objetosImportantes.Count)
        {
            Debug.LogWarning($"[RecorridoGuiado] Tamaños distintos: objetos={objetosImportantes.Count}, puntosDeLuz={puntosDeLuz.Count}. Se usará el índice disponible en ambas listas.");
        }

        // Apagar todas las luces
        for (int i = 0; i < puntosDeLuz.Count; i++)
            if (puntosDeLuz[i] != null) puntosDeLuz[i].SetActive(false);

        // Iniciar en el elemento 0
        MostrarSiguiente();
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

        // ¿El objeto clicado es hijo del objetivo, o el objetivo es hijo del clicado?
        return clicado.transform.IsChildOf(objetivo.transform) || objetivo.transform.IsChildOf(clicado.transform);
    }

    private void MostrarSiguiente()
    {
        // Fin del recorrido
        if (indiceActual >= objetosImportantes.Count)
        {
            // Asegurar que todas las luces queden apagadas
            for (int i = 0; i < puntosDeLuz.Count; i++)
                if (puntosDeLuz[i] != null) puntosDeLuz[i].SetActive(false);

            Debug.Log("[RecorridoGuiado] Recorrido terminado ✅");
            MostrarPanelProgreso();
            OnRecorridoCompleto?.Invoke();
            return;
        }

        objetivoActual = objetosImportantes[indiceActual];
        if (objetivoActual == null)
        {
            Debug.LogWarning($"[RecorridoGuiado] Objeto en índice {indiceActual} es NULL. Saltando.");
            indiceActual++;
            MostrarSiguiente();
            return;
        }

        // Encender SOLO el punto de luz del índice actual
        for (int i = 0; i < puntosDeLuz.Count; i++)
        {
            if (puntosDeLuz[i] == null) continue;
            bool activar = (i == indiceActual);
            puntosDeLuz[i].SetActive(activar);
        }
        if (indiceActual < puntosDeLuz.Count && puntosDeLuz[indiceActual] != null)
            Debug.Log($"[RecorridoGuiado] 💡 Encendido PuntoDeLuz[{indiceActual}] para: {objetivoActual.name}");
        else
            Debug.LogWarning($"[RecorridoGuiado] No hay PuntoDeLuz para índice {indiceActual}.");

        // Partículas (opcional)
        if (particlePrefab != null)
        {
            if (psActual == null)
            {
                psActual = Instantiate(particlePrefab, GetPosicionHighlight(objetivoActual), Quaternion.identity);
            }
            else
            {
                psActual.gameObject.SetActive(true);
                psActual.transform.position = GetPosicionHighlight(objetivoActual);
            }

            if (pegarAlObjeto)
            {
                psActual.transform.SetParent(objetivoActual.transform, true);
                psActual.transform.localPosition = offset;
            }
            else
            {
                psActual.transform.SetParent(null);
            }

            var main = psActual.main;
            main.loop = true;
            psActual.Play();
        }

        ActualizarUI();
    }

    private void Avanzar()
    {
        // Apagar partículas opcionales
        if (psActual != null)
        {
            psActual.Stop();
            psActual.gameObject.SetActive(false);
        }

        // Apagar el punto actual
        if (indiceActual < puntosDeLuz.Count && puntosDeLuz[indiceActual] != null)
            puntosDeLuz[indiceActual].SetActive(false);

        // Incrementar índice y mostrar siguiente
        indiceActual++;
        MostrarSiguiente();
    }

    public void ReiniciarRecorrido()
    {
        if (psActual != null)
        {
            psActual.Stop();
            psActual.gameObject.SetActive(false);
        }

        for (int i = 0; i < puntosDeLuz.Count; i++)
            if (puntosDeLuz[i] != null) puntosDeLuz[i].SetActive(false);

        indiceActual = 0;
        if (panelProgreso != null) panelProgreso.SetActive(false);
        MostrarSiguiente();
    }

    private Vector3 GetPosicionHighlight(GameObject obj)
    {
        if (pegarAlObjeto)
            return obj.transform.position + offset;

        var rend = obj.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            var center = rend.bounds.center;
            return center + offset;
        }
        return obj.transform.position + offset;
    }

    private void MostrarPanelProgreso()
    {
        if (panelProgreso != null) panelProgreso.SetActive(true);
    }

    private void ActualizarUI()
    {
        if (textoProgreso != null)
            textoProgreso.text = $"Seleccionaste {Mathf.Min(indiceActual + 1, objetosImportantes.Count)}/{objetosImportantes.Count} objetos.";
    }
}
