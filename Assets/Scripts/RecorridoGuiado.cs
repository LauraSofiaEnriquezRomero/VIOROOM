using System.Collections.Generic;
using UnityEngine;

public class RecorridoGuiado : MonoBehaviour
{
    public static RecorridoGuiado instancia;

    [Header("Prefab de Partículas (Highlight)")]
    public ParticleSystem particlePrefab;      // ← arrastra PS_ObjetivoGuia aquí
    public Vector3 offset = new Vector3(0, 1.5f, 0);   // Altura sobre el objeto
    public bool pegarAlObjeto = true;                 // Parent al target

    [Header("Lista (en orden) de objetos a guiar")]
    public List<GameObject> objetosImportantes = new List<GameObject>();

    private int indiceActual = 0;
    private ParticleSystem psActual;
    private GameObject objetivoActual;

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        // Si ya tienes la lista, iniciamos
        if (objetosImportantes != null && objetosImportantes.Count > 0)
            MostrarSiguiente();
        else
            Debug.LogWarning("[RecorridoGuiado] No hay objetos en la lista 'objetosImportantes'.");
    }

    /// <summary>
    /// Llamado por OutlineSelection (o sistema de clicks) cuando el usuario selecciona algún objeto.
    /// </summary>
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

    /// <summary>
    /// Muestra el efecto de partículas sobre el siguiente objetivo.
    /// </summary>
    private void MostrarSiguiente()
    {
        if (indiceActual >= objetosImportantes.Count)
        {
            Debug.Log("[RecorridoGuiado] Recorrido terminado ✅");
            // Aquí puedes disparar el siguiente paso (por ejemplo abrir un panel o iniciar evaluación).
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

        // Instanciar/posicionar partículas
        if (particlePrefab != null)
        {
            // Si ya existe, destruir para recrear en nueva pos
            if (psActual != null) Destroy(psActual.gameObject);

            psActual = Instantiate(particlePrefab, GetPosicionHighlight(objetivoActual), Quaternion.identity);

            if (pegarAlObjeto)
            {
                psActual.transform.SetParent(objetivoActual.transform, true);
                psActual.transform.localPosition = offset; // relativo al objeto
            }

            var main = psActual.main;
            main.loop = true; // Asegurar looping
            psActual.Play();

            Debug.Log($"[RecorridoGuiado] → Mostrando partículas en: {objetivoActual.name}");
        }
        else
        {
            Debug.LogError("[RecorridoGuiado] particlePrefab es NULL. Asigna el prefab de partículas en el Inspector.");
        }
    }

    /// <summary>
    /// Avanza al siguiente objetivo y reposiciona las partículas.
    /// </summary>
    private void Avanzar()
    {
        // Apagar el PS actual
        if (psActual != null)
        {
            psActual.Stop();
            Destroy(psActual.gameObject);
            psActual = null;
        }

        indiceActual++;
        MostrarSiguiente();
    }

    /// <summary>
    /// Permite reiniciar el recorrido desde el inicio.
    /// </summary>
    public void ReiniciarRecorrido()
    {
        if (psActual != null) Destroy(psActual.gameObject);
        indiceActual = 0;
        MostrarSiguiente();
    }

    /// <summary>
    /// Si no está pegado al objeto, calculamos posición por bounds (centro + offset).
    /// </summary>
    private Vector3 GetPosicionHighlight(GameObject obj)
    {
        if (pegarAlObjeto)
            return obj.transform.position + offset;

        // Si no está “pegado”, usamos el centro aproximado del renderer
        var rend = obj.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            var center = rend.bounds.center;
            return center + offset;
        }
        return obj.transform.position + offset;
    }
}
