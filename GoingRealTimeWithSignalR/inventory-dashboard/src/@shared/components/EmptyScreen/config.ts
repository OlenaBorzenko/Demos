import errorImage from './error.svg';
import notFoundImage from './notFound.svg';

export const emptyScreenParams = {
  notFound: {
    header: 'Nothing was found. Please try other keywords.',
    text: null,
    imageUrl: notFoundImage,
  },
  error: {
    header: 'No data to display',
    text: null,
    imageUrl: errorImage,
  },
};

export type EmptyScreenType = keyof typeof emptyScreenParams;
