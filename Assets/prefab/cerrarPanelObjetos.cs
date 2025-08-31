using UnityEngine;

public class cerrarPanelObjetos : MonoBehaviour
{
    // Lista ordenada de objetos que se deben activar/desactivar en secuencia
    public GameObject[] secuenciaDeObjetos;

    // Índice del objeto actualmente activo
    private int indiceActual = 0;

    // Este método se ejecuta automáticamente cuando el panel se desactiva
    
    private void OnDisable()
    {
        AlternarObjetos();
    }

    private void AlternarObjetos()
    {
        if (secuenciaDeObjetos.Length == 0) return;

        // 1. Desactivar el objeto actual si es válido
        if (indiceActual < secuenciaDeObjetos.Length && secuenciaDeObjetos[indiceActual] != null)
        {
            secuenciaDeObjetos[indiceActual].SetActive(false);
            Debug.Log($"❌ Objeto {indiceActual} desactivado.");
        }

        // 2. Pasar al siguiente índice
        indiceActual++;

        // 3. Activar el siguiente objeto si aún hay en la lista
        if (indiceActual < secuenciaDeObjetos.Length)
        {
            if (secuenciaDeObjetos[indiceActual] != null)
            {
                secuenciaDeObjetos[indiceActual].SetActive(true);
                Debug.Log($"✅ Objeto {indiceActual} activado.");
            }
        }
        else
        {
            Debug.Log("🔚 La secuencia de objetos ha terminado. Todos están desactivados.");
            // Si quieres reiniciar el ciclo para empezar de nuevo, descomenta esta línea:
            // indiceActual = 0;
        }
    }
}
