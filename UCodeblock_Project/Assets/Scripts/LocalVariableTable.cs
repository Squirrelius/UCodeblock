using System.Collections.Generic;

namespace UCodeblock
{
    /// <summary>
    /// Provides a lookup table for values locally defined in a <see cref="CodeblockSystem"/>.
    /// </summary>
    public class LocalVariableTable
    {
        private Dictionary<string, object> _variables;

        /// <summary>
        /// Initializes a new variable table.
        /// </summary>
        public LocalVariableTable()
        {
            _variables = new Dictionary<string, object>();
        }

        /// <summary>
        /// Returns a variable by its name.
        /// </summary>
        public object this[string name] => _variables[name];

        /// <summary>
        /// Returns a variable by its name. Returns the default value for <typeparamref name="T"/> if the variable doesnt exist in the table.
        /// </summary>
        public T GetVariable<T>(string name)
        {
            if (_variables.ContainsKey(name))
                return (T)_variables[name];
            else return default(T);
        }
        
        /// <summary>
        /// Creates a new variable in the table.
        /// </summary>
        public void CreateVariable<T>(string name)
        {
            CreateVariable(name, default(T));
        }
        /// <summary>
        /// Creates a new variable in the table, with a default value.
        /// </summary>
        public void CreateVariable<T>(string name, T value)
        {
            _variables.Add(name, value);
        }
        
        /// <summary>
        /// Deletes a variable by name.
        /// </summary>
        public void DeleteVariable(string name)
        {
            _variables.Remove(name);
        }

        /// <summary>
        /// Sets the value of a variable by name.
        /// </summary>
        public void SetVariable<T>(string name, T value)
        {
            _variables[name] = value;
        }
    }
}