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
    public partial class EtapeDetail
    {
        partial void Etape_Loaded(bool succeeded)
        {
            this.SetDisplayNameFromEntity(this.Etape);
        }

        partial void Etape_Changed()
        {
            this.SetDisplayNameFromEntity(this.Etape);
        }

        partial void EtapeDetail_Saved()
        {
            this.SetDisplayNameFromEntity(this.Etape);
        }
    }
}