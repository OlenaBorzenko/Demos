import { $defaultScrollWidth, $baseFontSize } from './dimensions';
import { $grey, $grey2, $primaryColor } from './colors';
import { CSSProperties } from '@material-ui/styles';

export const $flex = (
  direction: CSSProperties['flexDirection'] = 'row',
  justify = 'flex-start',
  align = 'stretch',
): CSSProperties => {
  return {
    display: 'flex',
    flexDirection: direction,
    justifyContent: justify,
    alignItems: align,
  };
};

export const $setCustomScroll = (
  width: string = $defaultScrollWidth,
  thumbColor: string = $grey,
  trackColor: string = $grey2,
  borderRadius: string = '0px',
): CSSProperties => {
  const styles: CSSProperties = {
    button: 'auto',
    overflowX: 'hidden',
    '-webkit-overflow-scrolling': 'touch',
    '&::-webkit-scrollbar': {
      width,
    },
    '&::-webkit-scrollbar-track': {
      width,
      borderRadius,
      backgroundColor: trackColor,
    },
    '&::-webkit-scrollbar-thumb': {
      borderRadius,
      backgroundColor: thumbColor,
    },
    scrollbarColor: thumbColor,
    scrollbarWidth: 'thin',
  };

  return styles;
};

export const $positionCenter = (): CSSProperties => {
  return {
    position: 'absolute',
    top: '50%',
    left: '50%',
  };
};

export const $font = (
  size: string = $baseFontSize,
  weight: CSSProperties['fontWeight'] = 'normal',
  color = $primaryColor,
): CSSProperties => {
  return {
    color,
    fontSize: size,
    fontWeight: weight,
  };
};

export const $textOverflowDots = (): CSSProperties => {
  return {
    whiteSpace: 'nowrap',
    overflow: 'hidden',
    textOverflow: 'ellipsis',
  };
};
