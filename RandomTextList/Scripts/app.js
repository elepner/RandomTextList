var randomReaderModule = angular.module('randomReader', []);
randomReaderModule.controller('randomReaderController', [
    '$http', '$scope', '$timeout', '$location', function ($http, $scope, $timeout, $location) {
        $scope.records = [];
        $scope.offset = 0;
        $scope.loading = false;
        $scope.MAX_ROW_NUMBER = 1000;

        $scope.removeAll = function () {
            $scope.removing = true;
            $http.get('api/cleanup').then(function () {
                $scope.records = [];
                $scope.removing = false;
            }, function(err) {
                console.log(err);
                $scope.removing = false;
            });
        }

        $scope.getStatus = function() {
            $http.get('api/status').then(function (result) {
                $scope.status = result;
            });
        }

        $scope.start = function() {
            $http.get('api/start_writing').then(function (data) {
                $scope.status = data.data;
            }, function(err) {

            });
        }

        $scope.stop = function() {
            $http.get('api/stop_writing').then(function (data) {
                $scope.status = data.data;
            }, function(err) {

            });
        }

        $scope.getLinesNumber = function (index) {
            var idx = index + $scope.offset + 1;
            if (idx % 9 === 0) return 5;
            if (idx % 7 === 0) return 4;
            return 3;
        }
        $scope.getImageId = function (record, index) {
            if (typeof (record.recordType) === 'undefined') {
                var idx = index + $scope.offset + 1;
                if (idx % 7 === 0) {
                    record.recordType = 1;
                }
                else if (idx % 11 === 0) {
                    record.recordType = 2;
                }
                else if (idx % 17 === 0) {
                    record.recordType = 3;
                }
                else if (Math.random() > 0.5) {
                    record.recordType = 4;
                } else {
                    record.recordType = 5;
                }
            }

            return record.recordType;
        }

        var debounce = false;
        $scope.loadMore = function (count) {

            if ($scope.records.length >= $scope.MAX_ROW_NUMBER) {
                return;
            }

            if ($scope.loading || debounce) return;
            $scope.loading = true;
            debounce = true;
            $timeout(function() {
                debounce = false;
            }, 1000);
            $http({
                url: '/api/get_records',
                method: "GET",
                params: { start: $scope.offset + $scope.records.length, count: count }
            }).then(function (result) {
                $scope.records = $scope.records.concat(result.data);
                $scope.loading = false;
            }, function (err) {
                console.log(err);
                $scope.loading = false;
            });
        }

        $scope.loadMore(100);

        $scope.$watch('offset', function(newVal) {
            if (typeof(newVal) !== 'undefined') {
                $scope.records = [];
                $scope.loadMore(50);
            }
        });

        function pollStatus() {
            $http.get('api/status').then(function (result) {
                //All records are deleted, but service is running => one can try to query more data.
                $scope.status = result.data;
                if ($scope.records.length === 0 && $scope.status.running) {
                    $scope.loadMore(50);
                }
                
                $timeout(pollStatus, 1500);
            });
        };

        pollStatus();
    }
]);

randomReaderModule.directive('scrolled', function() {
    return {
        restrict: 'A',
        link: function($scope, element, attrs) {
            var raw = element[0];

            element.bind('scroll', function () {
                console.log(raw.scrollTop, raw.scrollHeight - raw.offsetHeight);

                if (raw.scrollTop >= (raw.scrollHeight - raw.offsetHeight) - 1) {
                    $scope.$apply(attrs.scrolled);
                }
            });
        }
    }
});

randomReaderModule.directive('scrollPosition', function () {
    return {
        restrict: 'A',
        link: function ($scope, element, attrs) {
            var raw = element[0];
            element.bind('scroll', function () {
                if (raw.scrollTop === (raw.scrollHeight - raw.offsetHeight)) {
                    $scope.$apply(attrs.scrolled);
                }
            });
        }
    }
});

//Prevent IE from caching GET queries. http://stackoverflow.com/a/19771501/1453239
randomReaderModule.config(['$httpProvider', function ($httpProvider) {
    //initialize get if not there
    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.get = {};
    }

    // Answer edited to include suggestions from comments
    // because previous version of code introduced browser-related errors

    //disable IE ajax request caching
    $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
    // extra
    $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
    $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
}]);

var app = angular.module('randomReaderApp', ['randomReader']);

