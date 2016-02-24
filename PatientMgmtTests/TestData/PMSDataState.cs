using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEmosoft;
using iEmosoft.Automation;
using iEmosoft.Automation.HelperObjects;

namespace PatientMgmtTests.TestData
{
    public static class PMSDataState
    {
        static RandomTestData randomData = new RandomTestData();

        #region LoggedInUserModel
        public static LoggedInUser FetchUser(string role)
        {
            return new LoggedInUser() { UserName = "validUserName", Password = "validPassword", Role = role };
        }

        public static LoggedInUser FetchInvalidUser()
        {
            return new LoggedInUser() { UserName = "invalidUserName", Password = "invalidPassword", Role = "Doesn't Matter" };
        }

        public static LoggedInUser FetchDefaultUser()
        {
            return FetchUser("Lab Technicain");
        }

        #endregion

        #region HospitalModel
        public static HospitalModel GenerateRandomHosptialModel()
        {
            HospitalModel result = new HospitalModel()
            {
                Address = randomData.GetRandomDigits(3, 5),
                City = randomData.GetRandomCity(),
                HospitalName = randomData.GetRandomCompanyName(3),
                State = randomData.GetRandomState()
            };

            result.SetOriginalValues();
            return result;
        }

        public static HospitalModel FetchARandomHospitalFromDatabase()
        {
            //HERE IS AN EXAMPLE WHO HOW WE WOULD DO THIS
            //string sql = "SELECT TOP 100 FROM Hospital";
           //return GetRandomHospitalFromSQLStatement(sql);

            //Hard coding this because this example has no db
            var result = new HospitalModel() { HospitalName = "Childrens", Address = "123 Jump st", City = "Minneapolis", State = "MN" };
            result.SetOriginalValues();
            return result;

        }

        public static HospitalModel FetchHospitalFromDatabase_ById(Guid id)
        {
            //string sql = "Select * from Hosptial where HospitalId = " + id;
            //return GetRandomHospitalFromSQLStatement(sql);

            var result = new HospitalModel()
            {
                HospitalId = id,
                HospitalName = "Childrens",
                Address = "123 jump st",
                City = "mpls",
                State = "mn"
            };

            result.SetOriginalValues();
            return result;
        }

        public static HospitalModel FetchHospitalFromDatabase_ByName(string name)
        {
            //string sql = "Select * from Hospital where name = " + name;
            //return GetRandomHospitalFromSQLStatement(sql);

            //SEE THE COMMENTS ON THE PRVIOUS LINES
            var result = new HospitalModel(){ HospitalName = name,
             Address = "123 jump st",
             City = "mpls",
             State = "mn"};

            result.SetOriginalValues();
            return result;
        }

       
        public static HospitalModel FetchHospitalRandomFromDatabase_ByState(string state)
        {
            //string sql = "select * from hosptial where state = " + state;
            //return GetRandomHosptialFromSQLStatement(sql);

            var result = new HospitalModel()
            {
                HospitalName = "Childrens",
                Address = "123 jump st",
                City = "mpls",
                State = state
            };

            result.SetOriginalValues();
            return result;
        }

        public static HospitalModel FetchAReallyReallyBigHosptial()
        {
            //string sql = "SELECT h.* FROM Hosptial h INNER JOIN blb bal WHERE COUNT(PATIENTS) > 1000000;
            //return GetRandomHospitalFromSQLStatement(sql);

            return null;
        }
        #endregion

    }
}
