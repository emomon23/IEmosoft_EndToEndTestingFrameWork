angular.module("PMSApp")
.config(function ($routeProvider) {
    $routeProvider.when("/HospitalList",
        { templateUrl: "html/HospitalList.html" });

    $routeProvider.when("/Hospital",
        { templateUrl: "html/Hospital.html" });

    $routeProvider.when("/PatientList",
        { templateUrl: "html/PatientList.html" });

    $routeProvider.when("/Patient",
        { templateUrl: "html/Patient.html" });

    $routeProvider.when("/PhysicianList",
        { templateUrl: "html/PhysicianList.html" });

    $routeProvider.when("/Physician",
        { templateUrl: "html/Physician.html" });

    $routeProvider.otherwise({ templateUrl: "html/Login.html" });
});