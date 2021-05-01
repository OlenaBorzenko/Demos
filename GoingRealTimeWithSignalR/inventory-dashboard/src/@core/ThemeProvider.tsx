import { createMuiTheme, MuiThemeProvider } from '@material-ui/core/styles';
import React from 'react';
import { jssPreset, createGenerateClassName, StylesProvider } from '@material-ui/styles';
import { create } from 'jss';
import { useStyles } from '@styles/styles';
import * as c from '@resources/colors';

const generateClassName = createGenerateClassName();
const jss = create({
  plugins: jssPreset().plugins,
});
const theme = createMuiTheme({
  overrides: {
    MuiPaper: {
      root: {
        color: c.$grey5,
      },
    },
  },
  typography: {
    fontSize: 14,
    h2: {
      fontSize: 32,
      fontWeight: 500,
      color: c.$primaryColor,
    },
    h3: {
      fontSize: 24,
      fontWeight: 500,
      color: c.$primaryColor,
    },
    h4: {
      fontSize: 20,
      fontWeight: 500,
      color: c.$subtitleColor,
    },
    h5: {
      fontSize: 16,
      fontWeight: 500,
      color: c.$subtitleColor,
    },
  },
});

export const ThemeProvider: React.FC = ({ children }) => {
  jss.createStyleSheet(useStyles()).attach();

  return (
    <StylesProvider jss={jss} generateClassName={generateClassName}>
      <MuiThemeProvider theme={theme}>
        {children}
      </MuiThemeProvider>
    </StylesProvider>
  );
};
