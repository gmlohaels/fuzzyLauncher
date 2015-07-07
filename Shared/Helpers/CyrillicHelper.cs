using System.Collections.Generic;
using System.Text;

namespace Shared.Helpers
{
    public static class CyrillicHelper
    {


        private const string Cyrillic = "ЙйЦцУуКкЕеНнГгШшЩщЗзХхЪъФфЫыВвАаПпРрОоЛлДдЖжЭэЯяЧчСсМмИиТтЬьБбЮюЁё";
        private const string LatinSameKey = "QqWwEeRrTtYyUuIiOoPp{[}]AaSsDdFfGgHhJjKkLl:;\"'ZzXxCcVvBbNnMm<,>.~`";

        public static string ConvertLayout(string input)
        {

            var b = new StringBuilder(input.Length);


            var ruToLat = new Dictionary<char, char>();
            var latToRus = new Dictionary<char, char>();

            for (int x = 0; x < Cyrillic.Length; x++)
            {
                ruToLat.Add(Cyrillic[x], LatinSameKey[x]);
                latToRus.Add(LatinSameKey[x], Cyrillic[x]);
            }


            foreach (var s in input)
            {

                if (ruToLat.ContainsKey(s))
                {
                    b.Append(ruToLat[s]);
                    continue;
                }


                if (latToRus.ContainsKey(s))
                {
                    b.Append(latToRus[s]);
                    continue;
                }

                b.Append(s);

            }


            return b.ToString();
        }
    }
}
