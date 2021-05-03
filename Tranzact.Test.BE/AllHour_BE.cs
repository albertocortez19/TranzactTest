using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tranzact.Test.BE
{
    public class AllHour_BE
    {
        public string DOMAIN_CODE { get; set; }
        public string PAGE_TITLE { get; set; }
        public int count_view { get; set; }
    }
    public class AllHour_BEtemp : IEquatable<AllHour_BEtemp>
    {
        public string DOMAIN_CODE { get; set; }
        public string PAGE_TITLE { get; set; }

        public bool Equals(AllHour_BEtemp other)
        {
            return this.DOMAIN_CODE.ToLower() == other.DOMAIN_CODE.ToLower() && this.PAGE_TITLE.ToLower() == other.PAGE_TITLE.ToLower();
        }
    }
}
