import { makeStyles } from '@material-ui/core';
import { $flex, $font } from '@resources/mixins';
import { $grey2, $grey5, $grey6 } from '@resources/colors';
import { $smallFontSize } from '@resources/dimensions';

export const useStyles = makeStyles({
  '@global': {
    'div, p': {
      fontFamily: '"Roboto", sans-serif',
    },
  },
  wrapper: {
    position: 'absolute',
    top: '0',
    left: '0',
    height: '100vh',
    width: '100vw',
    backgroundColor: $grey2,
    ...$flex('column', 'space-around', 'center'),
  },
  content: {
    width: '100%',
    maxHeight: '100%',
    flex: 1,
    textAlign: 'center',
    color: $grey5,
    ...$flex('row', 'center', 'center'),
  },
  primaryTitle: {
    fontSize: '200px',
    color: $grey6,
    margin: '0 0 20px 0',
    fontWeight: 'bold',
    lineHeight: '1em',
  },
  secondaryTitle: {
    margin: '0 0 20px 0',
    fontWeight: 'bold',
    lineHeight: '1em',
    marginBottom: '64px',
    fontSize: '80px',
    color: $grey6,
  },
  subTitle: {
    margin: '0 0 20px 0',
    fontWeight: 'bold',
    lineHeight: '1em',
    fontSize: '38px',
  },
  description: {
    ...$flex('column', 'center', 'center'),
    margin: '0 0 20px 0',
    fontSize: '16px',
    fontWeight: 'normal',
    flexWrap: 'wrap',
    lineHeight: 1.5,
  },
  buttons: {
    marginTop: '50px',
    ...$flex('row', 'center', 'center'),
    'button + button': {
      marginLeft: '16px',
    },
  },
  footer: {
    marginBottom: '40px',
    ...$flex('row', 'center', 'flex-end'),
    ...$font($smallFontSize, 'normal', $grey5),
  },
});
