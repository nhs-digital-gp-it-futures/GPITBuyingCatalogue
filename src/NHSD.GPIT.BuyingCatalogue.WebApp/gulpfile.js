/// <binding AfterBuild='min' />
"use strict";
const gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-clean-css"),
    terser = require('gulp-terser'),
    merge = require("merge-stream"),
    del = require("del"),
    sass = require("gulp-sass")(require('sass')),
    bundleconfig = require("./bundleconfig.json");
const paths = {
    scss: 'wwwroot/scss/'
};

const regex = {
    css: /\.css$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

gulp.task("sass", function () {
    return gulp.src(paths.scss + '**/*.scss', ['!_variables.scss'])
        .pipe(sass())
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task("min:js", async function () {
    const tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(terser())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("min:css", async function () {
    const tasks = getBundles(regex.css).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(cssmin())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
});

gulp.task("clean", function () {
    const files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName;
    });
    return del(files);
});

gulp.task('watch', function () {
    gulp.watch([paths.scss + '**/*.scss'], gulp.series(['sass','min:css']));
});

function getBundles(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}

gulp.task("min", gulp.series("clean", "sass", "min:js", "min:css"));
