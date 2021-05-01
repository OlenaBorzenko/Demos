const path = require('path');
const paths = require('./config/paths');

module.exports = {
  roots: [
    paths.src,
  ],
  transform: {
    ".tsx?$": "ts-jest"
  },
  preset: 'ts-jest',
  testMatch: ["**/?(*.)+(spec|test).ts?(x)"],
  testPathIgnorePatterns: ["/dist/", "/node_modules/", "/vendor/"],
  testEnvironment: "enzyme",
  verbose: true,
  moduleNameMapper: {
    "\\.(gif|ttf|eot|svg|png|jpg)$": "identity-obj-proxy",
    "Home/(.*)$": "<rootDir>/src/Home/$1",
    "@mocks/(.*)$": "<rootDir>/src/@mocks/$1",
    "@shared/(.*)$": "<rootDir>/src/@shared/$1",
    "@assets/(.*)$": "<rootDir>/src/@assets/$1",
    "@core/(.*)$": "<rootDir>/src/@core/$1",
    "@styles/(.*)$": "<rootDir>/src/@styles/$1",
    "@utils/(.*)$": "<rootDir>/src/@utils/$1",
    "@typings/(.*)$": "<rootDir>/src/@typings/$1",
    "@resources/(.*)$": "<rootDir>/src/@styles/resources/$1",
  },
  setupFilesAfterEnv: ["./config/setupEnzyme"],
  moduleFileExtensions: [
    "ts",
    "tsx",
    "js",
    "jsx",
    "json",
    "node"
  ],
  globals: {
    'ts-jest': {
      diagnostics: false,
      tsConfig: paths.tsconfig
    }
  },
  "collectCoverageFrom": [
    "src/**/*.{ts,tsx}",
    "!src/@core/**",
    "!src/routes/**",
    "!src/@mocks/**",
  ],
  "coverageDirectory": "coverage"
};
