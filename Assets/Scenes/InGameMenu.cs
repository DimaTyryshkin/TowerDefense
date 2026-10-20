using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.UI;

namespace IncreKindom
{
    public class InGameMenu : MonoBehaviour
    {
        [SerializeField, IsntNull] Button shopToggle;
        [SerializeField, IsntNull] Button upgradeToggle;

        [SerializeField, IsntNull] GameObject shopPanel;
        [SerializeField, IsntNull] GameObject upgradePanel;

        private void Start()
        {
            shopToggle.onClick.AddListener(() => SetActive(shopPanel));
            upgradeToggle.onClick.AddListener(() => SetActive(upgradePanel));

            SetActive(shopPanel);
        }

        private void SetActive(GameObject panel)
        {
            shopPanel.SetActive(shopPanel == panel);
            upgradePanel.SetActive(upgradePanel == panel);
        }
    }
}
