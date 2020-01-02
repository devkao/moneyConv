using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MoneyConv.Common.Interfaces;

namespace MoneyConv.Implementation.V1
{
    public class MoneyConvImplV1 : IMoneyConvV1
    {
        //string values for response
        const string PreCommaUnitSingular = "dollar";
        const string PreCommaUnitPlural = PreCommaUnitSingular + "s";
        const string AfterCommaUnitSingular = "cent";
        const string AfterCommaUnitPlural = AfterCommaUnitSingular + "s";
        const string Hundred = "hundred";
        const string And = "and";

        readonly string[] _customWording020 = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        readonly string[] _customWording = { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        readonly string[] _digitGroups = { "", "thousand", "million" };

        /// <summary>
        /// checks string for correct format
        /// </summary>
        readonly Regex _regexpat = new Regex(@"^(([0-9]{1,3}) *){1,3}(,[0-9]{1,2})?$", RegexOptions.Compiled);

        /// <summary>
        /// converts a currency string (123 456,28) into its representation in words 
        /// </summary>
        /// <param name="currencyValue">string to convert (eg. 123 456,78)</param>
        /// <returns>converted string (eg. one hundred twenty-three thousand four hundred fifty-six dollars and seventy-eight cents)</returns>
        public ApiResponse<string> ConvertNumberToWords(string currencyValue)
        {
            var resp = new ApiResponse<string> { Success = false };

            //input validation
            if (!_regexpat.IsMatch(currencyValue))
            {
                resp.Message = "input value is not in the correct format.";
                return resp;
            }

            //split input into its parts
            var x = SplitInput(currencyValue);

            var firstPart = NumberToWord(x.PartBeforeComma);
            var secondPart = NumberToWord(x.PartAfterComma);

            //convert first part
            var finalRes = firstPart + GetUnitWording(x.PartBeforeComma, PreCommaUnitSingular, PreCommaUnitPlural);

            //convert and add second part if present
            if (!string.IsNullOrEmpty(secondPart))
                finalRes += $" {And} {secondPart} " + GetUnitWording(x.PartAfterComma, AfterCommaUnitSingular, AfterCommaUnitPlural);

            resp.Success = true;
            resp.Message = string.Empty;
            //make return value more readable
            resp.Value = BeautifyString(finalRes);

            return resp;
        }

        /// <summary>
        /// makes a given string better to read by removing unnecessary characters like whitespaces
        /// </summary>
        /// <param name="finalRes"></param>
        /// <returns></returns>
        private string BeautifyString(string finalRes)
        {
            return finalRes
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate("", (past, next) => past + " " + next)
                .Trim();
        }

        /// <summary>
        /// determines the wording for the used currency. returns singular if the value is 1, plural otherwise
        /// </summary>
        /// <param name="preCommaPart">value to check</param>
        /// <param name="preCommaUnitSingular">wording for the singular</param>
        /// <param name="preCommaUnitPlural">wording for the plural</param>
        /// <returns></returns>
        private string GetUnitWording(string preCommaPart, string UnitSingular, string UnitPlural)
        {
            if (int.TryParse(preCommaPart, out int intVal) && intVal == 1)
                return UnitSingular;

            return UnitPlural;
        }

        /// <summary>
        /// splits a string into two groups based on the presence of a ,
        /// </summary>
        /// <param name="currencyValue">value to be split</param>
        /// <returns></returns>
        private (string PartBeforeComma, string PartAfterComma) SplitInput(string currencyValue)
        {
            var partBeforeComma = string.Empty;
            var partAfterComma = string.Empty;

            var input = currencyValue.Trim()
                .Replace(" ", string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //first group
            if (input.Length > 0)
                partBeforeComma = input[0];

            //second group
            if (input.Length > 1)
            {
                partAfterComma = input[1];

                //append 0 to ,1 to keep the tens alive ;)
                if (partAfterComma.Length == 1)
                    partAfterComma += "0";
            }

            return (partBeforeComma, partAfterComma);
        }

        /// <summary>
        /// Convert an integer string to corresponding wording
        /// </summary>
        /// <param name="numberToProcess">12345</param>
        /// <returns>twelve thousand three hundred forty-five</returns>
        private string NumberToWord(string numberToProcess)
        {
            var res = new List<string>();
            int curWorkedOnLength = 0;

            //split chunks
            var lengthOfFirstGrp = numberToProcess.Length % 3;

            // 12345 -> 12 & 345
            if (lengthOfFirstGrp != 0)
            {
                res.Add(numberToProcess.Substring(0, lengthOfFirstGrp));
                curWorkedOnLength += lengthOfFirstGrp;
            }

            while (curWorkedOnLength < numberToProcess.Length)
            {
                res.Add(numberToProcess.Substring(curWorkedOnLength, 3));
                curWorkedOnLength += 3;
            }

            var responseValue = string.Empty;
            for (var i = 0; i < res.Count; i++)
            {
                responseValue += GetWordForNumberWithNames(res[i], res.Count - 1 - i);
            }
            return responseValue;
        }

        /// <summary>
        /// returns the number in words including the group names like "thousand","hundred",...
        /// </summary>
        /// <param name="number">number to get the wording for</param>
        /// <param name="grpindex">zero based index of 3 digit groups. counted from right to left</param>
        /// <returns>wording of number including billion/thousand/hundred/...</returns>
        private string GetWordForNumberWithNames(string number, int groupindex)
        {
            var res = string.Empty;
            var numberAsInt = Convert.ToInt32(number, CultureInfo.InvariantCulture);
            var numberAsCharArray = number.ToCharArray();
            var noMoreWork = false;

            if (numberAsCharArray.Length == 3)
            {
                var hundreds = Convert.ToInt32(Char.GetNumericValue(numberAsCharArray[0]));
                res += _customWording020[hundreds] + $" {Hundred} ";
                numberAsInt -= hundreds * 100;
                numberAsCharArray = new[] { numberAsCharArray[1], numberAsCharArray[2] };
                if (numberAsInt == 0) //stop processing if we have "round" numbers like 100,200,500,...
                    noMoreWork = true;
            }

            if (numberAsInt >= 20)
            {
                if (numberAsCharArray.Length == 2)
                {
                    var no1 = Convert.ToInt32(Char.GetNumericValue(numberAsCharArray[0]));
                    var no2 = Convert.ToInt32(Char.GetNumericValue(numberAsCharArray[1]));

                    res += $"{_customWording[no1]}";
                    if (no2 != 0)
                        res += $"-{_customWording020[no2]}";
                }
            }
            else if (!noMoreWork)
            {
                res += $"{_customWording020[numberAsInt]}";
            }

            res += $" {_digitGroups[groupindex]} ";
            return res;
        }
    }
}
