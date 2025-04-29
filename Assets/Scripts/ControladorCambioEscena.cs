using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorCambioEscena : MonoBehaviour
{
    public void IrAEscenaPrincipal()
    {
        SceneManager.LoadScene("MM01");
    }

    public void IrAInterfazInicio()
    {
        SceneManager.LoadScene("InterfazInicio");
    }
}
