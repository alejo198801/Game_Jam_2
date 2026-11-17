using UnityEngine;

public class PuertaTrigger : MonoBehaviour
{
    public Animator animatorPuerta;

    [SerializeField] private AudioSource sonidoApertura;

    private bool yaAbierta = false;

    private void OnTriggerEnter(Collider other)
    {
        if (yaAbierta || !other.CompareTag("Player"))
            return;

        if (animatorPuerta == null)
            return;

        animatorPuerta.SetTrigger("Abrir");
        yaAbierta = true;

      
        if (sonidoApertura != null)
            sonidoApertura.Play();
    }
}