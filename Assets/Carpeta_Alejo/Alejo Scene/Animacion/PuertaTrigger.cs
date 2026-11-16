using UnityEngine;

public class PuertaTrigger : MonoBehaviour
{
    public Animator animatorPuerta;
    private bool yaAbierta = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!yaAbierta && other.CompareTag("Player"))
        {
            animatorPuerta.SetTrigger("Abrir");
            yaAbierta = true;
        }
    }
}
