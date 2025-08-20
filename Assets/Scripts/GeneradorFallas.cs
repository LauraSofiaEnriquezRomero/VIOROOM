using System.Collections.Generic;
using UnityEngine;

public enum FaseSimulacion
{
    Exploracion,
    Evaluacion
}

public class GeneradorFallas : MonoBehaviour
{
    // 🔹 Estado global de la simulación
    public static FaseSimulacion faseActual = FaseSimulacion.Exploracion;

    [Header("Fallas por categoría (arrastrar objetos que tengan FallaInteractiva)")]
    public FallaInteractiva[] fallasInfraestructura;
    public FallaInteractiva[] fallasDotacion;
    public FallaInteractiva[] fallasElectrica;

    [Header("Fallas representadas como objetos ocultables (ej: mesa, monitor, bisturí...)")]
    public FallaInteractiva[] fallasOcultables;

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
        Debug.Log("[GeneradorFallas] Iniciar Fase 2 (Evaluación)");

        // 🔹 Cambiar estado a Evaluación
        faseActual = FaseSimulacion.Evaluacion;

        DesactivarTodas();
        activadas.Clear();

        ActivarFallasAleatorias(fallasInfraestructura, 2);
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

    public List<GameObject> GetFallasDotacionActivas()
    {
        List<GameObject> activas = new List<GameObject>();
        foreach (var f in fallasDotacion)
        {
            if (f != null && f.gameObject.activeSelf)
                activas.Add(f.gameObject);
        }
        return activas;
    }

    public List<FallaInteractiva> GetDotacionActiva()
    {
        List<FallaInteractiva> activas = new List<FallaInteractiva>();
        foreach (var f in fallasDotacion)
        {
            if (f != null && f.gameObject.activeSelf)
                activas.Add(f);
        }
        return activas;
    }

    // 🔹 Devuelve TODAS las opciones de dotación (para UI, independientemente del estado activo/inactivo)
    public List<FallaInteractiva> GetOpcionesDotacion()
    {
        var lista = new List<FallaInteractiva>();
        foreach (var f in fallasDotacion)
            if (f != null) lista.Add(f);
        return lista;
    }

    // 🔹 NUEVO: Devuelve todos los objetos que se pueden ocultar en el entorno
    // (estos son los que SIEMPRE deben mostrarse en el panel de dotación)
    public List<GameObject> GetOpcionesOcultables()
    {
        List<GameObject> opciones = new List<GameObject>();

        if (fallasOcultables != null)
        {
            foreach (var f in fallasOcultables)
                if (f != null) opciones.Add(f.gameObject);
        }

        return opciones;
    }

    public List<FallaInteractiva> GetFallasActivadas()
    {
        return new List<FallaInteractiva>(activadas);
    }
}
