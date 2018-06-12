using System.Collections.Specialized;
using System;
using System.Windows.Controls;
using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.LightSwitch;
using System.Linq;
using LightSwitchApplication.UserCode;
using SilverlightCustomControls;
using System.Collections.Generic;
using LightSwitchApplication.UserCode.Shared;
using Microsoft.LightSwitch.Presentation;
using System.Windows;
using Microsoft.LightSwitch.Threading;

namespace LightSwitchApplication
{
    public partial class ProjetDetail
    {
        private ModalWindow windowProduit;
        private ModalWindow windowCommande;
        private Tuple<string, string, string, string, List<string>, List<string>> selectedProduitInfo;
        private Tuple<string, int, decimal> selectedProjetProduitInfo;

        partial void ProjetDetail_InitializeDataWorkspace(List<IDataService> saveChangesTo)
        {
            windowProduit = new ModalWindow(ProjetProduits, "NouveauProduit", "Nouveau Produit");
            windowCommande = new ModalWindow(ProjetProduits, "Commande", "Commande");

            // Wire up an event to detect when the Modal window is closed
            this.FindControl("NouveauProduit").ControlAvailable += ((o, e) =>
            {
                ModalWindow controlProduits = e.Control as ModalWindow;

                ChildWindow window = (ChildWindow)e.Control;
                window.Closed += new EventHandler(Window_Closed);
            });
        }

        partial void ProjetDetail_Created()
        {
            windowProduit.Initialise();
            windowCommande.Initialise();

            var controlEtapes = this.FindControl("Etapes");
            controlEtapes.ControlAvailable -= InitControlEtapes;
            controlEtapes.ControlAvailable += InitControlEtapes;
        }

        private void InitControlEtapes(object o, ControlAvailableEventArgs e)
        {
            EtapesControl etapes = e.Control as EtapesControl;

            etapes.SetEtapeChanged(EtapeChanged);
        }

        private void EtapeChanged(object o, SelectionChangedEventArgs e)
        {
            ComboBox cb = o as ComboBox;
            if (cb == null) return;

            if (cb.SelectedItem != null && cb.SelectedItem is ProjetEtape)
            {
                ProjetEtape selectedProjetEtape = cb.SelectedItem as ProjetEtape;
                ProjetEtape previousProjetEtape = cb.Tag as ProjetEtape;

                if (selectedProjetEtape.Etape != ProjetProperty.EtapeEnCours)
                {
                    if (selectedProjetEtape.Etape.Ordre < ProjetProperty.EtapeEnCours.Ordre)
                    {
                        Details.Dispatcher.BeginInvoke(() =>
                        {
                            string ancienneEtape = "Voules-vous vraiment retourner à une étape antérieure ?";
                            if (this.ShowMessageBox(ancienneEtape, "Attention", MessageBoxOption.YesNo) == MessageBoxResult.No)
                            {
                                Dispatchers.Main.BeginInvoke(() => {
                                    cb.SelectedItem = previousProjetEtape;
                                    ProjetEtapes.SelectedItem = cb.SelectedItem as ProjetEtape;
                                });
                                return;
                            }
                            else
                            {
                                ProjetProperty.ProjetEtapes.Where(pe => pe.Etape.Ordre > selectedProjetEtape.Etape.Ordre).ToList()
                                    .ForEach((pe) => { pe.DateDebut = null; pe.DateFin = null; pe.Estime = 0M; pe.DateProdEstime = null; pe.Active = false; });
                                ProjetProperty.ProjetEtapes.Single(et => et.Etape == ProjetProperty.EtapeEnCours).DateFin = DateTime.Now;
                                ProjetProperty.EtapeEnCours = selectedProjetEtape.Etape;
                                ProjetProperty.ProjetEtapes.Single(et => et.Etape == ProjetProperty.EtapeEnCours).DateDebut = DateTime.Now;
                            }
                        });
                    }
                    else
                    {
                        Details.Dispatcher.BeginInvoke(() =>
                        {
                            ProjetProperty.ProjetEtapes.Single(et => et.Etape == ProjetProperty.EtapeEnCours).DateFin = DateTime.Now;
                            ProjetProperty.EtapeEnCours = selectedProjetEtape.Etape;
                            ProjetProperty.ProjetEtapes.Single(et => et.Etape == ProjetProperty.EtapeEnCours).DateDebut = DateTime.Now;
                        });
                    }

                    ProjetEtapes.SelectedItem = cb.SelectedItem as ProjetEtape;
                }
            }
        }

