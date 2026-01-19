using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        DateTime today = DateTime.Today;
        Random rnd = new Random();
        string path = Path.Combine(AppContext.BaseDirectory, "bioritmus.sql");

        string[] nevek = {
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

        Console.WriteLine("Adatgenerálás indítása...");
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
                string nev = nevek[rnd.Next(nevek.Length)];
                DateTime szulDatum = RandomSzuletesiDatum(rnd);

                int n = (today - szulDatum).Days;

                double fizikai = SzamolKepletSzerint(n, 26);
                double erzelmi = SzamolKepletSzerint(n, 28);
                double szellemi = SzamolKepletSzerint(n, 33);

                string sqlRow = string.Format(CultureInfo.InvariantCulture,
                    "INSERT INTO Bioritmus (sorszam, keresztnev, nem, szuletesi_datum, fizikai_ertek, erzelmi_ertek, szellemi_ertek, rogzitve) " +
                    "VALUES ({0}, '{1}', 'F', '{2:yyyy-MM-dd}', {3:0.00}, {4:0.00}, {5:0.00}, '{6:yyyy-MM-dd}');",
                    sorszam, nev, szulDatum, fizikai, erzelmi, szellemi, today);

                sw.WriteLine(sqlRow);

                Console.WriteLine($"#{sorszam} | {nev} ({n} napos)");
                Console.WriteLine($"   Fizikai (26):  {fizikai:0.00}%");
                Console.WriteLine($"   Érzelmi (28):  {erzelmi:0.00}%");
                Console.WriteLine($"   Szellemi (33): {szellemi:0.00}%");
                Console.WriteLine("--------------------------------------------------");
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