using System;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace UCodeblock.UI
{
    public class DynamicCreationTest : MonoBehaviour
    {
        public Transform Parent;

        private void Start()
        {
            GenerateEntryBlock();
            GenerateAllPossibleCodeblocks();
        }

        private T TestBlockCreation<T>() where T : CodeblockItem
        {
            CodeblockItem x = Activator.CreateInstance<T>();
            Transform transform = UICodeblock.Generate(x).transform;
            transform.SetParent(Parent, false);

            return x as T;
        }
        private CodeblockItem Create(Type type)
        {
            CodeblockItem item = (CodeblockItem)Activator.CreateInstance(type);
            Transform transform = UICodeblock.Generate(item).transform;
            transform.SetParent(Parent, false);

            transform.position = GenerateRandomScreenPosition(new Vector2(Screen.width, Screen.height) - UCodeblockSettings.Instance.MinBlockSize);

            return item;
        }

        private void GenerateEntryBlock ()
        {
            Transform transform = UICodeblock.GenerateEntryBlock().transform;
            transform.SetParent(Parent, false);
        }

        private Vector2 GenerateRandomScreenPosition (Vector2 limits)
        {
            float x = UnityEngine.Random.value, y = UnityEngine.Random.value;
            return new Vector2(x, y) * limits;
        }

        private void GenerateAllPossibleCodeblocks ()
        {
            foreach (Type t in GetAllCodeblockTypes())
            {
                Create(t);
            }
        }
        private Type[] GetAllCodeblockTypes ()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(d => d.GetTypes())
                .Where(t => typeof(CodeblockItem).IsAssignableFrom(t) 
                    && !t.ContainsGenericParameters
                    && t != typeof(CodeblockItem)).ToArray();
        }
    }
}