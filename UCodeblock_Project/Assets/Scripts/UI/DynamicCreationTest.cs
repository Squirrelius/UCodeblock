using UnityEngine;

namespace UCodeblock.UI
{
    public class DynamicCreationTest : MonoBehaviour
    {
        public Transform Parent;

        private void Start()
        {
            GenerateEntryBlock();

            for (int i = 0; i < 3; i++)
                TestBlockCreation<DebugLogBlock>();
        }

        private T TestBlockCreation<T>() where T : CodeblockItem
        {
            CodeblockItem x = System.Activator.CreateInstance<T>();
            Transform transform = UICodeblock.Generate(x).transform;
            transform.SetParent(Parent, false);
            transform.position = new Vector3(Screen.width * Random.value, Screen.height * Random.value);

            return x as T;
        }

        private void GenerateEntryBlock ()
        {
            Transform transform = UICodeblock.GenerateEntryBlock().transform;
            transform.SetParent(Parent, false);
            transform.position = new Vector3(Screen.width * Random.value, Screen.height * Random.value);
        }
    }
}