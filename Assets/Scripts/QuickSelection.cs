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
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }

        // Raycast desde el mouse
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    outline.OutlineColor = Color.magenta;
                    outline.OutlineWidth = 7.0f;
                }
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
                if (selection != null)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                }

                selection = raycastHit.transform;
                selection.gameObject.GetComponent<Outline>().enabled = true;
                highlight = null;

                GameObject seleccionado = selection.gameObject;
                if (!objetosSeleccionados.Contains(seleccionado))
                {
                    objetosSeleccionados.Add(seleccionado);
                    Debug.Log("Objeto guardado: " + seleccionado.name);
                }

                var mostrarInfo = seleccionado.GetComponent<MostrarInfoObjeto>();

                // Oculta el panel anterior si es diferente
                // if (panelAnterior != null && panelAnterior != mostrarInfo)
                // {
                //     panelAnterior.OcultarInformacion();
                // }

                if (mostrarInfo != null)
                {
                    mostrarInfo.MostrarInformacion();
                    panelAnterior = mostrarInfo;
                }
            }
            else
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection = null;
                }

                // Si se hace clic en un espacio vacío, ocultar el panel actual
                // if (panelAnterior != null)
                // {
                //     panelAnterior.OcultarInformacion();
                //     panelAnterior = null;
                // }
            }
        }
    }
}
