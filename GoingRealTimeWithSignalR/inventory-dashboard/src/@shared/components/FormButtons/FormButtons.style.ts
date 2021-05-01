import { makeStyles } from '@material-ui/styles';
import { $baseFontSize } from '@resources/dimensions';
import { $grey5, $grey2, $white, $primaryColor } from '@resources/colors';

export const useStyles = makeStyles({
  save: {
    marginRight: '20px',
    color: $white,
    backgroundColor: $primaryColor,
    textTransform: 'none',
    fontSize: $baseFontSize,
    boxShadow: 'none',
  },
  cancel: {
    color: $grey5,
    backgroundColor: $grey2,
  },
});
