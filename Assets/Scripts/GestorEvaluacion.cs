using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            TMP_Text tmpText = btn.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = obj.name;
            }
            btn.onClick.AddListener(() => SeleccionarDotacion(obj));

            Debug.Log($"[Evaluación] Botón creado para: {obj.name}");
        }
    }

    private void SeleccionarDotacion(GameObject seleccion)
    {
        if (seleccion == null)
        {
            Debug.LogWarning("⚠️ SeleccionarDotacion fue llamado con un objeto nulo.");
            return;
        }

        Debug.Log($"[Evaluación] → Dotación seleccionada: {seleccion.name}");
        panelDotacion.SetActive(false);

        // 🔹 Enviamos el GameObject directamente
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

        foreach (Transform t in contenedorResumen)
            Destroy(t.gameObject);

        var gf = FindObjectOfType<GeneradorFallas>();
        if (gf == null)
        {
            Debug.LogError("⚠️ No se encontró GeneradorFallas en la escena al mostrar resumen");
            return;
        }

        var fallas = gf.GetFallasActivadas();

        // Listas para agrupar resultados
        List<string> fallasCorrectas = new List<string>();
        List<string> fallasNoIdentificadas = new List<string>();
        List<string> elementosSinFalla = new List<string>();

        // ✅ Clasificar las fallas
        foreach (var falla in fallas)
        {
            if (falla == null || falla.gameObject == null) continue;

            if (seleccionesUsuario.Contains(falla.gameObject))
                fallasCorrectas.Add(falla.name);
            else
                fallasNoIdentificadas.Add(falla.name);
        }

        // ✅ Buscar selecciones incorrectas (no eran fallas)
        foreach (var obj in seleccionesUsuario)
        {
            if (obj == null) continue;
            bool esFalla = fallas.Exists(f => f != null && f.gameObject == obj);
            if (!esFalla)
                elementosSinFalla.Add(obj.name);
        }

        // 🔹 Sección 1: Fallas correctamente identificadas
        CrearLineaResumen("✅ Identificó correctamente las siguientes fallas:");
        if (fallasCorrectas.Count > 0)
            foreach (var nombre in fallasCorrectas)
                CrearLineaResumen("   • " + nombre);
        else
            CrearLineaResumen("   • Ninguna");

        // 🔹 Sección 2: Fallas no identificadas
        CrearLineaResumen("\n⚠️ No identificó estas fallas:");
        if (fallasNoIdentificadas.Count > 0)
            foreach (var nombre in fallasNoIdentificadas)
                CrearLineaResumen("   • " + nombre);
        else
            CrearLineaResumen("   • Ninguna");

        // 🔹 Sección 3: Elementos sin falla marcados
        CrearLineaResumen("\n❌ Marcó elementos que no tenían falla:");
        if (elementosSinFalla.Count > 0)
            foreach (var nombre in elementosSinFalla)
                CrearLineaResumen("   • " + nombre);
        else
            CrearLineaResumen("   • Ninguno");

        // 🔹 Evaluación de dotación
        string nombreDotacion = (seleccionDotacion != null) ? seleccionDotacion.name : "(objeto eliminado)";
        bool aciertoDotacion = (dotacionActiva != null && seleccionDotacion == dotacionActiva.gameObject);

        string resultadoDotacion = aciertoDotacion
            ? $"\n✅ Seleccionó correctamente la dotación con falla: {nombreDotacion}"
            : $"\n❌ Seleccionó una dotación incorrecta: {nombreDotacion}";

        CrearLineaResumen(resultadoDotacion);
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
