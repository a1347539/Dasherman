using FYP.Global.InGame;
using FYP.InGame.PlayerInstance;
using System;
using UnityEngine;


namespace FYP.PlayerRegistration
{
    public class ClassSelectionUI : MonoBehaviour
    {
        public static Action<ScriptableCharacter> onClassButtonSelected = delegate { };

        public static Action<int> onClassConfirmSelection = delegate { };

        [SerializeField]
        private Transform modelContainer;
        [SerializeField]
        private Transform classButtonContainer;
        [SerializeField]
        private GameObject characterStand;
        [SerializeField]
        private GameObject classButtonPrefab;
        private GameObject currentSpriteInDisplay;
        private int currentCharacterID;

        [SerializeField]
        private GameObject StatRowContainer;

        [SerializeField]
        private GameObject ConfirmButton;

        public ScriptableCharacter[] charactersDatas { get; private set; }

        private void Awake()
        {
            onClassButtonSelected += handleButtonSelected;
            onClassButtonSelected += handleDisplayCharacterModel;
            onClassButtonSelected += handleDisplayStats;
        }

        private void OnDestroy()
        {
            onClassButtonSelected -= handleButtonSelected;
            onClassButtonSelected -= handleDisplayCharacterModel;
            onClassButtonSelected -= handleDisplayStats;
        }

        private void Start()
        {
            charactersDatas = Resources.LoadAll<ScriptableCharacter>(PlayerKeys.scriptableCharacterPathPrefix);
            foreach (ScriptableCharacter data in charactersDatas)
            {
                GameObject button = Instantiate(classButtonPrefab, classButtonContainer);
                button.GetComponent<ClassButton>().initializeButton(data);
            }
        }

        private void handleButtonSelected(ScriptableCharacter data) {
            ConfirmButton.SetActive(true);
            currentCharacterID = data.characterId;
        }

        private void handleDisplayCharacterModel(ScriptableCharacter data)
        {
            characterStand.SetActive(true);
            if (currentSpriteInDisplay != null)
            {
                Destroy(currentSpriteInDisplay);
            }
            currentSpriteInDisplay = Instantiate(data.characterPrefab.transform.Find("Sprite").gameObject, modelContainer);
            currentSpriteInDisplay.transform.localScale = Vector3.one * 8;
        }


        private void handleDisplayStats(ScriptableCharacter data)
        {
            StatRowContainer.SetActive(true);
            StatRowContainer.GetComponent<StatRowContainer>().initializeRows(data);
        }

        public void onCharacterSelectionConfirmButtonClick() {
            onClassConfirmSelection?.Invoke(currentCharacterID);
        }
    }
}