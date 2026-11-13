// COMPARTIDO: lo acuerdan los dos. No cambiar sin avisar al otro.
namespace GameJam2.Core
{
    /// Todo lo que el jugador puede usar con la tecla E implementa esto.
    public interface IInteractable
    {
        /// Texto que se muestra al mirar el objeto, por ejemplo "Recoger nota".
        string InteractPrompt { get; }

        /// Se llama cuando el jugador presiona E mirando el objeto.
        void Interact(UnityEngine.GameObject interactor);
    }
}
