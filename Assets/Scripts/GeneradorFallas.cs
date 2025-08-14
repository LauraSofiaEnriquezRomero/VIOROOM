using System.Collections.Generic;
using UnityEngine;

public class GeneradorFallas : MonoBehaviour
{
    [Header("Fallas por categoría (arrastrar objetos que tengan FallaInteractiva)")]
    public FallaInteractiva[] fallasInfraestructura;
    public FallaInteractiva[] fallasDotacion;
    public FallaInteractiva[] fallasElectrica;

    [Header("Opciones")]
    public bool desactivarTodasAntes = true;

    private List<FallaInteractiva> activadas = new List<FallaInteractiva>();

    void Start()
    {
        if (desactivarTodasAntes)
            DesactivarTodas();
    }

    public void IniciarFase2()
    {
        Debug.Log("[GeneradorFallas] Iniciar Fase 2");
        DesactivarTodas();
        activadas.Clear();

        ActivarFallasAleatorias(fallasInfraestructura, 3);
        ActivarFallasAleatorias(fallasDotacion, 1);
        ActivarFallasAleatorias(fallasElectrica, 2);

        Debug.Log("[GeneradorFallas] Total fallas activadas: " + activadas.Count);
    }

    void ActivarFallasAleatorias(FallaInteractiva[] lista, int cantidad)
    {
        if (lista == null || lista.Length == 0) return;

        List<FallaInteractiva> disponibles = new List<FallaInteractiva>(lista);
        int pickCount = Mathf.Min(cantidad, disponibles.Count);

        for (int i = 0; i < pickCount; i++)
        {
            int idx = Random.Range(0, disponibles.Count);
            FallaInteractiva fi = disponibles[idx];
            disponibles.RemoveAt(idx);

            if (fi != null)
            {
                fi.ActivarFalla();
                activadas.Add(fi);
            }
        }
    }

    public void DesactivarTodas()
    {
        if (fallasInfraestructura != null)
            foreach (var f in fallasInfraestructura) if (f != null) f.RestaurarFalla();

        if (fallasDotacion != null)
            foreach (var f in fallasDotacion) if (f != null) f.RestaurarFalla();

        if (fallasElectrica != null)
            foreach (var f in fallasElectrica) if (f != null) f.RestaurarFalla();
    }
}
