using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JhpDataSystem.model
{
    public static class TextValidator
    {
        private static Regex digitsOnlyRegEx = new Regex(@"[^\d]");
        public static string removeNonDigits(string textToClean)
        {
            return digitsOnlyRegEx.Replace(textToClean, "");
        }
        public static DataValidationIssue validates(ValidationObject fieldView)
        {
            if (fieldView.isRequired)
            {
                if (string.IsNullOrWhiteSpace (fieldView.value)) return
                        new DataValidationIssue()
                        {
                            message = "Value is required",
                            validationObject = fieldView
                        };
            }

            if (Constants.VALIDATION_PHONE.CompareTo(fieldView.validation) == 0)
            {
                //we look at the value specified
                var value = fieldView.value;
                if (string.IsNullOrWhiteSpace(value)) return null;

                value = value.Trim();
                var allDigits = removeNonDigits(value);

                var is10Chars = allDigits.Length == Constants.VALIDATION_PhoneNumberLength;
                var looksLikeCell = allDigits.StartsWith(Constants.VALIDATION_HowCellStarts);
                var looksLikePhone = allDigits.StartsWith(Constants.VALIDATION_HowPhoneStarts);

                if (allDigits.Length != value.Length || !(is10Chars && (looksLikeCell || looksLikePhone)))
                {
                    return
                        new DataValidationIssue()
                        {
                            message = "Phone not in correct format",
                            validationObject = fieldView
                        };
                }
            }

            return null;
        }
    }
    public static class DateValidator
    {
        static string validatorTag = "JhpiegoApp";
        public static DataValidationIssue validates(ValidationObject fieldView)
        {
            if (fieldView.isRequired)
            {
                if (isNullDate(fieldView.value)) return
                        new DataValidationIssue() {
                            message = "Value is required",
                            validationObject = fieldView };
            }
            return null;
        }
        private static bool isGreaterThanExpected(string value, DateTime expected)
        {
            return false;
        }
        private static bool isFutureDate(string value)
        {
            return false;
        }
        private static bool isNullDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;

            //we check if the value is a valid date
            //todo: complete this
            Log.Info(validatorTag, "Not checking isNullDate for valid date");
            return false;
        }
    }

    public class viewValidations
    {
        public List<FieldValuePair> TemporalViewData { get; set; }
        public List<DataValidationIssue> Validations { get; set; }
    }

    public class ValidationObject
    {
        public string label;
        public int viewId;
        public bool isRequired;
        public string value;
        public string validation;
        public Android.Views.View viewObject;
    }
    public class DataValidationIssue
    {
        public string message { get; set; }
        public ValidationObject validationObject { get; set; }
    }
}
