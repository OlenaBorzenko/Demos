module.exports =  {
    parser:  '@typescript-eslint/parser',
    extends:  [
      'eslint:recommended',
      'plugin:react/recommended', 
      'plugin:@typescript-eslint/recommended', 
      'prettier/@typescript-eslint',
      'plugin:prettier/recommended',
      'prettier'
    ],
    parserOptions:  {
      ecmaVersion:  2018, 
      sourceType:  'module', 
      ecmaFeatures:  {
        jsx:  true,
      },
    },
    rules:  {
      "arrow-parens": ["warn", "as-needed"],
      "arrow-spacing": "error",
      "brace-style": ["warn", "1tbs", { "allowSingleLine": true }],
      "comma-dangle": ["error", {
        "arrays": "only-multiline",
        "objects": "always-multiline",
        "imports": "only-multiline",
        "exports": "only-multiline",
        "functions": "only-multiline"
      }],
      "comma-spacing": ["error", { "before": false, "after": true }],
      "curly": "warn",
      "indent": ["error", 2, { "SwitchCase": 1 }],
      "import/order": "off",
      "keyword-spacing": ["error", { "before": true, "after": true }],
      "max-len": ["error", { "code": 120, "tabWidth": 2 }],
      "newline-before-return": "error",
      "no-console": "error",
      "no-extra-boolean-cast": "error",
      "no-multiple-empty-lines":  ["error", { "max": 1, "maxEOF": 1, "maxBOF": 0 }],
      "no-trailing-spaces": "error",
      "no-undef": "off",
      "no-unreachable": "warn",
      "padding-line-between-statements": "warn",
      "prettier/prettier": "off",
      "quotes": ["warn", "single"],
      "react/jsx-no-lambda": "off",
      "react/display-name": "off",
      "react/no-render-return-value": "off",
      "react/prop-types": "off", 
      "require-atomic-updates": "off",
      "sort-keys": "off",
      "space-before-function-paren": ["error", "always"],
      "@typescript-eslint/consistent-type-definitions": "off",
      "@typescript-eslint/explicit-function-return-type": "off",
      "@typescript-eslint/interface-name-prefix": "off",
      "@typescript-eslint/explicit-member-accessibility": "off",
      "@typescript-eslint/no-inferrable-types": "off",
      "@typescript-eslint/no-explicit-any": "off",
      "@typescript-eslint/no-use-before-define": "off",
      "@typescript-eslint/no-empty-interface": "off",
      "@typescript-eslint/no-empty-function": "off",
      "@typescript-eslint/no-var-requires": "off",
      "@typescript-eslint/member-ordering": ["warn", { "default": ["public-static-field"] }],
      "@typescript-eslint/no-array-constructor": "off",
    },
    settings:  {
      react:  {
        version:  'detect', 
      },
    },
  };