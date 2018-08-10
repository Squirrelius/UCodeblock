using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UCodeblock.UI
{
    public abstract class CodeblockPropertyField : MonoBehaviour
    {
        public UICodeblock Content { get; set; }

        public TMP_InputField _input;
        
        protected void GenerateLayout()
        {
            GameObject inputFieldPrefab = Resources.Load<GameObject>("Codeblock_InputField");

            GameObject inputField = Instantiate(inputFieldPrefab, transform);
            TMP_InputField field = inputField.GetComponent<TMP_InputField>();

            _input = field;
        }
    }
}