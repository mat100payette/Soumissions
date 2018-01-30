using Microsoft.LightSwitch;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LightSwitchApplication
{
    public partial class OptionBC
    {
        partial void Nom_Changed()
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(Nom))
            {
                Nom = string.Empty;
            }
            else
            {
                Nom = Nom.ToUpper();
            }
        }
    }
}