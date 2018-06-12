using System;
using System.Collections.Generic;
using System.Linq;

namespace LightSwitchApplication
{
    public partial class ProjetProduit
    {
        public Tuple<string, int, decimal> GetProjetProduitInfo()
        {
            return new Tuple<string, int, decimal>(
                    this.Description, this.Quantite, this.PrixUnitaire
                );
        }

        public void SetProjetProduitInfo(Tuple<string, int, decimal> info)
        {
            this.Description = info.Item1;
            this.Quantite = info.Item2;
            this.PrixUnitaire = info.Item3;
        }

        partial void ProjetProduit_Created()
        {
            Quantite = 1;
            PrixUnitaire = 0;
            PrixTotal = 0;
        }

        partial void Quantite_Changed()
        {
            UpdateTotal();
        }

        partial void PrixUnitaire_Changed()
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            PrixTotal = Quantite * PrixUnitaire;
        }

        partial void PrixTotal_Changed()
        {
            Projet.UpdateTotal();
        }

        public void DeleteTags()
        {
            List<int> ids = ProduitsProduction.Select(pp => pp.Id).ToList();
            foreach (int id in ids)
            {
                var tag = ProduitsProduction.Single(pp => pp.Id == id);
                ProduitsProduction.Remove(tag);
                tag.Delete();
            }
        }

        public void CreateTags(int qty)
        {
            DeleteTags();
            for (int i = 0; i < qty; i++)
            {
                ProduitProduction tag = ProduitsProduction.AddNew();
                tag.ProjetProduit = this;
                tag.Tag = "BQ" + i;
            }
        }
    }
}