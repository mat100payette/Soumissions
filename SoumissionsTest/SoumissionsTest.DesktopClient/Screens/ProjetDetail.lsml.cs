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
        private Produit selectedProduit;
        private ProjetProduit selectedProjetProduit;

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

            Items.SelectedItem = null;
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

                validated = controlProduits.validate();
            });

            if (validated)
            {

                List<string> optionsBC = null;
                List<string> modeles = null;

                List<ProduitOptionBC> produitOptionsBC = (from po in ProduitOptionsBC
                                                                where po.Produit.Equals(ProjetProduits.SelectedItem.Produit)
                                                                select po).ToList();

                List<ModeleInsere> modelesInseres = (from mi in ModelesInseres
                                                           where mi.Produit.Equals(ProjetProduits.SelectedItem.Produit)
                                                           select mi).ToList();

                List<OptionBC> produitOptionsBC_options = new List<OptionBC>();
                foreach (ProduitOptionBC prodOptionBC in produitOptionsBC)
                {
                    produitOptionsBC_options.Add(prodOptionBC.OptionBC);
                }

                List<Modele> modeleInsere_modele = new List<Modele>();
                foreach (ModeleInsere modInsere in modelesInseres)
                {
                    modeleInsere_modele.Add(modInsere.Modele);
                }

                this.FindControl("Modeles").ControlAvailable += ((o, e) =>
                {
                    ProduitEditor controlProduits = e.Control as ProduitEditor;

                    optionsBC = controlProduits.getOptionsBC();
                    modeles = controlProduits.getInsertionsBC();
                    controlProduits.updateAll();
                    controlProduits.resetColors();
                });

                if (optionsBC.Count != 0)
                {
                    List<ProduitOptionBC> oldProduitOptions = ProduitOptionsBC.Where(o => o.Produit.Equals(ProjetProduits.SelectedItem.Produit)).ToList();

                    foreach (ProduitOptionBC prodOption in oldProduitOptions)
                    {
                        prodOption.Delete();
                    }

                    foreach (string option in optionsBC)
                    {
                        OptionBC convertedOptionBC = OptionsBC.Where(op => op.Nom.Equals(option)).SingleOrDefault();
                        if (convertedOptionBC != null)
                        {
                            ProduitOptionBC newProduitOptionBC = DataWorkspace.ApplicationData.ProduitOptionsBC.AddNew();
                            newProduitOptionBC.Produit = ProjetProduits.SelectedItem.Produit;
                            newProduitOptionBC.OptionBC = convertedOptionBC;

                            ProjetProduits.SelectedItem.Produit.ProduitOptionsBC.Add(newProduitOptionBC);
                        }
                    }
                }

                if (modeles.Count != 0)
                {
                    List<ModeleInsere> oldModeleInseres = ModelesInseres.Where(mo => mo.Produit.Equals(ProjetProduits.SelectedItem.Produit)).ToList();

                    foreach (ModeleInsere modInsere in oldModeleInseres)
                    {
                        modInsere.Delete();
                    }

                    foreach (string modele in modeles)
                    {
                        Modele convertedModele = Modeles.Where(mo => mo.NomComplet.Equals(modele)).SingleOrDefault();
                        if (convertedModele != null)
                        {
                            ModeleInsere newInsertionBC = DataWorkspace.ApplicationData.ModelesInseres.AddNew();
                            newInsertionBC.Produit = ProjetProduits.SelectedItem.Produit;
                            newInsertionBC.Modele = convertedModele;

                            ProjetProduits.SelectedItem.Produit.ModelesInseres.Add(newInsertionBC);
                        }
                    }
                }

                ProjetProduits.SelectedItem.Produit.UpdateNom();

                Details.Dispatcher.BeginInvoke(() =>
                {
                    DataWorkspace.ApplicationData.SaveChanges();
                    ProjetProduits.SelectedItem.Produit.UpdateNom();
                    DataWorkspace.ApplicationData.SaveChanges();
                });

                windowProduit.DialogOk();
            }
        }

        partial void ProjetProduitsAddAndEditNew_CanExecute(ref bool result)
        {
            result = true;
        }

        partial void ProjetProduitsAddAndEditNew_Execute()
        {
            this.FindControl("Ajouter").ControlAvailable += ((o, e) =>
            {
                Button bouton = e.Control as Button;

                bouton.Content = "Ajouter";
            });

            selectedProjetProduit = DataWorkspace.ApplicationData.ProjetProduits.AddNew();
            selectedProduit = DataWorkspace.ApplicationData.Produits.AddNew();

            selectedProduit.ProjetProduits.Add(selectedProjetProduit);
            
            selectedProjetProduit.Projet = ProjetProperty;
            selectedProjetProduit.Produit = selectedProduit;

            Produits.SelectedItem = selectedProduit;
            ProjetProduits.SelectedItem = selectedProjetProduit;
            Items.SelectedItem = null;
            
            initProduitWindow();

            windowProduit.setEntityName("Nouveau Produit");
            windowProduit.AddEntity();
        }

        partial void ProjetProduitsEditSelected_Execute()
        {
            this.FindControl("Ajouter").ControlAvailable += ((o, e) =>
            {
                Button bouton = e.Control as Button;

                bouton.Content = "Modifier";
            });
            
            selectedProjetProduit = ProjetProduits.SelectedItem;
            selectedProduit = selectedProjetProduit.Produit;
            Produits.SelectedItem = selectedProduit;

            if (Produits.SelectedItem.Item != null)
            {
                Items.SelectedItem = Produits.SelectedItem.Item;
            }
            
            initProduitWindow();

            windowProduit.setEntityName("Modifier Produit");
            windowProduit.ViewEntity();
        }

        void Window_Closed(object sender, EventArgs e)
        {
            ChildWindow window = (ChildWindow)sender;
            
            if (!(window.DialogResult.HasValue))
            {
                foreach (Produit produit in DataWorkspace.ApplicationData.Details.GetChanges().AddedEntities.OfType<Produit>())
                {
                    produit.Details.DiscardChanges();
                }
                foreach (ProjetProduit projetProduit in DataWorkspace.ApplicationData.Details.GetChanges().AddedEntities.OfType<ProjetProduit>())
                {
                    projetProduit.Details.DiscardChanges();
                }
                foreach (ProduitOptionBC produitOptionBC in DataWorkspace.ApplicationData.Details.GetChanges().AddedEntities.OfType<ProduitOptionBC>())
                {
                    produitOptionBC.Details.DiscardChanges();
                }
                foreach (ModeleInsere modeleInsere in DataWorkspace.ApplicationData.Details.GetChanges().AddedEntities.OfType<ModeleInsere>())
                {
                    modeleInsere.Details.DiscardChanges();
                }
                if (ProjetProduits.Any())
                {
                    ProjetProduits.SelectedItem = ProjetProduits.Last();
                    Produits.SelectedItem = ProjetProduits.SelectedItem.Produit;
                }
            }
        }

        /// <summary>
        /// Initializes the Modeles control.
        /// </summary>
        private void initProduitWindow()
        {
            setSelectedInsertionsBC();
            setSelectedOptionsBC();
        }

        /// <summary>
        /// Sets the selected OptionsBC in the Modeles control.
        /// </summary>
        private void setSelectedOptionsBC()
        {
            List<string> selectedOptionsBC = new List<string>();

            if (selectedProjetProduit != null)
            {
                foreach (ProduitOptionBC pOption in selectedProjetProduit.Produit.ProduitOptionsBC)
                {
                    selectedOptionsBC.Add(pOption.OptionBC.Nom);
                }
                
                this.FindControl("Modeles").ControlAvailable += ((o, e) =>
                {
                    ProduitEditor controlProduits = e.Control as ProduitEditor;
                    controlProduits.setOptionsBC(selectedOptionsBC);
                });
            }
        }

        /// <summary>
        /// Sets the selected InsertionsBC in the Modeles control.
        /// </summary>
        private void setSelectedInsertionsBC()
        {
            List<string> selectedInsertionsBC = new List<string>();

            if (selectedProjetProduit != null)
            {
                foreach (ModeleInsere modeleInsere in selectedProjetProduit.Produit.ModelesInseres)
                {
                    selectedInsertionsBC.Add(modeleInsere.Modele.NomComplet);
                }

                this.FindControl("Modeles").ControlAvailable += ((o, e) =>
                {
                    ProduitEditor controlProduits = e.Control as ProduitEditor;
                    controlProduits.setInsertionsBC(selectedInsertionsBC);
                });
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
            string etape = ProjetProperty.EtapeEnCours.ToString();
            result = (etape == EtapeList.DESSIN || etape == EtapeList.PRODUCTION || etape == EtapeList.LIVRE);
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
                FeuilleCommande controlProjet = e.Control as FeuilleCommande;

                controlProjet.setNumProjet(ProjetProperty.NumProjet);
                controlProjet.setProjetProduits(ProjetProduits.Select(
                        pp => 
                        new Tuple<int, string, string, decimal, decimal>(pp.Quantite, pp.Produit.CachedNom, pp.Description, pp.PrixUnitaire, pp.PrixTotal)
                    ).ToList());
            });
        }

    }
}