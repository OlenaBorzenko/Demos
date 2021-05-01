import { makeStyles } from '@material-ui/styles';
import { $secondaryColor } from '@resources/colors';
import * as d from '@resources/dimensions';
import { $flex } from '@resources/mixins';

export const useStyles = makeStyles({
  header: {
    backgroundColor: `${$secondaryColor}`,
  },
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
  toolbar: {
    ...$flex('row', 'space-between', 'center'),
    minHeight: 'auto',
    height: '56px',
  },
});
