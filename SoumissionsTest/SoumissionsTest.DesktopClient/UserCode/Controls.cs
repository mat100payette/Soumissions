using Microsoft.LightSwitch.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace LightSwitchApplication.UserCode
{
    public static class Controls
    {
        public static void add(this IContentItemProxy proxy, EventHandler<ControlAvailableEventArgs> handler)
        {
            EventHandler<ControlAvailableEventArgs> h = null;
            h = ((obj, ev) =>
            {
                handler.BeginInvoke(obj, ev, new AsyncCallback((r) => { proxy.ControlAvailable -= h; }), proxy);
            });

            proxy.ControlAvailable += h;
        }

        public static void ActOnControl<TControl>(this IScreenObject screen, int controlIndex, Action<TControl> action)
        {
            string screenName = screen.Name;
            string enumName = screenName + "ctrl";

            string ctrlName = Enum.GetName(Type.GetType(enumName, true, true), controlIndex);
            screen.FindControl(ctrlName).ControlAvailable += ((obj, ev) =>
            {
                TControl control = (TControl)ev.Control;

                action(control);
            });
        }
    }
}
