import { makeStyles } from '@material-ui/styles';
import * as d from '@resources/dimensions';

export const useStyles = makeStyles({
  top: {
    height: d.$topSectionHeight,
    marginBottom: d.$margin,
  },
  main: {
    height: `calc(100% - ${d.$topSectionHeight} - ${d.$margin})`,
  },
});
