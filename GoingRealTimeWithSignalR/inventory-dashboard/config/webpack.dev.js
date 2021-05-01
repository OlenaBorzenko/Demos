/* eslint-disable @typescript-eslint/no-unused-vars */
const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');

const baseConfig = require('./webpack.config');
const paths = require('./paths');
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');

const host = 'http://host.com';

module.exports = merge(baseConfig.config, {
  mode: 'development',
  devtool: 'cheap-module-eval-source-map',
  output: {
    globalObject: 'this', // workaround with HMR https://github.com/webpack/webpack/issues/6642
  },
  devServer: {
    inline: true,
    historyApiFallback: true,
    proxy: [
      {
        context: ['/api'],
        target: `${host}`,
        secure: false,
        changeOrigin: true,
      },
      {
        context: ['/signalR'],
        target: `${host}`,
        secure: false,
        changeOrigin: true,
      },
    ],
    port: 1111,
  },
  plugins: [
    new ForkTsCheckerWebpackPlugin({
      tsconfig: paths.tsconfig,
    })
  ],
  module: {
    rules: [
      {
        test: /\.(svg|png|jpg|gif)$/,
        use: [
          baseConfig.fileLoader
        ],
      },
      {
        test: /\.(ttf)$/,
        use: [
          baseConfig.fontsLoader
        ],
      }
    ],
  },
});