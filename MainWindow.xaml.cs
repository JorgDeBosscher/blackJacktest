using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace blackJack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // generators voor kaartnr
        Random kaartNummer = new Random();
        //stringbuilders
        StringBuilder speler = new StringBuilder();
        StringBuilder bank = new StringBuilder();
        StringBuilder lijst = new StringBuilder();  
        //variabellen
        bool isSpeler = true;
        int totaalSpeler = 0;
        int totaalBank = 0;
        int verborgenTotaal = 0;
        string verborgenKaart;
        string kaartmethode;
        double inzet;
        double kapitaalSpeler;
        // images
        string kaartImgSpeler;
        string kaartImgBank;
        // muziek
        MediaPlayer achtergrond = new MediaPlayer();
        // aantal azen
        int aantalAzenSpeler = 0;
        int aantalAzenBank = 0;
        // dispatcher voor timer
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer timer2 = new DispatcherTimer();
        private DispatcherTimer timer3 = new DispatcherTimer();
        private DispatcherTimer tijd = new DispatcherTimer();
        // aantalkaarten lijst
        int lijstwaarde = 51;
        int kaartnummer;

        //uniek deck maken
        List<Kaart> deck = new List<Kaart> ();
        private List<string> kaartSoortLijst = new List<string>() { "harten_", "klaveren_", "ruiten_", "schoppen_" };
        private List<string> naamLijst = new List<string>() { "aas", "2", "3", "4", "5", "6", "7", "8", "9", "10", "boer", "dame", "koning" };
        private List<int> waardeLijst = new List<int>() { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };
        private List<string> rondeNummers = new List<string>() { "1","2","3","4","5","6","7","8","9","10"};
       private List<string> rondeGegevens = new List<string>() { "","","","","","","","","", ""};
        private List<string> historieklijst = new List<string>() { "", "", "", "", "", "", "", "", "", "" };
        // spelerhand
        private List<Kaart> spelerHand = new List<Kaart> ();
        //bank hand
        private List<Kaart> bankHand = new List<Kaart>();
            
            
         

        
       public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0,0,2);
            timer2.Interval = new TimeSpan(0,0,1);
            timer3.Interval = new TimeSpan(0,0,1);
            timer.Tick += Timer_TickSpeler;
            timer2.Tick += Timer_TickBank;
            timer3.Tick+= Timer_TickBankStand;
            MaakDeck();

            tijd.Tick += Tijd_Tick;
            tijd.Interval = new TimeSpan (0,0,1);
            tijd.Start();



        }

        private void Tijd_Tick(object sender, EventArgs e)
        {
            LbTijd.Content = DateTime.Now.ToLongTimeString();
        }

        public void MaakDeck()
        {
            string kaartSoortD;
            string naam;
            int waarde;
            int teller1 = 0;
            for (int i = 0; i < 4; i++)
            {

                for (int j = 0; j < 13; j++)
                {
                    deck.Add(new Kaart()
                    {
                        kaartsoortd = kaartSoortLijst[i],
                        naam = naamLijst[j],
                        waarde = waardeLijst[j]
                    });
                    kaartSoortD = deck[teller1].kaartsoortd.ToString();
                    naam = deck[teller1].naam.ToString();
                    waarde = deck[teller1].waarde;
                    
                    
                    teller1++;
                }
                

            }

        }

        private void ControleHoeveelheiKaarten()
        {
            if (lijstwaarde<= 0)
            {   
                MaakDeck();
                lijstwaarde = 51;          
                MessageBox.Show("Het deck is(bijna) op \nvanaf nu gebuiken we een nieuw deck.", "Kaarten op", MessageBoxButton.OK, MessageBoxImage.Error);
               
               
            }



        }
       private void GeefSpelerKaart()
        {
            kaartnummer = kaartNummer.Next(0, lijstwaarde);
            kaartmethode = $"{deck[kaartnummer].kaartsoortd}{deck[kaartnummer].naam}";
            spelerHand.Add(new Kaart()
            {
                kaartsoortd = deck[kaartnummer].kaartsoortd,
                naam = deck[kaartnummer].naam,
                waarde = deck[kaartnummer].waarde
            });
            lijstwaarde--;
            speler.AppendLine(kaartmethode);
            TotaalSpeler();
            ControleAzenSpeler();
            AasWaardeAanpassenSpeler();
            deck.RemoveAt(kaartnummer);
        }
       private void GeefBankKaart()
        {
            kaartnummer = kaartNummer.Next(0, lijstwaarde);

            kaartmethode = $"{deck[kaartnummer].kaartsoortd}{deck[kaartnummer].naam}";

            bankHand.Add(new Kaart()
            {
                kaartsoortd = deck[kaartnummer].kaartsoortd,
                naam = deck[kaartnummer].naam,
                waarde = deck[kaartnummer].waarde

            });
            lijstwaarde--;
            bank.AppendLine(kaartmethode);
            TotaalBank();
            VerborgenTotaal();
            ControleAzenBank();
            deck.RemoveAt(kaartnummer);
        }
        private void GeefBankVerborgenKaart()
        {
            kaartnummer = kaartNummer.Next(0, lijstwaarde);
            kaartmethode = $"{deck[kaartnummer].kaartsoortd}{deck[kaartnummer].naam}";
            bankHand.Add(new Kaart()
            {
                kaartsoortd = deck[kaartnummer].kaartsoortd,
                naam = deck[kaartnummer].naam,
                waarde = deck[kaartnummer].waarde
            });
            lijstwaarde--;
            VerborgenTotaal();
            ControleAzenBank();
            verborgenKaart = kaartmethode;
            deck.RemoveAt(kaartnummer);
        }
        void TotaalSpeler()
        {
                totaalSpeler += deck[kaartnummer].waarde;
        }
        private void VertragerStand()
        {
            timer3.Start();
        }
        private void Timer_TickBankStand(object sender, EventArgs e)
        {
            ControleHoeveelheiKaarten();
            if (verborgenTotaal < 17)
            {

                    
                    GeefBankKaart();
                AantalKaarten.Content = lijstwaarde +1;
                ControleAzenBank();
                    KaartBankTonen();
                    AasWaardeAanpassenBank();
                    ImageConverterenBank();
                     LblBank.Content = verborgenTotaal;

                
               
            }
            else
            {

                VerliesHit();
                VerliesStand();
                Winst();
                 Push();
                timer3.Stop();


            }

            //VerborgenKaartBankTonen();
            
            
        }
        private void Timer_TickBank(object sender, EventArgs e)
        {
            timer2.Stop();

            //2de kaart
           ControleHoeveelheiKaarten();
           GeefBankKaart();
            AantalKaarten.Content = lijstwaarde+1;
            ControleAzenBank();
           ImageConverterenBank();
           KaartBankTonen();
           BankKaartTonen();
           AasWaardeAanpassenBank();
            
            
            
        }
        private void VertragerBank()
        {
            timer2.Start();
        }
        private void VertragerSpeler()
        {
            timer.Start();
        }
        private void Timer_TickSpeler(object sender, EventArgs e)
        {
            timer.Stop();
            ControleHoeveelheiKaarten();
            GeefSpelerKaart();
            AantalKaarten.Content = lijstwaarde + 1;
            //ControleAzenSpeler();
            ImageConverterenSpeler();
            KaartspelerTonen();
            SpelerKaartTonen();
            AasWaardeAanpassenSpeler();

            
            
        }
        private int ControleAzenBank()
        {
            if (deck[kaartnummer].naam == "aas")
            {
                aantalAzenBank++;

                return aantalAzenBank;
            }
            return aantalAzenBank;
        }
        private int AasWaardeAanpassenBank()
        {
            if (verborgenTotaal > 21 && aantalAzenBank > 0)
            {
                verborgenTotaal -= 10;
                aantalAzenBank--;
                return verborgenTotaal;
            }
            else
            {
                return verborgenTotaal;
            }
        }
        private int ControleAzenSpeler()
        {
            if (deck[kaartnummer].naam =="aas")
            {
                aantalAzenSpeler++;

                return aantalAzenSpeler;
            }
            return aantalAzenSpeler;
        }
        private int AasWaardeAanpassenSpeler()
        {
            if (totaalSpeler > 21 && aantalAzenSpeler > 0)
            {
                totaalSpeler -= 10;
                aantalAzenSpeler--;
                return totaalSpeler;
            }
            else
            {
                return totaalSpeler;
            }
        }
        private void BtnDeel_Click(object sender, RoutedEventArgs e)
        {
            SLInzet.IsEnabled = false;
            NieuweRonde();
            VBBank.Visibility = Visibility.Visible;
            VBSpeler.Visibility = Visibility.Visible;
            //knoppen disabelen en enablen
            BtnDeel.IsEnabled = false;
            BtnHit.IsEnabled = true;
            BtnStand.IsEnabled = true;
            DubbeleInzetZichtbaar();
            // speler kaarten oproepen + totaal aanmaken
            ControleHoeveelheiKaarten();
            GeefSpelerKaart();
            AantalKaarten.Content = lijstwaarde + 1;
            ImageConverterenSpeler();
            KaartspelerTonen();
            //TotaalSpeler();
            AasWaardeAanpassenSpeler();
            SpelerKaartTonen();
            VertragerSpeler();
            // kaarten bank maken            
            VertragerBank();
            //verborgen kaart
            ControleHoeveelheiKaarten();
            GeefBankVerborgenKaart();
            AantalKaarten.Content = lijstwaarde + 1;
            
            
            
        }
        private void BtnHit_Click(object sender, RoutedEventArgs e)
        {
            //kaart maker
            ControleHoeveelheiKaarten();
            GeefSpelerKaart();
            AantalKaarten.Content = lijstwaarde + 1;
            ImageConverterenSpeler();
            KaartspelerTonen();
            AasWaardeAanpassenSpeler();
            //tekstspeleraanpassen
            SpelerKaartTonen();
            // kijken op bust
            VerliesHit();

            
        }
        private void BtnStand_Click(object sender, RoutedEventArgs e)
        {

            ControleHoeveelheiKaarten();
            VertragerStand();
            SpeelKnoppenInactief();
            //bank kaarten maken          
            LblBank.Content = verborgenTotaal;
            TxtBank.Text = $"{verborgenKaart}\n{bank}";
            VerborgenKaartBankTonen();
            
           
        }
        private void BtnNieuwSpel_Click(object sender, RoutedEventArgs e)
        {
            BtnNieuwSpel.Visibility = Visibility.Hidden;
            kapitaalSpeler = 100;
            NieuweRonde();
            TxtKapitaalSpeler.Text = kapitaalSpeler.ToString();
            TxtInzetSpeler.Text = "0";
            SLInzet.IsEnabled = true;
        }
        private void KapitaalControle()
        {
            if (kapitaalSpeler <= 0)
            {
                MessageBox.Show("Je geld is op \nom opnieuw te spelen druk nieuw spel", "out of money", MessageBoxButton.OK, MessageBoxImage.Error);
                BtnNieuwSpel.Visibility = Visibility.Visible;
                BtnNieuwSpel.IsEnabled = true;
                SLInzet.IsEnabled = false;
            }
        }
        private void NieuweRonde()
        {
            ImageSpeler.Source = new BitmapImage(new Uri("/assets/achterzijde.png", UriKind.RelativeOrAbsolute));
            ImageVerborgeKaartBank.Source = new BitmapImage(new Uri("/assets/achterzijde.png", UriKind.RelativeOrAbsolute));
            ImageKaartBank.Source = new BitmapImage(new Uri("/assets/achterzijde.png", UriKind.RelativeOrAbsolute));
            TxtBank.Text = String.Empty;
            TxtSpeler.Text = String.Empty;
            LblBank.Content = 0;
            LblSpeler.Content = 0;
            LblResultaat.Content = " ";
            totaalBank = 0;
            totaalSpeler = 0;
            verborgenTotaal = 0;
            inzet = 0;
            aantalAzenBank = 0;
            aantalAzenSpeler = 0;
            speler.Clear();
            bank.Clear();
            bankLB.Items.Clear();
            SpelerLB.Items.Clear();
            spelerHand.Clear();
            bankHand.Clear();
            ImageSpelerDubbelDown.Visibility= Visibility.Hidden;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kapitaalSpeler = 100;
            BtnNieuwSpel.Visibility = Visibility.Hidden;
            DubbeleInzetOnzichtbaar();
            TxtKapitaalSpeler.Text = kapitaalSpeler.ToString();
            TxtInzetSpeler.Text = "0";
            BtnDeel.IsEnabled = false;
            VWTekstSlider.Visibility = Visibility.Visible;
            //muziek
            achtergrond.Open(new Uri(@"../../assets/achtergrondmuziek.mp3", UriKind.RelativeOrAbsolute));
            achtergrond.Play();
        }
        private void GeldAanpassen()
        {
            TxtKapitaalSpeler.Text = $"{kapitaalSpeler}";
            TxtInzetSpeler.Text = "0";
        }

        

        private void Winst()
        {
            kapitaalSpeler = Convert.ToDouble(TxtKapitaalSpeler.Text);
            inzet = Convert.ToDouble(TxtInzetSpeler.Text);
            if (verborgenTotaal > 21)
            {
                TxtBank.Text = $"{verborgenKaart}\n{bank}";
                LblResultaat.Content = "Gewonnen";
                LblResultaat.Foreground = Brushes.Green;
                LblBank.Content = verborgenTotaal;
                SLInzet.IsEnabled = true;
                kapitaalSpeler = kapitaalSpeler + (inzet * 2);
                LbLaatsteSpel.Content= $"+{inzet*2} - {totaalSpeler} / {verborgenTotaal}";
                GeldAanpassen();
                DubbeleInzetOnzichtbaar();
                SpeelKnoppenInactief();
                
                VWTekstSlider.Visibility = Visibility.Visible;
                ControleHoeveelheiKaarten();
                string tijdelijk = $"+{inzet * 2} - {totaalSpeler} / {verborgenTotaal}";
                rondeGegevens.Insert(0, tijdelijk);
                rondeGegevens.RemoveAt(10);

            }
            else if (totaalSpeler > verborgenTotaal )
            {
                TxtBank.Text = $"{verborgenKaart}\n{bank}";
                LblResultaat.Content = "Gewonnen";
                LblResultaat.Foreground = Brushes.Green;
                LblBank.Content = verborgenTotaal;
                SLInzet.IsEnabled = true;
                kapitaalSpeler = kapitaalSpeler + (inzet * 2);
                LbLaatsteSpel.Content = $"+{inzet * 2} - {totaalSpeler} / {verborgenTotaal}";
                GeldAanpassen();
                DubbeleInzetOnzichtbaar();
                SpeelKnoppenInactief();
                
                VWTekstSlider.Visibility = Visibility.Visible;
                ControleHoeveelheiKaarten();
                string tijdelijk = $"+{inzet * 2} - {totaalSpeler} / {verborgenTotaal}";
                rondeGegevens.Insert(0,tijdelijk);
                rondeGegevens.RemoveAt(10);

            }


        }
        private void Push()
        {
            inzet = Convert.ToDouble(TxtInzetSpeler.Text);
            if (totaalSpeler == verborgenTotaal)
            {
                TxtBank.Text = $"{verborgenKaart}\n{bank}";
                LblResultaat.Content = "Push";
                LblResultaat.Foreground = Brushes.Black;
                LblBank.Content = verborgenTotaal;
                kapitaalSpeler = Math.Floor(kapitaalSpeler + inzet);
                LbLaatsteSpel.Content = $"Push {inzet} - {totaalSpeler} / {verborgenTotaal}";

                GeldAanpassen();
                DubbeleInzetOnzichtbaar();
                SLInzet.IsEnabled = true;
                VWTekstSlider.Visibility = Visibility.Visible;
                ControleHoeveelheiKaarten();

                string tijdelijk = $"Push {inzet} - {totaalSpeler} / {verborgenTotaal}";
                rondeGegevens.Insert(0, tijdelijk);
                rondeGegevens.RemoveAt(10);

            }
        }
        private void VerliesHit()
        {
            inzet = Convert.ToDouble(TxtInzetSpeler.Text);
            if (totaalSpeler > 21)
            {
                // tekst verloren
                LblResultaat.Content = "Verloren";
                LblResultaat.Foreground = Brushes.Red;
                // knoppen terug gezet
                SpeelKnoppenInactief();
                kapitaalSpeler = Convert.ToDouble(TxtKapitaalSpeler.Text);
                LbLaatsteSpel.Content = $"-{inzet} - {totaalSpeler} / {verborgenTotaal}";

                SLInzet.IsEnabled = true;
                DubbeleInzetOnzichtbaar();
                KapitaalControle();
                VWTekstSlider.Visibility = Visibility.Visible;
                ControleHoeveelheiKaarten();


                string tijdelijk = $"-{inzet} - {totaalSpeler} / {verborgenTotaal}";
                rondeGegevens.Insert(0, tijdelijk);
                rondeGegevens.RemoveAt(10);


            }
        }
        private void VerliesStand()
        {
            inzet = Convert.ToDouble(TxtInzetSpeler.Text);
            if (totaalSpeler < verborgenTotaal)
            {
                TxtBank.Text = $"{verborgenKaart}\n{bank}";
                LblResultaat.Content = "verloren";
                LblResultaat.Foreground = Brushes.Red;
                LblBank.Content = verborgenTotaal;
                kapitaalSpeler = Convert.ToDouble(TxtKapitaalSpeler.Text);
                DubbeleInzetOnzichtbaar();
                LbLaatsteSpel.Content = $"-{inzet} - {totaalSpeler} / {verborgenTotaal}";

                KapitaalControle();
                SLInzet.IsEnabled = true;
                VWTekstSlider.Visibility = Visibility.Visible;
                ControleHoeveelheiKaarten();

                string tijdelijk = $"-{inzet} - {totaalSpeler} / {verborgenTotaal}";
                rondeGegevens.Insert(0, tijdelijk);
                rondeGegevens.RemoveAt(10);


            }
        }
        private void SpeelKnoppenInactief()
        {
            BtnDeel.IsEnabled = false;
            BtnHit.IsEnabled = false;
            BtnStand.IsEnabled = false;
        }
        private void SpelerKaartTonen()
        {
            TxtSpeler.Text = speler.ToString();
            LblSpeler.Content = totaalSpeler;
        }
       private void VerborgenTotaal()
        {
            verborgenTotaal += deck[kaartnummer].waarde;
        }
        private void TotaalBank()
        {
            totaalBank += deck[kaartnummer].waarde;
        }
        private void BankKaartTonen()
        {
            TxtBank.Text = bank.ToString();
            LblBank.Content = totaalBank;
        }
        private void DubbeleInzetZichtbaar()
        {
            BtnDubbelInzet.Visibility = Visibility.Visible;
            BtnDubbelInzet.IsEnabled = true;
        }
        private void DubbeleInzetOnzichtbaar()
        {
            BtnDubbelInzet.Visibility = Visibility.Hidden;
            BtnDubbelInzet.IsEnabled = false;
        }
        private void DubbeleInzet()
        {
            kapitaalSpeler = Convert.ToDouble(TxtKapitaalSpeler.Text);
            inzet = Convert.ToDouble(TxtInzetSpeler.Text);
            kapitaalSpeler = kapitaalSpeler - inzet;
            inzet *= 2;
            TxtInzetSpeler.Text = inzet.ToString();
            TxtKapitaalSpeler.Text = kapitaalSpeler.ToString();
            

        }
        private void BtnDubbelInzet_Click(object sender, RoutedEventArgs e)
        {
            if (kapitaalSpeler > (Convert.ToDouble(TxtInzetSpeler.Text))*2)
            {
                ImageSpelerDubbelDown.Visibility = Visibility.Visible;
                BtnDeel.IsEnabled= false;
                BtnHit.IsEnabled = false;
                BtnStand.IsEnabled = false;
                DubbeleInzet();
                DubbeleInzetOnzichtbaar();
                GeefSpelerKaart();
                ImageConverterenSpeler();
                AasWaardeAanpassenSpeler();
                SpelerKaartTonen();
                KaartspelerTonenDubbelDown();

                if (totaalSpeler<21)
                {
                    BtnStand_Click(sender, e);
                }
                else
                {

                    VerliesHit();
                }

                 

            }
            else
            {

                MessageBox.Show("Je hebt niet genoeg geld om te verdubbelen\n" +
                    "speel tot je minstens dubbel de inzet hebt", "niet genoeg geld", MessageBoxButton.OK, MessageBoxImage.Error);
                BtnDubbelInzet.IsEnabled = false;

            }



           

        }
        private void SLInzet_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VWTekstSlider.Visibility = Visibility.Hidden;
            DeelKnopActief();
            SLInzet.Minimum = Math.Ceiling(kapitaalSpeler / 10);
            SLInzet.Maximum = kapitaalSpeler;
            TxtInzetSpeler.Text = Convert.ToString(Math.Ceiling(SLInzet.Value));
            TxtKapitaalSpeler.Text = Convert.ToString(Math.Floor(kapitaalSpeler - SLInzet.Value));
            
        }
        private void DeelKnopActief()
        {
            BtnDeel.IsEnabled = true;
        }
        private void KaartspelerTonenDubbelDown()
        {
            
            BitmapImage dubbelDown = new BitmapImage();
            dubbelDown.BeginInit();
            dubbelDown.UriSource = new Uri("/assets/" + kaartmethode + ".png", UriKind.RelativeOrAbsolute);

            dubbelDown.Rotation = Rotation.Rotate90;
            dubbelDown.EndInit();
            
            ImageSpelerDubbelDown.Source = dubbelDown;
        }
        private void KaartspelerTonen()
        {
            kaartImgSpeler = kaartmethode;
            ImageSpeler.Source = new BitmapImage(new Uri("/assets/" + kaartImgSpeler + ".png", UriKind.RelativeOrAbsolute));
        }
        private void VerborgenKaartBankTonen()
        {
            ImageVerborgeKaartBank.Source = new BitmapImage(new Uri("/assets/" + verborgenKaart + ".png", UriKind.RelativeOrAbsolute));
        }
        private void KaartBankTonen()
        {
            kaartImgBank = kaartmethode;
            ImageKaartBank.Source = new BitmapImage(new Uri("/assets/" + kaartImgBank + ".png", UriKind.RelativeOrAbsolute));
        }
        public void ImageConverterenBank()
        {
            //listbox voor afbeeldingen
            //string imagePath;
            Image bankImage = new Image();
            bankImage.Source = new BitmapImage(new Uri(@"/assets/" + kaartmethode + ".png", UriKind.RelativeOrAbsolute));
            //HandSpeler.Add(image);
            ListBoxItem item = new ListBoxItem();
            item.Content = bankImage;
            bankLB.Items.Add(item);

            //StackPanel panel = new StackPanel();
            //panel.Children.Add(bankImage);
        }
        private void ImageConverterenSpeler()
        {
            Image spelerImage = new Image();
            spelerImage.Source = new BitmapImage(new Uri(@"/assets/" + kaartmethode + ".png", UriKind.RelativeOrAbsolute));
            //HandSpeler.Add(image);
            ListBoxItem item = new ListBoxItem();
            item.Content = spelerImage;
            SpelerLB.Items.Add(item);
        }
        private void GeluidAan_Click(object sender, RoutedEventArgs e)
        {
            achtergrond.Play();
        }
        private void GeluidUit_Click(object sender, RoutedEventArgs e)
        {
            achtergrond.Pause();
        }

        private void LbHistoriek_MouseEnter_1(object sender, MouseEventArgs e)
        {
            TxbHistoriek.Visibility = Visibility.Visible;
            string temp;
            int lijstnummer = 0;
            for (int i = 0; i < 10; i++)
            {
                temp = $"{rondeNummers[i]}: {rondeGegevens[i]} ";
                historieklijst.Insert(0, temp);
                historieklijst.RemoveAt(10);
                
            }

            historieklijst.Reverse();

            foreach (string item in historieklijst)
            {
                lijst.AppendLine(historieklijst[lijstnummer]);
                lijstnummer++;

            }
            TxbHistoriek.Text ="Historiek\n"+ lijst.ToString();
            lijst.Clear();
        }

        private void LbHistoriek_MouseLeave_1(object sender, MouseEventArgs e)
        {   TxbHistoriek.Visibility = Visibility.Hidden;

        }
    }
}

