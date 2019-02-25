using System.Linq;

namespace LightSwitchApplication
{
    public partial class ProjetEtape
    {
        partial void ProjetEtape_Created()
        {
            Estime = 0M;
            CachedNom = string.Empty;
        }

        partial void Projet_Changed()
        {
            UpdateNom();
        }

        partial void Etape_Changed()
        {
            UpdateNom();
        }

        private void UpdateNom()
        {
            if (Etape != null && Projet != null)
            {
                if (Projet.NumProjet > 0)
                {
                    string nom = Projet.Nom + " - " + Etape.Nom + " : " + Projet.NumProjet;
                    if (!CachedNom.Equals(nom)) { CachedNom = nom; }
                    if (!Details.ValidationResults.Errors.Any())
                    {
                        DataWorkspace.ApplicationData.SaveChanges();
                    }
                }
            }
            else
            {
                CachedNom = "_Placeholder_";
            }
        }

        partial void Estime_Changed()
        {
            Projet.UpdateTotal(false);
        }
    }
}