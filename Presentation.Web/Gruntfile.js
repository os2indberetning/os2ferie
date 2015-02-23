'use strict';

module.exports = function(grunt) {
  grunt.loadNpmTasks('grunt-karma');
 
  grunt.initConfig({
    karma: {
		unit:{
		    configFile: 'karma.conf.js',
            singlerun: false
		}	     
    }});
	
  // Default task.
  grunt.registerTask('unittest', ['karma:unit:run']);
  grunt.registerTask('default', ['karma:unit']);
};
