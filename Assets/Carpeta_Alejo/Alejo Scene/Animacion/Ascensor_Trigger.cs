using UnityEngine;

public class ControlAscensor : MonoBehaviour
{
    public Animator animatorAscensor;
    public float retrasoAlCerrar = 0.8f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke(nameof(CerrarPuertas));
            if (animatorAscensor != null)
                animatorAscensor.SetBool("EstaAbierto", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke(nameof(CerrarPuertas), retrasoAlCerrar);
        }
    }

    private void CerrarPuertas()
    {
        if (animatorAscensor != null)
            animatorAscensor.SetBool("EstaAbierto", false);
    }
}
