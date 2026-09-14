using System;
using System.IO;
using System.Linq;

namespace DnsAnalizis
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "DNA.txt";
            string outputFile = "DNA-result.txt";

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Hiba: A '{inputFile}' fájl nem található!");
                return;
            }

            string header = "";
            long countA = 0, countC = 0, countG = 0, countT = 0;
            long totalLength = 0;

            // STREAM OLVASÁS: Nem töltjük be a teljes fájlt a memóriába, soronként dolgozzuk fel
            using (StreamReader sr = new StreamReader(inputFile))
            {
                // Első sor a fejléc / leírás (nem része a DNS szekvenciának)
                header = sr.ReadLine() ?? "";

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    foreach (char ch in line)
                    {
                        char c = char.ToUpper(ch);
                        switch (c)
                        {
                            case 'A': countA++; totalLength++; break;
                            case 'C': countC++; totalLength++; break;
                            case 'G': countG++; totalLength++; break;
                            case 'T': countT++; totalLength++; break;
                        }
                    }
                }
            }

            // ----------------------------------------------------
            // 1. Feladat: Szekvencia hossza
            // ----------------------------------------------------
            Console.WriteLine("=== 1. FELADAT ===");
            Console.WriteLine($"A DNS-bázis szekvencia hossza: {totalLength} bázis.\n");

            // ----------------------------------------------------
            // 2. Feladat: Bázisok darabszáma és százalékos aránya
            // ----------------------------------------------------
            double pctA = totalLength > 0 ? (double)countA / totalLength * 100 : 0;
            double pctC = totalLength > 0 ? (double)countC / totalLength * 100 : 0;
            double pctG = totalLength > 0 ? (double)countG / totalLength * 100 : 0;
            double pctT = totalLength > 0 ? (double)countT / totalLength * 100 : 0;

            Console.WriteLine("=== 2. FELADAT ===");
            Console.WriteLine("Bázisok statisztikája:");
            Console.WriteLine($"  A: {countA,10} db ({pctA,6:F2}%)");
            Console.WriteLine($"  C: {countC,10} db ({pctC,6:F2}%)");
            Console.WriteLine($"  G: {countG,10} db ({pctG,6:F2}%)");
            Console.WriteLine($"  T: {countT,10} db ({pctT,6:F2}%)");
            Console.WriteLine();

            // ----------------------------------------------------
            // 3. Feladat: Átlagtól való szignifikáns eltérés (> 5%)
            // ----------------------------------------------------
            Console.WriteLine("=== 3. FELADAT ===");
            double avg = (double)totalLength / 4;
            Console.WriteLine($"Bázisok átlagos száma: {avg:F2}");

            var baseData = new (char Symbol, long Count, double Pct)[]
            {
                ('A', countA, pctA),
                ('C', countC, pctC),
                ('G', countG, pctG),
                ('T', countT, pctT)
            };

            bool hasSignificantDiff = false;
            foreach (var b in baseData)
            {
                double diffPct = Math.Abs(b.Count - avg) / avg * 100;
                if (diffPct > 5.0)
                {
                    string direction = b.Count > avg ? "nagyobb" : "kisebb";
                    Console.WriteLine($"  - A(z) '{b.Symbol}' bázis száma ({b.Count}) szignifikánsan eltér az átlagtól ({diffPct:F2}%-kal {direction}).");
                    hasSignificantDiff = true;
                }
            }

            if (!hasSignificantDiff)
            {
                Console.WriteLine("  - Egyik bázis száma sem tér el szignifikánsan (>5%) az átlagtól.");
            }
            Console.WriteLine();

            // ----------------------------------------------------
            // 4. Feladat: Vízszintes oszlopdiagram (* karakterekből)
            // ----------------------------------------------------
            Console.WriteLine("=== 4. FELADAT ===");
            Console.WriteLine("Bázisok oszlopdiagramja:");

            long maxCount = baseData.Max(b => b.Count);
            int maxBarWidth = 50; // Maximum szélesség a képernyőn sortörés ellen

            foreach (var b in baseData)
            {
                int barLength = maxCount > 0 ? (int)Math.Round((double)b.Count / maxCount * maxBarWidth) : 0;
                string bar = new string('*', barLength);
                Console.WriteLine($"{b.Symbol} | {bar} ({b.Count} db)");
            }
            Console.WriteLine();

            // ----------------------------------------------------
            // 5. Feladat: Eredmények mentése a DNA-result.txt állományba
            // ----------------------------------------------------
            using (StreamWriter sw = new StreamWriter(outputFile))
            {
                sw.WriteLine(header);
                sw.WriteLine($"A\t{countA}\t{pctA:F2}%");
                sw.WriteLine($"C\t{countC}\t{pctC:F2}%");
                sw.WriteLine($"G\t{countG}\t{pctG:F2}%");
                sw.WriteLine($"T\t{countT}\t{pctT:F2}%");
            }

            Console.WriteLine("=== 5. FELADAT ===");
            Console.WriteLine($"Az eredmények sikeresen elmentve a '{outputFile}' állományba.");

            Console.WriteLine("\nNyomjon meg egy gombot a kilépéshez...");
            Console.ReadKey();
        }
    }
}