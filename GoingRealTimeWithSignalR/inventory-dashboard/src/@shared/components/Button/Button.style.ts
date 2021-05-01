import { makeStyles } from '@material-ui/core';
import { $grey2, $white, $brightColor, $secondaryColor } from '@resources/colors';
import { $smallFontSize } from '@resources/dimensions';

export const useStyles = makeStyles({
  button: {
    height: '40px',
    border: 'none',
    padding: '10px 40px',
    borderRadius: '5px',
    color: $white,
    cursor: 'pointer',
    outline: 'none',
    fontWeight: 'bold',
    lineHeight: 1.5,
    fontSize: $smallFontSize,
    fontFamily: '"Roboto", sans-serif',
    whiteSpace: 'nowrap',

    '&:disabled': {
      opacity: 0.4,
      pointerEvents: 'none',
    },
    '&.small': {
      padding: '10px 16px',
    },
    '&.medium': {
      padding: '10px 40px',
    },
    '&.large': {
      padding: '10px 60px',
    },
    '&.fitToParent': {
      width: '100%',
    },
    '&.blue': {
      background: $brightColor,

      '&:hover, &:active': {
        background: `lighten(${$brightColor}, 10%)`,
      },
    },
    '&.grey': {
      backgroundColor: $grey2,
      color: $secondaryColor,

      '&:hover, &:active': {
        background: `darken(${$grey2}, 10%)`,
      },
    },
    '&.link': {
      height: '22px',
      fontSize: '16px',
      background: 'none',
      backGround: $brightColor,
      padding: '0',
      lineHeight: 'normal',

      '&:hover, &:active': {
        background: `lighten(${$brightColor}, 10%)`,
      },
    },
    '&.chip': {
      height: '28px',
      padding: '2px 12px',
      backgroundColor: $grey2,
      color: $brightColor,
      borderRadius: '24px',
      alignItems: 'center',
    },
  },
});
