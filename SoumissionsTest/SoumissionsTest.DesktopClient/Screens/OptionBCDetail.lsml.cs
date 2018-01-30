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
    public partial class OptionBCDetail
    {
        partial void OptionBC_Loaded(bool succeeded)
        {
            // Écrivez votre code ici.
            this.SetDisplayNameFromEntity(this.OptionBC);
        }

        partial void OptionBC_Changed()
        {
            // Écrivez votre code ici.
            this.SetDisplayNameFromEntity(this.OptionBC);
        }

        partial void OptionBCDetail_Saved()
        {
            // Écrivez votre code ici.
            this.SetDisplayNameFromEntity(this.OptionBC);
        }
    }
}