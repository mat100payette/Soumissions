namespace LightSwitchApplication
{
    public partial class ProjetProduit
    {
        partial void PrixTotal_Compute(ref decimal result)
        {
            result = PrixUnitaire * Quantite;
        }

        partial void ProjetProduit_Created()
        {
            Quantite = 1;
            Tag = string.Empty;
            E_Accept = false;
            E_Approb = false;
            E_D100 = false;
            E_D85 = false;
            E_Diagramme = false;
            E_Implem = false;
            E_Kickoff = false;
            E_Refreg = false;
            E_SeqCtrl = false;
            ReadyProd = false;
            Note = string.Empty;
        }

        private void UpdateReadyProd()
        {
            ReadyProd = (E_Accept && E_Approb && E_D100 && E_D85 && E_Diagramme && E_Implem && E_Kickoff && E_Refreg && E_SeqCtrl);
        }

        partial void E_Accept_Changed()
        {
            UpdateReadyProd();
        }

        partial void E_Approb_Changed()
        {
            UpdateReadyProd();
        }

        partial void E_D100_Changed()
        {
            UpdateReadyProd();
        }

        partial void E_Refreg_Changed()
        {
            UpdateReadyProd();
        }

        partial void E_SeqCtrl_Changed()
        {
            UpdateReadyProd();
        }

        partial void E_Implem_Changed()
        {
            UpdateReadyProd();
        }

        partial void E_Diagramme_Changed()
        {
            UpdateReadyProd();
        }

        partial void E_D85_Changed()
        {
            UpdateReadyProd();
        }

        partial void E_Kickoff_Changed()
        {
            UpdateReadyProd();
        }
    }
}