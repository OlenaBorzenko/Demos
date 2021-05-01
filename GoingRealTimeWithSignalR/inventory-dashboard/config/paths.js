const path = require('path');
const appDir = require('app-root-path').path;
const _mapValues = require('lodash/mapValues');

const publicPath = '/';

const paths = _mapValues(
  {
    indexHTML: './static/index.html',
    src: './src',
    tsconfig: './tsconfig.json',
    eslintConfig: './.eslintrc.js',
  },
  filePath => path.join(appDir, filePath),
);

const srcPaths = _mapValues(
  {
    entry: './@core/main.tsx',
    colors: './@styles/resources/colors.ts',
    mixins: './@styles/resources/mixins.ts',
    dimensions: './@styles/resources/dimensions.ts',
  },
  filePath => path.join(paths.src, filePath),
);

module.exports = {
  appDir,
  publicPath,
  ...paths,
  ...srcPaths,
};
