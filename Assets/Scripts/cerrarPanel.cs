using UnityEngine;

public class CerrarPanel : MonoBehaviour
{
    public GameObject panelInfo;

    public void CerrarPanelDesdeBoton()
    {
        if (panelInfo != null)
        {
            panelInfo.SetActive(false);
            Debug.Log("Panel cerrado desde el botón.");
        }
        else
        {
            Debug.LogWarning("No se asignó el panelInfo en el script CerrarPanel.");
        }
    }
}
