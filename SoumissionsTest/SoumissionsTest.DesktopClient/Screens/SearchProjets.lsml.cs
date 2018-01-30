using LightSwitchApplication.UserCode.Shared;
using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.LightSwitch.Threading;
using SilverlightCustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LightSwitchApplication
{
    public partial class SearchProjets
    {

        partial void ProjetsFilterQueryAddAndEditNew_CanExecute(ref bool result)
        {
            result = true;
        }

        partial void ProjetsFilterQueryAddAndEditNew_Execute()
        {
            Projet newProjet = DataWorkspace.ApplicationData.Projets.AddNew();
            newProjet.Nom = "_Nouveau Projet_";
            newProjet.EtapeEnCours = Etapes.SingleOrDefault(e => e.Ordre == Etapes.Min(et => et.Ordre));
            ProjetsFilterQuery.SelectedItem = newProjet;
            this.OpenModalWindow("NouveauProjet");
        }

        partial void SearchProjets_Created()
        {

            bool initNouveauProjet = true;
            bool initFilterWindow = true;
            bool initFilter = true;

            this.FindControl("NouveauProjet").ControlAvailable += ((obj, ev) => 
            {
                if (initNouveauProjet)
                {
                    ChildWindow window = (ChildWindow)ev.Control;
                    window.Closed += NouveauProjet_Closed;

                    initNouveauProjet = false;
                }
            });

            this.FindControl("FilterWindow").ControlAvailable += ((obj, ev) =>
            {
                if (initFilterWindow)
                {
                    ChildWindow window = (ChildWindow)ev.Control;
                    window.MinWidth = 400;
                    window.MinHeight = 300;

                    initFilterWindow = false;
                }
            });

            this.FindControl("Filter").ControlAvailable += ((obj, ev) =>
            {
                if (initFilter)
                {
                    ProjetFilterControl filtre = (ProjetFilterControl)ev.Control;
                    filtre.addFilterHandler(FilterHandler);

                    IEnumerable<string> allContacts;

                    Details.Dispatcher.BeginInvoke(() =>
                    {
                        allContacts = DataWorkspace.ApplicationData.ProjetsQuery().Execute().Where(p => p.Contact != null).Select(p => p.Contact).Distinct().OrderBy(c => c);

                        Dispatchers.Main.BeginInvoke(() =>
                        {
                            filtre.SetContacts(allContacts);
                        });
                    });

                    initFilter = false;
                }
            });
        }

        public void FilterHandler(object sender, RoutedEventArgs e)
        {
            bool filter = true;

            this.FindControl("Filter").ControlAvailable += ((obj, ev) =>
            {
                if (filter)
                {
                    ProjetFilterControl filtre = (ProjetFilterControl)ev.Control;

                    string[] filterStrings = filtre.GetFilter();

                    Num = filterStrings[0];
                    Nom = filterStrings[1];
                    Etape = filterStrings[2];
                    Vendeurs = filterStrings[3];
                    Ingenieurs = filterStrings[4];
                    Entrepreneurs = filterStrings[5];
                    Distributeurs = filterStrings[6];
                    Contacts = filterStrings[7];
                    Produit = filterStrings[8];

                    this.CloseModalWindow("FilterWindow");

                    filter = false;
                }
            });
        }

        void NouveauProjet_Closed(object sender, EventArgs e)
        {
            ChildWindow window = (ChildWindow)sender;
            Projet nouveauProjet = ProjetsFilterQuery.SelectedItem;

            if (!(window.DialogResult.HasValue))
            {
                Details.Dispatcher.BeginInvoke(() =>
                {
                    foreach (ProjetEtape projetEtape in DataWorkspace.ApplicationData.Details.GetChanges().AddedEntities.OfType<ProjetEtape>())
                    {
                        projetEtape.Details.DiscardChanges();
                    }

                    nouveauProjet.Details.DiscardChanges();
                });
            }
            else
            {
                Details.Dispatcher.BeginInvoke(() =>
                {
                    if (!nouveauProjet.Details.ValidationResults.Errors.Any())
                    {
                        DataWorkspace.ApplicationData.SaveChanges();
                    }
                });
            }
        }

        partial void SaveProjet_Execute()
        {
            Projet nouveauProjet = ProjetsFilterQuery.SelectedItem;

            Details.Dispatcher.BeginInvoke(() =>
            {
                if (!nouveauProjet.Details.ValidationResults.Errors.Any())
                {
                    Dispatchers.Main.BeginInvoke(() =>
                    {
                        this.FindControl("NouveauProjet").ControlAvailable += ((obj, ev) =>
                        {
                            ChildWindow window = (ChildWindow)ev.Control;
                            window.DialogResult = true;
                        });
                        this.CloseModalWindow("NouveauProjet");
                    });
                }
                else
                {
                    this.ShowMessageBox("Le Projet contient des erreurs.");
                }
            });
        }

        partial void Filtrer_Execute()
        {
            this.OpenModalWindow("FilterWindow");
        }

        partial void ProjetsFilterQueryDeleteSelected_CanExecute(ref bool result)
        {
            result = true;
        }

        partial void ProjetsFilterQueryDeleteSelected_Execute()
        {
            MessageBoxResult result = this.ShowMessageBox("Voulez-vous vraiment effacer ce projet ?", "Attention", MessageBoxOption.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                ProjetsFilterQuery.SelectedItem.Delete();
            }
        }
    }
}