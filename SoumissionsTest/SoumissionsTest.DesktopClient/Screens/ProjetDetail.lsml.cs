using System;
using System.Windows.Controls;
using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.LightSwitch;
using System.Linq;
using LightSwitchApplication.UserCode;
using SilverlightCustomControls;
using System.Collections.Generic;
using LightSwitchApplication.UserCode.Shared;
using System.Windows;
using Microsoft.LightSwitch.Threading;
using System.ComponentModel;

namespace LightSwitchApplication
{
    public partial class ProjetDetail
    {
        private ModalWindow windowTags;
        private Button tagsBtn;
        private ListBox tagsList;
        private int nbTagsToDelete = 0;
        private bool deleteTagsClicked = false;
        private ModalWindow windowProduit;
        private ProduitEditor editor;
        private bool loadProduitInfo;
        private Button editorBtn;
        private string editorBtnLabel;
        private ModalWindow windowCommande;
        private FeuilleCommande commande;
        private EtapesControl etapesControl;
        private Tuple<string, string, string, string, List<string>, List<string>> selectedProduitInfo;
        private Tuple<string, int, decimal> selectedProjetProduitInfo;
        private bool isNewProduit = false;
        private List<ProduitProduction> tags;

        partial void ProjetDetail_Created()
        {
            windowProduit = new ModalWindow(this, "NouveauProduit", "Nouveau Produit", editorClosed);
            windowCommande = new ModalWindow(this, "Commande", "Commande");
            windowTags = new ModalWindow(this, "DeleteTags", "Tags à retirer", onClosing: tagsClosing);

            this.FindControl("Modeles").ControlAvailable += ((o, e) =>
            {
                editor = (ProduitEditor)e.Control;
                if (loadProduitInfo)
                {
                    editor.SetProduit(selectedProduitInfo);
                    editor.SetProjetProduit(selectedProjetProduitInfo);
                }
            });

            this.FindControl("AjouterPP").ControlAvailable += ((o, e) =>
            {
                editorBtn = (Button)e.Control;
                editorBtn.Content = editorBtnLabel;
            });

            this.FindControl("DeleteTagsBtn").ControlAvailable += ((o, e) =>
            {
                tagsBtn = (Button)e.Control;
            });

            this.FindControl("ListTags").ControlAvailable += ((o, e) =>
            {
                tagsList = (ListBox)e.Control;
                tagsList.DisplayMemberPath = "Tag";
                tagsList.ItemsSource = tags;
            });

            this.FindControl("Etapes").ControlAvailable += ((o, e) =>
            {
                etapesControl = (EtapesControl)e.Control;
            });

            this.FindControl("Projet").ControlAvailable += ((o, e) =>
            {
                commande = (FeuilleCommande)e.Control;
                commande.SetVenduSelectionChanged(CommandeVenduSelectionChanged);
                commande.SetVenduASource(new List<string>() {
                    ProjetProperty.Ingenieur == null ? string.Empty : ProjetProperty.Ingenieur.Entreprise,
                    ProjetProperty.Entrepreneur == null ? string.Empty : ProjetProperty.Entrepreneur.Entreprise,
                    ProjetProperty.Distributeur == null ? string.Empty : ProjetProperty.Distributeur.Entreprise
                }, ProjetProperty.VenduCie);
            });

            etapesControl.SetEtapeChanged(EtapeChanged);
        }

