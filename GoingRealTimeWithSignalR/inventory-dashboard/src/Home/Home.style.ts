import { makeStyles } from '@material-ui/styles';
import { $setCustomScroll } from '@resources/mixins';
import * as d from '@resources/dimensions';

export const useStyles = makeStyles({
  content: {
    ...$setCustomScroll(),
    height: `calc(100vh - ${d.$headerHeight})`,
    marginTop: '64px',
    padding: '30px',
  },
  buttonsGroup: {
    display: 'flex',
    justifyContent: 'space-between',
  },
});
