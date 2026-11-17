using UnityEngine;

public class PuertaTrigger : MonoBehaviour
{
    public Animator animatorPuerta;
    public float retrasoAlCerrar = 1.0f;
    private bool yaAbierta = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke(nameof(CerrarPuerta));

            if (!yaAbierta)
            {
                animatorPuerta.SetTrigger("Abrir");
                yaAbierta = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (yaAbierta && other.CompareTag("Player"))
        {
            Invoke(nameof(CerrarPuerta), retrasoAlCerrar);
        }
    }

    private void CerrarPuerta()
    {
        animatorPuerta.SetTrigger("Cerrar");
        yaAbierta = false;
    }
}
