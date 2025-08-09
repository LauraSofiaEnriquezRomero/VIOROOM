using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MeenuuInicial : MonoBehaviour
{
    public void SiguienteEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}