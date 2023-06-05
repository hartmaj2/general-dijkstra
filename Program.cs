namespace GeneralDijkstra;

using System;
using System.Collections.Generic;

/*
 * A program to demonstrate the usage of delegate functions to create a general Dijkstra's algorithm
 * depending on which functions we use ((+,min) to find shortest path or (min,max) to find path
 * with largest capacity)
 * 
 * INPUT: Two numbers, n - number of vertices, m - number of edges
 *      following m entries are 3 numbers each representing an edge: output vertex, input vertex and weigh
 *      
 * OUTPUT: The result of the General Dijkstra algorithm
 * 
 * EXAMPLE INPUT:
 * 6 7
 * 0 2 3
 * 0 1 2
 * 2 4 1
 * 1 4 3
 * 4 5 1
 * 3 5 4
 * 
 * EXAMPLE SETTINGS: ObecnyDijkstra(0, Secti, Min, graf); cilovyVrchol = 5
 * 
 * EXAMPLE OUTPUT:
 * Path total value 5
 * Path: 5 -> 4 -> 2 -> 0
 */

class Program
{
    static void Main(string[] args)
    {
        OhodnocenyGraf grafik = CteckaOhodnocenychGrafu.NactiVstupDoGrafuAVrat();
        //PisarkaOhodnocenychGrafu.VypisGraf(grafik);

        int pocatecniHodnota = 0;
        ObecnyDijksta dijkstra = new ObecnyDijksta(pocatecniHodnota, Secti, Min, grafik);
        dijkstra.SpocitejNejkratsiCestu(0);

        int cilovyVrchol = 5;
        int nejkratsi = dijkstra.VratDelkuSpocitaneCesty(cilovyVrchol);
        Console.WriteLine($"Path total value {nejkratsi}");
        dijkstra.VypisSpocitanouCestu(cilovyVrchol);

    }

    public static int Min(int x, int y)
    {
        if (x < y) return x;
        return y;
    }

    public static int Max(int x, int y)
    {
        if (x > y) return x;
        return y;
    }

    public static int Secti(int x, int y)
    {
        return x + y;
    }
}

class CteckaOhodnocenychGrafu
{

    private static int Posledni;

    public static OhodnocenyGraf NactiVstupDoGrafuAVrat()
    {
        Posledni = 0;
        int pocetVrcholu = PrectiInt();
        int pocetHran = PrectiInt();
        OhodnocenyGraf graf = new OhodnocenyGraf(pocetVrcholu);
        for (int i = 0; i < pocetHran; i++)
        {
            graf.PridejHranu(PrectiInt(), PrectiInt(), PrectiInt());
        }
        return graf;

    }

    private static int PrectiInt()
    {
        int x = 0;
        int predchozi = 0;
        int znak = Posledni;
        while (znak < '0' || znak > '9')
        {
            predchozi = znak;
            znak = Console.Read();
        }
        while (znak >= '0' && znak <= '9')
        {
            x = 10 * x;
            x += znak - '0';
            znak = Console.Read();
        }
        if (predchozi == '-')
        {
            x = -x;
        }
        Posledni = znak;
        return x;
    }

}

class OhodnocenyGraf
{
    //DULEZITE: VRCHOLY MUSI ZACINAT CISLEM 0 A POSTUPNE JIT AZ DO N - 1 (N je pocet vrcholu)
    // seznam sousedu, ktery pro kazdeho souseda obsahuje vzdalenost do nej
    // pro dany vrchol seznam obsahuje vsechny sousedy DO kterych z daneho vrcholu vede hrana spolu s jeji vahou
    private readonly LinkedList<Tuple<int, int>>[] seznamSouseduSVahami;

    // nainicializuje prazdny graf o danem poctu vrcholu
    public OhodnocenyGraf(int pocetVrcholu)
    {
        seznamSouseduSVahami = new LinkedList<Tuple<int, int>>[pocetVrcholu];
        for (int i = 0; i < seznamSouseduSVahami.Length; i++)
        {
            seznamSouseduSVahami[i] = new LinkedList<Tuple<int, int>>();
        }
    }

