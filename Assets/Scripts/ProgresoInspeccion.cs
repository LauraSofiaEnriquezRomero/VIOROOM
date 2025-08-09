using System.Collections.Generic;
using UnityEngine;


public class ProgresoInspeccion : MonoBehaviour
{
    [Header("Configuración")]
    public int objetosNecesarios = 10;
    public GameObject panelPopup; // popup que aparece al completar
    public bool pausarConTimeScale = true;
    public bool desactivarInteractors = true;

    private HashSet<string> objetosVisitados = new HashSet<string>();
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] cachedInteractors;

    void Start()
    {
        if (panelPopup != null)
            panelPopup.SetActive(false);

        if (desactivarInteractors)
            cachedInteractors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
    }

    // Llamar desde MostrarInfoObjeto (por ejemplo) con un id único
    public void RegistrarObjeto(string idObjeto)
    {
        if (string.IsNullOrEmpty(idObjeto)) return;

        if (!objetosVisitados.Contains(idObjeto))
        {
            objetosVisitados.Add(idObjeto);
            Debug.Log($"[Progreso] Registrado: {idObjeto}  Total: {objetosVisitados.Count}/{objetosNecesarios}");
        }

        if (objetosVisitados.Count >= objetosNecesarios)
            ActivarPopup();
    }

    void ActivarPopup()
    {
        Debug.Log("[Progreso] Objetos alcanzados. Mostrando popup.");
        if (pausarConTimeScale) Time.timeScale = 0f;

        if (desactivarInteractors && cachedInteractors != null)
        {
            foreach (var it in cachedInteractors)
                if (it != null) it.enabled = false;
        }

        if (panelPopup != null)
            panelPopup.SetActive(true);
    }

    // Conectar al botón Iniciar del popup
    public void ContinuarAFase2()
    {
        Debug.Log("[Progreso] Continuando a Fase 2...");
        if (pausarConTimeScale) Time.timeScale = 1f;

        if (desactivarInteractors && cachedInteractors != null)
        {
            foreach (var it in cachedInteractors)
                if (it != null) it.enabled = true;
        }

        if (panelPopup != null)
            panelPopup.SetActive(false);

        // Lanzar generador de fallas
        var gen = FindObjectOfType<GeneradorFallas>();
        if (gen != null) gen.IniciarFase2();
        else Debug.LogWarning("[Progreso] No se encontró GeneradorFallas en escena.");
    }

    // método público para reiniciar progreso (si quieres)
    public void ResetProgreso()
    {
        objetosVisitados.Clear();
        Debug.Log("[Progreso] Reiniciado.");
    }
}