        private void LoadEtapes(object o, ControlAvailableEventArgs e)
        {
            EtapesControl etapes = e.Control as EtapesControl;
            ComboBox cbEtapes = etapes.GetComboBox();
            cbEtapes.DisplayMemberPath = ProjetEtape.DetailsClass.PropertySetProperties.Etape.Name();
            cbEtapes.ItemsSource = ProjetEtapes.Select(pe => pe).OrderBy(pe => pe.Etape.Ordre).ToList();
            cbEtapes.SelectedItem = ProjetEtapes.SingleOrDefault(pe => pe.Etape == ProjetProperty.EtapeEnCours);
            cbEtapes.Tag = cbEtapes.SelectedItem;
            ProjetEtapes.SelectedItem = cbEtapes.SelectedItem as ProjetEtape;
        }

        partial void ProjetProduitsEditSelected_CanExecute(ref bool result)
        {
            result = (ProjetProduits.SelectedItem != null);
        }

        partial void SaveProduit_Execute()
        {
            bool validated = false;

            this.FindControl("Modeles").ControlAvailable += ((o, e) =>
            {
                ProduitEditor controlProduits = e.Control as ProduitEditor;

                validated = controlProduits.Validate();
            });

            if (validated)
            {
                DataWorkspace workspace = DataWorkspace;

                ProjetProduit selectedPP = ProjetProduits.SelectedItem ?? ProjetProduits.AddNew();
                Produit newProduit = null;

                this.FindControl("Modeles").ControlAvailable += ((o, e) =>
                {
                    ProduitEditor controlProduits = e.Control as ProduitEditor;
                    var produitInfo = controlProduits.GetProduit();

                    Details.Dispatcher.BeginInvoke(() => {
                        newProduit = Produit.CreateProduit(produitInfo, workspace);
                        Produit existingProduit = newProduit.ExistsOther(workspace);

                        Dispatchers.Main.BeginInvoke(() => {
                            selectedPP.SetProjetProduitInfo(controlProduits.GetProjetProduit());
                            controlProduits.ResetColors();
                            
                            if (existingProduit != null)
                            {
                                selectedPP.Produit = existingProduit;
                                newProduit.Details.DiscardChanges();
                            }
                            else
                            {
                                selectedPP.Produit = newProduit;
                            }
                            
                            Details.Dispatcher.BeginInvoke(() =>
                            {
                                if (!DataWorkspace.ApplicationData.Details.GetChanges().Any(c => c.Details.ValidationResults.HasErrors))
                                {
                                    DataWorkspace.ApplicationData.SaveChanges();

                                    workspace.ApplicationData.UpdateModeles().FirstOrDefault();

                                    if (selectedPP.Produit.Modele != null)
                                        selectedPP.CreateTags(selectedPP.Quantite);
                                    else
                                        selectedPP.DeleteTags();

                                    DataWorkspace.ApplicationData.SaveChanges();
                                }

                                Dispatchers.Main.BeginInvoke(() => {
                                    windowProduit.DialogOk();
                                });
                            });
                        });
                    });
                });
            }
        }

        partial void ProjetProduitsAddAndEditNew_Execute()
        {
            this.FindControl("Ajouter").ControlAvailable += ((o, e) =>
            {
                Button bouton = e.Control as Button;
                bouton.Content = "Ajouter";
            });

            windowProduit.setEntityName("Nouveau Produit");
            windowProduit.AddEntity();
        }

        partial void ProjetProduitsEditSelected_Execute()
        {
            LoadProduitInfo();

            this.FindControl("Ajouter").ControlAvailable += ((o, e) =>
            {
                Button bouton = e.Control as Button;
                bouton.Content = "Modifier";
            });

            windowProduit.setEntityName("Modifier Produit");
            windowProduit.ViewEntity();
        }

        private void LoadProduitInfo()
        {
            this.FindControl("Modeles").ControlAvailable += ((o, e) =>
            {
                ProduitEditor controlProduits = e.Control as ProduitEditor;
                controlProduits.SetProduit(selectedProduitInfo);
                controlProduits.SetProjetProduit(selectedProjetProduitInfo);
            });
        }

