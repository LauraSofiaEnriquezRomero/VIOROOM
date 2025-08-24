using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VRButton3D : MonoBehaviour
{
    [Header("Configuración del botón")]
    [Tooltip("Objetos que se ocultarán al presionar el botón")]
    public GameObject[] objectsToHide;

    [Tooltip("Objetos que se mostrarán al presionar el botón")]
    public GameObject[] objectsToShow;

    [Tooltip("Duración en segundos que estarán visibles (0 = permanente)")]
    public float showForSeconds = 0f;

    [Header("Cambio de escena (opcional)")]
    [Tooltip("Deja vacío para no cambiar de escena")]
    public string sceneToLoad = "";

    [Tooltip("Si es true, se cargará la siguiente escena del build index")]
    public bool loadNextScene = false;

    [Header("Raycast")]
    public float rayLength = 10f;
    private GameObject currentHit;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            currentHit = hit.collider.gameObject;
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

            // Click (mouse o trigger VR con Input System)
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                VRButton3D button = currentHit.GetComponent<VRButton3D>();
                if (button != null)
                {
                    button.OnPressed();
                }
            }
        }
        else
        {
            currentHit = null;
        }
    }

    // Acción al presionar el botón
    public void OnPressed()
    {
        Debug.Log("🚀 Botón presionado: " + gameObject.name);

        if (showForSeconds > 0)
        {
            StopAllCoroutines();
            StartCoroutine(ShowTemporal(showForSeconds));
        }
        else
        {
            SwapObjects();
        }

        HandleSceneChange();
    }

    // Ocultar y mostrar al mismo tiempo
    private void SwapObjects()
    {
        foreach (var h in objectsToHide)
        {
            if (h != null) h.SetActive(false);
        }

        foreach (var s in objectsToShow)
        {
            if (s != null) s.SetActive(true);
        }
    }

    // Mostrar temporalmente y luego revertir
    private IEnumerator ShowTemporal(float seconds)
    {
        SwapObjects();
        yield return new WaitForSeconds(seconds);
        RevertObjects();
    }

    private void RevertObjects()
    {
        foreach (var h in objectsToHide)
        {
            if (h != null) h.SetActive(true);
        }

        foreach (var s in objectsToShow)
        {
            if (s != null) s.SetActive(false);
        }
    }

    // --- Cambio de escena ---
    private void HandleSceneChange()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else if (loadNextScene)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
