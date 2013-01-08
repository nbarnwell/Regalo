using System;
using System.Collections.Generic;
using System.Linq;

namespace Regalo.Core
{
    public class TypeInspector
    {
        public IEnumerable<Type> GetTypeHierarchy(Type input)
        {
            // Traverse up the inheritence hierarchy but return
            // in top-down order

            if (input == null) return Enumerable.Empty<Type>();

            var stack = new Stack<Type>();
            AddTypeAndInterfaces(stack, input);

            Type baseType = input;
            while ((baseType = baseType.BaseType) != null)
            {
                AddTypeAndInterfaces(stack, baseType);
            }

            return stack.ToList();
        }

        private static void AddTypeAndInterfaces(Stack<Type> stack, Type type)
        {
            // Because we're adding to a stack, the interfaces (more abstract)
            // will actually be returned *after* the implementation (less abstract)
            stack.Push(type);

            // Always return the interfaces in the order they are implemented
            var interfaces = type.GetInterfaces();
            for (int i = interfaces.Length - 1; i >= 0; i--)
            {
                stack.Push(interfaces[i]);
            }
        }
    }
}