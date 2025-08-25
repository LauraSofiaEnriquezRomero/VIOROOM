using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VRButton3D : MonoBehaviour
{
    [Header("Configuración del botón")]
    public GameObject[] objectsToHide;
    public GameObject[] objectsToShow;
    public float showForSeconds = 0f;

    [Header("Cambio de escena (opcional)")]
    public string sceneToLoad = "";
    public bool loadNextScene = false;

    [Header("Eventos personalizados")]
    public UnityEvent onPressed;

    [Header("Raycast (modo mouse)")]
    public float rayLength = 10f;
    private GameObject currentHit;

    private void Start()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (interactable != null)
            interactable.selectEntered.AddListener((args) => OnPressed());
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
        {
            currentHit = hit.collider.gameObject;
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                VRButton3D button = currentHit.GetComponent<VRButton3D>();
                if (button != null) button.OnPressed();
            }
        }
        else currentHit = null;
    }

    public void OnPressed()
    {
        Debug.Log("🚀 Botón presionado: " + gameObject.name);

        if (showForSeconds > 0)
        {
            StopAllCoroutines();
            StartCoroutine(ShowTemporal(showForSeconds));
        }
        else SwapObjects();

        HandleSceneChange();
        onPressed?.Invoke();
    }

    private void SwapObjects()
    {
        foreach (var h in objectsToHide) if (h != null) h.SetActive(false);
        foreach (var s in objectsToShow) if (s != null) s.SetActive(true);
    }

    private IEnumerator ShowTemporal(float seconds)
    {
        SwapObjects();
        yield return new WaitForSeconds(seconds);
        RevertObjects();
    }

    private void RevertObjects()
    {
        foreach (var h in objectsToHide) if (h != null) h.SetActive(true);
        foreach (var s in objectsToShow) if (s != null) s.SetActive(false);
    }

    private void HandleSceneChange()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
        else if (loadNextScene)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
