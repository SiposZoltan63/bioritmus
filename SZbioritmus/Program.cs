using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

class Program
{
    static void Main()
    {
        DateTime today = DateTime.Today;
        Random rnd = new Random();
        string path = Path.Combine(AppContext.BaseDirectory, "bioritmus.sql");

        // Férfi nevek listája
        string[] ferfiNevek = {
            "Aba", "Ábel", "Ádám", "Adolf", "Adorján", "Ágoston", "Ahmed", "Ákos", "Aladár", "Albert",
            "Alex", "Ali", "Álmos", "Ambrus", "Andor", "András", "Antal", "Anton", "Áron", "Árpád",
            "Attila", "Aurél", "Balázs", "Bálint", "Barnabás", "Béla", "Bence", "Bendegúz", "Benedek",
            "Benjámin", "Bertalan", "Botond", "Csaba", "Csongor", "Dániel", "Dávid", "Dénes", "Dezső",
            "Dominik", "Elek", "Elemér", "Emil", "Endre", "Erik", "Ernő", "Ervin", "Ferenc", "Gábor",
            "Gáspár", "Gellért", "Gergely", "Géza", "Gusztáv", "György", "Gyula", "Henrik", "Ignác",
            "Imre", "István", "Iván", "János", "Jenő", "József", "Kálmán", "Károly", "Kevin", "Kornél",
            "Kristóf", "Lajos", "László", "Levente", "Lóránt", "Lukács", "Marcell", "Márió", "Márk",
            "Martin", "Máté", "Mátyás", "Mihály", "Miklós", "Milán", "Nándor", "Norbert", "Olivér",
            "Ottó", "Pál", "Patrik", "Péter", "Richárd", "Róbert", "Roland", "Rudolf", "Sándor",
            "Szabolcs", "Szilárd", "Tamás", "Tibor", "Tivadar", "Viktor", "Vilmos", "Zoltán", "Zsolt", "Zsombor"
        };

        // Női nevek listája (ugyanolyan formában hozzáadva)
        string[] noiNevek = {
            "Abigél", "Adél", "Adrienn", "Ági", "Ágnes", "Alexandra", "Alíz", "Amália", "Amanda", "Andrea",
            "Angéla", "Anikó", "Anita", "Anna", "Annamária", "Antónia", "Aranka", "Barbara", "Beáta", "Beatrix",
            "Bernadett", "Betta", "Bianka", "Boglárka", "Borbála", "Brigitta", "Cecília", "Cintia", "Csenge", "Csilla",
            "Diána", "Dóra", "Dorina", "Dorka", "Dorottya", "Edina", "Edit", "Eleonóra", "Eliza", "Ella",
            "Elvira", "Emese", "Emma", "Enikő", "Erika", "Erzsébet", "Eszter", "Etelka", "Éva", "Evelin",
            "Fanni", "Fatime", "Felícia", "Flóra", "Fruzsina", "Gabriella", "Georgina", "Gerda", "Gertrúd", "Gizella",
            "Gréta", "Gyöngyi", "Györgyi", "Hajnalka", "Hanga", "Hanna", "Hedvig", "Heléna", "Helga", "Henrietta",
            "Ibolya", "Ida", "Ildikó", "Ilona", "Ingrid", "Irén", "Irma", "Ivett", "Izabella", "Jázmin",
            "Johanna", "Jolán", "Judit", "Júlia", "Julianna", "Kamilla", "Karolina", "Katalin", "Kati", "Kitti",
            "Klára", "Klaudia", "Kornélia", "Krisztina", "Laura", "Lea", "Leila", "Léna", "Lilla", "Lívia",
            "Liza", "Luca", "Lujza", "Magdolna", "Maja", "Margit", "Mária", "Mariann", "Márta", "Martina",
            "Matild", "Melinda", "Mercédesz", "Míra", "Mónika", "Natália", "Nikolett", "Noémi", "Nóra", "Olga",
            "Olívia", "Orsolya", "Ottília", "Pálma", "Panna", "Patrícia", "Paula", "Petra", "Piroska", "Polla",
            "Ramóna", "Rebeka", "Regina", "Réka", "Renáta", "Rita", "Roberta", "Rozália", "Rózsa", "Sára",
            "Sarolta", "Szabina", "Szalome", "Szandra", "Szilvia", "Szonja", "Szofi", "Tamara", "Tekla", "Teodóra",
            "Terézia", "Tímea", "Tünde", "Valéria", "Vanessza", "Veronika", "Viktória", "Vilma", "Viola", "Virág",
            "Vivien", "Zita", "Zoé", "Zsanett", "Zsófia", "Zsuzsanna", "Zsuzska"
        };

        Console.WriteLine("Adatgenerálás indítása...");
        Console.WriteLine($"Férfi nevek: {ferfiNevek.Length} db");
        Console.WriteLine($"Női nevek: {noiNevek.Length} db");
        Console.WriteLine("--------------------------------------------------");

        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine(@"
            CREATE TABLE IF NOT EXISTS Bioritmus (
                sorszam INT PRIMARY KEY,
                keresztnev VARCHAR(50),
                nem CHAR(1),
                szuletesi_datum DATE,
                fizikai_ertek DECIMAL(5,2),
                erzelmi_ertek DECIMAL(5,2),
                szellemi_ertek DECIMAL(5,2),
                rogzitve DATE
            );");
            sw.WriteLine("DELETE FROM Bioritmus;");
            sw.WriteLine("BEGIN TRANSACTION;");

            for (int sorszam = 0; sorszam <= 6512; sorszam++)
            {
                // Véletlenszerűen döntünk a nemről (0 vagy 1)
                // Ha 0 -> Férfi, Ha 1 -> Nő
                bool isNo = rnd.Next(2) == 1;

                string nev;
                string nemKod;

                if (isNo)
                {
                    nev = noiNevek[rnd.Next(noiNevek.Length)];
                    nemKod = "N"; // Nő
                }
                else
                {
                    nev = ferfiNevek[rnd.Next(ferfiNevek.Length)];
                    nemKod = "F"; // Férfi
                }

                DateTime szulDatum = RandomSzuletesiDatum(rnd);

                int n = (today - szulDatum).Days;

                double fizikai = SzamolKepletSzerint(n, 26);
                double erzelmi = SzamolKepletSzerint(n, 28);
                double szellemi = SzamolKepletSzerint(n, 33);

                // SQL generálás: 'nem' mező dinamikusan beillesztve a {2} helyre
                string sqlRow = string.Format(CultureInfo.InvariantCulture,
                    "INSERT INTO Bioritmus (sorszam, keresztnev, nem, szuletesi_datum, fizikai_ertek, erzelmi_ertek, szellemi_ertek, rogzitve) " +
                    "VALUES ({0}, '{1}', '{2}', '{3:yyyy-MM-dd}', {4:0.00}, {5:0.00}, {6:0.00}, '{7:yyyy-MM-dd}');",
                    sorszam, nev, nemKod, szulDatum, fizikai, erzelmi, szellemi, today);

                sw.WriteLine(sqlRow);

                // Konzolra írás (csak minden 100-adikat írjuk ki, hogy gyorsabban fusson, de látszódjon a haladás)
                if (sorszam % 100 == 0)
                {
                    Console.WriteLine($"#{sorszam} | {nev} ({nemKod}) | {n} napos");
                    Console.WriteLine($"   F: {fizikai:0.00}% | É: {erzelmi:0.00}% | Sz: {szellemi:0.00}%");
                    Console.WriteLine("--------------------------------------------------");
                }
            }

            sw.WriteLine("COMMIT;");
        }

        Console.WriteLine($"\nKész! Az adatok mentve ide: {path}");
        Console.ReadLine();
    }

    static double SzamolKepletSzerint(int napok, int periodus)
    {
        double maradek = napok % periodus;
        return (maradek / periodus) * 100.0;
    }

    static DateTime RandomSzuletesiDatum(Random rnd)
    {
        int ev = rnd.Next(1960, 2006);
        int honap = rnd.Next(1, 13);
        int nap = rnd.Next(1, DateTime.DaysInMonth(ev, honap) + 1);
        return new DateTime(ev, honap, nap);
    }
}