
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ActualizarObjetoInfoDesdeCSV
{
    private const string csvPath = "Assets/Editor/Objetos_info.csv";
    private const string outputFolder = "Assets/Informacion/";

    [MenuItem("Herramientas/Actualizar ObjetoInfo desde CSV")]
    public static void CrearObjetosInfoDesdeCSV()
    {
        if (!File.Exists(csvPath))
        {
            Debug.LogError("No se encontró el archivo CSV en: " + csvPath);
            return;
        }

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        string[] lineas = File.ReadAllLines(csvPath);

        for (int i = 1; i < lineas.Length; i++) // Saltar encabezado
        {
            string[] datos = lineas[i].Split(';');
            if (datos.Length < 3) continue;

            string nombreArchivo = datos[0].Trim();
            string descripcion = datos[1].Trim().Replace("\n", "");
            string categoria = datos[2].Trim();

            ObjetoInfoSO obj = ScriptableObject.CreateInstance<ObjetoInfoSO>();
            obj.nombreObjeto = nombreArchivo;
            obj.descripcion = descripcion;
            obj.categoria = categoria;

            string assetPath = outputFolder + nombreArchivo + ".asset";
            AssetDatabase.CreateAsset(obj, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Actualización de ObjetoInfoSO desde CSV completada.");
    }
}
