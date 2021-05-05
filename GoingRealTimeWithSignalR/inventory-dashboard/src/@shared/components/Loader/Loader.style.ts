import { Theme, makeStyles, createStyles } from '@material-ui/core/styles';
import { $positionCenter } from '@resources/mixins';

export const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    progress: {
      ...$positionCenter(),
      margin: theme.spacing(2),
      color: 'primary',
      zIndex: 1,
    },
  }),
);
