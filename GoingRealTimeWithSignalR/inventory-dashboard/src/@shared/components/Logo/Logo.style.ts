import { makeStyles } from '@material-ui/styles';
import { $flex } from '@resources/mixins';

export const useStyles = makeStyles({
  link: {
    color: 'inherit',
    textDecoration: 'none',
  },
  logo: {
    marginRight: '32px',
    ...$flex('row', 'flex-start', 'center'),
  },
  subLogo: {
    marginLeft: '30px',
    whiteSpace: 'nowrap',
    letterSpacing: '3px',
  },
  logoIcon: {
    width: '108px',
    height: '38px',
  },
});
