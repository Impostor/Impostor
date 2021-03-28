using System.Collections.Generic;

namespace Impostor.Server.Net.Inner
{
    internal class GameObject
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
                if (component is T c)
                {
                    result.Add(c);
                }
            }

            return result;
        }
    }
}
