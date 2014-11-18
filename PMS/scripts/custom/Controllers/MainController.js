//Dummy Data!
var PmsDAL = {
	nextHospitalId: 4,
	nextPhysicainId: 5,
	nextPatientId: 13,
	categories: [
		'Hospitals',
		'Physicians',
		'Patients'
	],
	hospitals: [
			    { id: 1, name: "St Mary's of Mpls", address: '1933 Cedar Ave South', city: 'Minneapolis', state: 'MN', category:'Hospitals' },
				{ id: 2, name: "Childrens", address: '10012 Minnetonka Blvd', city: 'Minnetonka', state: 'MN', category: 'Hospitals' },
				{ id: 3, name: "Amplats", address: '72322 Hiawatha Avenue South', city: 'Burnsville', state: 'MN', category: 'Hospitals' }
	],
	physicians: [
		{ id: 1, firstname: 'Mike', lastname: 'Emo', cellnumber: '612-555-2342', hospitalid: 1, category: 'Physicians' },
		{ id: 2, firstname: 'Alan', lastname: 'Haggerty', cellnumber: '763-555-9999', hospitalid: 1, category: 'Physicians' },
		{ id: 3, firstname: 'Michael', lastname: 'Colline', cellnumber: '612-555-0000', hospitalid: 2, category: 'Physicians' },
		{ id: 4, firstname: 'Daniel', lastname: 'Messer', cellnumber: '952-555-1111', hospitalid: 3, category: 'Physicians' },
	],
	patients: [
		{ id: 1, firstname: 'John', lastname: 'Travolta', nextvisit: '2/1/2015', physicianid: 1, category: 'Patients' },
		{ id: 2, firstname: 'Jennifer', lastname: 'Aniston', nextvisit: '1/1/2015', physicianid: 1, category: 'Patients' },
		{ id: 3, firstname: 'Angelina', lastname: 'Jolie', nextvisit: '1/9/2015', physicianid: 2, category: 'Patients' },
		{ id: 4, firstname: 'Joe', lastname: 'Montana', nextvisit: '1/21/2015', physicianid: 3, category: 'Patients' },
		{ id: 5, firstname: 'Teddy', lastname: 'Bridgewater', nextvisit: '2/5/2015', physicianid: 3, category: 'Patients' },
		{ id: 6, firstname: 'Brad', lastname: 'Pitt', nextvisit: '2/7/2015', physicianid: 3, category: 'Patients' },
		{ id: 7, firstname: 'Brette', lastname: 'Favre', nextvisit: '2/9/2015', physicianid: 4, category: 'Patients' },
		{ id: 8, firstname: 'Tommy', lastname: 'Krammer', nextvisit: '2/9/2015', physicianid: 4, category: 'Patients' },
		{ id: 9, firstname: 'Dan', lastname: 'Marino', nextvisit: '2/9/2015', physicianid: 1, category: 'Patients' },
		{ id: 10, firstname: 'Jim', lastname: 'McMannon', nextvisit: '2/9/2015', physicianid: 1, category: 'Patients' },
		{ id: 11, firstname: 'Payton', lastname: 'Manning', nextvisit: '3/3/2015', physicianid: 2, category: 'Patients' },
		{ id: 12, firstname: 'Aaron', lastname: 'Rodgers', nextvisit: '3/5/2015', physicianid: 1, category: 'Patients' },
	],
	getPhysicians: function (hospitalId) {
		var results = [];
		
		for (i = 0; i < this.physicians.length; i++) {
			var p = this.physicians[i];
			if (p.hospitalid == hospitalId) {
				results.push(p);
			}
		}

		return results;
	},
	getPhysician: function(physicianId){
	    var result = null;

	    for (i = 0; i < this.physicians.length; i++) {
	        if (this.physicians[i].id == physicianId) {
	            result = this.physicians[i];
	            break;
	        }
	    }

	    return result;
	},
	getHospital: function (hospitalId) {
	    var result = null;
        	   
	    for (i = 0; i < this.hospitals.length; i++) {
	        if (this.hospitals[i].id == hospitalId) {
	            result = this.hospitals[i];
	            break;
	        }
	    }

	    return result;
	},
	getPatients: function (physicianId) {
		var results = [];

		for (i = 0; i < this.patients.length; i++) {
			var p = this.patients[i];
			if (p.physicianid == physicianId) {
				results.push(p);
			}
		}

		return results;
	},
	addHospital: function (hosp) {

	    var hospital = { id: this.nextHospitalId, name: hosp.name, address: hosp.address, city: hosp.city, state: hosp.state, category: 'Hospitals' }
		this.hospitals.push(hospital);
		this.nextHospitalId += 1;

	},
	addPhysician: function(phys) {

	    var physician = {id:this.nextPhysicainId, firstname: phys.firstname, lastname: phys.lastname, cellnumber: phys.cellnumber, hospitalid: phys.hospitalid, category: 'Physician' };
	    this.physicians.push(physician);
		this.nextPhysicainId += 1;
	},
	addPatient: function (pat) {

	    var patient = { id: this.nextPatientId, firstname: pat.firstname, lastname: pat.lastname, nextvisit: pat.nextvisit, category: 'Patients', physicianid: pat.physicianid };
	    this.patients.push(patient);

		this.nextPatientId += 1;
	}
};



