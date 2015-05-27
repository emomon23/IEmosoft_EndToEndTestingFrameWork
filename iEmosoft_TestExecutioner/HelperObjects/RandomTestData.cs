using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iEmosoft.Automation.Helpers
{
    public class RandomTestData
    {
        Random rnd = new Random(DateTime.Now.Millisecond);

        public RandomTestData()
        {
            this.FirstNames = new List<string>() { "Mike", "Michael", "Michelle", "Tom", "Tommy", "Thomas", "Eric", "Erick", "Jon", "John", "Jonny", "Theo", "Theodore", "Teddy", "Gretchen", "Sarah", "Sara", "Grace", "Gracie", "Evie", "Eveline", "Evelyn", "Ron", "Ronny", "Beau", "Muriel", "Pat", "Patrick", "Elizabeth", "Liz", "Julie", "Helen", "Joe", "Joesph", "Joey", "Teresa", "Alex", "Betsy", "Katie", "Kate", "Jenny", "Jennifer", "Angela", "Sadie", "Melissa", "Missy" };
            this.LastNames = new List<string>() { "Jones", "Bridgewater", "Krammer", "Oden", "Thomason", "Jackson", "Nelson", "Wilson", "Olson", "Olsen", "Mullenham", "Emo", "Caauwe", "Mertz", "Linkert", "Washington", "Bush", "Aniston", "Montana", "Manning", "Ponder", "Bauer", "Salzwadel", "Mcguire", "Anderson", "Mulner", "Hagen", "Austin", "Williams", "Cobb", "Pucket", "Mauer", "Hurbek", "Erickson", "Walser", "Ryans", "Arnold", "Kelly", "Smith", "Johnson", "Wesson" };
            this.CompanyNames = new List<string>() { "Mircosoft", "iEmosoft", "Apple", "Apple Computer", "General Electric", "Protolabs", "ADP", "Make Music", "Portoco", "Sorin", "Eue, Rachie and Associates", "Target", "Sears", "Kmart", "Wards", "Ebay", "Amazon", "Walmart", "Tires Plus", "Discount Tire", "Bloomingdales", "Lunds", "Super Value", "Cub Foods", "Holiday Gas", "BP", "Verizon", "ATT", "Medtronic", "Boston Science", "Remington", "Smith and Wesson", "Good Year" };
            this.Countries = new List<string>() {"United States", "Canada", "Mexico", "Liberia", "Houndouras", "Brazil", "France", "Germany", "United Kingdom", "Russia", "Japan", "China", "Austrailia", "Sweden", "Norway" };
            this.States = new List<string>() {"Alabama", "Alaska", "Connecticut", "Minnesota", "Wisconsin", "Illinois", "Iowa", "North Dakota", "South Dakota", "Kentucky", "Tennesee", "Texas", "New York", "California", "Wyoming", "Oregon", "Maine", "Florida" };
            this.Cities = new List<string>() {"Jacksonville", "Minneapolis", "St. Paul", "Duluth", "Hopkins", "Hastings", "Little Falls", "Paris", "Rome", "London", "Moscow", "York", "Waconia", "Minnetonka", "Wayzata", "Jamestown", "Montgomery", "Excelsior" };

        }
        public List<string> FirstNames { get; set; }
        public List<string> LastNames { get; set; }
        public List<string> CompanyNames { get; set; }
        public List<string> Countries {get;set;}
        public List<string> Cities {get;set;}
        public List<string> States {get;set;}
        
        public string GetRandomFirstName(int appendGuidDigits = 0)
        {
            return FirstNames[rnd.Next(0, FirstNames.Count - 0)] + GetGuidSubString(appendGuidDigits);
        }

        public string GetRandomLastName(int appendGuidDigits = 0)
        {
            return LastNames[rnd.Next(0, LastNames.Count - 0)] + GetGuidSubString(appendGuidDigits);
        }

        public string GetRandomCompanyName(int appendGuidDigits = 0)
        {
            return CompanyNames[rnd.Next(0, CompanyNames.Count - 0)] + GetGuidSubString(appendGuidDigits);
        }

        public string GetRandomCity(int appendGuidDigits = 0)
        {
            return Cities[rnd.Next(0, Cities.Count - 0)] + GetGuidSubString(appendGuidDigits);
        }

        public string GetRandomState(){
            return States[rnd.Next(0, States.Count - 0)];
        }

        public string GetGuidSubString(int guidLength)
        {
            if (guidLength > 0)
            {
                return Guid.NewGuid().ToString().Substring(0, guidLength);
            }

            return "";
        }

        public string GetRandomCountry(){
            return Countries[rnd.Next(0, Countries.Count - 0)];
        }

        public string GetRandomPostalCode(int appendGuidDigits = 0)
        {
            return string.Format("{0}{1}", rnd.Next(3, 9), GetRandomDigits(4)) + GetGuidSubString(appendGuidDigits);
        }

        public string GetRandomEmailAddress(int appendGuidDigits = 0)
        {
            return GetRandomEmailAddress(GetRandomFirstName(), GetRandomLastName()) + GetGuidSubString(appendGuidDigits);
        }

        public bool GetRandomBoolean()
        {
            return rnd.Next(0, 1) == 1;
        }

        public string GetRandomEmailAddress(string firstName, string lastName)
        {
            return string.Format("{0}.{1}@{2}{3}.not{4}", firstName, lastName, GetRandomCompanyName(), rnd.Next(11, 99), GetRandomDomain());
        }

        public string GetRandomDate(int minYear, int maxYear)
        {
            int year = rnd.Next(minYear, maxYear);

            return GetRandomDate(null, null, year);
        }

        public string GetRandomDate(int ? desiredMonth, int ? desiredDay, int ? desiredYear)
        {
            if (!desiredMonth.HasValue)
                desiredMonth = rnd.Next(1, 12);

            if (!desiredDay.HasValue)
                desiredDay = rnd.Next(1, 28);

            if (!desiredYear.HasValue)
                desiredYear = rnd.Next(1700, DateTime.Now.Year + 25);

            return new DateTime(desiredYear.Value, desiredMonth.Value, desiredDay.Value).ToShortDateString();
        }

        public string GetRandomValueFromArray(List<string> list){
            return list[rnd.Next(0, list.Count -1)];
        }

        public string GetRandomPhoneNumber(string desiredAreaCode, string desiredPrefix, string desiredFourDigits)
        {
            if (desiredAreaCode.isNull())
                desiredAreaCode = GetRandomDigits(3);

            if (desiredPrefix.isNull())
                desiredPrefix = GetRandomDigits(3);

            if (desiredFourDigits.isNull())
                desiredFourDigits = GetRandomDigits(4);

            return string.Format("{0}-{1}-{2}", desiredAreaCode, desiredPrefix, desiredFourDigits);
        }

        public string GetRandomDigits(int numberOfDigets){
            string results = "";

            for (int i=0; i<numberOfDigets; i++){
                results += rnd.Next(1, 9).ToString();
            }

            return results;
        }

        public string GetRandomDigits(int lowDigits, int highDigets)
        {
            int numberOfDigets = rnd.Next(lowDigits, highDigets);
            return GetRandomDigits(numberOfDigets);
        }

        public string GetRandomDomain()
        {
                string[] domains = new string[] { ".com", ".net", ".org", ".gov", ".tv" };
                return domains[rnd.Next(0, domains.Length - 1)];
        }

        public string GetRandomDomainName()
        {
            return this.GetRandomCompanyName().Replace(" ", "");
        }

        public AdddressData GetRandomAddress()
        {
            AdddressData result = new AdddressData()
            {
                City = this.GetRandomCity(),
                Country = this.GetRandomCountry(),
                PostalCode = this.GetRandomPostalCode(),
                State = this.GetRandomState(),
                Street1 = this.GetRandomDigits(2, 5)
            };

            if (this.GetRandomBoolean())
            {
                result.Street2 = "# " + rnd.Next(1, 3000);
            }

            return result;
        }

        public CompanyData GetRandomCompany()
        {
            var result = new CompanyData()
            {
                BillingAddress = this.GetRandomAddress(),
                CompanyName = this.GetRandomCompanyName(),
                HQAddress = this.GetRandomAddress(),
                ShipToAddress = this.GetRandomAddress(),
            };

            result.WebSite = string.Format("http://www.{0}.{1}", result.CompanyName.Replace(" ", ""), GetRandomDomain());

            return result;
        }

        public PersonData GetRandomPerson()
        {
            int maxYear = DateTime.Now.Year - 10;
            int minYear = maxYear - 50;

            var person = new PersonData()
            {
                DateOfBirth = this.GetRandomDate(minYear, maxYear).ToDate(),
                DateOfDeath = this.GetRandomDate(maxYear + 2, maxYear + 8),
                FirstName = this.GetRandomFirstName(),
                LastName = this.GetRandomLastName(),
                Employer = this.GetRandomCompany(),
                EyeColor = this.GetRandomValueFromArray(new List<string>() { "Brown", "Blue", "Green" }),
                HairColor = this.GetRandomValueFromArray(new List<string>() { "Red", "Blond", "Brown", "Black", "Dyed" }),
                HeightFeet = rnd.Next(3, 6),
                HeightInches = rnd.Next(1, 11),
                HiredDate = this.GetRandomDate(maxYear + 2, maxYear + 8),
                HomeAddress = this.GetRandomAddress(),
                HomePhone = this.GetRandomPhoneNumber(null, "555", null),
                IsMarried = this.GetRandomBoolean(),
                MiddleName = this.GetRandomFirstName(),
                Password = "P@ssw0rd!",
                ShippingAddress = this.GetRandomAddress(),
                WorkPhone = this.GetRandomPhoneNumber(null, "555", null)
            };

            if (person.Age > 21){
                person.IsMarried = this.GetRandomBoolean();
                if (person.IsMarried)
                {
                    person.MarriedDate = this.GetRandomDate(person.DateOfBirth.AddYears(19).Year, person.DateOfBirth.AddYears(21).Year).ToDate();
                }
            }

            person.UserName = string.Format("{0}.{1}{2}", person.FirstName, person.LastName, rnd.Next(1, 222));
            person.EmailAddress = GetRandomEmailAddress(person.FirstName, person.LastName);

            return person;
        }
    }

    public static class ExtensionMethods
    {
        public static bool isNull(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static DateTime ToDate(this string str)
        {
            return DateTime.Parse(str);
        }

        public static DateTime AddYears(this DateTime dt, int years){
            return dt.AddDays(years * 365);
        }

    }

    public class PersonData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string EmailAddress { get; set; }
        public string HiredDate { get; set; }
        public bool IsMarried { get; set; }
        public DateTime MarriedDate {get;set;}
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DateOfDeath { get; set; }
        public int Age
        {
            get
            {
                double  days = (this.DateOfBirth - DateTime.Now).TotalDays;
                return (int)(days * (double)365.4);
            }
        }
        public int HeightFeet { get; set; }
        public int HeightInches { get; set; }
        public string EyeColor { get; set; }
        public string HairColor { get; set; }
        public bool IsMale { get; set; }
        public AdddressData HomeAddress { get; set; }
        public AdddressData ShippingAddress { get; set; }
        public CompanyData Employer { get; set; }
    }

    public class AdddressData {
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }

    public class CompanyData
    {
        public string CompanyName { get; set; }
        public string WebSite { get; set; }
        public AdddressData HQAddress { get; set; }
        public AdddressData BillingAddress { get; set; }
        public AdddressData ShipToAddress { get; set; }

    }
}
