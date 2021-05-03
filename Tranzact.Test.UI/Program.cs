using System;
using Tranzact.Test.BE;
using Tranzact.Test.BL;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Tranzact.Test.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            string rutaInput = ConfigurationManager.AppSettings["rutaInput"].ToString();
            string rutaOutput = ConfigurationManager.AppSettings["rutaOutput"].ToString();


            AllHour_BL oAllHour_BL = new AllHour_BL();
            oAllHour_BL.downloadFiles(rutaInput, rutaOutput);

            List<AllHour_BE> listHours = new List<AllHour_BE>();
            listHours = oAllHour_BL.GetMaxCountViews(rutaOutput);

            foreach(var element in listHours)
            {
                Console.WriteLine(element.DOMAIN_CODE + "   "+ element.PAGE_TITLE + "  " + element.count_view);

            }
            Console.WriteLine();
        }
    }
}
