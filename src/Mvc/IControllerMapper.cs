using System;

namespace src.Mvc
{
    public interface IControllerMapper
    {
        string GetControllerName(Type type);
        bool ControllerHasAction(string controllerName, string actionName);
    }
}