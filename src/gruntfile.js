module.exports = function(grunt) {

    grunt.initConfig({
        pkg: grunt.file.readJSON("package.json"),
        src: {
            b2cRoot: "AvenueClothing.B2C/",
            directories: ["bin/*.dll", "app_config/include/*.config", "views/**"],
            destination: "c:/inetpub/sc8/website"
        } ,
        
        copy: {
            b2c: {
                files: [                    
                    { expand: true, cwd: "<%= src.b2cRoot %>", src: "<%= src.directories %>", dest: "<%= src.destination %>", nonull: true }
           
                ]
            }
        },
        
        watch: {
            b2c: {
                files: "<%= src.b2cRoot %>",
                tasks: ["copy"],
                livereload: true
            }
        }
    });

    grunt.loadNpmTasks("grunt-contrib-copy");
    grunt.loadNpmTasks("grunt-contrib-watch");
};