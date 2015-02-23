describe("DefaultTest", function () { 
    var testVar = 0;

    // Test setup 
    beforeEach(function() {
        testVar = 1;
    });

    // actual test
    it("Test Variable should be 1", function() {
        expect(testVar).toBe(1);
    });
})