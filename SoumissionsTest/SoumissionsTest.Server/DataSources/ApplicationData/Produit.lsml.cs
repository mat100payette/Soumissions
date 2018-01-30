using Microsoft.LightSwitch;
using System.Linq;
using System.Collections.Generic;

namespace LightSwitchApplication
{
    public partial class Produit
    {
        partial void Nom_Compute(ref string result)
        {
            result = UpdateNom();
        }

        public string UpdateNom()
        {
            string nom = "";

            nom += Modele == null ? string.Empty : Modele.NomComplet;
            nom += Item == null ? string.Empty : Item.Nom;
            nom += Autre;

            nom += AjoutBC == null ? string.Empty : ("-(" + AjoutBC.Nom + ")");

            if (ModelesInseres != null)
            {
                List<string> mi = ModelesInseres.OrderBy(m => m.Modele.NomComplet).Select(m => m.Modele.NomComplet).Distinct().ToList();

                foreach (string modele in mi)
                {
                    int count = ModelesInseres.Where(m => m.Modele.NomComplet.Equals(modele)).Count();

                    nom += "-" + modele;
                    if (count > 1)
                    {
                        nom += "(x" + count + ")";
                    }
                }
            }

            if (ProduitOptionsBC != null)
            {
                List<string> prodOptions = ProduitOptionsBC.OrderBy(o => o.OptionBC.Nom).Select(o => o.OptionBC.Nom).Distinct().ToList();

                foreach (string op in prodOptions)
                {
                    int count = ProduitOptionsBC.Where(po => po.OptionBC.Nom.Equals(op)).Count();

                    nom += "-" + op;
                    if (count > 1)
                    {
                        nom += "(x" + count + ")";
                    }
                }
            }

            if (!CachedNom.Equals(nom)) { CachedNom = nom; }

            return nom;
        }

        partial void IsModele_Compute(ref bool result)
        {
            result = (Modele != null);
        }

        partial void Produit_Created()
        {
            CachedNom = string.Empty;
        }
    }
}