angular.module("PMSApp")
    .filter('physiciansListView', function () {
        return function (listOfPhysicians) {
            var results = [];
            var dal = window.PmsDAL;
          
            if (window.physicianViewList != null && window.physicianViewList != undefined) {
                return window.physicianViewList;
            }

            angular.forEach(listOfPhysicians, function (physician) {
                var hospital = dal.getHospital(physician.hospitalid);
                
                var mergedView = { id: physician.id, firstname: physician.firstname, lastname: physician.lastname, cellnumber: physician.cellnumber, hospitalname: hospital.name };
                results.push(mergedView);
            });

            window.physicianViewList = results;

            return results;
        }
    })
    .filter('patientListView',  function() {
        return function (listOfPatients) {
            if (window.patientViewList != null && window.patientViewList != undefined) {
                return window.patientViewList;
            }

         var results = [];
         var dal = window.PmsDAL;
          
         angular.forEach(listOfPatients, function (patient) {
             var physician = dal.getPhysician(patient.physicianid);
             var hospital = dal.getHospital(physician.hospitalid);
                
             var mergedView = { id: patient.id, firstname: patient.firstname, lastname: patient.lastname, nextvisit: patient.nextvisit, physician: physician.firstname + ' ' + physician.lastname, hospitalname: hospital.name };
             results.push(mergedView);
         });

         window.patientViewList = results;
         return results;
     }
 });

