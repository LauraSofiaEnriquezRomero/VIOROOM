using UnityEngine;

public class FallaInteractiva : MonoBehaviour
{
    public enum TipoFalla
    {
        DesactivarObjeto,
        CambiarMaterial,
        CambiarObjeto
    }

    [Header("Configuración de la falla")]
    public TipoFalla tipoFalla;
    public GameObject objetoObjetivo;       // El objeto que será modificado
    public Material nuevoMaterial;          // Material a aplicar si aplica
    public Texture nuevaTexture;            // Textura a aplicar si aplica
    public GameObject nuevoPrefab;          // Prefab para reemplazar el objeto

    private GameObject objetoInstanciado;
    private Renderer objetoRenderer;
    private bool fallaActiva = false;

    private void Awake()
    {
        if (objetoObjetivo != null)
            objetoRenderer = objetoObjetivo.GetComponent<Renderer>();
    }

    /// <summary>
    /// Activa la falla según su tipo configurado.
    /// </summary>
    public void ActivarFalla()
    {
        if (fallaActiva) return;

        switch (tipoFalla)
        {
            case TipoFalla.DesactivarObjeto:
                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(false);
                break;

            case TipoFalla.CambiarMaterial:
                if (objetoRenderer != null)
                {
                    if (nuevoMaterial != null)
                    {
                        // Aplica un nuevo material completo
                        objetoRenderer.material = new Material(nuevoMaterial);
                    }
                    else if (nuevaTexture != null)
                    {
                        // Copia el material actual y le aplica la nueva textura
                        Material mat;
                        if (objetoRenderer.sharedMaterial != null)
                            mat = new Material(objetoRenderer.sharedMaterial);
                        else
                            mat = new Material(Shader.Find("Standard"));

                        mat.mainTexture = nuevaTexture;
                        objetoRenderer.material = mat;
                    }
                }
                break;

            case TipoFalla.CambiarObjeto:
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

    /// <summary>
    /// Restaura el estado original del objeto.
    /// </summary>
    public void RestaurarFalla()
    {
        if (!fallaActiva) return;

        switch (tipoFalla)
        {
            case TipoFalla.DesactivarObjeto:
                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(true);
                break;

            case TipoFalla.CambiarMaterial:
                if (objetoRenderer != null)
                {
                    // Restaura el material original
                    objetoRenderer.material = objetoRenderer.sharedMaterial;
                }
                break;

            case TipoFalla.CambiarObjeto:
                if (objetoInstanciado != null)
                    Destroy(objetoInstanciado);
                if (objetoObjetivo != null)
                    objetoObjetivo.SetActive(true);
                break;
        }

        fallaActiva = false;
    }
}
