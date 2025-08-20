using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestorEvaluacion : MonoBehaviour
{
    public static GestorEvaluacion instancia;

    [Header("Paneles")]
    public GameObject panelDotacion;
    public GameObject panelResumen;

    [Header("UI Dotación")]
    public Transform contenedorBotonesDotacion;
    public Button prefabBotonDotacion;

    [Header("UI Resumen")]
    public Transform contenedorResumen;
    public Text prefabTextoResumen;

    private List<GameObject> seleccionesUsuario = new List<GameObject>();
    private FallaInteractiva dotacionActiva;
    private bool evaluacionEnCurso = false; // ✅ evita seguir registrando después de mostrar el panel

private void Awake()
{
    instancia = this;

    Debug.Log($"[DEBUG] Referencias en Awake(): " +
              $"\n panelDotacion={panelDotacion}" +
              $"\n panelResumen={panelResumen}" +
              $"\n contenedorBotonesDotacion={contenedorBotonesDotacion}" +
              $"\n prefabBotonDotacion={prefabBotonDotacion}" +
              $"\n contenedorResumen={contenedorResumen}" +
              $"\n prefabTextoResumen={prefabTextoResumen}");

    if (panelDotacion != null) panelDotacion.SetActive(false);
    if (panelResumen != null) panelResumen.SetActive(false);
}


    public void RegistrarSeleccion(GameObject obj)
    {
        if (evaluacionEnCurso)
        {
            Debug.Log("[Evaluación] Ya está en curso, no se registran más selecciones.");
            return;
        }

        if (seleccionesUsuario.Contains(obj))
        {
            Debug.Log($"[Evaluación] {obj.name} ya había sido seleccionado, se ignora.");
            return;
        }

        seleccionesUsuario.Add(obj);
        Debug.Log($"[Evaluación] Selección: {obj.name} ({seleccionesUsuario.Count}/5)");

        if (seleccionesUsuario.Count >= 5)
        {
            Debug.Log("[Evaluación] → Se alcanzaron 5 selecciones. Intentando activar panelDotacion...");
            evaluacionEnCurso = true;
            MostrarPanelDotacion();
        }
    }

    private void MostrarPanelDotacion()
    {
        Debug.Log("[Evaluación] → Entrando en MostrarPanelDotacion()");

        if (panelDotacion == null)
        {
            Debug.LogError("⚠️ panelDotacion está NULL en el Inspector");
            return;
        }

        panelDotacion.SetActive(true);
        Debug.Log("[Evaluación] → PanelDotacion activado");

        // 🔹 Limpiar botones previos
        foreach (Transform t in contenedorBotonesDotacion)
            Destroy(t.gameObject);

        var gf = FindObjectOfType<GeneradorFallas>();
        if (gf == null)
        {
            Debug.LogError("⚠️ No hay GeneradorFallas en la escena.");
            return;
        }

        // 🔹 Guardamos cuál era la dotación realmente activada en la fase
        var dotacionActivas = gf.GetDotacionActiva();
        dotacionActiva = dotacionActivas.Count > 0 ? dotacionActivas[0] : null;

        // ✅ Ahora solo listamos las fallas ocultables (estén activas o no)
        var opciones = gf.GetOpcionesOcultables();

        if (opciones == null || opciones.Count == 0)
        {
            Debug.LogWarning("⚠️ No hay fallas ocultables configuradas en GeneradorFallas");
            return;
        }

        foreach (var obj in opciones)
        {
            if (prefabBotonDotacion == null)
            {
                Debug.LogError("⚠️ prefabBotonDotacion no está asignado en el Inspector");
                return;
            }

            Button btn = Instantiate(prefabBotonDotacion, contenedorBotonesDotacion);
            btn.GetComponentInChildren<Text>().text = obj.name;
            btn.onClick.AddListener(() => SeleccionarDotacion(obj));

            Debug.Log($"[Evaluación] Botón creado para: {obj.name}");
        }
    }

    private void SeleccionarDotacion(GameObject seleccion)
    {
        panelDotacion.SetActive(false);
        MostrarResumen(seleccion);
    }

    private void MostrarResumen(GameObject seleccionDotacion)
    {
        if (panelResumen == null)
        {
            Debug.LogError("⚠️ panelResumen no está asignado en el Inspector");
            return;
        }

        panelResumen.SetActive(true);
        Debug.Log("[Evaluación] → Mostrando resumen final");

        // 🔹 Limpiar textos previos
        foreach (Transform t in contenedorResumen)
            Destroy(t.gameObject);

        var gf = FindObjectOfType<GeneradorFallas>();
        if (gf == null)
        {
            Debug.LogError("⚠️ No se encontró GeneradorFallas en la escena al mostrar resumen");
            return;
        }

        var fallas = gf.GetFallasActivadas();

        // ✅ Revisamos TODAS las fallas activadas
        foreach (var falla in fallas)
        {
            string estado = seleccionesUsuario.Contains(falla.gameObject) ? "Correcto ✅" : "Faltante ⚠️";
            CrearLineaResumen($"{falla.name} → {estado}");
        }

        // ✅ Revisamos dotación
        bool aciertoDotacion = (dotacionActiva != null && seleccionDotacion == dotacionActiva.gameObject);
        CrearLineaResumen($"Dotación seleccionada: {seleccionDotacion.name} → {(aciertoDotacion ? "Correcto ✅" : "Incorrecto ❌")}");

        // ✅ Revisamos selecciones extra que NO eran fallas
        foreach (var obj in seleccionesUsuario)
        {
            bool esFalla = fallas.Exists(f => f.gameObject == obj);
            if (!esFalla)
                CrearLineaResumen($"{obj.name} → Extra ❌");
        }
    }

    private void CrearLineaResumen(string texto)
    {
        if (prefabTextoResumen == null)
        {
            Debug.LogError("⚠️ prefabTextoResumen no está asignado en el Inspector");
            return;
        }

        Text t = Instantiate(prefabTextoResumen, contenedorResumen);
        t.text = texto;
    }
}
