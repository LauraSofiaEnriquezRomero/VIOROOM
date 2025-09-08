using UnityEngine;

public class CerrarPanel : MonoBehaviour
{
    public GameObject panelInfo;
    [HideInInspector] public MostrarInfoObjeto objetoAsociado;

    public void CerrarPanelDesdeBoton()
    {
        if (panelInfo != null)
        {
            panelInfo.SetActive(false);
            Debug.Log("📕 Panel cerrado desde el botón.");

            if (objetoAsociado != null)
                objetoAsociado.RestaurarObjetos();
        }
        else Debug.LogWarning("⚠ No se asignó panelInfo en CerrarPanel.");
    }
        public void SalirAplicacion()
    {
        Debug.Log("🚪 Cerrando la aplicación...");
        Application.Quit();

        // Solo para pruebas en el Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
