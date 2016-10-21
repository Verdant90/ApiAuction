using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class AdminSettingsViewModel
    {
        public AdminMenuModel adminMenuModel;

        [Required(ErrorMessage = "This field must not be empty.")]
        [NumbersSeparatedByComma(ErrorMessage = "Must be a list of numbers separated by a comma!")]
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


    public class NumbersSeparatedByCommaAttribute : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            string strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                List<string> numbers = strValue.Split(',').ToList();
                foreach(string s in numbers)
                {
                    foreach(char c in s)
                    {
                        if (c < '0' || c > '9')
                            return false;
                    }
                }
            }
            return true;
        }
    }
}
