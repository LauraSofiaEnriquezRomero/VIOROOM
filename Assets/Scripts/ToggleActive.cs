using System.Collections;
using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    [Tooltip("Objetos que se van a mostrar/ocultar")]
    public GameObject[] targets;

    // Muestra todos los targets (SetActive(true))
    public void Show()
    {
        SetAll(true);
    }

    // Oculta todos los targets (SetActive(false))
    public void Hide()
    {
        SetAll(false);
    }

    // Alterna el estado actual (si está activo lo desactiva y viceversa)
    public void Toggle()
    {
        foreach (var t in targets)
        {
            if (t != null) t.SetActive(!t.activeSelf);
        }
    }

    // Muestra por X segundos y luego oculta
    public void ShowForSeconds(float seconds)
    {
        StopAllCoroutines();
        StartCoroutine(ShowTemporal(seconds));
    }

    private void SetAll(bool state)
    {
        foreach (var t in targets)
        {
            if (t != null) t.SetActive(state);
        }
    }

    private IEnumerator ShowTemporal(float seconds)
    {
        SetAll(true);
        yield return new WaitForSeconds(seconds);
        SetAll(false);
    }
}
