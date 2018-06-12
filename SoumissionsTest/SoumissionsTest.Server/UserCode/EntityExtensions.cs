using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Details;
using System.Collections.Generic;
using System.Linq;

namespace LightSwitchApplication.UserCode
{
    public static class EntityExtensions
    {
        public static IEnumerable<IEntityProperty> ChangedProperties(this IEntityObject entity)
        {
            return entity.Details.Properties.All().OfType<IEntityTrackedProperty>().Where(p => p.IsChanged);
        }

        public static Produit ExistsOther(this Produit produit, DataWorkspace workspace)
        {
            int id = produit.Id;
            string nom = produit.Nom;
            var prod = workspace.ApplicationData.Produits.Where(p => p.Nom == nom && p.Id != id).Execute();
            if (prod.Any())
                return prod.First();
            else
                return null;
        }
    }
}