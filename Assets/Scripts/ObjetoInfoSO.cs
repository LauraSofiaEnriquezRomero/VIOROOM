using UnityEngine;

[CreateAssetMenu(fileName = "NuevoObjetoInfo", menuName = "Informacion/ObjetoInfo")]
public class ObjetoInfoSO : ScriptableObject
{
    public string nombreObjeto;
    [TextArea]
    public string descripcion;
    public string categoria;
}
