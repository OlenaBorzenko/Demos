const path = require('path');
const webpack = require('webpack');
const { merge } = require('webpack-merge');

const baseConfig = require('./webpack.config');
const paths = require('./paths');
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');
const DotenvPlugin = require('dotenv-webpack');

const host = '';

module.exports = merge(baseConfig.config, {
  mode: 'development',
  devtool: 'eval-cheap-module-source-map',
  output: {
    publicPath: '/',
    globalObject: 'this', // workaround with HMR https://github.com/webpack/webpack/issues/6642
  },
  devServer: {
    hot: true,
    historyApiFallback: true,
    proxy: [
      {
        context: ['/api'],
        target: `${host}`,
        secure: false,
        changeOrigin: true,
      },
    ],
    port: 5780,
  },
  plugins: [
    new webpack.HotModuleReplacementPlugin(),
    new DotenvPlugin({
      path: path.resolve('./.env'),
      safe: false,
    }),
    new ForkTsCheckerWebpackPlugin({
      typescript: {
        configFile: path.resolve(process.cwd(), 'tsconfig.json'),
        diagnosticOptions: {
          semantic: true,
          syntactic: true,
        },
        eslint: {
          enabled: true,
          files: './src/**/*.{ts,tsx,js,jsx}',
        },
      },
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