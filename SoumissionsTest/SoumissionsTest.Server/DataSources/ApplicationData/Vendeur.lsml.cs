using LightSwitchApplication.UserCode.Shared;
using Microsoft.LightSwitch;
using System.Linq;

namespace LightSwitchApplication
{
    public partial class Vendeur
    {
        partial void NomComplet_Compute(ref string result)
        {
            // Set result to the desired field value
            if (Prenom != null && Prenom.Length >= 1 && Nom != null && Nom.Length >= 2)
            {
                result = Prenom.Substring(0, 1).ToUpper() + "." + char.ToUpper(Nom[0]) + Nom.Substring(1);
            }

            if (CachedNom != result)
            {
                CachedNom = result ?? "";
            }
        }

        partial void ValeurTotaleProjets_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = (from p in ProjetsQuery
                      where p.Vendeur.Id == Id
                     select p).Execute().Sum(p => p.Total);
        }

        private int queryNbProjets(string nomEtape)
        {
            var queryEtape = (from e in DataWorkspace.ApplicationData.EtapesQuery().Execute()
                              where e.Nom == nomEtape
                              select e.Nom).Single();

            return (from p in ProjetsQuery
                    where p.Vendeur.Id == Id && p.EtapeEnCours.Equals(queryEtape)
                    select p).Execute().Count();
        }

        private decimal queryValProjets(string nomEtape)
        {
            var queryEtape = (from e in DataWorkspace.ApplicationData.EtapesQuery().Execute()
                              where e.Nom == nomEtape
                              select e.Nom).Single();

            return (from p in ProjetsQuery
                    where p.Vendeur.Id == Id && p.EtapeEnCours.Equals(queryEtape)
                    select p).Execute().Sum(p => p.Total);
        }

        public int GetNbProjets(Etape e)
        {
            return Projets.Where(p => e == p.EtapeEnCours).Count();
        }

        public decimal GetValProjets(Etape e)
        {
            return Projets.Where(p => e == p.EtapeEnCours).Sum(p => p.Total);
        }

        partial void NbProspection_Compute(ref int result)
        {
            // Set result to the desired field value
            result = queryNbProjets(EtapeList.PROSPECTION);
        }

        partial void NbBudget_Compute(ref int result)
        {
            // Set result to the desired field value
            result = queryNbProjets(EtapeList.BUDGET);
        }

        partial void ValBudget_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.BUDGET);
        }

        partial void NbSoumission_Compute(ref int result)
        {
            // Set result to the desired field value
            result = queryNbProjets(EtapeList.SOUMISSION);
        }

        partial void ValSoumission_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.SOUMISSION);
        }

        partial void NbSpecification_Compute(ref int result)
        {
            // Set result to the desired field value
            result = queryNbProjets(EtapeList.SPECIFICATION);
        }

        partial void ValSpecification_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.SPECIFICATION);
        }

        partial void NbPrevision_Compute(ref int result)
        {
            // Set result to the desired field value
            result = queryNbProjets(EtapeList.PREVISION);
        }

        partial void ValPrevision_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.PREVISION);
        }

        partial void NbAttente_Compute(ref int result)
        {
            // Set result to the desired field value
            result = queryNbProjets(EtapeList.ATTENTE);
        }

        partial void ValAttente_Compute(ref decimal result)
        {
            // Set result to the desired field value
            result = queryValProjets(EtapeList.ATTENTE);
        }

        partial void NbProduction_Compute(ref int result)
        {
            // Set result to the desired field value
            result = queryNbProjets(EtapeList.PRODUCTION);
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

        partial void NomEtNum_Compute(ref string result)
        {
            if (NumVendeur != null)
            {
                result = NomComplet + " - " + NumVendeur;
            } else
            {
                result = NomComplet + " - " + NumVendeur;
            }
        }
    }
}