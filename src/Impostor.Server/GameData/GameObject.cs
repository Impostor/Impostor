using System.Collections.Generic;

namespace Impostor.Server.GameData
{
    public class GameObject
    {
        public GameObject()
        {
            Components = new List<object>();
        }

        protected List<object> Components { get; }

        public List<T> GetComponentsInChildren<T>()
        {
            var result = new List<T>();

            foreach (var component in Components)
            {
                if (component.GetType().IsAssignableTo(typeof(T)))
                {
                    result.Add((T) component);
                }
            }

            return result;
        }
    }
}