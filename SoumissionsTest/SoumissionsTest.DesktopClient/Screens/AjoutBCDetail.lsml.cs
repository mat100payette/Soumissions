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
    public partial class AjoutBCDetail
    {
        partial void AjoutBC_Loaded(bool succeeded)
        {
            // Écrivez votre code ici.
            this.SetDisplayNameFromEntity(this.AjoutBC);
        }

        partial void AjoutBC_Changed()
        {
            // Écrivez votre code ici.
            this.SetDisplayNameFromEntity(this.AjoutBC);
        }

        partial void AjoutBCDetail_Saved()
        {
            // Écrivez votre code ici.
            this.SetDisplayNameFromEntity(this.AjoutBC);
        }
    }
}