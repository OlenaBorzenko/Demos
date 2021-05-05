import { createMuiTheme, MuiThemeProvider } from '@material-ui/core/styles';
import React from 'react';
import { jssPreset, createGenerateClassName, StylesProvider } from '@material-ui/styles';
import { create } from 'jss';
import { useStyles } from '@styles/styles';
import { grey } from '@material-ui/core/colors';

const generateClassName = createGenerateClassName();
const jss = create({
  plugins: jssPreset().plugins,
});

export const ThemeProvider: React.FC = ({ children }) => {
  jss.createStyleSheet(useStyles()).attach();
  const prefersDarkMode = true;

  const theme = React.useMemo(
    () =>
      createMuiTheme({
        palette: {
          type: prefersDarkMode ? 'dark' : 'light',
          secondary: grey,
        },
        typography: {
          fontSize: 18,
        },
        overrides: {
          MuiTableCell: {
            root: {
              fontWeight: 200,
            },
            head: {
              fontWeight: 300,
            },
          },
          MuiTab: {
            root: {
              fontWeight: 300,
            },
          },
          MuiTypography: {
            root: {
              fontWeight: 200,
            },
            body1: {
              fontWeight: 200,
            },
            body2: {
              fontWeight: 200,
            },
            h1: {
              fontWeight: 200,
            },
            h2: {
              fontWeight: 200,
            },
            h3: {
              fontWeight: 200,
            },
            h4: {
              fontWeight: 200,
            },
            h5: {
              fontWeight: 200,
            },
            h6: {
              fontWeight: 200,
            },
          },
        },
      }),
    [prefersDarkMode],
  );

  return (
    <StylesProvider jss={jss} generateClassName={generateClassName}>
      <MuiThemeProvider theme={theme}>
        {children}
      </MuiThemeProvider>
    </StylesProvider>
  );
};
