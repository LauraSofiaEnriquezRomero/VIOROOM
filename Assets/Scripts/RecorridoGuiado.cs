using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RecorridoGuiado : MonoBehaviour
{
    public static RecorridoGuiado instancia;

    [Header("Prefab de Partículas (Highlight)")]
    public ParticleSystem particlePrefab;      
    public Vector3 offset = new Vector3(0, 1.5f, 0);   // Altura sobre el objeto
    public bool pegarAlObjeto = true;                 // Parent al target

    [Header("Lista (en orden) de objetos a guiar")]
    public List<GameObject> objetosImportantes = new List<GameObject>();

    [Header("UI de Progreso")]
    public GameObject panelProgreso;   
    public Text textoProgreso;         

    [Header("Eventos")]
    public UnityEvent OnRecorridoCompleto;

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

        if (objetosImportantes != null && objetosImportantes.Count > 0)
            MostrarSiguiente();
        else
            Debug.LogWarning("[RecorridoGuiado] No hay objetos en la lista 'objetosImportantes'.");
    }

    public void RegistrarClick(GameObject obj)
    {
        if (objetivoActual == null)
        {
            Debug.Log("[RecorridoGuiado] No hay objetivo actual.");
            return;
        }

        if (obj == objetivoActual)
        {
            Debug.Log($"[RecorridoGuiado] ✔ Correcto: {obj.name}");
            Avanzar();
        }
        else
        {
            Debug.Log($"[RecorridoGuiado] ✖ {obj.name} no era el objetivo ({objetivoActual.name}).");
        }
    }

    private void MostrarSiguiente()
    {
        if (indiceActual >= objetosImportantes.Count)
        {
            Debug.Log("[RecorridoGuiado] Recorrido terminado ✅");
            MostrarPanelProgreso();
            OnRecorridoCompleto?.Invoke();
            return;
        }

        objetivoActual = objetosImportantes[indiceActual];
        if (objetivoActual == null)
        {
            Debug.LogWarning($"[RecorridoGuiado] Objeto en índice {indiceActual} es NULL. Avanzando.");
            indiceActual++;
            MostrarSiguiente();
            return;
        }

        // Instanciar o reusar partículas
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

            Debug.Log($"[RecorridoGuiado] → Mostrando partículas en: {objetivoActual.name}");
        }
        else
        {
            Debug.LogError("[RecorridoGuiado] particlePrefab es NULL. Asigna el prefab de partículas en el Inspector.");
        }

        ActualizarUI();
    }

    private void Avanzar()
    {
        if (psActual != null)
        {
            psActual.Stop();
            psActual.gameObject.SetActive(false); // 👈 en vez de Destroy
        }

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
            textoProgreso.text = $"Seleccionaste {indiceActual + 1}/{objetosImportantes.Count} objetos, ¿deseas continuar a la siguiente fase?";
    }
}
