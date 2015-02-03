var app = angular.module('ngMobileApp', []);
app.controller('MobileCtrl', ['$scope', '$http', function ($scope, $http, $filter) {
    $scope.InitBill = function () {
        $scope.PhoneList = null;
        $scope.Url = "GetMobileBill" + window.location.search;
        $scope.InputParametes = null;
        $http.get($scope.Url, { cache: false })
            .success(function (data) {
                $scope.PhoneList = data.phoneList[0];
            });
    }
}])
.filter('DateFormat', function () {
    return function (dateStr) {
        return dateStr != "" ? new Date(dateStr).toLocaleDateString() : "";
    }
})
.filter('MonthFormat', function () {
    return function (monthStr) {
        return monthStr.substring(0, 4) + '/' + monthStr.substring(4);
    }
})