    // prida do grafu danou hranu 
    public void PridejHranu(int zVrchol, int doVrchol, int vaha)
    {
        seznamSouseduSVahami[zVrchol].AddLast(new Tuple<int, int>(doVrchol, vaha));
    }

    public LinkedList<Tuple<int, int>> VratSeznamSouseduVrcholu(int vrchol)
    {
        return seznamSouseduSVahami[vrchol];
    }

    public int VratPocetVrcholu() => seznamSouseduSVahami.Length;

    public int VratPocetSousedu(int x) => seznamSouseduSVahami[x].Count;


}

class PisarkaOhodnocenychGrafu
{

    /*
     * Vypise graf, kde vrcholy jsou psany ve formatu cislo:vaha_hrany_do_nej
     */
    public static void VypisGraf(OhodnocenyGraf graf)
    {
        for (int i = 0; i < graf.VratPocetVrcholu(); i++)
        {
            Console.Write($"Vrchol {i} ->");
            foreach (Tuple<int, int> prvek in graf.VratSeznamSouseduVrcholu(i))
            {
                Console.Write($" {prvek.Item1}:{prvek.Item2}");
            }
            Console.WriteLine();
        }
    }
}

public delegate int PorovnavaciFce(int x, int y); // porovnavaci funkce, ktera je vyuzita jak pro dijkstru, tak i pro haldu

class Halda
{

    private Tuple<int, int>[] pole; // prvek je reprezentovan jako index vrcholu (item 1) a jeho hodnota/klic (item 2) 
    private int indexProDalsiPrvek;
    private int[] indexyVHalde;
    private PorovnavaciFce porovnavaci;

    public Halda(int pocetPrvku, PorovnavaciFce porovnavaci)
    {
        pole = new Tuple<int, int>[pocetPrvku + 1];
        indexyVHalde = new int[pocetPrvku];
        indexProDalsiPrvek = 1;
        this.porovnavaci = porovnavaci;

        for (int i = 0; i < indexyVHalde.Length; i++)
        {
            indexyVHalde[i] = -1;
        }
    }

    public void VypisIndexyVHalde()
    {
        Console.WriteLine("Indexy v halde:");
        for (int i = 0; i < indexyVHalde.Length; i++)
        {
            Console.WriteLine($"Vrchol {i} je v halde na indexu {indexyVHalde[i]}");
        }
    }

    /*
     * Vypise haldu, jak je ulozena v poli ve formatu prvek:hodnota (prvek:klic)
     */
    public void VypisObsahHaldy()
    {
        Console.WriteLine("Obsah haldy:");
        for (int i = 1; i < indexProDalsiPrvek; i++)
        {
            Console.Write($" -> {pole[i].Item1}:{pole[i].Item2}");
        }
        Console.WriteLine();
    }

    public int PocetPrvku()
    {
        return indexProDalsiPrvek - 1;
    }

    public void Insert(int indexVrcholu, int vaha)
    {
        pole[indexProDalsiPrvek] = new Tuple<int, int>(indexVrcholu, vaha);

        int aktualniIndexVHalde = indexProDalsiPrvek;

        indexyVHalde[indexVrcholu] = aktualniIndexVHalde;

        VybublejPrvekDoKorene(aktualniIndexVHalde);

        indexProDalsiPrvek++;

    }

    public void Decrease(int indexVrcholu, int novaHodnota)
    {

        int indexVHalde = indexyVHalde[indexVrcholu];

        pole[indexVHalde] = new Tuple<int, int>(indexVrcholu, novaHodnota);

        VybublejPrvekDoKorene(indexVHalde);

    }

