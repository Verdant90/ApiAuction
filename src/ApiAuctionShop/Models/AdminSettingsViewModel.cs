using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class AdminSettingsViewModel
    {
        public AdminMenuModel adminMenuModel;

        [Required(ErrorMessage = "This field must not be empty.")]
        [NumbersWithLetterSeparatedByCommaAttribute(ErrorMessage = "Must be a list of numbers separated by a comma!")]
        public string timePeriods { get; set; }

        public bool hasBuyNow { get; set; }

        public string colorTheme { get; set; }

        public string photoSize { get; set; }

        public string startMessage { get; set; }

        public AdminSettingsViewModel()
        {
            adminMenuModel = new AdminMenuModel(0, 0, 0, 0, 0);
        }
    }


    public class NumbersWithLetterSeparatedByCommaAttribute : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            string strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                Regex r = new Regex("([1-9][0-9]*[hdw])(','[1-9][0-9]*[hdw])*");
                List<string> periods = strValue.Split(',').ToList();
                foreach(string s in periods)
                {
                    if (!r.IsMatch(s)) return false;
                }
            }
            return true;
        }
    }
}
