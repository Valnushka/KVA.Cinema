using System;
using System.Collections.Generic;

namespace KVA.Cinema.Utilities
{
    public static class CheckUtilities
    {
        public static bool ContainsNullOrEmptyValue(params object[] args)
        {
            if (args == null)
            {
                return true;
            }

            Dictionary<Type, object> defaultValueTypes = new Dictionary<Type, object>();

            foreach (var arg in args)
            {
                if (arg == null || (arg is string && string.IsNullOrWhiteSpace((string)arg)))
                {
                    return true;
                }

                var type = arg.GetType();

                if (type.IsValueType)
                {
                    object def;

                    if (defaultValueTypes.ContainsKey(type))
                    {
                        def = defaultValueTypes[type];
                    }
                    else
                    {
                        def = Activator.CreateInstance(type);

                        defaultValueTypes.Add(type, def);
                    }

                    if (def.Equals(arg))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
