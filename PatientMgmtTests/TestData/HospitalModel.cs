using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMgmtTests.TestData
{
    public class HospitalModel
    {        
        private HospitalModel origianalValues = null;
        private string addressString = null;

        public HospitalModel() {
            this.HospitalId = Guid.NewGuid();
        }
        public HospitalModel(string name, string address, string city, string state, Guid? hospitalId = null)
        {
            this.HospitalName = name;
            this.Address = address;
            this.City = city;
            this.State = state;
            this.HospitalId = hospitalId.HasValue ? hospitalId.Value : Guid.NewGuid();
        }
        public HospitalModel OrigialValues { get { return origianalValues; } }

        public Guid HospitalId { get; set; }

        public string HospitalName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        //Using this for screen scraping
        public string CombinedAddressString
        {
            get
            {
                if (addressString == null)
                {
                    return string.Format("{0} {1} {2}", Address, City, State);
                }

                return addressString;
            }
            set
            {
                addressString = value;
            }
        }
        public void SetOriginalValues()
        {
            this.origianalValues = new HospitalModel(this.HospitalName, this.Address, this.City, this.State, this.HospitalId);
        }
    }
}
