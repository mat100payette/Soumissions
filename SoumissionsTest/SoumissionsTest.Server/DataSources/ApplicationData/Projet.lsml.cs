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
            Taux = 1M;
            SousTotal = 0M;
            Total = 0M;
            UniteExpress = false;
            UniteStandard = false;
            UniteCustom = false;
            Furnace = false;

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
            decimal tempSousTotal = 0M;

            if (produits.Count() > 0)
                tempSousTotal += produits.Sum(p => p.PrixTotal);

            tempSousTotal += LivraisonFlatBedPrix.GetValueOrDefault(0M) + LivraisonLtlPrix.GetValueOrDefault(0M) +
                             MiseMarchePrix.GetValueOrDefault(0M) + LocationGruePrix.GetValueOrDefault(0M);

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

                tempTotal = projetEtape == null ?
                    decimal.Zero : 
                    ProjetProduits.Any() ?
                        SousTotal * Taux : 
                        projetEtape.Estime;
            }

            Total = tempTotal;
        }

        partial void Contact_Changed()
        {
            // Check for empty string.
            Contact = string.IsNullOrEmpty(Contact) ? string.Empty : char.ToUpper(Contact[0]) + Contact.Substring(1);
        }

        partial void LivraisonFlatBedPrix_Changed()
        {
            UpdateTotal();
        }

        partial void LivraisonLtlPrix_Changed()
        {
            UpdateTotal();
        }

        partial void MiseMarchePrix_Changed()
        {
            UpdateTotal();
        }

        partial void LocationGruePrix_Changed()
        {
            UpdateTotal();
        }
    }
}