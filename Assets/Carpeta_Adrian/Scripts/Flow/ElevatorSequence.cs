// PROGRAMADOR B. En la cabina: un Collider "Is Trigger" que inicia la secuencia al entrar el jugador.
using System.Collections;
using GameJam2.Core;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Flow
{
    [RequireComponent(typeof(Collider))]
    public class ElevatorSequence : MonoBehaviour
    {
        [SerializeField] private Door elevatorDoor;
        [SerializeField] private float closeDelay = 0.5f;
        [SerializeField] private float rideDuration = 3f;

        [Header("Cambio de mundo")]
        [SerializeField] private GameObject normalOffice;
        [SerializeField] private GameObject corruptedOffice;

        [Header("Eventos para arte y sonido")]
        public UnityEvent OnRideStart;     // sonido de subida, luces que parpadean
        public UnityEvent OnWorldSwapped;
        public UnityEvent OnRideEnd;

        private bool used;

        private void Star()
        {
            if(elevatorDoor != null) 
            {
                elevatorDoor.Open();
                
            }
            if(corruptedOffice != null)
            {
                corruptedOffice.SetActive(false);
            }
        }
        private void Reset()
        {
            // TODO: Marcar el Collider como trigger al agregar el componente.
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            // TODO: Si entra el Player y no se ha usado, iniciar la corrutina Ride.
            if(used || !other.CompareTag(GameTags.Player))
            {
                return;
            }
            used = true;
            StartCoroutine(Ride());
        }

        private IEnumerator Ride()
        {
        PlayerControlLock.Lock();
        yield return new WaitForSeconds(closeDelay);

        if (elevatorDoor != null)
        {
          elevatorDoor.Close();
         }   
        OnRideStart?.Invoke();

        yield return new WaitForSeconds(rideDuration);

        if (normalOffice != null)
        {
            normalOffice.SetActive(false);
        }
        if (corruptedOffice != null)
        {
            corruptedOffice.SetActive(true);
        }
        OnWorldSwapped?.Invoke();

        if (elevatorDoor != null)
        {
            elevatorDoor.Open();
        }
        OnRideEnd?.Invoke();
        PlayerControlLock.Unlock();
        }
    }
}
