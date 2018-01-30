using C1.Silverlight.Chart;
using LightSwitchApplication.UserCode;
using LightSwitchApplication.UserCode.Shared;
using Microsoft.LightSwitch.Presentation.Extensions;
using Microsoft.LightSwitch.Presentation.Implementation.Screens;
using Microsoft.LightSwitch.Presentation.Utilities.Internal;
using Microsoft.LightSwitch.Threading;
using SilverlightCustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace LightSwitchApplication
{
    public partial class VendeurDetail
    {
        private Dictionary<string, Etape> stringEtapes;
        private Dictionary<Etape, double[]> etapesWeeklyTotals;
        private Dictionary<Etape, decimal> etapesTimeFrameTotal;
        private Dictionary<Etape, decimal> etapesAtTimeTotal;
        private List<ProjetEtape> projetEtapes;
        private List<Tuple<string, string, decimal, DateTime?, DateTime?>> peData;
        private List<Etape> etapesOnGraph;

        private Axis axeValeur;
        private Axis axeMois;
        private double axeWeeksMin;
        private double axeWeeksMax;

        private DateTime[] fridaysBefore = TimeComparator.getFridaysBeforeNow();
        private IEnumerable<string> stringDates;
        private int nbWeeks;

        private bool initDone = false;
        private bool initChartDone = false;
        private bool projetEtapesLoaded = false;
        private bool initOptionsDone = false;
        private bool dataCalculated = false;

        partial void VendeurDetail_InitializeDataWorkspace(List<Microsoft.LightSwitch.IDataService> saveChangesTo)
        {
            if (!initDone)
            {
                etapesWeeklyTotals = new Dictionary<Etape, double[]>();

                this.FindControl("DepuisDatePicker").ControlAvailable += ((obj, ev) => {
                    DatePicker depuisPicker = ev.Control as DatePicker;
                    depuisPicker.SelectedDate = DateTime.Now.AddMonths(-4);
                });

                this.FindControl("JusquaDatePicker").ControlAvailable += ((obj, ev) => {
                    DatePicker jusquaPicker = ev.Control as DatePicker;
                    jusquaPicker.SelectedDate = DateTime.Now;
                });

                this.FindControl("btnImprimer").ControlAvailable += ((obj, ev) =>
                {
                    BoutonPrint bouton = ev.Control as BoutonPrint;

                    bouton.addClickHandler(Print);
                });

                this.FindControl("OptionsWindow").ControlAvailable += ((obj, ev) =>
                {
                    ChildWindow window = (ChildWindow)ev.Control;
                    window.Closed += OptionsWindow_Closed;
                    window.MinWidth = 400;
                    window.MinHeight = 300;
                });

                this.FindControl("btnOptions").IsEnabled = false;

                initDone = true;
            }
        }

        private void OptionsWindow_Closed(object sender, EventArgs e)
        {
            this.FindControl("OptionsStats").ControlAvailable += ((obj, ev) =>
            {
                OptionsStats options = (OptionsStats)ev.Control;

            });
        }

        partial void Vendeur_Loaded(bool succeeded)
        {
            SetDisplayNameFromEntity(Vendeur);
        }

        partial void EtapesQuery_Loaded(bool succeeded)
        {
            stringEtapes = new Dictionary<string, Etape>();

            foreach (Etape etape in EtapesQuery)
            {
                stringEtapes.Add(etape.Nom, etape);
            }

            etapesOnGraph = EtapesQuery.Where(e => e.AfficheeParDefaut).OrderBy(etape => etape.Ordre).ToList();
            List<string> affichees = EtapesQuery.Where(e => e.AfficheeParDefaut).Select(e => e.Nom).ToList();
            List<string> cachees = EtapesQuery.Where(e => !e.AfficheeParDefaut).Select(e => e.Nom).ToList();

            this.FindControl("OptionsStats").ControlAvailable += ((obj, ev) =>
            {
                if (!initOptionsDone)
                {
                    OptionsStats options = (OptionsStats)ev.Control;

                    options.Init(affichees, cachees);
                    options.addOKHandler(OptionsHandler);

                    initOptionsDone = true;
                }
            });
        }

        private decimal getEtapeValueTimeFrame(Etape etape)
        {
            var totalEtape = (from p in projetEtapes
                              where p.Etape.Equals(etape) && TimeComparator.dateIsBetween(p.DateDebut, axeWeeksMin.FromOADate(), axeWeeksMax.FromOADate())
                              select p.Estime).Sum();

            return totalEtape;
        }

        private decimal getTotalEtapeAtTime(Etape etape, DateTime time)
        {
            decimal totalEtape = 0;

            totalEtape = (from p in projetEtapes
                          where p.Etape.Equals(etape) && TimeComparator.isActiveAtTime(p.DateDebut, p.DateFin, time)
                          select p.Estime).Sum();

            return totalEtape;
        }

        private double getTotalEtapeWeek(string etape, DateTime dateContenue, string vendeur)
        {
            double totalEtape = 0;

            totalEtape = Convert.ToDouble((from p in peData
                                           where p.Item2.Equals(etape) && vendeur.Equals(p.Item1) && TimeComparator.containsDay(dateContenue, p.Item4, p.Item5)
                                           select p.Item3).Sum());

            return totalEtape;
        }

        partial void Vendeur_Changed()
        {
            SetDisplayNameFromEntity(Vendeur);
        }

        partial void VendeurDetail_Saved()
        {
            SetDisplayNameFromEntity(Vendeur);
        }

        partial void ProjetEtapesQuery_Loaded(bool succeeded)
        {
            projetEtapes = ProjetEtapesQuery.Where(p => Vendeur.Equals(p.Projet.Vendeur)).ToList();
        }

        private void initChartData()
        {
            if (projetEtapes == null)
            {
                ProjetEtapesQuery.Load();
            }

            axeWeeksMin = DateTime.Now.AddMonths(-4).ToOADate();
            axeWeeksMax = DateTime.Now.ToOADate();

            fridaysBefore = TimeComparator.getFridaysBeforeNow();
            nbWeeks = fridaysBefore.Length;

            etapesWeeklyTotals = new Dictionary<Etape, double[]>();

            peData = projetEtapes
                .Select(pe => new Tuple<string, string, decimal, DateTime?, DateTime?>(pe.Projet.Vendeur.CachedNom, pe.Etape.ToString(), pe.Estime, pe.DateDebut, pe.DateFin))
                .ToList();

            foreach (Etape etape in EtapesQuery)
            {
                double[] weekly = new double[fridaysBefore.Length];
                for (int i = 0; i < fridaysBefore.Length; i++)
                {
                    weekly[i] = getTotalEtapeWeek(etape.Nom, fridaysBefore[i], Vendeur.CachedNom);
                }

                etapesWeeklyTotals.Add(etape, weekly);
            }

            if (!dataCalculated)
            {
                CalculateData();
            }

            initChart();
        }

        private void initChart()
        {
            if (!initChartDone)
            {
                this.FindControl("Tendance").ControlAvailable += ((o, e) => {
                    C1Chart graphique = e.Control as C1Chart;

                    graphique.BeginUpdate();
                    graphique.Reset(true);

                    graphique.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);

                    graphique.ChartType = ChartType.LineSymbols;

                    List<XYDataSeries> series = new List<XYDataSeries>();
                    stringDates = TimeComparator.getStringDates(fridaysBefore);

                    axeMois = new Axis();
                    axeMois.Name = "Temps";
                    axeMois.IsTime = true;
                    axeMois.Title = "Temps (Mois)";
                    axeMois.AnnoAngle = 60;
                    axeMois.AxisType = AxisType.X;
                    axeMois.Min = axeWeeksMin;
                    axeMois.Max = axeWeeksMax;

                    axeValeur = new Axis();
                    axeValeur.Name = "Valeur";
                    axeValeur.IsTime = false;
                    axeValeur.Title = "Valeur ($)";
                    axeValeur.AnnoAngle = 0;
                    axeValeur.AxisType = AxisType.Y;

                    graphique.View.AxisX = axeMois;
                    graphique.View.AxisY = axeValeur;

                    this.FindControl("Legende").ControlAvailable += ((obj, ev) => {
                        C1ChartLegend legende = ev.Control as C1ChartLegend;
                        legende.Title = "ÉTAPES:";
                        legende.Chart = graphique;
                    });

                    graphique.EndUpdate();
                });

                UpdateChart();
                initChartDone = true;
            }
        }

        private void UpdateChart()
        {
            this.FindControl("Tendance").ControlAvailable += ((o, e) => {
                C1Chart graphique = e.Control as C1Chart;

                graphique.BeginUpdate();

                graphique.Data.Children.Clear();

                ChartData data = new ChartData();
                Size symbolSize = new Size(4, 4);

                graphique.CustomPalette = etapesOnGraph.Select(etape => ClientUtilities.HSLColor(etape.Hue, 0.8, 0.4));

                foreach (Etape etape in etapesOnGraph)
                {
                    XYDataSeries serie = new XYDataSeries();
                    serie.ValuesSource = etapesWeeklyTotals[etape];
                    serie.XValuesSource = stringDates;
                    string value = etapesAtTimeTotal[etape] == 0M ? "0.00$" : string.Format("{0:#,###.00$}", decimal.Round(etapesAtTimeTotal[etape], 2));
                    serie.Label = etape.Nom + ": " + value;
                    data.Children.Add(serie);
                    serie.SymbolSize = symbolSize;
                }

                graphique.Data = data;

                graphique.EndUpdate();
            });
        }

        partial void DepuisDate_Changed()
        {
            if (DepuisDate.HasValue)
            {
                if (DepuisDate.Value.ToOADate() != axeWeeksMin)
                {
                    axeWeeksMin = DepuisDate.Value.ToOADate();
                    CalculateData();
                    this.SetChartMin(axeWeeksMin);
                }
            }
        }

        partial void JusquaDate_Changed()
        {
            if (JusquaDate.HasValue)
            {
                if (JusquaDate.Value.ToOADate() != axeWeeksMax)
                {
                    axeWeeksMax = JusquaDate.Value.ToOADate();
                    CalculateData();
                    this.SetChartMax(axeWeeksMax);
                }
            }
        }

        private void CalculateData()
        {
            etapesTimeFrameTotal = new Dictionary<Etape, decimal>();

            foreach (Etape etape in etapesOnGraph)
            {
                etapesTimeFrameTotal.Add(etape, getEtapeValueTimeFrame(etape));
            }

            etapesAtTimeTotal = new Dictionary<Etape, decimal>();
            decimal total = 0M;

            foreach (Etape etape in etapesOnGraph)
            {
                decimal value = getTotalEtapeAtTime(etape, axeWeeksMax.FromOADate());
                total += value;
                etapesAtTimeTotal.Add(etape, value);
            }

            this.setControlMoneyValue("txtTotal", "TOTAL:   ", total);

            List<string> selectedEtapes = etapesOnGraph.Select(etape => etape.Nom).ToList();

            if (DepuisDate.HasValue && JusquaDate.HasValue && selectedEtapes.Contains(EtapeList.PRODUCTION))
            {
                if (selectedEtapes.Contains(EtapeList.SPECIFICATION))
                    this.SetControlRatioValue("txtRatioProdSpec", "Ratio Spec / Prod:   ",
                        etapesTimeFrameTotal[stringEtapes[EtapeList.SPECIFICATION]], etapesTimeFrameTotal[stringEtapes[EtapeList.PRODUCTION]]);
                if (selectedEtapes.Contains(EtapeList.SOUMISSION))
                    this.SetControlRatioValue("txtRatioProdSoum", "Ratio Soum / Prod:   ",
                        etapesTimeFrameTotal[stringEtapes[EtapeList.SOUMISSION]], etapesTimeFrameTotal[stringEtapes[EtapeList.PRODUCTION]]);
                if (selectedEtapes.Contains(EtapeList.PERDU))
                    this.SetControlRatioValue("txtRatioPerduSoum", "Ratio Perdu / Soum:   ",
                        etapesTimeFrameTotal[stringEtapes[EtapeList.PERDU]], etapesTimeFrameTotal[stringEtapes[EtapeList.SOUMISSION]]);
            }
        }

        public void Print(object sender, RoutedEventArgs e)
        {
            BoutonPrint button = (e.OriginalSource as Button).Parent.GetParent() as BoutonPrint;
            button.setPrintArg( button.FindVisualParentByType<ScreenRoot>() );
            button.print();
        }

        partial void Stats_Execute()
        {
            Details.Dispatcher.BeginInvoke(() =>
            {
                if (!projetEtapesLoaded)
                {
                    initChartData();
                    projetEtapesLoaded = true;
                }

                Dispatchers.Main.BeginInvoke(() =>
                {
                    if (!projetEtapesLoaded)
                    {
                        CalculateData();
                    }

                    bool statsVisible = this.FindControl("GroupStats").IsVisible;
                    this.FindControl("GroupStats").IsVisible = !statsVisible;
                    this.FindControl("btnOptions").IsEnabled = !statsVisible;
                });
            });
        }

        partial void Options_Execute()
        {
            this.OpenModalWindow("OptionsWindow");
        }

        public void OptionsHandler(object sender, RoutedEventArgs e)
        {
            bool options = true;
            this.FindControl("OptionsStats").ControlAvailable += ((obj, ev) =>
            {
                if (options)
                {
                    OptionsStats control = (OptionsStats)ev.Control;

                    Tuple<List<string>, List<string>> rawLists = control.GetEtapes();
                    List<string> affichees = rawLists.Item1;
                    List<string> cachees = rawLists.Item2;

                    Tuple<List<Etape>, List<Etape>> lists = new Tuple<List<Etape>, List<Etape>>(new List<Etape>(), new List<Etape>());
                    lists.Item1.AddRange(EtapesQuery.Where(etape => affichees.Contains(etape.Nom)));
                    lists.Item2.AddRange(EtapesQuery.Where(etape => cachees.Contains(etape.Nom)));

                    etapesOnGraph = lists.Item1.OrderBy(etape => etape.Ordre).ToList();
                    CalculateData();
                    UpdateChart();

                    this.CloseModalWindow("OptionsWindow");

                    options = false;
                }
            });
        }
    }
}