        void Window_Closed(object sender, EventArgs e)
        {
            ChildWindow window = (ChildWindow)sender;
            
            if (!(window.DialogResult.HasValue))
            {
                EntityChangeSet changes = DataWorkspace.ApplicationData.Details.GetChanges();
                var added = changes.AddedEntities;

                foreach (Produit produit in added.OfType<Produit>())
                    produit.Details.DiscardChanges();

                foreach (ProjetProduit projetProduit in added.OfType<ProjetProduit>())
                    projetProduit.Details.DiscardChanges();

                foreach (ProduitOptionBC produitOptionBC in added.OfType<ProduitOptionBC>())
                    produitOptionBC.Details.DiscardChanges();

                foreach (ModeleInsere modeleInsere in added.OfType<ModeleInsere>())
                    modeleInsere.Details.DiscardChanges();

                if (ProjetProduits.Any())
                {
                    ProjetProduits.SelectedItem = ProjetProduits.Last();
                }
            }
        }

        /// <summary>
        /// Opens windowCommande.
        /// </summary>
        partial void Commande_Execute()
        {
            ProjetProperty.DateCommande = DateTime.Now;
            windowCommande.ViewEntity();
        }

        /// <summary>
        /// Checks if a ProjetProduit is selected.
        /// </summary>
        /// <param name="result">Is in production</param>
        partial void ProjetProduitsDeleteSelected_CanExecute(ref bool result)
        {
            result = (ProjetProduits.SelectedItem != null);
        }

        /// <summary>
        /// Deletes the selected ProjetProduit.
        /// </summary>
        partial void ProjetProduitsDeleteSelected_Execute()
        {
            ProjetProduits.SelectedItem.Delete();
        }

        /// <summary>
        /// Checks if the Projet is in Production or Livré.
        /// </summary>
        /// <param name="result">Is in Production or Livré</param>
        partial void Commande_CanExecute(ref bool result)
        {
            if (ProjetProperty == null)
            {
                result = false;
            }
            else
            {
                string etape = ProjetProperty.EtapeEnCours.ToString();
                result = (etape == EtapeList.DESSIN || etape == EtapeList.PRODUCTION || etape == EtapeList.LIVRE);
            } 
        }

        /// <summary>
        /// Sets the right ProjetEtape when the ProjetEtapes are loaded.
        /// </summary>
        /// <param name="succeeded">Loaded successfully</param>
        partial void ProjetEtapes_Loaded(bool succeeded)
        {
            if (Etapes == null) Etapes.Load();
            var etapesControl = this.FindControl("Etapes");
            etapesControl.ControlAvailable -= LoadEtapes;
            etapesControl.ControlAvailable += LoadEtapes;
        }

        /// <summary>
        /// Sets the Commande's project number to this Projet's when it's loaded.
        /// </summary>
        /// <param name="succeeded">Loaded successfully</param>
        partial void ProjetProperty_Loaded(bool succeeded)
        {
            this.FindControl("Projet").ControlAvailable += ((o, e) =>
            {
                FeuilleCommande commande = e.Control as FeuilleCommande;

                commande.SetNumProjet(ProjetProperty.NumProjet);
                commande.SetProjetProduits(ProjetProduits.Select(
                        pp => new Tuple<int, string, string, decimal, decimal>(pp.Quantite, pp.Produit.Nom, pp.Description, pp.PrixUnitaire, pp.PrixTotal)
                    ).ToList());
            });
        }

        partial void ProjetDetail_Saving(ref bool handled)
        {
            // Écrivez votre code ici.
            var ch = DataWorkspace.ApplicationData.Details.GetChanges();
            ;
        }

        partial void ProjetProduits_SelectionChanged()
        {
            if (ProjetProduits.SelectedItem != null)
            {
                var selected = ProjetProduits.SelectedItem;
                Details.Dispatcher.BeginInvoke(() =>
                {
                    if (selected.Produit == null)
                        selectedProduitInfo = null;
                    else
                        selectedProduitInfo = selected.Produit.GetMainInfo();
                    selectedProjetProduitInfo = selected.GetProjetProduitInfo();
                });
            }
        }
    }
}