using Microsoft.LightSwitch;
using System.Linq;
using System.Collections.Generic;

namespace LightSwitchApplication
{
    public partial class Projet
    {
        private const int INITIAL_COUNT = 30001;
        private const int FOLDER_CAPACITY = 1000;

        partial void Projet_Created()
        {
            MiseMarche = false;
            Transport = false;
            Taux = 1;
            CachedTotal = 0M;

            var etapes = DataWorkspace.ApplicationData.EtapesQuery().Execute().ToList();

            foreach (Etape etape in etapes)
            {
                ProjetEtape projEtape = ProjetEtapes.AddNew();
                projEtape.Etape = etape;
                projEtape.Projet = this;
                projEtape.Estime = 0M;
                projEtape.CachedNom = Nom + " - " + etape.Nom + " : " + NumProjet;
            }
        }

        partial void SousTotal_Compute(ref decimal result)
        {
            List<ProjetProduit> produits = (from p in ProjetProduits
                                            where p.Projet.Equals(this)
                                            select p).ToList();
            decimal sousTotal;

            if (produits.Count() > 0)
            {
                sousTotal = produits.Sum(p => p.PrixTotal) + LivraisonFlatBedPrix.GetValueOrDefault(0M) + LivraisonLtlPrix.GetValueOrDefault(0M) +
                                                                     MiseMarchePrix.GetValueOrDefault(0M) + LocationGruePrix.GetValueOrDefault(0M);
            } else
            {
                sousTotal = decimal.Zero;
            }

            result = sousTotal;
        }

        partial void Total_Compute(ref decimal result)
        {
            decimal total;

            if (EtapeEnCours == null)
            {
                total = decimal.Zero;
            }
            else
            {
                ProjetEtape projetEtape = ProjetEtapes.Where(p => p.Etape == EtapeEnCours && p.Projet.Equals(this)).SingleOrDefault();

                if (projetEtape != null)
                {
                    total = ProjetProduits.Any() ? (Taux == 0) ? decimal.Zero : SousTotal * Taux : projetEtape.Estime;
                }
                else
                {
                    total = decimal.Zero;
                }
            }

            if (CachedTotal != total) { CachedTotal = total; }

            if (EtapeEnCours != null)
            {
                ProjetEtape pEtape = ProjetEtapes.Where(pe => pe.Etape == EtapeEnCours).First();
                if (pEtape.Estime != CachedTotal) { pEtape.Estime = CachedTotal.Value; }
            }

            result = total;
        }

        partial void Contact_Changed()
        {
            // Check for empty string.
            Contact = string.IsNullOrEmpty(Contact) ? string.Empty : char.ToUpper(Contact[0]) + Contact.Substring(1);
        }
    }
}