    /*
     * Vrati koren jako prvek i s jeho hodnotou (klicem)
     */
    public Tuple<int, int> ExtractRoot()
    {
        Tuple<int, int> root = pole[1];
        indexyVHalde[root.Item1] = -1; // nastavim prave odebrany vrchol, ze se v halde nachazi na indexu -1 (tzn. neni v ni)
        indexyVHalde[pole[indexProDalsiPrvek - 1].Item1] = 1; // index v halde pro vrchol ktery vkladam prechodne do korene nastavim na 1
        pole[1] = pole[indexProDalsiPrvek - 1];
        pole[indexProDalsiPrvek - 1] = new Tuple<int, int>(-1, -1); // aby bylo poznat, ze posledni prvek byl odstraneny
        indexProDalsiPrvek--;

        //Nyni bublame nahoru

        int aktualniIndex = 1;

        while (true)
        {
            int indexPraveho = (aktualniIndex * 2) + 1;
            int indexLeveho = aktualniIndex * 2;

            int hodnotaAktualniho = pole[aktualniIndex].Item2;

            if (indexPraveho >= indexProDalsiPrvek) // pravy syn neexistuje
            {
                if (indexLeveho >= indexProDalsiPrvek) break; // oba synove neexistuji takze konec

                //existuje pouze levy syn
                int hodnotaLeveho = pole[indexLeveho].Item2;

                int vysledekPorovnani = porovnavaci(hodnotaLeveho, hodnotaAktualniho);

                if (hodnotaLeveho != vysledekPorovnani) break; // pokud neni nutne prohazovat, tak uz muzeme skoncit

                // je nutne prohodit prvky, levy syn je mensi/vetsi nez rodic

                ProhodPrvkyNaIndexech(indexLeveho, aktualniIndex);
                aktualniIndex = indexLeveho;

            }
            else // oba synove existuji
            {
                int hodnotaLeveho = pole[indexLeveho].Item2;
                int hodnotaPraveho = pole[indexPraveho].Item2;

                int vysledekTrojitehoPorovnani = porovnavaci(hodnotaAktualniho, porovnavaci(hodnotaLeveho, hodnotaPraveho));

                if (vysledekTrojitehoPorovnani == hodnotaAktualniho) break; // aktualni prvek je nejmensi/nejvetsi takze nemusim uz nic delat
                else if (vysledekTrojitehoPorovnani == hodnotaLeveho) // prohazuji s levym synem
                {
                    ProhodPrvkyNaIndexech(indexLeveho, aktualniIndex);
                    aktualniIndex = indexLeveho;
                }
                else // prohazuji s pravym synem
                {
                    ProhodPrvkyNaIndexech(indexPraveho, aktualniIndex);
                    aktualniIndex = indexPraveho;
                }

            }

        }

        return root;

    }

    private void ProhodPrvkyNaIndexech(int prvni, int druhy)
    {
        int prvniVrchol = pole[prvni].Item1;
        int druhyVrchol = pole[druhy].Item1;

        int pomocna = indexyVHalde[prvniVrchol];
        indexyVHalde[prvniVrchol] = indexyVHalde[druhyVrchol];
        indexyVHalde[druhyVrchol] = pomocna;

        Tuple<int, int> pomocnyTuple = pole[prvni];
        pole[prvni] = pole[druhy];
        pole[druhy] = pomocnyTuple;

    }

    private void VybublejPrvekDoKorene(int index)
    {

        int predek = index / 2;

        while (true)
        {
            if (predek == 0) break;

            int vysledekPorovnani = porovnavaci(pole[predek].Item2, pole[index].Item2);

            // predek byl vyhodnocen jako min/max pro nasi min/max haldu
            if (vysledekPorovnani == pole[index].Item2)
            {
                ProhodPrvkyNaIndexech(index, predek);

                index /= 2;
                predek /= 2;
            }
            else break;
        }
    }
}

class ObecnyDijksta
{

    public delegate int AditivniFce(int x, int y);

    private AditivniFce aditivni;
    private PorovnavaciFce porovnavaci;

