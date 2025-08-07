
using UnityEngine;
using UnityEditor;
using System.IO;

public class CrearInfoObjetos : EditorWindow
{
    private string carpetaDestino = "Assets/Informacion/";

    [MenuItem("Herramientas/Generar InfoObjetos")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CrearInfoObjetos), false, "Crear InfoObjetos");
    }

    void OnGUI()
    {
        GUILayout.Label("Crear ScriptableObjects para objetos", EditorStyles.boldLabel);
        carpetaDestino = EditorGUILayout.TextField("Carpeta destino:", carpetaDestino);

        if (GUILayout.Button("Generar desde nombres"))
        {
            CrearAssets();
        }
    }

    void CrearAssets()
    {
        string[] nombres = new string[] {
            "Info_alarma_de_gases",
            "Info_Gases",
            "Info_termohigrometro",
            "Info_tomac",
            "Info_Windows",  // Antes: Window1
            "Info_Atril_bombas",
            "Info_Basuras",  // Agrupa todas las basuras
            "Info_Calentador_de_mantas_",
            "Info_Carro_medico_Gen",
            "Info_Carro_medico_paro",
            "Info_Cielitica",
            "Info_Desfibrilador",
            "Info_Electrobisturi",
            "Info_Monitor_signos_vitales",
            "Info_Media_caña",
            "Info_Mesa_auxiliar_Rincon",
            "Info_Mesa_de_cirugia",
            "Info_Monitor_Esclava_1",
            "Info_Monitor_pared",
            "Info_Pendant_Anestesia",
            "Info_Pendant_endoscopia",
            "Info_Puerta_1",
            "Info_Sillas",
            "Info_Riñonera",
            "Info_Sillas",           // Trolley_ind_001 no tiene info directa, lo agrupamos
            "Info_Unidad_de_succion",
            "Info_Windows"
        };


        if (!Directory.Exists(carpetaDestino))
        {
            Directory.CreateDirectory(carpetaDestino);
        }

        foreach (string nombre in nombres)
        {
            ObjetoInfoSO asset = ScriptableObject.CreateInstance<ObjetoInfoSO>();
            asset.nombreObjeto = nombre;

            string ruta = Path.Combine(carpetaDestino, $"{nombre.Replace(' ', '_')}.asset");
            AssetDatabase.CreateAsset(asset, ruta);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Completado", "Todos los assets fueron creados.", "OK");
    }
}
