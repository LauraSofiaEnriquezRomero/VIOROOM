using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FallaManager : MonoBehaviour
{
    public int numInfraestructura = 3;
    public int numDotacion = 1;
    public int numElectrica = 2;

    void Start()
    {
        AsignarFallasAleatorias();
    }

    void AsignarFallasAleatorias()
    {
        Falla[] todas = FindObjectsOfType<Falla>();

        ActivarFallasAleatorias(todas, Falla.CategoriaFalla.Infraestructura, numInfraestructura);
        ActivarFallasAleatorias(todas, Falla.CategoriaFalla.Dotacion, numDotacion);
        ActivarFallasAleatorias(todas, Falla.CategoriaFalla.Electrica, numElectrica);
    }

    void ActivarFallasAleatorias(Falla[] todas, Falla.CategoriaFalla categoria, int cantidad)
    {
        var disponibles = todas.Where(f => f.categoria == categoria).OrderBy(_ => Random.value).Take(cantidad);

        foreach (var falla in disponibles)
        {
            falla.esFallaActiva = true;
            falla.gameObject.SetActive(true); // Asegura que sea visible
        }
    }

    public void EvaluarFallas()
    {
        Falla[] todas = FindObjectsOfType<Falla>();

        int totalReales = 0;
        int aciertos = 0;
        int errores = 0;

        foreach (var falla in todas)
        {
            if (falla.esFallaActiva) totalReales++;

            if (falla.marcadaPorUsuario && falla.esFallaActiva)
                aciertos++;
            else if (falla.marcadaPorUsuario && !falla.esFallaActiva)
                errores++;
        }

        Debug.Log($"🔎 Fallas reales: {totalReales}");
        Debug.Log($"✅ Aciertos del usuario: {aciertos}");
        Debug.Log($"❌ Falsos positivos (marcadas incorrectas): {errores}");
    }
}
