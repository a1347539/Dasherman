using UnityEngine;
using TMPro;
using FYP.InGame.PlayerInstance;
using UnityEngine.UI;

namespace FYP.PlayerRegistration
{
    public class ClassButton : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text classNameText;
        [SerializeField]
        private Image classIcon;

        private ScriptableCharacter data;

        public void initializeButton(ScriptableCharacter data) { 
            this.data = data;
            classNameText.text = data.characterName;
            classIcon.sprite = data.classIcon;
        }

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => {
                ClassSelectionUI.onClassButtonSelected?.Invoke(data);
            });
        }

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}