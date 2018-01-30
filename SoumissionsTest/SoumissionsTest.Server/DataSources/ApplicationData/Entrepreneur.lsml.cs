﻿namespace LightSwitchApplication
{
    public partial class Entrepreneur
    {
        partial void Ville_Changed()
        {
            if (string.IsNullOrEmpty(Ville))
            {
                Ville = string.Empty;
            } else
            {
                Ville = char.ToUpper(Ville[0]) + Ville.Substring(1);
            }
        }
    }
}