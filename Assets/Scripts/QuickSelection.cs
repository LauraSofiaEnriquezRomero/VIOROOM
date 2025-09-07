using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class OutlineSelection : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    public List<GameObject> objetosSeleccionados = new List<GameObject>();

    // Referencia al último panel mostrado
    private MostrarInfoObjeto panelAnterior;

    void Update()
    {
        // 🔹 Quitar highlight anterior
        if (highlight != null)
        {
            var outlineOld = highlight.GetComponent<Outline>();
            if (outlineOld != null) outlineOld.enabled = false;
            highlight = null;
        }

        // 🔹 Raycast desde el mouse
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                Outline outline = highlight.GetComponent<Outline>();
                if (outline == null)
                {
                    outline = highlight.gameObject.AddComponent<Outline>();
                    outline.OutlineColor = Color.magenta;
                    outline.OutlineWidth = 7.0f;
                }
                outline.enabled = true;
            }
            else highlight = null;
        }

        // 🔹 Selección con clic
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (highlight)
            {
                // Quitar outline anterior
                if (selection != null)
                {
                    var outlineSel = selection.GetComponent<Outline>();
                    if (outlineSel != null) outlineSel.enabled = false;
                }

                // Nueva selección
                selection = raycastHit.transform;
                var outlineNew = selection.GetComponent<Outline>();
                if (outlineNew != null) outlineNew.enabled = true;

                highlight = null;
                GameObject seleccionado = selection.gameObject;

                // Guardar solo si es nuevo
                if (!objetosSeleccionados.Contains(seleccionado))
                {
                    objetosSeleccionados.Add(seleccionado);
                    Debug.Log("🟢 Objeto seleccionado y guardado: " + seleccionado.name);

                    if (GeneradorFallas.faseActual == FaseSimulacion.Evaluacion && GestorEvaluacion.instancia != null)
                    {
                        GestorEvaluacion.instancia.RegistrarSeleccion(seleccionado);
                        Debug.Log("📊 Evaluación registrada para: " + seleccionado.name);
                    }

                    if (Recorrido.instancia != null)
                    {
                        Debug.Log("📌 Enviando selección al RecorridoGuiado: " + seleccionado.name);
                        Recorrido.instancia.RegistrarClick(seleccionado);
                    }
                }
                else Debug.Log("⚠ Objeto repetido (ya estaba guardado): " + seleccionado.name);

                // Mostrar panel de info
                var mostrarInfo = seleccionado.GetComponent<MostrarInfoObjeto>();
                if (mostrarInfo != null)
                {
                    if (panelAnterior != null && panelAnterior != mostrarInfo)
                    {
                        panelAnterior.CerrarPanel();
                        Debug.Log("📕 Cerrando panel anterior");
                    }

                    mostrarInfo.MostrarInformacion();
                    panelAnterior = mostrarInfo;
                    Debug.Log("📖 Mostrando panel de información para: " + seleccionado.name);
                }
            }
            else
            {
                // Deselección si hago click afuera
                if (selection)
                {
                    var outlineSel = selection.GetComponent<Outline>();
                    if (outlineSel != null) outlineSel.enabled = false;

                    Debug.Log("❌ Deseleccionando objeto: " + selection.name);
                    selection = null;
                }

                if (panelAnterior != null)
                {
                    panelAnterior.CerrarPanel();
                    Debug.Log("📕 Cerrando panel de información (click fuera)");
                    panelAnterior = null;
                }
                else Debug.Log("👆 Click fuera de objetos seleccionables.");
            }
        }
    }
}
