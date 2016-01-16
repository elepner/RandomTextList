var randomReaderModule = angular.module('randomReader', []);
randomReaderModule.controller('randomReaderController', [
    '$http', '$scope', '$timeout', function ($http, $scope, $timeout) {
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
        var cacheBuster = 0;

        $scope.removeAll = function() {
            $http.get('api/cleanup?cacheBust=' + cacheBuster).then(function () {
                $scope.records = [];
            }, function(err) {
                console.log(err);
            });
        }

        

        $scope.getStatus = function() {
            $http.get('api/status?cacheBust=' + cacheBuster).then(function (result) {
                $scope.status = result;
                cacheBuster++;
            });
        }

        $scope.start = function() {
            $http.get('api/start_writing?cacheBust=' + cacheBuster).then(function (data) {
                $scope.status = data.data;
                cacheBuster++;
            }, function(err) {

            });
        }

        $scope.stop = function() {
            $http.get('api/stop_writing?cacheBust=' + cacheBuster).then(function (data) {
                $scope.status = data.data;
                cacheBuster++;
            }, function(err) {

            });
        }
        
        function pollStatus() {
            $http.get('api/status?cacheBust=' + cacheBuster).then(function(result) {
                $scope.status = result.data;
                cacheBuster++;
                $timeout(pollStatus, 2000);
            });
        };

        pollStatus();


    }
]);

var app = angular.module('randomReaderApp', ['randomReader']);

