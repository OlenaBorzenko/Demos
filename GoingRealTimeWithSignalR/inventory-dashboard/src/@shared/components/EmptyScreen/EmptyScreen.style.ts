import { makeStyles } from '@material-ui/styles';
import * as d from '@resources/dimensions';
import { $flex, $font } from '@resources/mixins';
import { $grey5 } from '@resources/colors';

export const useStyles = makeStyles({
  notFound: {
    fontSize: d.$largeFontSize,
  },
  text: {
    ...$font(d.$largeFontSize, 400, $grey5),
    marginTop: '10px',
  },
  emptyScreen: {
    ...$flex('column', 'center', 'center'),
    width: '100%',
  },
  emptyScreenWrapper: {
    ...$flex('row', 'center', 'center'),
    height: `calc(100vh - (${d.$headerHeight} + ${d.$topSectionHeight} + 2 * ${d.$margin}))`,
  },
  image: {
    marginBottom: '32px',
  },
});
