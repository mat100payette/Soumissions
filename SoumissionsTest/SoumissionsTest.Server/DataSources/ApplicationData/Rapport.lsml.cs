using Microsoft.LightSwitch;
using System.Linq;
using System.Collections.Generic;
using LightSwitchApplication.UserCode.Shared;

namespace LightSwitchApplication
{
    public partial class Rapport
    {
        private List<Projet> projets;
        private int nbProjets;
        
        public List<Projet> getProjets()
        {
            projets = (from p in DataWorkspace.ApplicationData.ProjetsQuery().Execute()
                       join v in VendeurRapportsQuery.Execute() on p.Vendeur equals v.Vendeur
                       where v.Rapport == this
                       select p).ToList();

            nbProjets = projets.Count();

            return projets;
        }

        partial void Nom_Validate(EntityValidationResultsBuilder results)
        {
            /*if (Nom.ToLower().Equals("entreprise"))
            {
                results.AddPropertyError("Le nom Entreprise est réservé");
            }*/
        }

        private int queryNbProjets(string nomEtape)
        {
            var queryEtape = (from e in DataWorkspace.ApplicationData.EtapesQuery().Execute()
                              where e.Nom == nomEtape
                              select e.Nom).Single();

            var queryVendeurs = (from v in VendeurRapportsQuery.Execute()
                                 where v.Rapport.Id == Id
                                 select v.Vendeur);

            return (from p in DataWorkspace.ApplicationData.ProjetsQuery().Execute()
                    where queryVendeurs.Contains(p.Vendeur) && p.EtapeEnCours.Equals(queryEtape)
                    select p).Count();
        }

        private decimal queryValProjets(string nomEtape)
        {
            var queryEtape = (from e in DataWorkspace.ApplicationData.EtapesQuery().Execute()
                              where e.Nom == nomEtape
                              select e.Nom).Single();

            var queryVendeurs = (from v in VendeurRapportsQuery.Execute()
                                 where v.Rapport.Id == Id
                                 select v.Vendeur);

            return (from p in DataWorkspace.ApplicationData.ProjetsQuery().Execute()
                    where queryVendeurs.Contains(p.Vendeur) && p.EtapeEnCours.Equals(queryEtape)
                    select p).Sum(p => p.Total);
        }

        partial void ValSoumission_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.SOUMISSION);
        }

        partial void ValSpecification_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.SPECIFICATION);
        }

        partial void ValPrevision_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.PREVISION);
        }

        partial void ValAttente_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.ATTENTE);
        }

        partial void ValProduction_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.PRODUCTION);
        }

        partial void NbLivre_Compute(ref int result)
        {
            // Set result to the desired field value
            result = queryNbProjets(EtapeList.LIVRE);
        }

        partial void ValLivre_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.LIVRE);
        }
    }
}