import { makeStyles } from '@material-ui/styles';
import * as d from '@resources/dimensions';
import { $flex, $setCustomScroll } from '@resources/mixins';
import { $grey4, $white, $darkGrey } from '@resources/colors';
import { fade } from '@material-ui/core/styles/colorManipulator';

export const useStyles = makeStyles({
  wrapper: {
    height: '100%',
    position: 'relative',
    ...$flex('column'),
  },
  body: {
    maxHeight: `calc(100vh - (${d.$modalFooterHeight} + ${d.$modalHeaderHeight}))`,
    overflow: 'auto',
    padding: `${d.$verticalPadding} ${d.$horizontalPadding}`,
    ...$setCustomScroll(),
  },
  header: {
    padding: `${d.$verticalPadding} ${d.$horizontalPadding}`,
    height: d.$modalHeaderHeight,
    margin: 0,
    wordWrap: 'break-word',
    ...$flex('row', 'flex-start', 'center'),
  },
  footer: {
    padding: `${d.$footerPadding} ${d.$horizontalPadding}`,
    margin: 0,
    height: d.$modalFooterHeight,
    borderTop: `1px solid ${$grey4}`,
    ...$flex('row', 'flex-end', 'center'),
  },
  content: {
    position: 'relative',
    backgroundColor: $white,
    minWidth: d.$modalContentWidth,
    borderRadius: '4px',
    boxShadow: '0 4px 16px 0 rgba(62, 87, 116, 0.1)',
    ...$flex('column', 'center'),
    overflow: 'hidden',
    maxHeight: '100%',
  },
  container: {
    position: 'fixed',
    top: 0,
    left: 0,
    width: '100%',
    height: '100%',
    overflow: 'hidden',
    ...$flex('column', 'center', 'center'),
    padding: d.$horizontalPadding,
    zIndex: 100,
  },
  pinnedToTop: {
    justifyContent: 'flex-start',
  },
  overlay: {
    backgroundColor: fade($darkGrey, 0.5),
    position: 'absolute',
    left: 0,
    top: 0,
    width: '100%',
    height: '100%',
    zIndex: 101,
  },
  instance: {
    zIndex: 102,
    position: 'relative',
    ...$flex('column', 'center'),
    maxHeight: '100%',
  },
  text: {
    whiteSpace: 'pre-wrap',
  },
});
