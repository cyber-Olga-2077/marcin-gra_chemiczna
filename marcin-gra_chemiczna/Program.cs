using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace marcin_gra_chemiczna
{
    interface IDane
    {
	    void Wyswietl() ;
    }

    class Odpowiedz : IDane
    {
        bool CzyPoprawna;
        string TekstOdpowiedzi;

        public Odpowiedz()
        {
            
	    }

        public Odpowiedz(string TekstOdpowiedzi, bool CzyPoprawna)
        {
            this.TekstOdpowiedzi = TekstOdpowiedzi;
            
	    }

        public void Wyswietl()
        {

        }
    }

    class Pytanie : IDane
    {
        string TrescPytania;
        int Punkty;
        List<Odpowiedz> ListaOdpowiedzi;

        public void Wyswietl()
        {

        }

        public bool ZadajPytanieIUdzielOdpowiedzi()
        {

            return false;
        }

        public Pytanie(XmlNode PytanieNode)
        {

        }
    }

    class Dziedzina : IDane
    {
        List<Pytanie> ListaPytan;

        public void Wyswietl()
        {

        }

        int Graj()
        {
            int Licznik = 0;

            foreach (var pytanie in ListaPytan)
            {
                if (pytanie.ZadajPytanieIUdzielOdpowiedzi() == true)
			{
                    Licznik++;
                }
            }

            return Licznik;
        }

        public Dziedzina(string ConfigFilename)
        {
            XmlDocument config = new XmlDocument(); //utwórz obiekt z konfiguracją
            int Ilosc;
            List<string> PlikiPytan = new List<string>();

            config.Load(ConfigFilename); //wczytaj konfigurację

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
                    Pytania.Load(Plik);

                    if (!int.TryParse(Pytania.DocumentElement.SelectSingleNode("questions").Attributes["points"].Value, out Punkty)) //parsuje atrybut z XML do inta
                    {
                        throw new System.ArgumentException("Empty configuration");
                    }

                    if (Pytania.DocumentElement.SelectSingleNode("questions").ChildNodes.Count < Ilosc) //Sprawdza czy nie ma mniej pytan niz potrzeba
                    {
                        throw new System.ArgumentException("Empty configuration");
                    }
                    else
                    {
                        List<XmlNode> ListaPytanTymczasowa = new List<XmlNode>();
                        HashSet<int> NumeryPytan = new HashSet<int>();

                        foreach (XmlNode Pytanie in Pytania.DocumentElement.SelectSingleNode("questions").ChildNodes) //Dla każdego pytanie
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
                        }

                        foreach (int numerPytania in NumeryPytan)
                        {
                            ListaPytan.Add(new Pytanie(ListaPytanTymczasowa[numerPytania])); //Konstruuje pytania
                        }
                    }
                }
            }
        }
    }

    class Chemia : Dziedzina
    {
        public Chemia(string ConfigFilename) : base(ConfigFilename)
        { }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Dziedzina gra = new Chemia("resources/config.xml");
            Console.ReadLine();
        }
    }
}
