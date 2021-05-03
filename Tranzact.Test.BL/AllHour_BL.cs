using System;
using System.Collections.Generic;
using System.Text;
using Tranzact.Test.BE;
using System.Linq;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Tranzact.Test.BL
{
    public class AllHour_BL
    {
        public List<AllHour_BE> GetMaxCountViews( string rutaInput)
        {
            List<AllHour_BE> listHours = new List<AllHour_BE>();
            listHours = pullListHour(listHours, rutaInput);

            List<AllHour_BE> listHoursgrouped = new List<AllHour_BE>();
            List<AllHour_BE> listA = listHours.GroupBy(x => new { DOMAIN_CODE = x.DOMAIN_CODE.ToLower(), PAGE_TITLE = x.PAGE_TITLE.ToLower() }).Select(g => new AllHour_BE() { DOMAIN_CODE = g.Key.DOMAIN_CODE, PAGE_TITLE = g.Key.PAGE_TITLE, count_view = g.Count() }).ToList();
            List<AllHour_BE> listB = listA.GroupBy(x => x.DOMAIN_CODE).Select(g => new AllHour_BE() { DOMAIN_CODE = g.Key.ToString(), count_view = g.Max(x => x.count_view) }).ToList();
            List<AllHour_BE> listHoursSort = listA.Join(listB, a => new { a.DOMAIN_CODE, a.count_view }, b => new { b.DOMAIN_CODE, b.count_view }, (a, b) => new AllHour_BE() { DOMAIN_CODE = a.DOMAIN_CODE, PAGE_TITLE = a.PAGE_TITLE, count_view = a.count_view }).OrderByDescending(x => x.count_view).Take(100).ToList();

            return listHoursSort;
        }

        public List<AllHour_BE> pullListHour(List<AllHour_BE> ListHour, string rutaDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(rutaDirectory);
            FileInfo[] Files = d.GetFiles();
            foreach (FileInfo file in Files)
            {
                string rutaFile = Path.Combine(rutaDirectory, file.Name);
                string text = File.ReadAllText(rutaFile);
                var textLine = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                AllHour_BE oAllHour_BE;

                foreach (var element in textLine)
                {
                    var elementLine = element.Split(' ');
                    Regex rgx = new Regex(@"^.+\s.+\s\d\s\d$");
                    if (rgx.IsMatch(element)&&elementLine.Length==4)
                    {
                        oAllHour_BE = new AllHour_BE();

                        oAllHour_BE.DOMAIN_CODE = elementLine[0];
                        oAllHour_BE.PAGE_TITLE = elementLine[1];
                        oAllHour_BE.count_view = Convert.ToInt32(elementLine[2]);
                        ListHour.Add(oAllHour_BE);
                    }
                }
            }


            return ListHour;
        }
        public void downloadFiles(string rutainput, string rutaoutput)
        {
            string url = "https://dumps.wikimedia.org/other/pageviews/";

            List<string> urls = new List<string>();
            using (var client = new WebClient())
            {
                for (int i = 1; i <= 5; i++)
                {
                    string urlTemp = "";
                    string nameFile = "";
                    DateTime d = DateTime.Now.AddHours(-i);
                    urlTemp = string.Format("{0}{1}/{1}-{2:d2}/pageviews-{4:yyyyMMdd}-{3:d2}0000.gz", url, d.Year, d.Month, d.Hour, d);
                    nameFile = string.Format("pageviews-{0:yyyyMMdd}-{1:d2}0000.gz", d, d.Hour);
                    client.DownloadFile(urlTemp, Path.Combine(rutainput, nameFile));
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                string urlTemp = "";
                string nameFileInput = "";
                string nameFileOutput = "";
                DateTime d = DateTime.Now.AddHours(-i);
                nameFileInput = Path.Combine(rutainput, string.Format("pageviews-{0:yyyyMMdd}-{1:d2}0000.gz", d, d.Hour));
                nameFileOutput = Path.Combine(rutaoutput, string.Format("pageviews-{0:yyyyMMdd}-{1:d2}0000.txt", d, d.Hour));
                unZipFile(nameFileInput, nameFileOutput);
            }
        }

        public void unZipFile(string nameFileInput, string tramaOutput)
        {
            FileInfo fileToDecompress = new FileInfo(nameFileInput);
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(tramaOutput))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }
        }
    }
}
