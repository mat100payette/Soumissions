using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System;

namespace LightSwitchApplication
{
    public partial class SearchRapports
    {
        // Write your code here.
            
        partial void gridDeleteSelected_CanExecute(ref bool result)
        {
            // Write your code here.
            if (Rapports.SelectedItem.Nom == "Entreprise")
            {
                result = false;
            } else
            {
                result = true;
            }
        }

        partial void gridDeleteSelected_Execute()
        {
            // Write your code here.
            Rapports.SelectedItem.Delete();
        }
    
    }
}