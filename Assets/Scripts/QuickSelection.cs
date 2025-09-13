using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class OutlineSelection : MonoBehaviour
{
    private Outline outline;
    private XRBaseInteractable interactable;

    // 🔹 Selección con mouse
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    // 🔹 Selección global
    public static List<GameObject> objetosSeleccionados = new List<GameObject>();

    // 🔹 Referencia al último panel mostrado
    private static MostrarInfoObjeto panelAnterior;

    private void Awake()
    {
        // Asegurar que el objeto tenga Outline
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.magenta;
            outline.OutlineWidth = 7f;
        }
        outline.enabled = false;

        // Si este objeto es interactuable XR, nos suscribimos a sus eventos
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnSelectEnter);
            interactable.selectExited.AddListener(OnSelectExit);
        }
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnSelectEnter);
            interactable.selectExited.RemoveListener(OnSelectExit);
        }
    }

    private void Update()
    {
        // 🔹 Solo si hay mouse conectado → modo escritorio
        if (Mouse.current == null) return;

        // Quitar highlight anterior
        if (highlight != null)
        {
            var outlineOld = highlight.GetComponent<Outline>();
            if (outlineOld != null) outlineOld.enabled = false;
            highlight = null;
        }

        // Raycast desde la cámara
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                Outline outlineTemp = highlight.GetComponent<Outline>();
                if (outlineTemp == null)
                {
                    outlineTemp = highlight.gameObject.AddComponent<Outline>();
                    outlineTemp.OutlineColor = Color.magenta;
                    outlineTemp.OutlineWidth = 7.0f;
                }
                outlineTemp.enabled = true;
            }
            else highlight = null;
        }

        // Click izquierdo = selección
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (highlight)
            {
                SeleccionarObjeto(highlight.gameObject);
                highlight = null;
            }
            else DeseleccionarObjeto();
        }
    }

    // ---------------------- XR EVENTS ----------------------

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        outline.enabled = true;
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (!objetosSeleccionados.Contains(gameObject))
            outline.enabled = false;
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        SeleccionarObjeto(gameObject);
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log("❌ Deseleccionado (XR): " + gameObject.name);
    }

    // ---------------------- LÓGICA COMÚN ----------------------

    private void SeleccionarObjeto(GameObject seleccionado)
    {
        // Quitar selección previa (solo mouse)
        if (selection != null && seleccionado != selection.gameObject)
        {
            var outlineSel = selection.GetComponent<Outline>();
            if (outlineSel != null) outlineSel.enabled = false;
        }

        // Nueva selección
        selection = seleccionado.transform;
        var outlineNew = seleccionado.GetComponent<Outline>();
        if (outlineNew != null) outlineNew.enabled = true;

        if (!objetosSeleccionados.Contains(seleccionado))
        {
            objetosSeleccionados.Add(seleccionado);
            Debug.Log("🟢 Objeto seleccionado: " + seleccionado.name);

            if (GeneradorFallas.faseActual == FaseSimulacion.Evaluacion && GestorEvaluacion.instancia != null)
            {
                GestorEvaluacion.instancia.RegistrarSeleccion(seleccionado);
                Debug.Log("📊 Evaluación registrada para: " + seleccionado.name);
            }

            if (Recorrido.instancia != null)
            {
                Recorrido.instancia.RegistrarClick(seleccionado);
                Debug.Log("📌 Enviado al RecorridoGuiado: " + seleccionado.name);
            }
        }
        else Debug.Log("⚠ Objeto repetido: " + seleccionado.name);

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
            Debug.Log("📖 Mostrando info de: " + seleccionado.name);
        }
    }

    private void DeseleccionarObjeto()
    {
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
