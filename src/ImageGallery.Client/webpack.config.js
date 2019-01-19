var isDevBuild = process.argv.indexOf('--env.prod') < 0;
var path = require('path');
var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var CleanWebpackPlugin = require('clean-webpack-plugin');
var UglifyJsPlugin = require('uglifyjs-webpack-plugin');

// Configuration in common to both client-side and server-side bundles
module.exports = {
    mode: isDevBuild ? 'development' : 'production',
    devtool: isDevBuild ? 'inline-source-map' : false,
    resolve: {
        extensions: ['.ts', '.js', '.json', '.css', '.scss', '.html'],
        alias: {
            jquery: path.resolve(__dirname, 'node_modules/jquery/dist/jquery.js')
        }
    },

    entry: {
        'polyfills': './wwwroot/app/polyfills.ts',
        'vendor': './wwwroot/app/vendor.ts',
        'main-client': './wwwroot/app/main.ts'
    },

    output: {
        filename: '[name].js',
        publicPath: '/dist/',
        path: path.join(__dirname, './wwwroot/dist')
    },

    module: {
        rules: [{
            test: /\.ts$/,
            exclude: [/\.spec\.ts$/, /node_modules/],
            use: ['ts-loader', 'angular2-router-loader', 'angular2-template-loader']
        },
        /*{ test: /\.json$/, loader: 'json-loader' },*/
        {
            test: /\.html$/,
            loader: 'raw-loader'
        }, { // Load css files which are required in vendor.ts
            test: /\.css$/,
            loader: ExtractTextPlugin.extract({
                fallback: 'style-loader',
                use: {
                    loader: 'css-loader',
                    options: {
                        sourceMap: isDevBuild
                    }
                }
            })
        }, {
            test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|eot|ttf)$/,
            use: [{
                loader: 'url-loader',
                options: { limit: 100000 }
            }]
        },
        { test: /\.scss$/, use: ['raw-loader', 'sass-loader'] }
        ]
    },

    optimization: {
        minimizer: isDevBuild ? [] : [
            // we specify a custom UglifyJsPlugin here to get source maps in production
            new UglifyJsPlugin({
                sourceMap: false
            })
        ]
    },

    plugins: [
        new ExtractTextPlugin('[name].css'),
        /* deprecated in angular 4
        new webpack.optimize.CommonsChunkPlugin({
            name: ['main-client', 'vendor', 'polyfills']
        }),
        */

        // new CleanWebpackPlugin(['./wwwroot/dist/']),
        new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery', 'window.jQuery': 'jquery' })
    ]
};
