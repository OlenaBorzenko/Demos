import { yellow, blueGrey } from '@material-ui/core/colors';
import { makeStyles } from '@material-ui/styles';

export const useStyles = makeStyles({
  nestedCard: {
    margin: '10px',
    border: `1px solid ${blueGrey[500]}`,
    minWidth: '120px',
  },
  updated: {
    border: `1px solid ${yellow[800]}`,
  },
});