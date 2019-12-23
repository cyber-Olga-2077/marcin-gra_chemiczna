using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace marcin_gra_chemiczna
{
    interface IDane
    {
	    void Wyswietl();
    }

    class Odpowiedz : IDane
    {
        bool CzyPoprawna;
        string TekstOdpowiedzi;

        public Odpowiedz(string TekstOdpowiedzi, bool CzyPoprawna)
        {
            this.TekstOdpowiedzi = TekstOdpowiedzi;
            this.CzyPoprawna = CzyPoprawna;
        }

        public void Wyswietl()
        {
            Console.WriteLine(TekstOdpowiedzi);
        }

        public bool GetCzyPoprawna()
        {
            return this.CzyPoprawna;
        }
    }

    class Pytanie : IDane
    {
        string TrescPytania;
        int Punkty;
        List<Odpowiedz> ListaOdpowiedzi = new List<Odpowiedz>();

        public void Wyswietl()
        {
            Console.WriteLine(Punkty + " punkty. " + TrescPytania);
            for (int i = 0; i < this.ListaOdpowiedzi.Count; i++)
            {
                Console.Write((i + 1) + ". ");
                this.ListaOdpowiedzi[i].Wyswietl();
            }
        }

        public bool ZadajPytanieIUdzielOdpowiedzi()
        {
            this.Wyswietl();

            int OdpowiedzGracza;
            do
            {
                if (!int.TryParse(Console.ReadLine(), out OdpowiedzGracza))
                {
                    OdpowiedzGracza = 0;
                }

            } while (OdpowiedzGracza - 1 < 0 || OdpowiedzGracza - 1 > this.ListaOdpowiedzi.Count);

            return this.ListaOdpowiedzi[OdpowiedzGracza-1].GetCzyPoprawna();
        }

        public Pytanie(XmlNode PytanieNode, int Punkty)
        {
            this.Punkty = Punkty;
            this.TrescPytania = PytanieNode.Attributes["value"].Value;

            if(PytanieNode.ChildNodes.Count == 0)
            {
                throw new System.ArgumentException("Empty configuration");
            }
            else
            {
                foreach (XmlNode Odpowiedz in PytanieNode.ChildNodes)
                {
                    this.ListaOdpowiedzi.Add(new Odpowiedz(Odpowiedz.Attributes["value"].Value, bool.Parse(Odpowiedz.Attributes["is_good"].Value))); //kontruuje nowa odpowiedz
                }
            }
        }
    }

    class Dziedzina : IDane
    {
        string Nazwa = "GRA";

        List<Pytanie> ListaPytan = new List<Pytanie>();
        string TablicaWynikow;

        public void Wyswietl()
        {
            foreach (Pytanie pytanie in this.ListaPytan)
            {
                pytanie.Wyswietl();
                Console.WriteLine();
            }
        }

        public bool PetlaGry() //glowna petla programu
        {
            while(true)
            {
                Console.Clear();
                switch (Menu())
                {
                    case 1:
                        ZapiszWynik(this.Graj());
                        OdczytajWyniki();
                        break;
                    case 2:
                        OdczytajWyniki();
                        break;
                    case 3:
                        return true;
                }
            }
        }

        public int Menu()
        {
            Console.WriteLine(this.Nazwa + '\n');
            Console.WriteLine("Wybierz opcję z menu: ");
            Console.WriteLine("1. Graj");
            Console.WriteLine("2. Tablica wyyników");
            Console.WriteLine("3. Wyjdź");

            int OdpowiedzGracza;
            do
            {
                if (!int.TryParse(Console.ReadLine(), out OdpowiedzGracza))
                {
                    OdpowiedzGracza = 0;
                }

            } while (OdpowiedzGracza < 1 || OdpowiedzGracza > 3);

            return OdpowiedzGracza;
        }

        public Dictionary<string, int> PobierzTabliceWynikow()
        {
            if (File.Exists(this.TablicaWynikow))
            {
                StreamReader Tablica = new StreamReader(this.TablicaWynikow); //obiekt odczytujacy plik
                Dictionary<string, int> Wyniki = new Dictionary<string, int>(); //tablica par stringow i intow

                while(!Tablica.EndOfStream) //dopoki plik sie nie skonczyl
                {
                    string Gracz = Tablica.ReadLine();
                    int Wynik;

                    if (int.TryParse(Tablica.ReadLine(), out Wynik))
                    {
                        Wyniki.Add(Gracz, Wynik); //zapisz do tablicy
                    }
                }
                Tablica.Close();
                return Wyniki;
            }
            else
            {
                throw new System.ArgumentException("Brak pliku tablicy wyników");
            }
        }

        public void ZapiszWynik(int NowyWynik)
        {
            Dictionary<string, int> Wyniki = this.PobierzTabliceWynikow();

            string NazwaGracza;
            bool NazwaUzywana = false;

            do //tworzenie nazwy gracza
            {
                Console.Clear();
                Console.WriteLine(this.Nazwa + '\n');

                Console.WriteLine("Twój wynik to: " + NowyWynik);
                Console.WriteLine("Podaj swój nick: ");
                NazwaGracza = Console.ReadLine();

                NazwaUzywana = false;

                foreach (KeyValuePair<string, int> Wynik in Wyniki) //sprawdzaj czy takiego gracza juz nie ma
                {
                    if (NazwaGracza == Wynik.Key)
                    {
                        NazwaUzywana = true;
                    }
                }

            } while (NazwaUzywana == true);

            Wyniki.Add(NazwaGracza, NowyWynik); //dodaj nowy wynik

            if (File.Exists(this.TablicaWynikow))
            {
                StreamWriter Tablica = new StreamWriter(this.TablicaWynikow);
                
                foreach (KeyValuePair<string, int> Wynik in Wyniki) //zaisz wszystkie wyniki
                {
                    Tablica.WriteLine(Wynik.Key);
                    Tablica.WriteLine(Wynik.Value);
                }
                Tablica.Close();
            }
            else
            {
                throw new System.ArgumentException("Brak pliku tablicy wyników");
            }
        }
        
        public void OdczytajWyniki()
        {
            Console.Clear();
            Console.WriteLine(this.Nazwa + '\n');
            Console.WriteLine("Tablica Wyinków");
            Console.WriteLine("");

            Dictionary<string, int> Wyniki = this.PobierzTabliceWynikow();
            foreach (KeyValuePair<string, int> Wynik in Wyniki)
            {
                Console.WriteLine(Wynik.Key + " zdobył " + Wynik.Value + " punktów");
            }

            Console.WriteLine("");
            Console.WriteLine("Wciśnij dowolny klawisz żeby wrócić do menu");
            Console.ReadKey();
        }

        public int Graj()
        {
            int Licznik = 0;

            foreach (Pytanie pytanie in this.ListaPytan)
            {
                Console.Clear();
                Console.WriteLine(this.Nazwa + '\n');

                if (pytanie.ZadajPytanieIUdzielOdpowiedzi() == true)
			    {
                    Licznik++;
                }
            }

            return Licznik;
        }

        public Dziedzina(string FolderResources, string PlikKonfiguracyjny, string TablicaWynikow, string Name = "")
        {
            if (Name.Length > 0)
            {
                this.Nazwa = Name;
            }

            this.TablicaWynikow = TablicaWynikow;

            XmlDocument config = new XmlDocument(); //utwórz obiekt z konfiguracją
            int Ilosc;
            List<string> PlikiPytan = new List<string>();

            config.Load(FolderResources+PlikKonfiguracyjny); //wczytaj konfigurację

            if (!int.TryParse(config.DocumentElement.SelectSingleNode("amount").Attributes["value"].Value, out Ilosc)) //parsuje atrybut z XML do inta
            {
                throw new System.ArgumentException("Empty configuration");
            }

            if (config.DocumentElement.SelectSingleNode("files").ChildNodes.Count == 0) //Sprawdza ile dzieci ma node "files"
            {
                throw new System.ArgumentException("Empty configuration");
            }
            else
            {
                foreach (XmlNode Plik in config.DocumentElement.SelectSingleNode("files").ChildNodes) //Dla każdego dziecka node "files"
                {
                    PlikiPytan.Add(Plik.Attributes["name"].Value); //Dodaje do tablicy nazwę pliku
                }
            }

            if (PlikiPytan.Count == 0)
            {
                throw new System.ArgumentException("Empty configuration");
            }
            else
            {
                foreach (string Plik in PlikiPytan)
                {
                    XmlDocument Pytania = new XmlDocument(); //obiekt z plikiem pytań
                    int Punkty;
                    Pytania.Load(FolderResources+Plik);

                    if (!int.TryParse(Pytania.DocumentElement.Attributes["points"].Value, out Punkty)) //parsuje atrybut z XML do inta
                    {
                        throw new System.ArgumentException("Empty configuration");
                    }

                    if (Pytania.DocumentElement.ChildNodes.Count < Ilosc) //Sprawdza czy nie ma mniej pytan niz potrzeba
                    {
                        throw new System.ArgumentException("Empty configuration");
                    }
                    else
                    {
                        List<XmlNode> ListaPytanTymczasowa = new List<XmlNode>();
                        HashSet<int> NumeryPytan = new HashSet<int>();

                        foreach (XmlNode Pytanie in Pytania.DocumentElement.ChildNodes) //Dla każdego pytanie
                        {
                            ListaPytanTymczasowa.Add(Pytanie); //dodaje node z pytaniem do tablicy
                        }

                        Random Generator = new Random(); //generator liczb losowych

                        for (int i = 0; i < Ilosc; i++)
                        {
                            int numerPytania;

                            do
                            {
                                numerPytania = Generator.Next(0, ListaPytanTymczasowa.Count); //generuje losowa liczbe z zakresu 0 do liczby pytan w pliku
                            } while (NumeryPytan.Contains(numerPytania)); //Zapewnia unikatowosc

                            NumeryPytan.Add(numerPytania);
                        }

                        foreach (int numerPytania in NumeryPytan)
                        {
                            ListaPytan.Add(new Pytanie(ListaPytanTymczasowa[numerPytania], Punkty)); //Konstruuje pytania
                        }
                    }
                }
            }
        }
    }

    class Chemia : Dziedzina
    {
        public Chemia(string FolderResources, string PlikKonfiguracyjny, string TablicaWynikow) : base(FolderResources, PlikKonfiguracyjny, TablicaWynikow)
        { }
        
        public Chemia(string FolderResources, string PlikKonfiguracyjny, string TablicaWynikow, string Name = "") : base(FolderResources, PlikKonfiguracyjny, TablicaWynikow, Name)
        { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Dziedzina gra = new Chemia("resources/", "config.xml", "scoreboard.txt", "GRA CHEMICZNA");
            gra.PetlaGry();
        }
    }
}
