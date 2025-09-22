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
    private bool evaluacionEnCurso = false; 

    private void Awake()
    {
        instancia = this;

        if (panelDotacion != null) panelDotacion.SetActive(false);
        if (panelResumen != null) panelResumen.SetActive(false);
    }

    public void RegistrarSeleccion(GameObject obj)
    {
        if (evaluacionEnCurso) return;
        if (seleccionesUsuario.Contains(obj)) return;

        seleccionesUsuario.Add(obj);

        if (seleccionesUsuario.Count >= 5)
        {
            evaluacionEnCurso = true;
            MostrarPanelDotacion();
        }
    }

    private void MostrarPanelDotacion()
    {
        panelDotacion.SetActive(true);

        foreach (Transform t in contenedorBotonesDotacion)
            Destroy(t.gameObject);

        var gf = FindObjectOfType<GeneradorFallas>();
        if (gf == null) return;

        var dotacionActivas = gf.GetDotacionActiva();
        dotacionActiva = dotacionActivas.Count > 0 ? dotacionActivas[0] : null;

        var opciones = gf.GetOpcionesOcultables();

        foreach (var obj in opciones)
        {
            Button btn = Instantiate(prefabBotonDotacion, contenedorBotonesDotacion);
            btn.GetComponentInChildren<Text>().text = obj.name;
            btn.onClick.AddListener(() => SeleccionarDotacion(obj));
        }
    }

    private void SeleccionarDotacion(GameObject seleccion)
    {
        panelDotacion.SetActive(false);
        MostrarResumen(seleccion);
    }

    private void MostrarResumen(GameObject seleccionDotacion)
    {
        panelResumen.SetActive(true);

        foreach (Transform t in contenedorResumen)
            Destroy(t.gameObject);

        var gf = FindObjectOfType<GeneradorFallas>();
        if (gf == null) return;

        var fallas = gf.GetFallasActivadas();

        // --- Correctas ---
        CrearTituloSeccion("Correctas ✅");
        foreach (var falla in fallas)
        {
            // Caso normal
            if (seleccionesUsuario.Contains(falla.gameObject))
            {
                CrearLineaResumen(falla.name, falla.gameObject, "Correcto ✅");
            }
            // Caso especial: CambiarObjeto activado
            else if (falla.tipoFalla == FallaInteractiva.TipoFalla.CambiarObjeto && falla.EstaActiva())
            {
                CrearLineaResumen(falla.name, falla.objetoObjetivo, "Correcto (Reemplazado) ✅");
            }
        }

        // --- No seleccionadas ---
        CrearTituloSeccion("No seleccionadas ⚠️");
        foreach (var falla in fallas)
        {
            if (!seleccionesUsuario.Contains(falla.gameObject))
            {
                // Si es CambiarObjeto y está activo, ya se contó como Correcta
                if (falla.tipoFalla == FallaInteractiva.TipoFalla.CambiarObjeto && falla.EstaActiva())
                    continue;

                CrearLineaResumen(falla.name, falla.gameObject, "No seleccionada ⚠️");
            }
        }

        // --- Incorrectas ---
        CrearTituloSeccion("Incorrectas ❌");
        foreach (var obj in seleccionesUsuario)
        {
            bool esFalla = fallas.Exists(f => f.gameObject == obj);

            // Si no es una falla, está mal
            if (!esFalla)
                CrearLineaResumen(obj.name, obj, "Incorrecta ❌");
        }

        // --- Dotación ---
        CrearTituloSeccion("Dotación");
        bool aciertoDotacion = (dotacionActiva != null && seleccionDotacion == dotacionActiva.gameObject);
        CrearLineaResumen(seleccionDotacion.name, seleccionDotacion, aciertoDotacion ? "Correcto ✅" : "Incorrecto ❌");
    }

    private void CrearTituloSeccion(string titulo)
    {
        if (prefabTextoResumen == null) return;

        Text tituloUI = Instantiate(prefabTextoResumen, contenedorResumen);
        tituloUI.text = $"\n--- {titulo} ---";
        tituloUI.fontStyle = FontStyle.Bold;
    }

    private void CrearLineaResumen(string titulo, GameObject obj, string estado)
    {
        if (prefabTextoResumen == null) return;

        // Línea con nombre + estado
        Text t1 = Instantiate(prefabTextoResumen, contenedorResumen);
        t1.text = $"{titulo} → {estado}";

        // Línea con descripción
        string descripcion = ObtenerDescripcion(obj);
        Text t2 = Instantiate(prefabTextoResumen, contenedorResumen);
        t2.text = $"Descripción: {descripcion}";
    }

    private string ObtenerDescripcion(GameObject obj)
    {
        if (obj == null) return "Sin descripción";
        var info = obj.GetComponent<MostrarInfoObjeto>();
        if (info != null && info.infoObjeto != null && !string.IsNullOrEmpty(info.infoObjeto.descripcion))
            return info.infoObjeto.descripcion;
        return "Sin descripción";
    }
}
