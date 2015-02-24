
casper.test.begin('My Reports something to test...', function suite(test) {
    casper.start(domain + "#/", function() {
        test.assertTitle("Home Page", "Homepage title is the one expected");

    });

    casper.run(function () {
        test.done();
    });
});