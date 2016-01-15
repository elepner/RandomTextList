var randomReaderModule = angular.module('randomReader', []);
randomReaderModule.controller('randomReaderController', [
    '$http', '$scope', function($http, $scope) {
        $http({
            url: '/get_records',
            method: "GET",
            params: { start: 0, count: 100 }
        }).then(function(data) {
            console.log(data);
            $scope.randomRecord = data;
        }, function(err) {
            console.log(err);
        });
    }
]);

var app = angular.module('randomReaderApp', ['randomReader']);
app.filter('greet', function () {
    return function (name) {
        return 'Hello, ' + name + '!';
    };
});
