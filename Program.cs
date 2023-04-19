using System.IO;
using System.IO.Compression;
using System.Reflection.PortableExecutable;

namespace opstreetIO
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //zip file uitpakken
            if (!File.Exists(@"C:\data\ProvincieIDsVlaanderen.csv"))
            {
                string zipPath = @"C:\data\straatnamenInfo.zip";
                string extractPath = @"C:\data\";
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }

            //map straatnamen aanmaken
            if (!File.Exists(@"C:\data\straatnamen"))
            {
                Directory.CreateDirectory(@"C:\data\" + "straatnamen");
            }

            //alle provincie ids in een array steken
            string[] provincieIds = new string[0];
            using (StreamReader sr = File.OpenText(@"C:\data\ProvincieIDsVlaanderen.csv"))
            {
                string input = null;
                while ((input = sr.ReadLine()) != null)
                {

                    string values = input;
                    string[] valuesArray = values.Split(',');
                    provincieIds = new string[valuesArray.Length];
                    for (int i = 0; i < valuesArray.Length; i++)
                    {
                        provincieIds[i] = valuesArray[i];
                    }

                }
            }

            Dictionary<int, int> gmntToPr = new Dictionary<int, int>();
            //elke provincie bijvoegen en filtreren
            List<Provincie> provincies = new List<Provincie>();
            using (StreamReader sr = File.OpenText(@"C:\data\ProvincieInfo.csv"))
            {
                string input = null;
                string header = sr.ReadLine();
                while ((input = sr.ReadLine()) != null)
                {

                    string values = input;
                    string[] valuesArray = values.Split(';');
                    foreach (string id in provincieIds)
                    {
                        if (valuesArray[1] == id && valuesArray[2] == "nl")
                        {
                            provincies.Add(new Provincie() { Id = int.Parse(valuesArray[1]), GemeenteId = int.Parse(valuesArray[0]), Naam = valuesArray[3], Taalcode = valuesArray[2] });
                            gmntToPr.Add(int.Parse(valuesArray[0]), int.Parse(valuesArray[1]));
                        }
                    }


                }
            }

            //mappeen aangemaakt
            foreach (Provincie provincie in provincies)
            {
                if (!File.Exists(@$"C:\data\straatnamen\{provincie.Naam}"))
                {
                    Directory.CreateDirectory(@$"C:\data\straatnamen\" + provincie.Naam);
                }
            }

            //alle gemeentes toevoegen en filtreren
            List<Gemeente> gemeentes = new List<Gemeente>();
            using (StreamReader sr = File.OpenText(@"C:\data\Gemeentenaam.csv"))
            {
                string input = null;
                string header = sr.ReadLine();
                while ((input = sr.ReadLine()) != null)
                {

                    string values = input;
                    string[] valuesArray = values.Split(';');
                    if (valuesArray[2] == "nl")
                    {
                        gemeentes.Add(new Gemeente() { Id = int.Parse(valuesArray[1]), Naam = valuesArray[3], Taalcode = valuesArray[2] });
                    }

                }
            }

            //alle straten toevoegen
            List<Straat> straten = new List<Straat>();
            using (StreamReader sr = File.OpenText(@"C:\data\straatnamen.csv"))
            {
                string input = null;
                string header = sr.ReadLine();
                sr.ReadLine();
                while ((input = sr.ReadLine()) != null)
                {

                    string values = input;
                    string[] valuesArray = values.Split(';');

                    straten.Add(new Straat() { Id = int.Parse(valuesArray[0]), Naam = valuesArray[1] });


                }
            }

            //straten alfabetisch sorteren 
            straten.Sort((x, y) => x.Naam.CompareTo(y.Naam));

            Dictionary<int, int> strToGmnt = new Dictionary<int, int>();
            //straten hun gemeenteID + naam toevoegen
            using (StreamReader sr = File.OpenText(@"C:\data\StraatnaamID_gemeenteID.csv"))
            {
                string input = null;
                string header = sr.ReadLine();
                while ((input = sr.ReadLine()) != null)
                {
                    string values = input;
                    string[] valuesArray = values.Split(";");
                    strToGmnt.Add(int.Parse(valuesArray[0]), int.Parse(valuesArray[1]));

                }
                foreach (Straat straat in straten)
                {
                    if (strToGmnt.ContainsKey(straat.Id))
                    {
                        straat.GemeenteId = strToGmnt[straat.Id];
                    }
                    //gemeentenaam toekennen + provincieID toekennen
                    foreach (Gemeente gemeente in gemeentes)
                    {
                        if (gemeente.Id == straat.GemeenteId)
                        {
                            straat.GemeenteNaam = gemeente.Naam;
                            if (gmntToPr.ContainsKey(gemeente.Id))
                            {
                                straat.ProvincieId = gmntToPr[gemeente.Id];
                            }
                        }
                    }
                    //provincienaam toekennen
                    foreach (Provincie provincie in provincies)
                    {
                        if (provincie.Id == straat.ProvincieId)
                        {
                            straat.ProvincieNaam = provincie.Naam;
                        }
                    }
                }


            }
            //alle gemeentes binnen de provincie toevoegen 
            List<Gemeente> antwerpenGemeentes = new List<Gemeente>();
            List<Gemeente> vlaamsbrabantseGemeentes = new List<Gemeente>();
            List<Gemeente> westvlaamseGemeentes = new List<Gemeente>();
            List<Gemeente> oostvlaamseGemeentes = new List<Gemeente>();
            List<Gemeente> limburgseGemeentes = new List<Gemeente>();
            foreach (Gemeente gemeente in gemeentes)
            {
                foreach (Provincie provincie in provincies)
                {
                    if (gemeente.Id == provincie.GemeenteId)
                    {
                        if (provincie.Id == 1) // antwerpen
                        {
                            antwerpenGemeentes.Add(gemeente);
                            gemeente.ProvincieId = 1;
                        }
                        else if (provincie.Id == 2) // vlaams-brabant
                        {
                            vlaamsbrabantseGemeentes.Add(gemeente);
                            gemeente.ProvincieId = 2;
                        }
                        else if (provincie.Id == 4) // west-vlaanderen
                        {
                            westvlaamseGemeentes.Add(gemeente);
                            gemeente.ProvincieId = 4;
                        }
                        else if (provincie.Id == 5) // oost-vlaanderen
                        {
                            oostvlaamseGemeentes.Add(gemeente);
                            gemeente.ProvincieId = 5;
                        }
                        else if (provincie.Id == 8) // limburg
                        {
                            limburgseGemeentes.Add(gemeente);
                            gemeente.ProvincieId = 8;
                        }
                    }
                }
            }

            straten.RemoveAll(x => x.ProvincieId == 0);

            //textbestanden aanmaken  voor de gemeentes + straten erbij toevoegen
            gmntTxtStrt(antwerpenGemeentes, straten, "Antwerpen");
            gmntTxtStrt(limburgseGemeentes, straten, "Limburg");
            gmntTxtStrt(vlaamsbrabantseGemeentes, straten, "Vlaams-Brabant");
            gmntTxtStrt(oostvlaamseGemeentes, straten, "Oost-Vlaanderen");
            gmntTxtStrt(westvlaamseGemeentes, straten, "West-Vlaanderen");




            //textbestand aanmaken met daarin alle bijhorende info (povince,straat,gemeente)



            SorterComparer sorterComparer = new SorterComparer();
            straten.Sort(sorterComparer);


            if (!File.Exists(@"C:\data\straatnamen\straatnamen.txt"))
            {


                using (StreamWriter sw = File.CreateText(@"C:\data\straatnamen\straatnamen.txt"))
                {
                    foreach (Straat straat in straten)
                    {
                        if (straat.ProvincieId != 0)
                        {
                            sw.WriteLine($"{straat.ProvincieNaam},{straat.GemeenteNaam},{straat.Naam}");
                        }

                    }
                }
            }




            Console.WriteLine("Druk op x om alles te verwijderen");
            string keuze = Console.ReadLine();
            if (keuze == "x")
            {
                string directoryPath = @"C:\data\straatnamen";

                DeleteFilesInDirectory(directoryPath);

            }

        }










        static void gmntTxtStrt(List<Gemeente> gemeentes, List<Straat> straten, string provincie)
        {
            gemeentes.Sort((x, y) => x.Naam.CompareTo(y.Naam));
            foreach (Gemeente gemeente in gemeentes)
            {
                if (!File.Exists(@$"C:\data\straatnamen\{provincie}\{gemeente.Naam}.txt"))
                {
                    using (StreamWriter sw = File.CreateText(@$"C:\data\straatnamen\{provincie}\{gemeente.Naam}.txt"))
                    {
                        foreach (Straat straat in straten)
                        {
                            if (straat.GemeenteId == gemeente.Id)
                            {
                                sw.WriteLine(straat.Naam);
                            }
                        }
                    }
                }
            }

        }

        static void DeleteFilesInDirectory(string directoryPath)
        {
            string[] filePaths = Directory.GetFiles(directoryPath);

            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
            }

            string[] subdirectoryPaths = Directory.GetDirectories(directoryPath);

            foreach (string subdirectoryPath in subdirectoryPaths)
            {
                DeleteFilesInDirectory(subdirectoryPath);
            }

            Directory.Delete(directoryPath);
        }
    }
}