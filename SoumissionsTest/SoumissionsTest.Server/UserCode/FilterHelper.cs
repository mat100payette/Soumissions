using System.Data.Objects.SqlClient;
using System.Linq;

namespace LightSwitchApplication.UserCode
{
    public static class FilterHelper
    {
        public static bool NotEmpty(this string someString)
        {
            if (someString == null) { return false; }
            return !(someString.TrimStart(';') == string.Empty || someString.TrimStart(';') == "");
        }

        public static void FilterProjets(ref IQueryable<Projet> query, ProjetFilter filter)
        {
            if (filter.Num.NotEmpty())
            { query = query.Where(p => SqlFunctions.StringConvert((double)p.NumProjet).Trim().Contains(filter.Num)); }

            if (filter.Nom.NotEmpty())
            { query = query.Where(p => p.Nom.Contains(filter.Nom)); }

            if (filter.Etapes.NotEmpty())
            { query = query.Where(p => filter.Etapes.ToUpper().Contains(p.EtapeEnCours.Nom.ToUpper())); }

            if (filter.Vendeurs.NotEmpty())
            { query = query.Where(p => filter.Vendeurs.Contains(";" + SqlFunctions.StringConvert((double)p.Vendeur.Id).Trim() + ";")); }

            if (filter.Ingenieurs.NotEmpty())
            { query = query.Where(p => p.Ingenieur != null)
                    .Where(p => filter.Ingenieurs.Contains(";" + SqlFunctions.StringConvert((double)p.Ingenieur.Id).Trim() + ";")); }

            if (filter.Entrepreneurs.NotEmpty())
            { query = query.Where(p => p.Entrepreneur != null)
                    .Where(p => filter.Entrepreneurs.Contains(";" + SqlFunctions.StringConvert((double)p.Entrepreneur.Id).Trim() + ";")); }

            if (filter.Distributeurs.NotEmpty())
            { query = query.Where(p => p.Distributeur != null)
                    .Where(p => filter.Distributeurs.Contains(";" + SqlFunctions.StringConvert((double)p.Distributeur.Id).Trim() + ";")); }

            if (filter.Contacts.NotEmpty())
            { query = query.Where(p => filter.Contacts.ToUpper().Contains(p.Contact.ToUpper())); }
            
            if (filter.Produit.NotEmpty())
            { query = query.Where(p => p.ProjetProduits.Any(n => n.Produit.CachedNom.Contains(filter.Produit))); }
        }
    }

    public class ProjetFilter
    {
        public string Num { get; private set; } = string.Empty;
        public string Nom { get; private set; } = string.Empty;
        public string Etapes { get; private set; } = string.Empty;
        public string Vendeurs { get; private set; } = string.Empty;
        public string Ingenieurs { get; private set; } = string.Empty;
        public string Entrepreneurs { get; private set; } = string.Empty;
        public string Distributeurs { get; private set; } = string.Empty;
        public string Contacts { get; private set; } = string.Empty;
        public string Produit { get; private set; } = string.Empty;

        public ProjetFilter(string num, string nom, string etapes, string vendeurs, string ingenieurs, string entrepreneurs,
            string distributeurs, string contacts, string produit)
        {
            Num = num;
            Nom = nom;
            Etapes = etapes;
            Vendeurs = vendeurs;
            Ingenieurs = ingenieurs;
            Entrepreneurs = entrepreneurs;
            Distributeurs = distributeurs;
            Contacts = contacts;
            Produit = produit;
        }
    }
}