angular.module("PMSApp")
   .controller("PMSMainCTRL", function ($scope, $http, $location) {
      	$scope.data = PmsDAL;
      	$scope.data.user = { username: '', password: '', loggedIn: false };

      	$scope.currentView = 'Hospitals';
   		$scope.currentEntity = 'Hospital';

   		$scope.setViewCategory = function (category) {
   		    $scope.currentView = category;

   		    var view = '/HospitalList';
   		    var entity = 'Hospital';

   		    switch (category)
   		    {
   		        case 'Patients':
   		            view = '/PatientList';
   		            entity = 'Patient';
   		            break;
   		        case 'Physicians':
   		            view = '/PhysicianList';
   		            entity = 'Physician';
   		            break;
   		    }

   		    $scope.currentEntity = entity;
            $location.path(view);
   		}
   		$scope.getActiveViewCategoryStyle = function (category) {
   		    return category == $scope.currentView ? 'btn-primary' : '';
   		}
   		$scope.createNewEntity = function()
   		{
   		    var view = '/Hospital';

   		    switch ($scope.currentView) {
   		        case 'Physicians':
   		            $scope.data.editPhysician = {};
   		            view = '/Physician';
   		            break;
   		        case 'Patients':
   		            $scope.data.editPatient = {};
   		            view = '/Patient';
   		            break;
   		        default:
   		            $scope.data.editHospital = {};
   		    }

   		    $location.path(view);
   		}
        
   		$scope.editPatient = function (patient) {
   		    $scope.data.editPatient = patient;
   		    $location.path('/Patient');
   		}

   		$scope.editHospital = function (hospital) {
   		    $scope.data.editHospital = hospital;
   		    $location.path("/Hospital");
   		}

   		$scope.editPhysician = function (phys) {
   		    $scope.data.editPhysician = phys;
   		    $location.path('/Physician');
   		}

   		$scope.savePatient = function () {
   		    window.patientViewList = null;
   		    $scope.data.addPatient($scope.data.editPatient);
   		}

   		$scope.savePhysician = function () {
   		    window.physicianViewList = null;
   		    $scope.data.addPhysician($scope.data.editPhysician);
   		}

   		$scope.saveHospital = function () {
            $scope.data.addHospital($scope.data.editHospital);
   		}

   		$scope.login = function () {
   		    $scope.data.user.invalidCredentials = false;

   		    if ($scope.data.user.password.indexOf('invalid') >= 0)
   		    {
   		        $scope.data.user.invalidCredentials = true;
   		    }
   		    else
   		    {
   		        $scope.data.user.loggedIn = true;
   		        $location.path('/HospitalList');
   		    }
   		}
     
    });