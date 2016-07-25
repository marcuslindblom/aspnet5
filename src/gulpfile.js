/// <binding Clean='clean' ProjectOpened='default' />
"use strict";

var gulp = require('gulp'),
    sass = require('gulp-sass'),
    cssmin = require("gulp-cssmin"),
    browserify = require('browserify'),
    babelify = require('babelify'),
    source = require('vinyl-source-stream'),
    uglify = require("gulp-uglify"),
    buffer = require('vinyl-buffer'),
    rimraf = require('rimraf'),
    argv = require('yargs').argv,
    gulpif = require('gulp-if');


var webroot = "./wwwroot";

var paths = {
    js: {
        entry: "./Areas/Brics/Content/Scripts/app.js",
        watch: "./Areas/Brics/Content/Scripts/**/*.js",
        output: webroot + "/js",
    },
    scss: {
        entry: "./Areas/Brics/Content/Scss/app.scss",
        watch: "./Areas/Brics/Content/Scss/**/*.scss",
        output: webroot + "/css"
    }
};

//gulp.task("clean:js", function (cb) {
//    rimraf(paths.js.output, cb);
//});

//gulp.task("clean:scss", function (cb) {
//    rimraf(paths.scss.output, cb);
//});

//gulp.task("clean", ["clean:js", "clean:scss"]);

gulp.task('min:js', function () {
    return browserify({
        entries: paths.js.entry,
        debug: true
    })
    .transform(babelify, { presets: ["es2015"] })
    .bundle()
    .pipe(source('app.js'))
    .pipe(buffer())
    .pipe(gulpif(argv.dist, uglify()))
    .pipe(gulp.dest(paths.js.output));
});

gulp.task('min:scss', function () {
    return gulp.src(paths.scss.entry)
        .pipe(sass({
            errLogToConsole: true
        }))
        .pipe(gulpif(argv.dist, cssmin()))
        .pipe(gulp.dest(paths.scss.output));
});

gulp.task('watch', ['default'], function () {
    gulp.watch(paths.js.watch, ['min:js']);
    gulp.watch(paths.scss.watch, ['min:scss']);
});

gulp.task('default', ['min:js', 'min:scss']);