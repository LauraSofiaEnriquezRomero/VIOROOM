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
        // Quitar highlight anterior
        if (highlight != null)
        {
            var outlineOld = highlight.GetComponent<Outline>();
            if (outlineOld != null)
                outlineOld.enabled = false;

            highlight = null;
        }

        // Raycast desde el mouse
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
            else
            {
                highlight = null;
            }
        }

        // Selección con clic
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (highlight)
            {
                // Quitar outline del anterior
                if (selection != null)
                {
                    var outlineSel = selection.GetComponent<Outline>();
                    if (outlineSel != null)
                        outlineSel.enabled = false;
                }

                // Asignar nueva selección
                selection = raycastHit.transform;
                var outlineNew = selection.GetComponent<Outline>();
                if (outlineNew != null)
                    outlineNew.enabled = true;

                highlight = null;

                GameObject seleccionado = selection.gameObject;

                if (!objetosSeleccionados.Contains(seleccionado))
                {
                    objetosSeleccionados.Add(seleccionado);
                    Debug.Log("Objeto guardado: " + seleccionado.name);

                    // 🔹 Enviar al GestorEvaluacion (solo en fase de Evaluación)
                    if (GeneradorFallas.faseActual == FaseSimulacion.Evaluacion && GestorEvaluacion.instancia != null)
                    {
                        GestorEvaluacion.instancia.RegistrarSeleccion(seleccionado);
                    }

                    // 🔹 Notificar al Recorrido Guiado
                    if (RecorridoGuiado.instancia != null)
                    {
                        RecorridoGuiado.instancia.RegistrarClick(seleccionado);
                    }
                }

                // ✅ Mostrar panel de información
                var mostrarInfo = seleccionado.GetComponent<MostrarInfoObjeto>();
                if (mostrarInfo != null)
                {
                    // 🔹 Cerrar el panel anterior si existía
                    if (panelAnterior != null && panelAnterior != mostrarInfo)
                    {
                        panelAnterior.CerrarPanel();
                    }

                    mostrarInfo.MostrarInformacion();
                    panelAnterior = mostrarInfo;
                }
            }
            else
            {
                // Si hago click afuera, deselecciono
                if (selection)
                {
                    var outlineSel = selection.GetComponent<Outline>();
                    if (outlineSel != null)
                        outlineSel.enabled = false;

                    selection = null;
                }

                // 🔹 Cerrar panel si hago click afuera
                if (panelAnterior != null)
                {
                    panelAnterior.CerrarPanel();
                    panelAnterior = null;
                }
            }
        }
    }
}
