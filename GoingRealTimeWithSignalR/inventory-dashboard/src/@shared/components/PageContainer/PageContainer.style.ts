import { makeStyles } from '@material-ui/styles';
import * as d from '@resources/dimensions';
import { $flex } from '@resources/mixins';

export const useStyles = makeStyles({
  centeredContent: {
    margin: 'auto',
    width: '100%',
    maxWidth: d.$largeScreenMaxWidth,
    padding: '0 32px',

    [`@media screen and (max-width: ${d.$largeScreenSize} - 1)`]: {
      maxWidth: d.$smallScreenMaxWidth,
    },

    [`@media all and (min-width: ${d.$mediumScreenBreakpoint})`]: {
      padding: '0',
    },
  },
  container: {
    height: `calc(100vh - ${d.$headerHeight})`,
    paddingTop: '16px',
    ...$flex('column'),
  },
});
