using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightSwitchApplication.UserCode;

namespace LightSwitchApplication
{
    public partial class ApplicationDataService
    {
        private const int INITIAL_COUNT = 30001;
        private const int FOLDER_CAPACITY = 1000;
        private const string FOLDER_PATH = @"\\BQDATA01\projets\";

        partial void ProjetsRapportQuery_PreprocessQuery(string nomRapport, ref IQueryable<Projet> query)
        {
            query = query.Where(p => p.Vendeur.VendeurRapports.Any(r => r.Rapport.Nom.Equals(nomRapport)));
        }

        partial void Projets_Inserted(Projet entity)
        {
            Dispatchers.Current.BeginInvoke(() =>
            {
                IEnumerable<Projet> items = DataWorkspace.ApplicationData.Projets.GetQuery().Execute();

                if (items.Count() > 0)
                {
                    if (items.Max(i => i.NumProjet) < INITIAL_COUNT)
                    {
                        entity.NumProjet = INITIAL_COUNT;
                    }
                    else
                    {
                        entity.NumProjet = items.Max(p => p.NumProjet) + 1;
                    }

                }
                else
                {
                    entity.NumProjet = INITIAL_COUNT;
                }

                int thousandAndOne = (int)(decimal.Floor(entity.NumProjet / FOLDER_CAPACITY)) * FOLDER_CAPACITY + 1;
                string rangeDirPath = FOLDER_PATH + (thousandAndOne) + "-" + (thousandAndOne + FOLDER_CAPACITY - 1);

                bool networkFound = true;

                if (entity.NumProjet % FOLDER_CAPACITY - 1 == 0)
                {
                    if (!Directory.Exists(rangeDirPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(rangeDirPath);
                            networkFound = true;
                        }
                        catch
                        {
                            networkFound = false;
                        }
                    }
                }
                else
                {
                    networkFound = Directory.Exists(rangeDirPath);
                }

                string projectPath = rangeDirPath + @"\" + entity.NumProjet;

                if (!Directory.Exists(projectPath) && networkFound)
                {
                    // Nouveau dossier principal du projet
                    Directory.CreateDirectory(projectPath);

                    // Sous-dossiers
                    Directory.CreateDirectory(projectPath + @"\0_INFOS CLIENT");
                    Directory.CreateDirectory(projectPath + @"\0_INFOS CLIENT\DEVIS");
                    Directory.CreateDirectory(projectPath + @"\0_INFOS CLIENT\DEVIS ÉLECTRIQUE");
                    Directory.CreateDirectory(projectPath + @"\0_INFOS CLIENT\PLANS");

                    Directory.CreateDirectory(projectPath + @"\1_SPÉCIFICATION");

                    Directory.CreateDirectory(projectPath + @"\2_SOUMISSION");

                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\DEVIS_PO_INFO PROJET");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\BSQ_1");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\BSQ_1\ÉLECTRIQUE ATELIER");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\BSQ_1\RÉFRIGÉRATION");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\BSQ_1\MÉCANIQUE");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\BSQ_2");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\BSQ_2\ÉLECTRIQUE ATELIER");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\BSQ_2\RÉFRIGÉRATION");
                    Directory.CreateDirectory(projectPath + @"\3_DESSIN D'ATELIER\BSQ_2\MÉCANIQUE");

                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\COMPOSANTES APPROUVÉES");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\1_DESSIN ÉLECTRIQUE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\1_DESSIN ÉLECTRIQUE\OBSELÈTE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\2_SÉQUENCE DE CONTRÔLE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\2_SÉQUENCE DE CONTRÔLE\OBSELÈTE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\3_CONTRÔLEUR");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\3_CONTRÔLEUR\1_DÉVELOPPÉ");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\3_CONTRÔLEUR\1_DÉVELOPPÉ\OBSELÈTE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\3_CONTRÔLEUR\2_LISTE IO");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\3_CONTRÔLEUR\2_LISTE IO\OBSELÈTE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\3_CONTRÔLEUR\3_POINTS COM");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\3_CONTRÔLEUR\3_POINTS COM\OBSELÈTE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\3_CONTRÔLEUR\4_OBSELÈTE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\4_VARIATEUR");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\4_VARIATEUR\OBSELÈTE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\5_PIÈCES SPÉCIALES");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\5_PIÈCES SPÉCIALES\OBSELÈTE");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\ÉLECTRIQUE\6_NAGAS");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\TRAIN DE GAZ");
                    Directory.CreateDirectory(projectPath + @"\4_FABRICATION\SOLIN FABRICATION TÔLERIE MIRABEL");

                    Directory.CreateDirectory(projectPath + @"\5_LOGISTIQUE");
                    Directory.CreateDirectory(projectPath + @"\5_LOGISTIQUE\BILL OF LADING");
                    Directory.CreateDirectory(projectPath + @"\5_LOGISTIQUE\CENTRE DE GRAVITÉ");
                    Directory.CreateDirectory(projectPath + @"\5_LOGISTIQUE\DESSIN 3D HORS TOUT");
                    Directory.CreateDirectory(projectPath + @"\5_LOGISTIQUE\PHOTOS");
                    Directory.CreateDirectory(projectPath + @"\5_LOGISTIQUE\PLAN DE LEVAGE");
                    Directory.CreateDirectory(projectPath + @"\5_LOGISTIQUE\POIDS AUX QUATRE COINS");
                    Directory.CreateDirectory(projectPath + @"\5_LOGISTIQUE\SOUMISSION TRANSPORT");

                    Directory.CreateDirectory(projectPath + @"\6_FILIÈRE DE PRODUCTION");

                    Directory.CreateDirectory(projectPath + @"\7_SERVICE APRÈS VENTES");
                    Directory.CreateDirectory(projectPath + @"\7_SERVICE APRÈS VENTES\DEMANDE");
                    Directory.CreateDirectory(projectPath + @"\7_SERVICE APRÈS VENTES\MISE EN MARCHE");
                    Directory.CreateDirectory(projectPath + @"\7_SERVICE APRÈS VENTES\SERVICE");

                    Directory.CreateDirectory(projectPath + @"\8_AUTRES DOCUMENTS");
                    Directory.CreateDirectory(projectPath + @"\8_AUTRES DOCUMENTS\DISCUSSION COURRIEL");
                    Directory.CreateDirectory(projectPath + @"\8_AUTRES DOCUMENTS\FICHES TECHNIQUES");

                    Directory.CreateDirectory(projectPath + @"\9_DEMANDE DE CORRECTION");
                }
            });
        }

        partial void ProjetsFilterQuery_PreprocessQuery(string Num, string Nom, string Etape, string Vendeurs, string Ingenieurs, string Entrepreneurs, string Distributeurs, string Contacts, string Produit, ref IQueryable<Projet> query)
        {
            FilterHelper.FilterProjets(ref query, new ProjetFilter(Num, Nom, Etape, Vendeurs, Ingenieurs, Entrepreneurs, Distributeurs, Contacts, Produit));
        }

        partial void ProjetProduitsQuery_PreprocessQuery(ref IQueryable<ProjetProduit> query)
        {
            //query = query.Where(p => p.Projet.Etape.Equals(EtapeList.DESSIN));
        }

        partial void ProjetEtapes_Validate(ProjetEtape entity, EntitySetValidationResultsBuilder results)
        {
            string msgDate = "La date de début d'une étape ne peut pas être inférieure à la date de fin de celle qui la précède.";

            List<ProjetEtape> projetEtapes = entity.Projet.ProjetEtapes.OrderBy(pe => pe.Etape.Ordre).ToList();

            if (projetEtapes.Count > 1)
            {
                for (int i = 1; i < projetEtapes.Count; i++)
                {
                    if (projetEtapes[i].DateDebut.HasValue && projetEtapes[i - 1].DateFin.HasValue &&
                        projetEtapes[i].DateDebut < projetEtapes[i - 1].DateFin)
                    {
                        results.AddPropertyError(msgDate, entity.Details.Properties.DateDebut);
                        break;
                    }
                }
            }
        }

        partial void ProduitsProduction_Deleted(ProduitProduction entity)
        {
            if (entity.ProjetProduit != null)
            {
                entity.ProjetProduit.Quantite -= 1;
            }
        }

        partial void DeleteAll_PreprocessQuery(ref IQueryable<ServerAction> query)
        {
            DBExtension.DeleteAll();
        }

        partial void UnifyProduits_PreprocessQuery(ref IQueryable<ServerAction> query)
        {
            DBExtension.UnifyProduits();
        }

        partial void Produits_Updating(Produit entity)
        {
            entity.UpdateNom();
        }

        partial void Modeles_Updating(Modele entity)
        {
            entity.UpdateNom();
        }

        partial void UpdateModeles_PreprocessQuery(ref IQueryable<ServerAction> query)
        {
            DBExtension.UpdateModeles();
        }

        partial void CleanProduits_PreprocessQuery(ref IQueryable<ServerAction> query)
        {
            DBExtension.CleanProduits();
        }

        partial void Projets_Updating(Projet entity)
        {
            entity.UpdateTotal();
        }
    }
}