        private void CommandeVenduSelectionChanged(object o, SelectionChangedEventArgs e)
        {
            ComboBox combo = o as ComboBox;
            if (combo.SelectedItem != null)
            {
                string selected = combo.SelectedItem.ToString();
                if (selected != ProjetProperty.VenduCie)
                    ProjetProperty.VenduCie = selected;
            }
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

        partial void ProjetProduitsEditSelected_CanExecute(ref bool result)
        {
            result = (ProjetProduits.SelectedItem != null);
        }

        partial void SaveProduit_Execute()
        {
            bool validated = false;

            Dispatchers.Main.BeginInvoke(() =>
            {
                validated = editor.Validate();

                if (validated)
                {
                    Details.Dispatcher.BeginInvoke(() =>
                    {
                        ProjetProduit selectedPP = isNewProduit ? ProjetProduits.AddNew() : ProjetProduits.SelectedItem;
                        Produit newProduit = null;

                        Dispatchers.Main.BeginInvoke(() =>
                        {
                            var produitInfo = editor.GetProduit();

                            Details.Dispatcher.BeginInvoke(() =>
                            {
                                newProduit = Produit.CreateProduit(produitInfo, DataWorkspace);
                                Produit existingProduit = newProduit.ExistsOther(DataWorkspace);

                                Dispatchers.Main.BeginInvoke(() =>
                                {
                                    selectedPP.SetProjetProduitInfo(editor.GetProjetProduit());
                                    editor.ResetColors();

                                    // If there's already a Produit like that, discard the newly created identical one.
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
                                        bool needsTagDeletion = false;
                                        if (!DataWorkspace.ApplicationData.Details.GetChanges().Any(c => c.Details.ValidationResults.HasErrors))
                                        {
                                            // If the product changed, delete all tags and create new ones
                                            if (selectedPP.Details.Properties.Produit.IsChanged)
                                            {
                                                // If the product is no longer a model (i.e. Item or Autre), delete tags
                                                if (selectedPP.Produit.Modele == null) {
                                                    selectedPP.DeleteTags();
                                                }
                                                // Otherwise just delete all tags and create new ones
                                                else
                                                    selectedPP.CreateTags(selectedPP.Quantite, true);
                                            }
                                            else
                                            {
                                                // If the quantity changed, check the scenario
                                                if (selectedPP.Details.Properties.Quantite.IsChanged)
                                                {
                                                    // If we add tags, just create them
                                                    if (selectedPP.Details.Properties.Quantite.OriginalValue < selectedPP.Quantite) {
                                                        selectedPP.CreateTags(selectedPP.Quantite);
                                                    }
                                                    // If we remove tags
                                                    else
                                                    {
                                                        // If the quantity is 0, delete all tags
                                                        if (selectedPP.Quantite == 0)
                                                            selectedPP.DeleteTags();
                                                        // Else, have the user pick the tags to delete
                                                        else
                                                        {
                                                            needsTagDeletion = true;
                                                            nbTagsToDelete = selectedPP.Details.Properties.Quantite.OriginalValue - selectedPP.Quantite;
                                                            tags = selectedPP.ProduitsProduction.ToList();
                                                            windowTags.OpenModalWindow();
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        SetSelection();

                                        if (!needsTagDeletion)
                                        {
                                            Dispatchers.Main.BeginInvoke(() =>
                                            {
                                                windowProduit.CloseModalWindow();
                                            });
                                        }
                                    });
                                });
                            });
                        });
                    });
                }
            });
        }

        partial void ProjetProduitsAddAndEditNew_Execute()
        {
            isNewProduit = true;
            editorBtnLabel = "Ajouter";
            loadProduitInfo = false;
            windowProduit.setWindowName("Nouveau Produit");
            windowProduit.OpenModalWindow();
        }

        partial void ProjetProduitsEditSelected_Execute()
        {
            isNewProduit = false;
            editorBtnLabel = "Modifier";
            loadProduitInfo = true;
            windowProduit.setWindowName("Modifier Produit");
            windowProduit.OpenModalWindow();
        }

        private void editorClosed(object sender, EventArgs e)
        {
            Dispatchers.Main.BeginInvoke(() =>
            {
                editor.Clear();
            });
        }

        private void tagsClosing(object sender, CancelEventArgs e)
        {
            if (deleteTagsClicked)
            {
                if (tagsList.SelectedItems.Count == nbTagsToDelete)
                {
                    Details.Dispatcher.BeginInvoke(() =>
                    {
                        ProjetProduits.SelectedItem.DeleteTags(tagsList.SelectedItems.Cast<ProduitProduction>().Select(t => t.Id).ToList());
                        Dispatchers.Main.BeginInvoke(() =>
                        {
                            windowProduit.CloseModalWindow();
                        });
                    });
                }
                else
                {
                    MessageBox.Show("Vous devez choisir " + nbTagsToDelete + " tag" + (nbTagsToDelete > 1 ? "s" : " ") + " à retirer");
                    e.Cancel = true;
                }
                deleteTagsClicked = false;
            }
        }

        /// <summary>
        /// Opens windowCommande.
        /// </summary>
        partial void Commande_Execute()
        {
            ProjetProperty.DateCommande = DateTime.Now;
            windowCommande.OpenModalWindow();
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
            Dispatchers.Main.BeginInvoke(() =>
            {
                ComboBox cbEtapes = etapesControl.GetComboBox();
                cbEtapes.DisplayMemberPath = ProjetEtape.DetailsClass.PropertySetProperties.Etape.Name();
                cbEtapes.ItemsSource = ProjetEtapes.Select(pe => pe).ToList().OrderBy(pe => pe.Etape.Ordre).ToList();
                cbEtapes.SelectedItem = ProjetEtapes.SingleOrDefault(pe => pe.Etape == ProjetProperty.EtapeEnCours);
                cbEtapes.Tag = cbEtapes.SelectedItem;
                ProjetEtapes.SelectedItem = cbEtapes.SelectedItem as ProjetEtape;
            });
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

        partial void ProjetProduits_SelectionChanged()
        {
            SetSelection();
        }

        private void SetSelection()
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

        partial void DeleteTagsBtn_Execute()
        {
            deleteTagsClicked = true;
            windowTags.CloseModalWindow();
        }
    }
}