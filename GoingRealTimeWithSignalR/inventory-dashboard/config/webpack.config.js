const path = require('path');
const webpack = require('webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const CircularDependencyPlugin = require('circular-dependency-plugin');
const minimist = require('minimist');
const paths = require('./paths');

module.exports.config = {
  context: paths.src,
  entry: paths.entry,
  resolve: {
    alias: {
      Home: path.join(paths.src, 'Home/'),
      '@resources': path.join(paths.src, '@styles/resources/'),
      '@mocks': path.join(paths.src, '@mocks/'),
      '@shared': path.join(paths.src, '@shared/'),
      '@assets': path.join(paths.src, '@assets/'),
      '@core': path.join(paths.src, '@core/'),
      '@styles': path.join(paths.src, '@styles/'),
      '@utils': path.join(paths.src, '@utils/'),
      '@typings': path.join(paths.src, '@typings/'),
    },
    extensions: ['.ts', '.tsx', '.js', '.json'],
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: [
          {
            loader: 'ts-loader',
            options: {
              configFile: paths.tsconfig,
              transpileOnly: true,
            },
          },
        ],
        exclude: /node_modules/,
      },
      {
        test: /\.tsx?$/,
        enforce: 'pre',
        use: [
          {
            loader: 'eslint-loader',
            options: { eslintPath: require.resolve('eslint') },
          },
        ],
        exclude: /node_modules/,
      },
    ],
  },
  plugins: [
    new CopyWebpackPlugin({
      patterns: [
        { from: 'static/**/*', context: path.join(__dirname, '..') },
      ],
    }),
    new HtmlWebpackPlugin({
      template: paths.indexHTML,
    }),
    new CircularDependencyPlugin({
      exclude: /a\.js|node_modules/,
      failOnError: true,
      cwd: process.cwd(),
    }),
  ],
};

module.exports.fileLoader = {
  loader: 'file-loader',
  options: {
    name: '[name].[ext]',
    outputPath: 'images/',
  },
};

module.exports.fontsLoader = {
  loader: 'file-loader',
  options: {
    name: '[name].[ext]',
    outputPath: 'fonts/',
  },
};
