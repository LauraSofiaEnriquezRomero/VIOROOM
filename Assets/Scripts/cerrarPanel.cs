using UnityEngine;

public class CerrarPanel : MonoBehaviour
{
    public GameObject panelInfo;
    [HideInInspector] public MostrarInfoObjeto objetoAsociado; // 🔗 Script que abrió el panel

    public void CerrarPanelDesdeBoton()
    {
        if (panelInfo != null)
        {
            panelInfo.SetActive(false);
            Debug.Log("Panel cerrado desde el botón.");

            // 🔥 Restaurar los objetos del script que lo abrió
            if (objetoAsociado != null)
            {
                objetoAsociado.RestaurarObjetos();
            }
        }
        else
        {
            Debug.LogWarning("No se asignó el panelInfo en el script CerrarPanel.");
        }
    }
}
