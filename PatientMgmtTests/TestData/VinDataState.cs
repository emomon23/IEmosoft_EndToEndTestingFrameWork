using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iEmosoft.Automation;
using iEmosoft.Automation.Interfaces;
using System.Web.Script.Serialization;

namespace PatientMgmtTests.TestData
{
    public class VinDataState
    {
        private string availableVinsCSVPath = @"C:\VinDecoderTests\AvailableVins.txt";
        private string baselineTestResultsPath = @"C:\VinDecoderTests\TestBaseline.baseline";

        public List<string> GetTotalNumberOfAvailableVins()
        {
            return File.ReadAllText(availableVinsCSVPath).Split(',').ToList();
        }

        public BaselineTester CreateBaselineTester()
        {
            BaselineTester result = new BaselineTester(baselineTestResultsPath, typeof(DecodedVin));
            return result;
        }
    }


    public class DecodedVin : BaseTestResult
    {
        public DecodedVin()
        {
            Options = new Dictionary<string, string>();
        }

        public string Vin { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public Dictionary<string, string> Options { get; set; } 

        public override string TestResultKey
        {
            get { return Vin; }
        }

        public override BaselineDescrepencyCheck CompareResults(BaseTestResult testResult)
        {
            BaselineDescrepencyCheck result = new BaselineDescrepencyCheck();
            var compareTo = (DecodedVin) testResult;

            if (this.Make != compareTo.Make)
            {
                result.InsertMismatch("Make", Make, compareTo.Make);
            }

            if (this.Model != compareTo.Model)
            {
                result.InsertMismatch("Model", Model, compareTo.Model);
            }

            if (this.Year != compareTo.Year)
            {
                result.InsertMismatch("Year", Year, compareTo.Year);
            }

            foreach (var option in this.Options)
            {
                if (compareTo.Options.ContainsKey(option.Key))
                {
                    var compareOptionValue = compareTo.Options[option.Key];
                    if (compareOptionValue != option.Value)
                    {
                        result.InsertMismatch(option.Key, option.Value, compareOptionValue);
                    }
                }
                else
                {
                    result.InsertMismatch(option.Key, option.Value, "VALUE NOT FOUND");
                }
            }

            return result;
        }

        public override void ReadFromJSONString(string json)
        {
            var serailizer = new JavaScriptSerializer();
            var decodedVin = (DecodedVin)serailizer.Deserialize(json, typeof (DecodedVin));

            if (decodedVin != null)
            {
                this.Vin = decodedVin.Vin;
                this.Options = decodedVin.Options;
                this.Make = decodedVin.Make;
                this.Model = decodedVin.Model;
                this.Year = decodedVin.Year;
            }
        }
    }
}