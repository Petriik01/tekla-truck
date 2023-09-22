using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using System.Threading;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;

namespace Tekla_truck_simulator
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TSM.Model teklaTruck = new TSM.Model();

        TSM.Beam beamTruck = new TSM.Beam(TSM.Beam.BeamTypeEnum.BEAM);

        TSM.Beam beamBod = new TSM.Beam(TSM.Beam.BeamTypeEnum.BEAM);

        public MainWindow()
        {
            InitializeComponent();
        }
        bool engine;
        bool handBreak;

        public double truckX = 1000;
        public double truckXStart = 0;

        public double truckY = 0;
        public double truckYStart = 0;

        public double truckZ = 0;

        bool nahoru;
        bool dolu;
        bool doprava;
        bool doleva;

        int speed = 1000;

        double posun = 500;

        public double pointX;
        public double pointY;

        double pointXEnd;
        double pointYEnd;

        public int pointCounter;

        string cargo;
        string truck;
        int cena;
        int tah;
        int requeredTah;
        int money;
        int maxSpeed;

        bool picked;

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //buttonStart.IsEnabled = false;
            picked = false;
        }
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            generatePoint();
            beamTruck.Profile.ProfileString = "200*200";
            beamTruck.Material.MaterialString = "C45/55";
            beamTruck.Class = "7";

            beamTruck.StartPoint = new TSG.Point(truckXStart, truckYStart, truckZ);
            beamTruck.EndPoint = new TSG.Point(truckX, truckY, truckZ);

            beamTruck.Insert();

            teklaTruck.CommitChanges();
            beamTruck.Delete();

            nahoru = true;

            engine = false;
            handBreak = true;

            cargo = comboBoxOffers.Text;
            truck = comboBoxTruck.Text;
            

            new Thread(() =>
            {
                do
                {
                    proces();
                    System.Threading.Thread.Sleep(speed);

                } while (nahoru == true);
            }).Start();
        }
        private void proces()
        {

            if (engine == false || handBreak == true)
            {
                posun = 0;
            }
            else
            {
                posun = 500;

                truckXStart = truckX;
                truckX += posun;

                beamTruck.StartPoint = new TSG.Point(truckXStart, truckYStart, truckZ);
                beamTruck.EndPoint = new TSG.Point(truckX, truckY, truckZ);

                beamTruck.Insert();
                teklaTruck.CommitChanges();
                beamTruck.Delete();
            }
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // KLÁVESA W - POHYB NAHORU
                case Key.W:
                    if (dolu == true)
                    {
                        truckX += 2 * posun;
                    }
                    else
                    {
                        truckX += posun;
                    }

                    nahoru = true;

                    dolu = false;
                    doleva = false;
                    doprava = false;

                    truckY = truckYStart;
                    new Thread(() =>
                    {
                        do
                        {
                            if (engine == false || handBreak == true)
                            {
                                posun = 0;
                            }

                            beamTruck.StartPoint = new TSG.Point(truckXStart, truckYStart, truckZ);
                            beamTruck.EndPoint = new TSG.Point(truckX, truckY, truckZ);

                            beamTruck.Insert();

                            teklaTruck.CommitChanges();
                            beamTruck.Delete();

                            truckXStart = truckX;
                            truckX += posun;

                            checkPoint();
                            regeneratePoint();

                            System.Threading.Thread.Sleep(speed);
                        } while (nahoru == true);

                    }).Start();
                    break;
                    //KLÁVESA S - POHYB DOLU
                case Key.S:
                    if (nahoru == true)
                    {
                        truckX -= 2 * posun;
                    }
                    else
                    {
                        truckX -= posun;
                    }

                    dolu = true;

                    nahoru = false;
                    doleva = false;
                    doprava = false;

                    truckY = truckYStart;
                    new Thread(() =>
                    {
                        do
                        {
                            if (engine == false || handBreak == true)
                            {
                                posun = 0;
                            }
                            beamTruck.StartPoint = new TSG.Point(truckXStart, truckYStart, truckZ);
                            beamTruck.EndPoint = new TSG.Point(truckX, truckY, truckZ);

                            beamTruck.Insert();

                            teklaTruck.CommitChanges();
                            beamTruck.Delete();

                            truckXStart = truckX;
                            truckX -= posun;

                            checkPoint();
                            regeneratePoint();

                            System.Threading.Thread.Sleep(speed);
                        } while (dolu == true);

                    }).Start();
                    break;
                    // KLÁVESA D - POHYB DOPRAVA
                case Key.D:
                    if (doleva == true)
                    {
                        truckY -= 2 * posun;
                    }
                    else
                    {
                        truckY -= posun;
                    }

                    doprava = true;

                    doleva = false;
                    nahoru = false;
                    dolu = false;

                    truckX = truckXStart;

                    new Thread(() =>
                    {
                        do
                        {
                            if (engine == false || handBreak == true)
                            {
                                posun = 0;
                            }
                            beamTruck.StartPoint = new TSG.Point(truckXStart, truckYStart, truckZ);
                            beamTruck.EndPoint = new TSG.Point(truckX, truckY, truckZ);

                            beamTruck.Insert();

                            teklaTruck.CommitChanges();
                            beamTruck.Delete();

                            truckYStart = truckY;
                            truckY -= posun;

                            checkPoint();
                            regeneratePoint();

                            System.Threading.Thread.Sleep(speed);
                        } while (doprava == true);

                    }).Start();
                    break;
                    // KLÁVESA A - POHYB DOLEVA
                case Key.A:
                    if (doprava == true)
                    {
                        truckY += 2 * posun;
                    }
                    else
                    {
                        truckY += posun;
                    }

                    doleva = true;

                    nahoru = false;
                    dolu = false;
                    doprava = false;

                    truckX = truckXStart;

                    new Thread(() =>
                    {
                        do
                        {
                            
                            if (engine == false || handBreak == true)
                            {
                                posun = 0;
                            }
                            beamTruck.StartPoint = new TSG.Point(truckXStart, truckYStart, truckZ);
                            beamTruck.EndPoint = new TSG.Point(truckX, truckY, truckZ);

                            beamTruck.Insert();

                            teklaTruck.CommitChanges();
                            beamTruck.Delete();

                            truckYStart = truckY;
                            truckY += posun;

                            checkPoint();
                            regeneratePoint();

                            System.Threading.Thread.Sleep(speed);
                        } while (doleva == true);

                    }).Start();
                    break;
                // Zvýšení rychlosti
                case Key.LeftShift:
                    speed -= 50;
                    if (speed < 0)
                    {
                        speed = 0;
                    }
                    this.labelSpeed.Dispatcher.Invoke(() =>
                    {
                        this.labelSpeed.Content = "SPEED: " + speed;
                    });
                    break;
                //Snížení rychlosti
                case Key.LeftCtrl:
                    speed += 50;
                    this.labelSpeed.Dispatcher.Invoke(() =>
                    {
                        this.labelSpeed.Content = "SPEED: " + speed;
                    });
                    break;
                // Engine
                case Key.E:
                    if (engine == true)
                    {
                        engine = false;
                        this.labelEngine.Dispatcher.Invoke(() =>
                        {
                            this.labelEngine.Content = "Engine: OFF";
                        });
                    }
                    else
                    {
                        engine = true;
                        this.labelEngine.Dispatcher.Invoke(() =>
                        {
                            this.labelEngine.Content = "Engine: ON";
                        });
                    }
                    break;
                // Handbreak
                case Key.Q:
                    if (handBreak == true)
                    {
                        handBreak = false;
                        this.labelHandBreak.Dispatcher.Invoke(() =>
                        {
                            this.labelHandBreak.Content = "HandBreak: OFF";
                        });
                    }
                    else
                    {
                        handBreak = true;
                        this.labelHandBreak.Dispatcher.Invoke(() =>
                        {
                            this.labelHandBreak.Content = "HandBreak: ON";
                        });
                    }
                    break;
            }
        }
        // VYGENERUJE STATICKY BOD NA PLACOVNÍ PLOŠE PRO VYZVEDNUTÍ NÁKLADU
        private void generatePoint()
        {
            beamBod.Profile.ProfileString = "250*250";
            beamBod.Material.MaterialString = "C45/55";
            beamBod.Class = "2";

            pointX = 1500;
            pointY = 10500;

            beamBod.StartPoint = new TSG.Point(pointX - 250 / 2, pointY, 250 / 2);
            beamBod.EndPoint = new TSG.Point(pointX + 250 / 2, pointY, 250 / 2);

            beamBod.Insert();
            teklaTruck.CommitChanges();
        }

        // KONTROLUJE, JESTLI JE KAMION V BLIZKOSTI OBJEKTU NÁKLADU/VYKLÁDKY
        private void checkPoint()
        {
            double rozdil1 = Math.Abs(pointX - truckX);
            double rozdil2 = Math.Abs(pointY - truckY);

            double rozdilEnd1 = Math.Abs(pointXEnd - truckX);
            double rozdilEnd2 = Math.Abs(pointYEnd - truckY);

            if (rozdil1 <= 250 && rozdil2 <= 250)
            {
                picked = true;
                deletePoint();
            }
            if (rozdilEnd1 <= 250 && rozdilEnd2 <= 250)
            {
                picked = false;
                deleteEndPoint();
            }
        }
        // MĚNÍ BARVU KAMIONU NA CLASS 3 POKUD MÁ NALOŽENÝ NÁKLAD
        private void regeneratePoint()
        {
            if (picked)
            {
                beamTruck.Class = "3";
            }else if (picked == false)
            {
                beamTruck.Class = "7";
            }
        }
        // VYMAŽE POINT NAKLÁDKY, VYGENERUJE ENDPOINT
        private void deletePoint()
        {
            beamBod.Delete();
            picked = true;
            generateEndPoint();
        }
        // VYMAŽE ENDPOINT, POKUD KAMION DORUČIL ZÁSILKU
        private void deleteEndPoint()
        {
            beamBod.Delete();
            MessageBox.Show("Dovezl jsi náklad na místo určení");
        }
       
        // PO NALOŽENÍ NÁKLADU VYGENERUJE ENDPOINT, TEDY MÍSTO VYKLÁDKY
        private void generateEndPoint()
        {
            pointXEnd = 18500;
            pointYEnd = 18500;

            beamBod.StartPoint = new TSG.Point(pointXEnd - 250 / 2, pointYEnd, 250 / 2);
            beamBod.EndPoint = new TSG.Point(pointXEnd + 250 / 2, pointYEnd, 250 / 2);

            beamBod.Insert();
            teklaTruck.CommitChanges();
        }

        private void comboBoxOffers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cargo = comboBoxOffers.Text;
            this.labelNazevNakladu.Dispatcher.Invoke(() =>
            {
                this.labelNazevNakladu.Content = "Náklad: " + cargo;
            });
        }
        private void comboBoxTruck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            truck = comboBoxTruck.Text;
            this.labelKamion.Dispatcher.Invoke(() =>
            {
                this.labelKamion.Content = "Kamion: " + truck;
            });
        }

    }
}
