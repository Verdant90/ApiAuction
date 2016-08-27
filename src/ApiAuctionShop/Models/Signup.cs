
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ApiAuctionShop.Models
{
    public class Signup : IdentityUser
    {
        public override string Email { get; set; }
        public string ExpireTokenTime { get; set; }
        public bool IsTokenConfirmed { get; set; }
        public string Token { get; set; }
        //musi być zadeklarowana bo inaczej Auction traktuje jak zwykły object = null
        //auctions of a user
        public ICollection<Auctions> Auction { get; set; } = new List<Auctions>();

        public virtual ICollection<Auctions> AuctionsWon { get; set; }
    }

    public class Auctions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string author { get; set; }
        public string winnerID { get; set; }
        public virtual Signup winner { get; set; }

        //w perspektywie: wiecej zdjec
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "The description can't be empty!")]
        public string description { get; set; }

        //zmienic na decimal(2)
        public int price { get; set; }
        [Required(ErrorMessage = "Please enter the title for this auction.")]
        public string title { get; set; }
        public Signup Signup { get; set; }

        //id aukcji (rzeczywiste) 
        [Column("SignupId")]
        public string SignupId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "The start price must be greater than 0!")]
        public decimal startPrice { get; set; }

        [GreaterThan("startPrice", ErrorMessage = "Buy price must be greater than the start price!")]
        public decimal buyPrice { get; set; }

        public string state { get; set; } = "waiting";

        [Required(ErrorMessage = "Start date is required!")]
        [LaterThanNow(ErrorMessage = "Start date must be later than now!")]
        public string startDate { get; set; }

        [Required(ErrorMessage = "End date is required!")]
        [LaterThanNow(ErrorMessage = "End date must be later than now!")]
        [DateGreaterThan("startDate", ErrorMessage = "End date must be later than the start date!")]
        public string endDate { get; set; }
        public bool editable { get; set; } = true;
        public string cathegory { get; set; }
        public string bid { get; set; } = "";
        public virtual ICollection<Bid> bids { get; set; }
    }


    public class GreaterThan : ValidationAttribute
    {
        string otherPropertyName;
        public GreaterThan(string otherPropertyName)
        {
            this.otherPropertyName = otherPropertyName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            try
            {
                // Using reflection we can get a reference to the other date property, in this example the project start date
                var otherPropertyInfo = validationContext.ObjectType.GetProperty(this.otherPropertyName);
                // Let's check that otherProperty is of type DateTime as we expect it to be
                if (otherPropertyInfo.PropertyType.Equals(new Decimal().GetType()))
                {
                    Decimal toValidate = (Decimal)value;
                    Decimal referenceProperty = (Decimal)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
                    // if the end date is lower than the start date, than the validationResult will be set to false and return
                    // a properly formatted error message
                    if (toValidate.CompareTo(referenceProperty) < 1)
                    {
                        validationResult = new ValidationResult(ErrorMessageString);
                    }
                }
                else
                {
                    validationResult = new ValidationResult("An error occurred while validating the property. OtherProperty is not of type Decimal");
                }
            }
            catch (Exception ex)
            {
                // Do stuff, i.e. log the exception
                // Let it go through the upper levels, something bad happened
                throw ex;
            }

            return validationResult;
        }


    }
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        string otherPropertyName;

        public DateGreaterThanAttribute(string otherPropertyName)
        {
            this.otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            try
            {

                // Using reflection we can get a reference to the other date property, in this example the project start date
                var otherPropertyInfo = validationContext.ObjectType.GetProperty(this.otherPropertyName);
                String referenceProperty = (String)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

                DateTime startDate = DateTime.Parse(referenceProperty);
                DateTime endDate = DateTime.Parse(value.ToString());

                if (endDate.CompareTo(startDate) < 1)
                {
                    validationResult = new ValidationResult(ErrorMessageString);
                }

            }
            catch (Exception ex)
            {
                // Do stuff, i.e. log the exception
                // Let it go through the upper levels, something bad happened
                return null;
            }

            return validationResult;
        }
    }
    public class LaterThanNow : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            try
            {
                DateTime dateToCompare = DateTime.Parse(value.ToString());
                if (dateToCompare.CompareTo(DateTime.Now) < 1)
                {
                    validationResult = new ValidationResult(ErrorMessageString);
                }
                return validationResult;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}