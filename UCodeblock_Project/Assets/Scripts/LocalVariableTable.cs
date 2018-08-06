using System.Collections.Generic;

namespace UCodeblock
{
    public class LocalVariableTable
    {
        private Dictionary<string, object> _variables;

        public LocalVariableTable()
        {
            _variables = new Dictionary<string, object>();
        }

        public object this[string name] => _variables[name];

        public T GetVariable<T>(string name)
        {
            if (_variables.ContainsKey(name))
                return (T)_variables[name];
            else return default(T);
        }
        
        public void CreateVariable<T>(string name)
        {
            CreateVariable(name, default(T));
        }
        public void CreateVariable<T>(string name, T value)
        {
            _variables.Add(name, value);
        }
        
        public void DeleteVariable(string name)
        {
            _variables.Remove(name);
        }

        public void SetVariable<T>(string name, T value)
        {
            _variables[name] = value;
        }
    }
}