angular.module("cart", [])
 .factory("cart", function () {
     var cartContents = [];

     return {
         addProduct: function (id, name, price) {
             var gotItAlready = false;

             for (i = 0; i < cartContents.length; i++) {
                 var cartItem = cartContents[i];

                 if (cartItem.id == id) {
                     gotItAlready = true;
                     cartItem.count += 1;
                     break;
                 }
             }

             if (!gotItAlready) {
                 cartContents.push({ count: 1, id: id, price: price, name: name });
             }
         },

         removeProduct: function (id) {
             for (i = 0; i < cartContents.length; i++)
             {
                 if (cartContents[i].id == id)
                 {
                     cartContents.splice(i, 1);
                     break;
                 }
             }
         },

         getProducts: function () {
             return cartContents;
         }
     }
 })
 .directive("cartSummary", function(cart){
     return {
         restrict: "E",
         templateUrl: "cartsummary.html",
         controller: function ($scope) {
             var cartData = cart.getProducts();

             $scope.total = function () {
                 var total = 0;

                 for (i=0; i<cartData.length, i++){
                     total += (cartData[i].price * cartData[i].count);
                 }

                 return total;
             }

             $scope.itemCount = function(){
                 var total=0;

                 for (i=0; i<cartd.length; i++){
                     total += cartData[i].count;
                 }

                 return total;
             }
         }
     };
 });