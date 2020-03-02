using System;

namespace JhpDataSystem.projects.ppx
{
    public class PPDeviceSizes
    {
        internal bool isMonth;
        public PPDeviceSizes(DateTime placementDate)
        {
            setUp(placementDate.Day, 
                placementDate.Month, 
                placementDate.Year);
        }

        public PPDeviceSizes(int day, int month, int year)
        {
            setUp(day, month, year);
        }
        public void setUp(int day, int month, int year)
        {
            var dateValue = string.Format("{0}/{1}/{2}",
                (isMonth?"--":
                day < 10 ? "0" + day : day.ToString()),
                month < 10 ? "0" + month : month.ToString()
                , year);
            ddMMyyy = dateValue;
            PlacementDate = new DateTime(year, month, day);
            Unk = A = B = C = D = E = 0;
        }
        public DateTime PlacementDate { get; set; }
        public string ddMMyyy { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int E { get; set; }
        public int Unk { get; set; }

        public static string getHeader()
        {
            return "Date,           \t\tA,\tB,\tC,\tD,\tE,\tUnk,\t\t\tTotal";
        }

        public string toDisplay()
        {
            return string.Format("{0},\t\t{1},\t{2},\t{3},\t{4},\t{5},\t{6},\t\t\t\t{7}", ddMMyyy, A, B, C, D, E, Unk, (A + B + C + D + E + Unk));
        }
        public string size { get; set; }
        internal void Add(string deviceSize)
        {
            size = deviceSize;
            if (string.IsNullOrWhiteSpace(deviceSize))
            {
                Unk += 1;
                return;
            }

            var asLower = deviceSize.ToLowerInvariant();
            if (asLower == "a")
                A += 1;
            else if (asLower == "b")
                B += 1;
            else if (asLower == "c")
                C += 1;
            else if (asLower == "d")
                D += 1;
            else if (asLower == "e")
                E += 1;
            else
                Unk += 1;
        }
    }
}