using LightSwitchApplication.UserCode;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;
using SilverlightCustomControls;
using System;
using System.Linq;
using System.Windows.Controls;

namespace LightSwitchApplication
{
    public partial class NouveauProjet
    {
        private ProjetEtape pe = null;

        partial void NouveauProjet_InitializeDataWorkspace(global::System.Collections.Generic.List<global::Microsoft.LightSwitch.IDataService> saveChangesTo)
        {
            ProjetProperty = new Projet();
        }

        partial void NouveauProjet_Saved()
        {
            Close(false);
            Application.Current.ShowDefaultScreen(ProjetProperty);
        }

        partial void NouveauProjet_Created()
        {
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
                pe = cb.SelectedItem as ProjetEtape;
                ProjetEtapes.SelectedItem = pe;
            }
        }

        private void LoadEtapes(object o, ControlAvailableEventArgs e)
        {
            EtapesControl etapes = e.Control as EtapesControl;
            ComboBox cbEtapes = etapes.GetComboBox();
            cbEtapes.DisplayMemberPath = ProjetEtape.DetailsClass.PropertySetProperties.Etape.Name();
            cbEtapes.ItemsSource = ProjetEtapes.Select(pe => pe).OrderBy(pe => pe.Etape.Ordre).ToList();
            cbEtapes.SelectedItem = ProjetEtapes.First();
        }

        partial void NouveauProjet_Saving(ref bool handled)
        {
            ProjetProperty.EtapeEnCours = pe.Etape;
            ProjetProperty.ProjetEtapes.Single(et => et.Etape == ProjetProperty.EtapeEnCours).DateDebut = DateTime.Now;
        }

        partial void ProjetEtapes_Loaded(bool succeeded)
        {
            if (Etapes == null) Etapes.Load();
            var etapesControl = this.FindControl("Etapes");
            etapesControl.ControlAvailable -= LoadEtapes;
            etapesControl.ControlAvailable += LoadEtapes;
        }
    }
}