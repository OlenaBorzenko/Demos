/* eslint-disable @typescript-eslint/no-unused-vars */
const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');

const baseConfig = require('./webpack.config');
const paths = require('./paths');
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');

module.exports = merge(baseConfig.config, {
  mode: 'production',
  devtool: 'source-map',
  output: {
    publicPath: paths.publicPath,
  },
  module: {
    rules: [
      {
        test: /\.(svg|png|jpg|gif)$/,
        use: [
          {
            ...baseConfig.fileLoader,
            options: {
              ...baseConfig.fileLoader.options,
              publicPath: `${paths.publicPath}images/`,
            },
          },
        ],
      },
      {
        test: /\.(ttf)$/,
        use: [
          {
            ...baseConfig.fontsLoader,
            options: {
              ...baseConfig.fontsLoader.options,
              publicPath: `${paths.publicPath}fonts/`,
            },
          }
        ],
      }
    ],
  },
  plugins: [
    new ForkTsCheckerWebpackPlugin({
      tsconfig: paths.tsconfig,
      async: false,
    })
  ],
});