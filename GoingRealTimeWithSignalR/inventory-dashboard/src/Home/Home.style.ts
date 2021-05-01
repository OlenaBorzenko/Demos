import { makeStyles } from '@material-ui/styles';
import * as d from '@resources/dimensions';
import { $setCustomScroll } from '@resources/mixins';

export const useStyles = makeStyles({
  content: {
    ...$setCustomScroll(),
    height: `calc(100vh - ${d.$headerHeight} - ${d.$topSectionHeight} - ${d.$margin} * 2)`,
  },
});
