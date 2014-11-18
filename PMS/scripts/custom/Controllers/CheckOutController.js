angular.module("APP NAME HERE")
.controller("checkoutCTRL", function ($scope, $location, $http, cart, saveOrderURL) {
    $scope.cartData = cart.getProducts();

    $scope.total = cart.getCartPrice();
    
    $scope.itemsCount = cart.getCartItemsCount();
    
    $scope.remove = function (id) {
        cart.removeFromCart(id);
    }

    $scope.sendOrder = function () {
        var order = angular.copy($scope.data.shipping);
        order.products = cart.getProducts();

        $http.post(saveOrderURL, order)
           .success(function (confCode) {
               $scope.data.saveOrderMessage = confCode;
               cart.emptyCart();

           })
           .error(function (error) {
               $scope.data.saveOrderMessage = error.message;
           })
           .finally(function () {
               $location.path('/complete');
           });
    }
    

});