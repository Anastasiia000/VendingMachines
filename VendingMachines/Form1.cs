using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VendingMachines
{
    public partial class Form1 : Form
    {
        static float anPrice=0.0F;
        static float anSuma=0.0F; //jest identyczną zmienną do anPrice, do etapu wydawania reszty
        static float[] anMozliweNominaly = { 5, 2, 1, 0.5F};
        static float[] anMozliweNominalyEURiUSD = { 2, 1, 0.5F, 0.2F };
        static byte[] anIloscNominalowCoZostalyPLN ={ 1, 4, 0, 0};
        static byte[] anIloscNominalowCoZostalyEUR = {2, 2, 2, 2}; 
        static byte[] anIloscNominalowCoZostalyUSD = {3, 3, 3, 3 }; 
        Nominaly[] anNominalies = new Nominaly[anMozliweNominaly.Length];
        Nominaly[] anNominaliesEUR = new Nominaly[anMozliweNominalyEURiUSD.Length];
        Nominaly[] anNominaliesUSD = new Nominaly[anMozliweNominalyEURiUSD.Length];

        public Form1()
        {
            InitializeComponent();
            for (byte i = 0; i < anNominalies.Length; i++)
            {
                anNominalies[i].anIlosc = anIloscNominalowCoZostalyPLN[i];
                anNominalies[i].anNominal = anMozliweNominaly[i];
                anNominaliesEUR[i].anIlosc = anIloscNominalowCoZostalyEUR[i];
                anNominaliesEUR[i].anNominal = anMozliweNominalyEURiUSD[i];
                anNominaliesUSD[i].anIlosc = anIloscNominalowCoZostalyUSD[i];
                anNominaliesUSD[i].anNominal = anMozliweNominalyEURiUSD[i];

                switch (anNominalies[i].anNominal)
                {
                    case 5:
                        txtReszta5.Text = Convert.ToString(anNominalies[i].anIlosc);
                        break;
                    case 2:
                        txtReszta2.Text = Convert.ToString(anNominalies[i].anIlosc);
                        break;
                    case 1:
                        txtReszta1.Text = Convert.ToString(anNominalies[i].anIlosc);
                        break;
                    case 0.5F:
                        txtReszta0_5.Text = Convert.ToString(anNominalies[i].anIlosc);
                        break;
                }
            }
        }
    
        private void anWydanieReszty(float anReszta, byte anNumerNominalu, Nominaly[] nominalies)
        {
            anReszta *= (-1);
            short anliczbaNominalow, anindeksNominalu = 0; //w cyklu while bedzie zmieniony anindeksNominalu
            string label = "Dziękujemy. Odbierz:\n";
            if (anCzyStarczyIlosciNominalow(anReszta, nominalies)) //zaczyna sie proces wydania reszty
            {
                while (anReszta > 0.0F && anindeksNominalu < nominalies.Length)
                {
                    anliczbaNominalow = (short)(anReszta / nominalies[anindeksNominalu].anNominal);
                    if (anliczbaNominalow > nominalies[anindeksNominalu].anIlosc)
                    {
                        anliczbaNominalow = (short)nominalies[anindeksNominalu].anIlosc;
                        nominalies[anindeksNominalu].anIlosc = 0;
                    }
                    else nominalies[anindeksNominalu].anIlosc = 
                            (byte)(nominalies[anindeksNominalu].anIlosc - anliczbaNominalow);
                    anReszta -= anliczbaNominalow * nominalies[anindeksNominalu].anNominal;
                    label = anliczbaNominalow>0? label+$"Nominalu {nominalies[anindeksNominalu].anNominal} " +
                        $"dostaniesz {anliczbaNominalow} sztuk\n":label;
                    anindeksNominalu++;//przechodzimy do mniejszego nominalu, zmieniamy indeks
                }
            }
            else
            {
                label=("Niestety w automacie nie ma pieniędzy do wydania. Zabierz " +
                  "wrzucone przed chwilą\nJeśli chcesz kontynować zakupy plać bez reszty");
                nominalies[anNumerNominalu].anIlosc -= 1;
                panelZaplata.Enabled = false;
                if (anSuma != anPrice)
                {
                     anSuma -= anPrice;
                    while (anSuma > 0.0F && anindeksNominalu < nominalies.Length)
                    {
                        anliczbaNominalow = (short)(anSuma / nominalies[anindeksNominalu].anNominal);
                        if (anliczbaNominalow > nominalies[anindeksNominalu].anIlosc)
                        {
                            anliczbaNominalow = (short)nominalies[anindeksNominalu].anIlosc;
                            nominalies[anindeksNominalu].anIlosc = 0;
                        }
                        else nominalies[anindeksNominalu].anIlosc =
                                (byte)(nominalies[anindeksNominalu].anIlosc - anliczbaNominalow);
                        anSuma -= anliczbaNominalow * nominalies[anindeksNominalu].anNominal;
                        anindeksNominalu++;
                        //label = $"Nominalu {nominalies[anindeksNominalu].anNominal} " +
                       //$"dostaniesz {anliczbaNominalow} sztuk\n"; //anliczbaNominalow > 0 ? label + : label;
                    }
                       
                }
               
            }
            txtboxPrice.Text = Convert.ToString(anPrice = 0);
            MessageBox.Show(label);
            panelMagazyn.Enabled = true;
            cmbWaluta.Enabled = true;
        }

        private bool anCzyStarczyIlosciNominalow(float anReszta, Nominaly[] nominalies)
        {
            short anliczbaNominalow;
            short anindeksNominalu = 0; //w cyklu while bedzie zmieniony
            Nominaly[] T = new Nominaly[nominalies.Length];

            for (int i =0; i<anNominalies.Length; i++)
            {
                T[i].anNominal = nominalies[i].anNominal;
                T[i].anIlosc = nominalies[i].anIlosc;
            }
            while (anReszta > 0.0F && anindeksNominalu < T.Length)
            {
                anliczbaNominalow = (short)(anReszta / T[anindeksNominalu].anNominal);
                 if (anliczbaNominalow > T[anindeksNominalu].anIlosc)
                {
                    anliczbaNominalow = (short)T[anindeksNominalu].anIlosc;
                    T[anindeksNominalu].anIlosc = 0;
                }
                else T[anindeksNominalu].anIlosc = (byte)(T[anindeksNominalu].anIlosc - anliczbaNominalow);
                anReszta -= anliczbaNominalow * T[anindeksNominalu].anNominal;
                anindeksNominalu++;                
            }
            return anReszta == 0;
        }


        private void anDodanieProduktu(float anOplata)
        {
            cmbWaluta.Enabled = false;
            panelZaplata.Enabled = true;
            anPrice += anOplata;
            anSuma += anOplata;
            txtboxPrice.Text = Convert.ToString(anPrice);

        }
        private void btnMoneta(float anMoneta, byte anNumerNominalu, Nominaly []nominalies)
        {
            anPrice -= anMoneta;
            txtboxPrice.Text = Convert.ToString(anPrice);
            //anWrzuconeMonety.Add(anNumerNominalu);
            if (anPrice == 0)
            {
                MessageBox.Show("Dziękujemy");
                panelZaplata.Enabled = false;
                panelMagazyn.Enabled = true;
                cmbWaluta.Enabled = true;
            }
            else
            {
                panelMagazyn.Enabled = false;
                cmbWaluta.Enabled = false;
            }
            nominalies[anNumerNominalu].anIlosc += 1;
            if (anPrice < 0)
                anWydanieReszty(anPrice, anNumerNominalu, nominalies);
            anUpdateNominalies(nominalies);           
        }
        private void button6_Click(object sender, EventArgs e)
        {            Close();        }
        #region Przyciski produktow
        private void button1_Click(object sender, EventArgs e)
        {
            anDodanieProduktu(Convert.ToSingle(labelKawa.Text));
        }

        private void btnlemonade_Click(object sender, EventArgs e)
        {
            anDodanieProduktu(Convert.ToSingle(labellemonade.Text));
        }

        private void btnwater_Click(object sender, EventArgs e)
        {
            anDodanieProduktu(Convert.ToSingle(labelWater.Text));
        }

        private void btnapple_Click(object sender, EventArgs e)
        {
            anDodanieProduktu(Convert.ToSingle(labelApple.Text));
        }

        private void btniceCream_Click(object sender, EventArgs e)
        {
            anDodanieProduktu(Convert.ToSingle(labelIcecream.Text));
        }
        #endregion
        #region Przyciski monet 
        private void btnMoneta0_5_Click(object sender, EventArgs e)
        {
            if (cmbWaluta.Text == "PLN")
                btnMoneta(Convert.ToSingle(btnMoneta0_5.Text), 3, anNominalies);
            if (cmbWaluta.Text == "EUR")
                btnMoneta(Convert.ToSingle(btnMoneta0_5.Text), 2, anNominaliesEUR);
            if (cmbWaluta.Text == "USD")
                btnMoneta(Convert.ToSingle(btnMoneta0_5.Text), 2, anNominaliesUSD);
        }

        private void btnMoneta1_Click(object sender, EventArgs e)
        {
            if (cmbWaluta.Text == "PLN")
                btnMoneta(Convert.ToSingle(btnMoneta1.Text), 2, anNominalies);
            if (cmbWaluta.Text == "EUR")
                btnMoneta(Convert.ToSingle(btnMoneta1.Text), 1, anNominaliesEUR);
            if (cmbWaluta.Text == "USD")
                btnMoneta(Convert.ToSingle(btnMoneta1.Text), 1, anNominaliesUSD);
        }

        private void btnMoneta2_Click(object sender, EventArgs e)
        {
            if (cmbWaluta.Text == "PLN")
                btnMoneta(Convert.ToSingle(btnMoneta2.Text), 1, anNominalies);
            if (cmbWaluta.Text == "EUR")
                btnMoneta(Convert.ToSingle(btnMoneta2.Text), 0, anNominaliesEUR);
            if (cmbWaluta.Text == "USD")
                btnMoneta(Convert.ToSingle(btnMoneta2.Text), 0, anNominaliesUSD);
        }

        private void btnMoneta5_Click(object sender, EventArgs e)
        {
            if (cmbWaluta.Text == "PLN")
                btnMoneta(Convert.ToSingle(btnMoneta5.Text), 0, anNominalies);
            if (cmbWaluta.Text == "EUR")
                btnMoneta(Convert.ToSingle(btnMoneta5.Text), 3, anNominaliesEUR);
            if (cmbWaluta.Text == "USD")
                btnMoneta(Convert.ToSingle(btnMoneta5.Text), 3, anNominaliesUSD);
            
        }
        #endregion

        private void button7_Click(object sender, EventArgs e) //platnosc karta
        {
            anPrice = 0;
            txtboxPrice.Text = "0,0";
            panelMagazyn.Enabled = true;
            panelZaplata.Enabled = false;
            cmbWaluta.Enabled = true;
            MessageBox.Show("Dziękujemy");

        }

        private void anUpdateNominalies (Nominaly []nominaly)
        {

            for (byte i = 0; i < anNominalies.Length; i++)
            {
                if (cmbWaluta.Text == "PLN")
                {
                    switch (nominaly[i].anNominal)
                    {
                        
                        case 5:
                            txtReszta5.Text = Convert.ToString(nominaly[i].anIlosc);
                            break;
                        case 2:
                            txtReszta2.Text = Convert.ToString(nominaly[i].anIlosc);
                            break;
                        case 1:
                            txtReszta1.Text = Convert.ToString(nominaly[i].anIlosc);
                            break;
                        case 0.5F:
                            txtReszta0_5.Text = Convert.ToString(nominaly[i].anIlosc);
                            break;
                    }
                }
                if (cmbWaluta.Text == "USD" || cmbWaluta.Text == "EUR")
                {
                    switch (nominaly[i].anNominal)
                    {
                        case 0.2F:
                            txtReszta0_5.Text = Convert.ToString(nominaly[i].anIlosc);
                            break;
                        case 2:
                            txtReszta5.Text = Convert.ToString(nominaly[i].anIlosc);
                            break;
                        case 1:
                            txtReszta2.Text = Convert.ToString(nominaly[i].anIlosc);
                            break;
                        case 0.5F:
                            txtReszta1.Text = Convert.ToString(nominaly[i].anIlosc);
                            break;
                    }
                }
                
            }
        }

        private void anZmianaWaluty(object sender, EventArgs e)
        {
            switch (cmbWaluta.Text)
            {
                case "PLN":
                    labelKawa.Text ="2,5";
                    labellemonade.Text = "3,0";
                    labelWater.Text = "2,0";
                    labelApple.Text = "1,0";
                    labelIcecream.Text = "1,5";
                    btnMoneta5.Text = "5,0";
                    txtReszta5.Text = Convert.ToString(anNominalies[0].anIlosc);
                    txtReszta2.Text = Convert.ToString(anNominalies[1].anIlosc);
                    txtReszta1.Text = Convert.ToString(anNominalies[2].anIlosc);
                    txtReszta0_5.Text =Convert.ToString(anNominalies[3].anIlosc);
                    labelMniejszeNominaly.Text = "Nominal\n0,5\n1,0";
                    labelWiekszeNominaly.Text = "Nominal\n2,0\n5,0";
                    anUpdateNominalies(anNominalies);
                    break;

                case "USD":
                    labelKawa.Text = "1,5";
                    labellemonade.Text = "2,0";
                    labelWater.Text = "1,0";
                    labelApple.Text = "0,50";
                    labelIcecream.Text = "1,0";
                    btnMoneta5.Text = "0,2";
                    txtReszta5.Text = Convert.ToString(anNominaliesUSD[0].anIlosc);
                    txtReszta2.Text = Convert.ToString(anNominaliesUSD[1].anIlosc);
                    txtReszta1.Text = Convert.ToString(anNominaliesUSD[2].anIlosc);
                    txtReszta0_5.Text = Convert.ToString(anNominaliesUSD[3].anIlosc);
                    labelMniejszeNominaly.Text = "Nominal\n0,2\n0,5";
                    labelWiekszeNominaly.Text = "Nominal\n1,0\n2,0";
                    anUpdateNominalies(anNominaliesUSD);

                    break;

                case "EUR":
                    labelKawa.Text = "1,5";
                    labellemonade.Text = "2,2";
                    labelWater.Text = "1,20";
                    labelApple.Text = "1,0";
                    labelIcecream.Text = "1,2";
                    btnMoneta5.Text = "0,2";
                    txtReszta5.Text = Convert.ToString(anNominaliesEUR[0].anIlosc);
                    txtReszta2.Text = Convert.ToString(anNominaliesEUR[1].anIlosc);
                    txtReszta1.Text = Convert.ToString(anNominaliesEUR[2].anIlosc);
                    txtReszta0_5.Text = Convert.ToString(anNominaliesEUR[3].anIlosc);
                    labelMniejszeNominaly.Text = "Nominal\n0,2\n0,5";
                    labelWiekszeNominaly.Text = "Nominal\n1,0\n2,0";
                    anUpdateNominalies(anNominaliesEUR);

                    break;

            }
        }

        private void anbtnCancel_Click(object sender, EventArgs e)
        {
            panelZaplata.Enabled = false;
            cmbWaluta.Enabled = true;
            txtboxPrice.Text = "0,0";             
            panelMagazyn.Enabled = true;
            
            short anliczbaNominalow, anindeksNominalu = 0; //w cyklu while bedzie zmieniony anindeksNominalu
            
            if (anSuma != anPrice)
            {
                anSuma -= anPrice;

                switch (cmbWaluta.Text)
                {
                    case "PLN":
                        while (anSuma > 0.0F && anindeksNominalu < anNominalies.Length)
                        {
                            anliczbaNominalow = (short)(anSuma / anNominalies[anindeksNominalu].anNominal);
                            if (anliczbaNominalow > anNominalies[anindeksNominalu].anIlosc)
                            {
                                anliczbaNominalow = (short)anNominalies[anindeksNominalu].anIlosc;
                                anNominalies[anindeksNominalu].anIlosc = 0;
                            }
                            else anNominalies[anindeksNominalu].anIlosc =
                                    (byte)(anNominalies[anindeksNominalu].anIlosc - anliczbaNominalow);
                            anSuma -= anliczbaNominalow * anNominalies[anindeksNominalu].anNominal;
                            anindeksNominalu++;//przechodzimy do mniejszego nominalu, zmieniamy indeks
                        }
                        anUpdateNominalies(anNominalies);

                        break;
                    case "USD":
                        while (anSuma > 0.0F && anindeksNominalu < anNominaliesUSD.Length)
                        {
                            anliczbaNominalow = (short)(anSuma / anNominaliesUSD[anindeksNominalu].anNominal);
                            if (anliczbaNominalow > anNominaliesUSD[anindeksNominalu].anIlosc)
                            {
                                anliczbaNominalow = (short)anNominaliesUSD[anindeksNominalu].anIlosc;
                                anNominaliesUSD[anindeksNominalu].anIlosc = 0;
                            }
                            else anNominaliesUSD[anindeksNominalu].anIlosc =
                                    (byte)(anNominaliesUSD[anindeksNominalu].anIlosc - anliczbaNominalow);
                            anSuma -= anliczbaNominalow * anNominaliesUSD[anindeksNominalu].anNominal;
                            anindeksNominalu++;//przechodzimy do mniejszego nominalu, zmieniamy indeks
                        }
                        anUpdateNominalies(anNominaliesUSD);

                        break;
                    case "EUR":
                        while (anSuma > 0.0F && anindeksNominalu < anNominaliesEUR.Length)
                        {
                            anliczbaNominalow = (short)(anSuma / anNominaliesEUR[anindeksNominalu].anNominal);
                            if (anliczbaNominalow > anNominaliesEUR[anindeksNominalu].anIlosc)
                            {
                                anliczbaNominalow = (short)anNominaliesEUR[anindeksNominalu].anIlosc;
                                anNominaliesEUR[anindeksNominalu].anIlosc = 0;
                            }
                            else anNominaliesEUR[anindeksNominalu].anIlosc =
                                    (byte)(anNominaliesEUR[anindeksNominalu].anIlosc - anliczbaNominalow);
                            anSuma -= anliczbaNominalow * anNominaliesEUR[anindeksNominalu].anNominal;
                            anindeksNominalu++;//przechodzimy do mniejszego nominalu, zmieniamy indeks
                        }
                        anUpdateNominalies(anNominaliesEUR);

                        break;
                }
                MessageBox.Show("ODBIERZ PIENIĘDZY");
            }
            anSuma = 0.0F;
            anPrice = 0.0F;
        }        
    }
}
