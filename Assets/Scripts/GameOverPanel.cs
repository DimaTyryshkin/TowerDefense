using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    class GameOverPanel : MonoBehaviour
    {
        [SerializeField, IsntNull] Button nextButton;

        internal event UnityAction Click;

        private void Start()
        {
            nextButton.onClick.AddListener(() => Click.Invoke());
        }

        internal void Show() => gameObject.SetActive(true);

        internal void Hide() => gameObject.SetActive(false);
    }
}
