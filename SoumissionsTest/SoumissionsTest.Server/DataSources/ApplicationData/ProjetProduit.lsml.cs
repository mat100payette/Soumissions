using Microsoft.LightSwitch;
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
            if (Description != info.Item1)
                Description = info.Item1;

            if (Quantite != info.Item2)
                Quantite = info.Item2;

            if (PrixUnitaire != info.Item3)
                PrixUnitaire = info.Item3;
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

        public void DeleteTags(List<int> idsToDelete = null)
        {
            // By default, delete all tags.
            List<int> ids = idsToDelete == null ? ProduitsProduction.Select(pp => pp.Id).ToList() : idsToDelete;
            foreach (int id in ids)
            {
                List<ProduitProduction> pp = DataWorkspace.ApplicationData.ProduitsProduction.GetQuery().Execute().Where(p => p.Id == id).ToList();
                if (pp.Any())
                    pp.First().Delete();
            }
        }

        public void CreateTags(int qty, bool deleteAll = false)
        {
            if (deleteAll)
                DeleteTags();

            for (int i = ProduitsProduction.Count(); i < qty; i++)
            {
                ProduitProduction tag = ProduitsProduction.AddNew();
                tag.ProjetProduit = this;
                tag.Tag = "BQ" + i;
            }
        }
    }
}