using SmartAttributes.InspectorButton;
using SmartAttributes.MultiDraft.Attributes;
using UnityEngine;

namespace SmartAttributes.MultiDraft
{
    [InspectorButtonClass]
    public class AttributesTest : MonoBehaviour
    {
        
        
        
        private void OnGUI()
        {
            if (GUILayout.Button("TestOnGUI"))
            {
                Debug.Log("TestOnGUI");
            }
        }

        [Required]
        [Disabled]
        public GameObject FieldRequiredDisabled;

        public bool A;
        [ShowIf(nameof(A))]
        public GameObject ShowIfA;
        [HideIf(nameof(A))]
        public GameObject HideIfA;

        [Disabled]
        public GameObject FieldDisabled;

        [Required]
        public GameObject FieldRequired;


        public TestVisibilityEnum TestVisibilityEnum;
        [ShowIf(nameof(TestVisibilityEnum), TestVisibilityEnum.EnumA)]
        public GameObject FieldEnumA;
        [ShowIf(nameof(TestVisibilityEnum), TestVisibilityEnum.EnumB)]
        public GameObject FieldEnumB;
        [ShowIf(nameof(TestVisibilityEnum), TestVisibilityEnum.EnumC)]
        public GameObject FieldEnumC;
        
        
        
        [ShowIf(nameof(A), true)]
        public GameObject FieldTEST;


        [InspectorButton("Inspector button")]
        public void InspectorButton()
        {
            Debug.Log("InspectorButton Invoke");
        }
    }


    public enum TestVisibilityEnum
    {
        EnumA,
        EnumB,
        EnumC,
    }
}