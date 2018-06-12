using Microsoft.LightSwitch;
using System.Linq;
using System.Collections.Generic;
using LightSwitchApplication.UserCode.Shared;

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
            Taux = 1M;
            SousTotal = 0M;
            Total = 0M;

            List<Etape> etapes = DataWorkspace.ApplicationData.EtapesQuery().Execute().ToList();

            foreach (Etape etape in etapes)
            {
                ProjetEtape projEtape = ProjetEtapes.AddNew();
                projEtape.Etape = etape;
                projEtape.Projet = this;
                projEtape.Estime = 0M;
                projEtape.CachedNom = Nom + " - " + etape.Nom + " : " + NumProjet;
            }
        }

        public void UpdateSousTotal()
        {
            List<ProjetProduit> produits = ProjetProduits.Where(pp => pp.Projet.Equals(this)).ToList();
            decimal tempSousTotal;

            if (produits.Count() > 0)
            {
                tempSousTotal = produits.Sum(p => p.PrixTotal) + LivraisonFlatBedPrix.GetValueOrDefault(0M) + LivraisonLtlPrix.GetValueOrDefault(0M) +
                                                                     MiseMarchePrix.GetValueOrDefault(0M) + LocationGruePrix.GetValueOrDefault(0M);
            }
            else
            {
                tempSousTotal = decimal.Zero;
            }

            SousTotal = tempSousTotal;
        }

        public void UpdateTotal(bool updateSousTotal = true)
        {
            if (updateSousTotal) UpdateSousTotal();

            decimal tempTotal;

            if (EtapeEnCours == null)
            {
                tempTotal = decimal.Zero;
            }
            else
            {
                ProjetEtape projetEtape = ProjetEtapes.Where(pe => pe.Etape == EtapeEnCours && pe.Projet.Equals(this)).SingleOrDefault();
                
                tempTotal = projetEtape == null ? (ProjetProduits.Any() ? (Taux == 0) ? decimal.Zero : SousTotal * Taux : projetEtape.Estime) : decimal.Zero;
            }

            if (EtapeEnCours != null)
            {
                ProjetEtape pEtape = ProjetEtapes.Where(pe => pe.Etape == EtapeEnCours).First();
                if (pEtape.Estime != Total)
                    pEtape.Estime = Total;
            }

            Total = tempTotal;
        }

        partial void Contact_Changed()
        {
            // Check for empty string.
            Contact = string.IsNullOrEmpty(Contact) ? string.Empty : char.ToUpper(Contact[0]) + Contact.Substring(1);
        }
    }
}