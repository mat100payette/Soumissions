using Microsoft.LightSwitch.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace LightSwitchApplication.UserCode
{
    public static class ControlsUI
    {
        private const string ROOT = "RootContentItem";

        public static void doOnAllScreenElements(this IScreenObject screen, Action<object> function)
        {
            screen.FindControl(ROOT).ControlAvailable += ((obj, ev) =>
            {
                Control root = ev.Control as Control;

                IContentItem contentItem = root.DataContext as IContentItem;

                contentItem.traverseTree(function);
            });
        }

        public static void traverseTree(this IContentItem item, Action<object> function)
        {
            if (!item.Name.StartsWith("$") && !item.Name.EndsWith("Template"))
            {
                IContentItemProxy proxy = item.Screen.FindControl(item.Name);

                try
                {
                    proxy.ControlAvailable += (obj, ev) => { function.Invoke(ev.Control); };

                    foreach (var i in item.ChildItems)
                    {
                        i.traverseTree(function);
                    }
                }
                catch (Exception e) { Debug.WriteLine(e.Message + "\n" + e.StackTrace); }
                finally { }
            }
        }
    }
}
