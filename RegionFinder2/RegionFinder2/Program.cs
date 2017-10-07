using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegionFinder2
{

    class MainClass
    {

        //List of all markers
        private static List<Marker> markers = new List<Marker>();
        //List of all regions
        private static List<Region> regions = new List<Region>();
        //List of list of all markers arranged by chromosome
        private static List<List<Marker>> mbc = new List<List<Marker>>();
        //List of list of all markers arranged by chromosome
        private static List<List<Region>> rbc = new List<List<Region>>();


        public class Marker
        {
            int position;
            String name;
            int chr;
            double p;
            int region;
            int isSignificant;
            int hasNeighbor;
            int sumSigHasNeighbor;
            bool upstreamLimit;
            bool downstreamLimit;
            bool suggestive;

            public int Position { get => position; set => position = value; }
            public int Chr { get => chr; set => chr = value; }
            public double P { get => p; set => p = value; }
            public int Region { get => region; set => region = value; }
            public int IsSignificant { get => isSignificant; set => isSignificant = value; }
            public int HasNeighbor { get => hasNeighbor; set => hasNeighbor = value; }
            public int SumSigHasNeighbor { get => sumSigHasNeighbor; set => sumSigHasNeighbor = value; }
            public bool UpstreamLimit { get => upstreamLimit; set => upstreamLimit = value; }
            public bool DownstreamLimit { get => downstreamLimit; set => downstreamLimit = value; }
            public bool Suggestive { get => suggestive; set => suggestive = value; }
            public string Name { get => name; set => name = value; }
        }

        public class Region
        {
            int regionNumber;
            int chr;
            int regionStart;
            int regionStop;
            int numSigMarkers;
            int numSugMarkers;
            int numTotalMarkers;
            int sizeOfRegion;
            String minMarkerName;
            int minMarkerPosition;
            double minP;

            public int RegionNumber { get => regionNumber; set => regionNumber = value; }
            public int Chr { get => chr; set => chr = value; }
            public int RegionStart { get => regionStart; set => regionStart = value; }
            public int RegionStop { get => regionStop; set => regionStop = value; }
            public int NumSigMarkers { get => numSigMarkers; set => numSigMarkers = value; }
            public int NumSugMarkers { get => numSugMarkers; set => numSugMarkers = value; }
            public int NumTotalMarkers { get => numTotalMarkers; set => numTotalMarkers = value; }
            public int SizeOfRegion { get => sizeOfRegion; set => sizeOfRegion = value; }
            public string MinMarkerName { get => minMarkerName; set => minMarkerName = value; }
            public int MinMarkerPosition { get => minMarkerPosition; set => minMarkerPosition = value; }
            public double MinP { get => minP; set => minP = value; }
        }

        public static void ReadFile(String file)
        {



            StreamReader sr;

            sr = new StreamReader(file);

            String currentLine;

            String[] currentTokens;

            sr.ReadLine();

            Regex r = new Regex(" +");


            while (!sr.EndOfStream)
            {
                currentLine = sr.ReadLine();

                currentTokens = r.Split(currentLine);

                Marker currentMarker = new Marker();

                currentMarker.Name = currentTokens[0];
                currentMarker.Chr = Convert.ToInt32(currentTokens[1]);
                currentMarker.Position = Convert.ToInt32(currentTokens[2]);

                try
                {

                    currentMarker.P = Convert.ToDouble(currentTokens[3]);

                }

                catch (Exception e)
                {
                    Console.Write(e.Message + Environment.NewLine);
                    Console.Write("Marker " + currentMarker.Name + " has an invalid P value." + Environment.NewLine);
                    currentMarker.P = 1.00;
                }


                markers.Add(currentMarker);

            }

            sr.Close();

        }

        private static void sortMarkersByChromosome()
        {

            for (int x = 1; x <= 23; x++)
            {

                List<Marker> lm = new List<Marker>();

                foreach (Marker m in markers)
                {

                    if (m.Chr == x)
                    {

                        lm.Add(m);
                    }

                }

                mbc.Add(lm);

            }
        }

        private static void sortRegionsByChromosome()
        {

            for (int x = 1; x <= 23; x++)
            {



                foreach (Region r in regions)
                {

                    if (r.Chr == x)
                    {

                        List<Region> lr = new List<Region>();

                        lr.Add(r);
                        rbc.Add(lr);
                    }

                }



            }
        }

        private static void writeFile(String file)
        {




            StreamWriter sw = new StreamWriter(file);




			sw.WriteLine("Region\t"
				+ "MarkerName\t\t" + "Chr\t\t" + "Position\t\t"
				+ "P-value\t\t" + "RegionStart\t\t" + "RegionStop\t\t"
				+ "NumSigMarkers\t" + "NumSugMarkers\t"
				+ "NumTotalMarkers\t" + "RegionSize");




			foreach (List<Region> l in rbc)
            {

                foreach (Region r in l)
                {


                    sw.WriteLine( "{0,0}{1,15}{2,10}{3,15}{4,15}{5,15}{6,15}{7,15}{8,15}{9,15}{10,15}",
                        r.RegionNumber,
                        r.MinMarkerName,
                        r.Chr,
                        r.MinMarkerPosition,
                        r.MinP,
                        r.RegionStart,
                        r.RegionStop,
                        r.NumSigMarkers,
                        r.NumSugMarkers,
                        r.NumTotalMarkers,
                        r.SizeOfRegion);


                    sw.Flush();

                }

            }

            sw.Close();
        }


        private static void characterizeMarkers(double index, double suggestive, int space)
        {

            foreach (List<Marker> l in mbc)
            {

                List<Marker> sugs = new List<Marker>();

                foreach (Marker m in l)
                {
                    if (m.P < suggestive)
                    {
                        m.Suggestive = true;
                        sugs.Add(m);
                    }

                }

                sugs.OrderBy(o => o.Position);



                foreach (Marker m in sugs)
                {

                    if (m.P < index)
                    {
                        m.IsSignificant = 1;
                    }

                }

                foreach (Marker m in sugs)
                {

                    if (m.IsSignificant == 1)
                    {
                        foreach (Marker s in sugs)
                        {
                            if (Math.Abs(m.Position - s.Position) <= space)
                            {
                                m.HasNeighbor = 1;
                                s.HasNeighbor = 1;
                            }
                        }
                    }
                }

                foreach (Marker m in sugs)
                {
                    if (m.HasNeighbor == 1)
                    {

                        foreach (Marker s in sugs)
                        {
                            if (Math.Abs(m.Position - s.Position) <= space)
                            {

                                s.HasNeighbor = 1;
                            }
                        }
                    }

                }

                foreach (Marker m in sugs)
                {
                    m.SumSigHasNeighbor = (m.HasNeighbor + m.IsSignificant);
                }

                for (int x = 1; x < sugs.Count() - 1; x++)
                {



                    if ((sugs[x - 1].SumSigHasNeighbor == 0)
                            && (sugs[x].SumSigHasNeighbor != 0)
                            && (sugs[x + 1].SumSigHasNeighbor == 0))
                    {

                        sugs[x].UpstreamLimit = true;
                        sugs[x].DownstreamLimit = true;
                    }

                    if ((sugs[x - 1].SumSigHasNeighbor == 0)
                            && ((sugs[x].SumSigHasNeighbor) != 0)
                            && (sugs[x + 1].SumSigHasNeighbor != 0))
                    {

                        sugs[x].UpstreamLimit = true;

                    }

                    if ((sugs[x - 1].SumSigHasNeighbor != 0)
                            && (sugs[x].SumSigHasNeighbor != 0)
                            && (sugs[x + 1].SumSigHasNeighbor == 0))
                    {

                        sugs[x].DownstreamLimit = true;

                    }

                    if ((x - 1 == 0)
                            && (sugs[x - 1].SumSigHasNeighbor != 0)
                        && (sugs[x].SumSigHasNeighbor != 0))
                    {

                        sugs[x-1].UpstreamLimit = true;

                    }

                    if ((sugs[x].SumSigHasNeighbor != 0)
                            && (sugs[x + 1].SumSigHasNeighbor != 0)
                        && (x + 2 == sugs.Count))
                    {

                        sugs[x + 1].DownstreamLimit = true;

                    }




                }

            }

        }
        private static void characterizeRegions()
        {

            foreach (List<Marker> l in mbc)
            {

                List<Marker> sugs = new List<Marker>();

                foreach (Marker m in l)
                {

                    if (m.Suggestive == true)
                    {

                        sugs.Add(m);

                    }

                }

                foreach (Marker m in sugs)
                {

                    foreach (Marker n in sugs)
                    {

                        if (m.UpstreamLimit && n.DownstreamLimit
                            && (n.Position > m.Position))
                        {





                            Region r = new Region();

                            r.RegionStart = m.Position;
                            r.RegionStop = n.Position;
                            r.Chr = m.Chr;

                            int totalMarkers = 0;

                            foreach (Marker o in l)
                            {
                                if ((o.Position >= r.RegionStart)
                            && o.Position <= r.RegionStop)
                                {
                                    totalMarkers += 1;
                                }
                                r.NumTotalMarkers = totalMarkers;
                            }

                            int sugMarkers = 0;

                            int sigMarkers = 0;

                            foreach (Marker p in sugs)
                            {

                                if (p.Position >= r.RegionStart
                                            && (p.Position <= r.RegionStop))
                                {
                                    sugMarkers += 1;
                                    if (p.IsSignificant == 1)
                                    {
                                        sigMarkers += 1;

                                    }
                                }
                            }
                            r.NumSigMarkers = sigMarkers;
                            r.NumSugMarkers = sugMarkers;
                            r.SizeOfRegion = r.RegionStop - r.RegionStart + 1;

                            List<Marker> markersInRegion = new List<Marker>();

                            foreach (Marker o in sugs)
                            {

                                if (o.Position >= r.RegionStart
                                            && o.Position <= r.RegionStop)
                                {
                                    markersInRegion.Add(o);
                                }

                            }
                           


                            r.MinP = markersInRegion.Min(o => o.P);

                            foreach(Marker mm in markersInRegion){

                                if (mm.P == r.MinP){
                                    r.MinMarkerPosition = mm.Position;
                                    r.MinMarkerName = mm.Name;

								}
                            }



                            regions.Add(r);

                            foreach (Marker q in sugs)
                            {
                                if ((q.Position > r.RegionStart) &&
                                (q.Position < r.RegionStop) &&
                            (q.SumSigHasNeighbor == 0))
                                {

                                    regions.Remove(r);

                                }

                            }


                        }
                    }

                }

            }

        }

        public static void addRegionNumber()
        {

            int regionNum = 0;

            foreach (List<Region> l in rbc)
            {
                l.OrderBy(o => o.RegionStart);


                for (int x = 0; x <= l.Count() - 1; x++)
                {

                    regionNum += 1;

                    l[x].RegionNumber = regionNum;

                }

            }

        }



        public static void Main(string[] args)
        {
            try
            {
                ReadFile("input.txt");


            }

            catch (FileNotFoundException e)
            {

                Console.Write(e.Message + Environment.NewLine);
                Console.Write("Please check file name and directory and try again");
            }

            sortMarkersByChromosome();

            characterizeMarkers(
                    0.00001,
                    0.0001,
                   500000

            );




            characterizeRegions();

            sortRegionsByChromosome();



            addRegionNumber();


            try
            {
                writeFile("output.txt");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message + Environment.NewLine);
                Console.Write("Could not write to file.");

                return;
            }



        }
    }
}


