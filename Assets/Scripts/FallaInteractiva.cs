using UnityEngine;

public class FallaInteractiva : MonoBehaviour
{
    public enum TipoFalla
    {
        DesactivarObjeto,
        ActivarObjeto,     // 🔹 Usado para la alarma de gases
        CambiarMaterial,   // 🔹 Puede afectar a un objeto o a todos sus hijos
        CambiarObjeto      // 🔹 Maneja receptáculos correctos/incorrectos o prefabs
    }

    public enum CategoriaFalla
    {
        Infraestructura,
        SeguridadElectrica,
        Dotacion,
        Multiple   // 🔹 Nueva: aparece en más de una categoría
    }

    [Header("Configuración de la falla")]
    public TipoFalla tipoFalla;

    [Header("Categoría normativa")]
    public CategoriaFalla categoria = CategoriaFalla.Infraestructura;

    [Header("Para activar/desactivar o cambiar material")]
    public GameObject objetoObjetivo;       // 🔹 El objeto o el padre que contiene hijos
    public Material nuevoMaterial;          // Material a aplicar si aplica
    public Texture nuevaTexture;            // Textura a aplicar si aplica
    public GameObject nuevoPrefab;          // Prefab para reemplazar el objeto

    [Header("Para receptáculos (correctos/incorrectos)")]
    public GameObject[] objetosCorrectos;   // Objetos correctos visibles al inicio
    public GameObject[] objetosIncorrectos; // Objetos incorrectos ocultos al inicio

    private GameObject objetoInstanciado;
    private Renderer[] renderersHijos;      // 🔹 Todos los renderers del padre/hijos
    private Material[] materialesOriginales;
    private bool fallaActiva = false;

    private void Awake()
    {
        if (objetoObjetivo != null)
        {
            // 🔹 Buscar TODOS los renderers en el objetoObjetivo y sus hijos
            renderersHijos = objetoObjetivo.GetComponentsInChildren<Renderer>(true);

            if (renderersHijos != null && renderersHijos.Length > 0)
            {
                materialesOriginales = new Material[renderersHijos.Length];
                for (int i = 0; i < renderersHijos.Length; i++)
                {
                    materialesOriginales[i] = renderersHijos[i].sharedMaterial;
                }
            }
        }
    }

    public void ActivarFalla()
    {
        if (fallaActiva) return;

        switch (tipoFalla)
        {
            case TipoFalla.DesactivarObjeto:
                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(false);
                break;

            case TipoFalla.ActivarObjeto: // 🔹 Para la alarma de gases
                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(true);
                break;

            case TipoFalla.CambiarMaterial:
                if (renderersHijos != null)
                {
                    foreach (Renderer r in renderersHijos)
                    {
                        if (nuevoMaterial != null)
                        {
                            r.material = new Material(nuevoMaterial);
                        }
                        else if (nuevaTexture != null)
                        {
                            Material mat = new Material(r.material);
                            mat.mainTexture = nuevaTexture;
                            r.material = mat;
                        }
                    }
                }
                break;

            case TipoFalla.CambiarObjeto:
                // 🔹 Caso receptáculos: ocultar correctos y mostrar incorrectos
                if (objetosCorrectos != null)
                    foreach (var obj in objetosCorrectos) if (obj != null) obj.SetActive(false);

                if (objetosIncorrectos != null)
                    foreach (var obj in objetosIncorrectos) if (obj != null) obj.SetActive(true);

                // 🔹 Caso alternativo: instanciar un prefab
                if (objetoObjetivo != null && nuevoPrefab != null)
                {
                    Vector3 pos = objetoObjetivo.transform.position;
                    Quaternion rot = objetoObjetivo.transform.rotation;
                    objetoObjetivo.SetActive(false);
                    objetoInstanciado = Instantiate(nuevoPrefab, pos, rot);
                }
                break;
        }

        fallaActiva = true;
    }

    public void RestaurarFalla()
    {
        if (!fallaActiva) return;

        switch (tipoFalla)
        {
            case TipoFalla.DesactivarObjeto:
                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(true);
                break;

            case TipoFalla.ActivarObjeto: // 🔹 Restaurar = esconderlo
                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(false);
                break;

            case TipoFalla.CambiarMaterial:
                if (renderersHijos != null && materialesOriginales != null)
                {
                    for (int i = 0; i < renderersHijos.Length; i++)
                    {
                        renderersHijos[i].material = materialesOriginales[i];
                    }
                }
                break;

            case TipoFalla.CambiarObjeto:
                // 🔹 Restaurar receptáculos
                if (objetosCorrectos != null)
                    foreach (var obj in objetosCorrectos) if (obj != null) obj.SetActive(true);

                if (objetosIncorrectos != null)
                    foreach (var obj in objetosIncorrectos) if (obj != null) obj.SetActive(false);

                // 🔹 Si instanció un prefab
                if (objetoInstanciado != null)
                    Destroy(objetoInstanciado);

                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(true);
                break;
        }

        fallaActiva = false;
    }
}