    private OhodnocenyGraf graf;
    private Halda prioritniFronta;
    private bool[] uzavreny;
    private int[] predkove;
    private int[] nejkratsiVzdalenosti;
    private int pocatecniHodnota;

    public ObecnyDijksta(int pocatecniHodnota, AditivniFce aditivni, PorovnavaciFce porovnavaci, OhodnocenyGraf graf)
    {
        this.pocatecniHodnota = pocatecniHodnota;
        this.aditivni = aditivni;
        this.porovnavaci = porovnavaci;
        this.graf = graf;

        predkove = new int[graf.VratPocetVrcholu()];
        nejkratsiVzdalenosti = new int[graf.VratPocetVrcholu()];
        uzavreny = new bool[graf.VratPocetVrcholu()];

        for (int i = 0; i < predkove.Length; i++)
        {
            predkove[i] = -1;
            nejkratsiVzdalenosti[i] = -1;
            uzavreny[i] = false;
        }

        prioritniFronta = new Halda(graf.VratPocetVrcholu(), porovnavaci);

    }

    public void SpocitejNejkratsiCestu(int indexPocatku)
    {

        // pocatecni hodnota neni definovana explicitne, jelikoz pro hledani max cesty to musi byt neco velkeho a naopak
        prioritniFronta.Insert(indexPocatku, pocatecniHodnota);
        nejkratsiVzdalenosti[indexPocatku] = pocatecniHodnota;


        while (prioritniFronta.PocetPrvku() != 0)
        {

            Tuple<int, int> aktualniPrvek = prioritniFronta.ExtractRoot();
            int aktualniVrchol = aktualniPrvek.Item1;
            //if (uzavreny[aktualniVrchol]) continue;
            uzavreny[aktualniVrchol] = true;

            //Console.WriteLine($"Prohledavam vrchol s indexem {aktualniVrchol}");

            foreach (Tuple<int, int> ohodnocenaHrana in graf.VratSeznamSouseduVrcholu(aktualniVrchol))
            {
                int soused = ohodnocenaHrana.Item1;
                int vaha = ohodnocenaHrana.Item2;

                if (!uzavreny[soused])
                {

                    if (nejkratsiVzdalenosti[soused] == -1) // soused jeste nebyl navstiven, takze ho bude potreba pridat do haldy
                    {
                        predkove[soused] = aktualniVrchol;
                        nejkratsiVzdalenosti[soused] = aditivni(nejkratsiVzdalenosti[aktualniVrchol], vaha);
                        //frontaOtevrenychVrcholu.Enqueue(soused, nejkratsiVzdalenosti[soused]);
                        prioritniFronta.Insert(soused, nejkratsiVzdalenosti[soused]);
                    }
                    else
                    {
                        // pokud porovnavaci funkce vratila jinou hodnotu nez tu, kterou tam mel soused doposud, tak relaxujeme vrchol
                        if (nejkratsiVzdalenosti[soused] != porovnavaci(aditivni(nejkratsiVzdalenosti[aktualniVrchol], vaha), nejkratsiVzdalenosti[soused]))
                        {
                            predkove[soused] = aktualniVrchol;
                            nejkratsiVzdalenosti[soused] = porovnavaci(aditivni(nejkratsiVzdalenosti[aktualniVrchol], vaha), nejkratsiVzdalenosti[soused]);
                            //frontaOtevrenychVrcholu.Enqueue(soused, nejkratsiVzdalenosti[soused]);
                            prioritniFronta.Decrease(soused, nejkratsiVzdalenosti[soused]);
                        }
                    }
                }

            }

        }

    }

    public int VratDelkuSpocitaneCesty(int indexCile)
    {
        return nejkratsiVzdalenosti[indexCile];
    }

    public void VypisSpocitanouCestu(int indexCile)
    {
        int predek = indexCile;
        Console.Write($"Path: {predek}");
        while (true)
        {
            predek = predkove[predek];
            if (predek == -1) break;
            Console.Write($" -> {predek}");
        }
        Console.WriteLine();
    }

}

