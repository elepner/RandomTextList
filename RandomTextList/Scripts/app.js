var randomReaderModule = angular.module('randomReader', []);
randomReaderModule.controller('randomReaderController', [
    '$http', '$scope', function ($http, $scope) {
        $scope.records = [];
        $http({
            url: '/api/get_records',
            method: "GET",
            params: { start: 0, count: 100 }
        }).then(function(data) {
            $scope.records = data.data;
        }, function(err) {
            console.log(err);
        });

        $scope.removeAll = function() {
            $http.get('api/cleanup').then(function () {
                $scope.records = [];
            }, function(err) {
                console.log(err);
            });
        }
    }
]);

var app = angular.module('randomReaderApp', ['randomReader']);

