using UnityEngine;

public class Falla : MonoBehaviour
{
    public enum CategoriaFalla { Infraestructura, Dotacion, Electrica }

    public string nombreFalla;
    public CategoriaFalla categoria;
    public bool esFallaActiva; // Se activa si la falla está presente en la partida

    [HideInInspector]
    public bool marcadaPorUsuario = false; // Se marca si el usuario la identifica
}
