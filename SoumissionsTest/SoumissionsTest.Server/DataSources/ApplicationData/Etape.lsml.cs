using LightSwitchApplication.UserCode;
using System.Windows.Media;


namespace LightSwitchApplication
{
    public partial class Etape
    {
        public const double COL_SATURATION = 0.5;
        public const double COL_LIGHTING = 0.5;

        public override string ToString()
        {
            return Nom;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this == null) return false;
            switch (obj.GetType().Name.ToLower())
            {
                case "string":
                    return (Nom.Equals(((string)obj).ToUpper()));
                default:
                    return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        partial void Nom_Changed()
        {
            if (Nom != null)
            {
                Nom = Nom.ToUpper();
            }
        }

        public Color GetColor()
        {
            return ClientUtilities.HSLColor(Hue, COL_SATURATION, COL_LIGHTING);
        }

        partial void Etape_Created()
        {
            Hue = 0;
            AfficheeParDefaut = true;
        }
    }
}