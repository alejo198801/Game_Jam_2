// PROGRAMADOR A. Lógica del teclado. Los botones del Canvas llaman PressDigit, Delete, Submit y Close.
using GameJam2.Core;
using TMPro;
using UnityEngine;

namespace GameJam2.Puzzles
{
    public class KeypadPanel : MonoBehaviour
    {
        [SerializeField] private CodeManager codeManager;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text display;

        private string input = "";
        public bool IsOpen => panelRoot.activeSelf;

        private void Start()
        {
            // TODO: Ocultar el panel al iniciar.
        }

        public void Show()
        {
            // TODO: Limpiar la entrada, mostrar el panel y bloquear al jugador con cursor visible.
        }

        public void Close()
        {
            // TODO: Ocultar el panel y desbloquear al jugador.
        }

        public void PressDigit(int digit)
        {
            // TODO: Agregar el dígito si no se pasó del largo del código y refrescar la pantalla.
        }

        public void Delete()
        {
            // TODO: Borrar el último dígito y refrescar.
        }

        public void Submit()
        {
            // TODO: Enviar al CodeManager. Si acierta, cerrar; si no, limpiar la entrada.
        }

        private void Update()
        {
            // TODO: Cerrar el panel con Escape.
        }

        private void Refresh()
        {
            // TODO: Mostrar la entrada en pantalla rellenando con guiones bajos.
        }
    }
}
