/// <binding AfterBuild='min' />
"use strict";
const gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-clean-css"),
    terser = require('gulp-terser'),
    merge = require("merge-stream"),
    del = require("del"),
    sass = require("gulp-sass")(require('sass')),
    bundleconfig = require("./bundleconfig.json"),
    rename = require('gulp-rename');

const root = 'wwwroot/';

const paths = {
    scss: root + 'scss/',
    govuk: root + 'govuk-frontend/',
    nhsuk: root + 'nhsuk-frontend/',
    webfonts: root + 'webfonts',
    nodemod: 'node_modules/'
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

gulp.task('nhsuk', function () {
    return gulp.src(paths.nodemod + 'nhsuk-frontend/dist/nhsuk.min.js')
        .pipe(gulp.dest(paths.nhsuk + 'assets/js'));
});

gulp.task('govuk', function () {
    return gulp.src(paths.nodemod + 'govuk-frontend/govuk/all.js')
        .pipe(rename('govuk-frontend.min.js'))
        .pipe(gulp.dest(paths.govuk + 'assets/js'))
});

gulp.task('fonts', function () {
    return gulp.src(paths.nodemod + '@fortawesome/fontawesome-free/webfonts/*')
        .pipe(gulp.dest(paths.webfonts))
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
    gulp.watch([paths.scss + '**/*.scss'], gulp.series(['sass', 'min:css']));
});

function getBundles(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}

gulp.task("min", gulp.series("clean", "sass", "nhsuk", "govuk", 'fonts', "min:js", "min:css"));
