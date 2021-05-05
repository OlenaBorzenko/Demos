import { makeStyles } from '@material-ui/styles';
import { createStyles, Theme } from '@material-ui/core/styles';
import { blueGrey, red } from '@material-ui/core/colors';

export const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    customCard: {
      margin: '20px 0 20px 0',
      border: `1px solid ${blueGrey[500]}`,
    },
    cardContent: {
      display: 'flex',
    },
    nestedCard: {
      margin: '10px',
      border: `1px solid ${blueGrey[500]}`,
    },
    updated: {
      border: `1px solid ${red[500]}`,
    },
    cardHeader: {
      padding: '10px',
    },
    media: {
      height: 0,
      paddingTop: '56.25%', // 16:9
    },
    expand: {
      transform: 'rotate(0deg)',
      marginLeft: 'auto',
      transition: theme.transitions.create('transform', {
        duration: theme.transitions.duration.shortest,
      }),
    },
    expandOpen: {
      transform: 'rotate(180deg)',
    },
  }),
);