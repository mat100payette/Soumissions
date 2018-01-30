using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System;
using C1.Silverlight.Chart;
using LightSwitchApplication.UserCode;

namespace LightSwitchApplication
{
    public partial class CommandesHebdo
    {
        private XYDataSeries seriesProduction;

        private Axis axeValeur;
        private Axis axeMois;
        private double axeMoisMin;
        private double axeMoisMax;
        private DateTime[] mondaysBefore;
        private int nbMois;

        partial void CommandesHebdo_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            axeMoisMin = TimeComparator.getOldestTime().ToOADate();
            axeMoisMax = DateTime.Now.ToOADate();
        }
    }
}