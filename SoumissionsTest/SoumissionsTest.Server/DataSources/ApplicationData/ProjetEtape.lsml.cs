using Microsoft.LightSwitch;
using System.Collections.Generic;
using System.Linq;

namespace LightSwitchApplication
{
    public partial class ProjetEtape
    {
        partial void ProjetEtape_Created()
        {
            Estime = 0M;
            CachedNom = string.Empty;
            Active = false;
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

        partial void Active_Changed()
        {
            List<ProjetEtape> projetEtapes = Projet.ProjetEtapes.OrderBy(pe => pe.Etape.Ordre).ToList();


            if (projetEtapes.Any(pe => pe.Active && pe.Etape.Ordre > Etape.Ordre))
            {
                
            }
        }
    }
}