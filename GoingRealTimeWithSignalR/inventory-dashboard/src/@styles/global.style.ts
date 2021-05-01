import { makeStyles } from '@material-ui/styles';
import { $grey1 } from '@resources/colors';
import { $smallScreenBreakpoint } from '@resources/dimensions';

export const useStyles = makeStyles({
  '@global': {
    body: {
      margin: '0',
      minHeight: '100%',
      backgroundColor: $grey1,
      minWidth: `calc(${$smallScreenBreakpoint} + 1px)`,
    },
    'html, body': {
      boxSizing: 'border-box',
      height: '100%',
    },
    '*, *:before, *:after': {
      boxSizing: 'inherit',
    },
    h1: {
      fontSize: '32px',
      fontWeight: 'bold',
    },
    h2: {
      fontSize: '24px',
      fontWeight: 'bold',
    },
    h3: {
      fontSize: '18px',
      fontWeight: 'bold',
    },
    h4: {
      fontSize: '16px',
      fontWeight: 'bold',
    },
  },
});
