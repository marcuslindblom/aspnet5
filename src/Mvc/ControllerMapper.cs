using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace src.Mvc
{
    public class ControllerMapper : IControllerMapper
    {
        private static readonly ConcurrentDictionary<Type, string> ControllerMap =
            new ConcurrentDictionary<Type, string>();

        private static readonly ConcurrentDictionary<string, string[]> ControllerActionMap =
            new ConcurrentDictionary<string, string[]>();

        public ControllerMapper(ControllerFeature feature)
        {
            foreach (var type in feature.Controllers)
            {
                ControllerMap.TryAdd(type.GetType(), type.Name.Replace("Controller", ""));
                var methodNames = type.GetMethods().Select(x => x.Name).ToArray();
                ControllerActionMap.TryAdd(type.Name.Replace("Controller", ""), methodNames);
            }
        }

        public string GetControllerName(Type type)
        {
            string name;
            ControllerMap.TryGetValue(type, out name);
            return name;
        }

        public bool ControllerHasAction(string controllerName, string actionName)
        {
            if (controllerName == null) return false;
            return ControllerActionMap.ContainsKey(controllerName) &&
                   ControllerActionMap[controllerName].Any(
                       action => string.Equals(action, actionName, StringComparison.OrdinalIgnoreCase));
        }
    }
}