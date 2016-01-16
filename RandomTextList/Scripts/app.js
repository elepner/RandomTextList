var randomReaderModule = angular.module('randomReader', []);
randomReaderModule.controller('randomReaderController', [
    '$http', '$scope', '$timeout', function ($http, $scope, $timeout) {
        $scope.records = [];
        
        var cacheBuster = 0;
        $scope.loading = false;

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

        $scope.loadMore = function () {
            if ($scope.loading) return;
            $scope.loading = true;
            $http({
                url: '/api/get_records',
                method: "GET",
                params: { start: $scope.records.length, count: 100 }
            }).then(function (result) {
                $scope.records = $scope.records.concat(result.data);
                $scope.loading = false;
            }, function (err) {
                console.log(err);
                $scope.loading = false;
            });
        }

        $scope.loadMore();
        
        function pollStatus() {
            $http.post('api/status?cacheBust=' + cacheBuster).then(function(result) {
                $scope.status = result.data;
                cacheBuster++;
                $timeout(pollStatus, 2000);
            });
        };

        pollStatus();


    }
]);

randomReaderModule.directive('scrolled', function() {
    return {
        restrict: 'A',
        link: function ($scope, element, attrs) {
            var raw = element[0];
            element.bind('scroll', function () {
                console.log('in scroll');
                console.log(raw.scrollTop + raw.offsetHeight);
                console.log(raw.scrollHeight);
                if (raw.scrollTop + raw.offsetHeight >= raw.scrollHeight-1) {
                    console.log("I am at the bottom");
                    $scope.$apply(attrs.scrolled);
                }
            });
        }
    }
})

var app = angular.module('randomReaderApp', ['randomReader']);

