angular.module("APP NAME HERE")
.config(function ($routeProvider) {
    $routeProvider.when("/checkout",
        { templateUrl: "/html/CartSummary.html" });

    $routeProvider.when('/products',
        { templateUrl: "/html/productsList.html" });

    $routeProvider.when('/placeOrder',
       { templateUrl: "/html/PlaceOrder.html" });

    $routeProvider.when('/complete',
       { templateUrl: "/html/ThankYou.html" });

    $routeProvider.otherwise({
        templateUrl: '/html/productsList.html'
    });
});