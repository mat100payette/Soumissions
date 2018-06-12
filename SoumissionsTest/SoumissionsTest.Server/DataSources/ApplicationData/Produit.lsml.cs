using Microsoft.LightSwitch;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LightSwitchApplication
{
    public partial class Produit
    {
        public static Produit CreateProduit(Tuple<string, string, string, string, List<string>, List<string>> info, DataWorkspace workspace)
        {
            Produit newProduit = workspace.ApplicationData.Produits.AddNew();
            if (info.Item1 != string.Empty)
                newProduit.Modele = workspace.ApplicationData.Modeles.Where(m => m.NomComplet == info.Item1).Execute().First();
            if (info.Item2 != string.Empty)
                newProduit.Item = workspace.ApplicationData.Items.Where(i => i.Nom == info.Item2).Execute().First();

            newProduit.Autre = info.Item3;
            if (info.Item4 != string.Empty)
                newProduit.AjoutBC = workspace.ApplicationData.AjoutsBC.Where(a => a.Nom == info.Item4).Execute().First();

            foreach (string modele in info.Item5)
            {
                ModeleInsere newModeleInsere = newProduit.ModelesInseres.AddNew();
                newModeleInsere.Modele = workspace.ApplicationData.Modeles.Where(mo => mo.NomComplet.Equals(modele)).Execute().SingleOrDefault();
                newModeleInsere.Produit = newProduit;
            }

            foreach (string opt in info.Item6)
            {
                ProduitOptionBC newProduitOption = newProduit.ProduitOptionsBC.AddNew();
                newProduitOption.OptionBC = workspace.ApplicationData.OptionsBC.Where(op => op.Nom.Equals(opt)).Execute().SingleOrDefault();
                newProduitOption.Produit = newProduit;
            }

            newProduit.UpdateNom();

            return newProduit;
        }

        public Tuple<string, string, string, string, List<string>, List<string>> GetMainInfo()
        {
            return new Tuple<string, string, string, string, List<string>, List<string>>(
                    Modele == null ? string.Empty : Modele.NomComplet, 
                    Item == null ? string.Empty : Item.Nom, 
                    Autre,
                    AjoutBC == null ? string.Empty : AjoutBC.Nom,
                    GetStringModeles(false), GetStringOptionsBC(false)
                );
        }

        public List<string> GetStringModeles(bool distinct = true)
        {
            var insertions = ModelesInseres.Select(o => o.Modele.NomComplet).OrderBy(m => m);
            if (distinct)
                return insertions.Distinct().ToList();
            else
                return insertions.ToList();
        }

        public List<string> GetStringOptionsBC(bool distinct = true)
        {
            var optionsBC = ProduitOptionsBC.Select(o => o.OptionBC.Nom).OrderBy(o => o);
            if (distinct)
                return optionsBC.Distinct().ToList();
            else
                return optionsBC.ToList();
        }

        public void UpdateNom()
        {
            string tempNom = string.Empty;

            tempNom += Modele == null ? string.Empty : Modele.NomComplet;
            tempNom += Item == null ? string.Empty : Item.Nom;
            tempNom += Autre;

            tempNom += AjoutBC == null ? string.Empty : ("-(" + AjoutBC.Nom + ")");

            if (ModelesInseres != null)
            {
                List<string> mi = GetStringModeles();

                foreach (string modele in mi)
                {
                    int count = ModelesInseres.Where(m => m.Modele.NomComplet.Equals(modele)).Count();

                    tempNom += "-" + modele;
                    if (count > 1)
                    {
                        tempNom += "(x" + count + ")";
                    }
                }
            }

            if (ProduitOptionsBC != null)
            {
                List<string> prodOptions = GetStringOptionsBC();

                foreach (string op in prodOptions)
                {
                    int count = ProduitOptionsBC.Where(po => po.OptionBC.Nom.Equals(op)).Count();

                    tempNom += "-" + op;
                    if (count > 1)
                    {
                        tempNom += "(x" + count + ")";
                    }
                }
            }

            Nom = tempNom;
        }

        partial void Produit_Created()
        {
            Nom = string.Empty;
        }
    }
}