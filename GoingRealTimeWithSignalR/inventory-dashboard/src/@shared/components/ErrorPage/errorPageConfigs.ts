export type ErrorPageConfig = Partial<{
  primaryTitle: string;
  secondaryTitle: string;
  subTitle: string
  description: string[];
  buttons: {};
}>;

export const getNotFoundPageConfig = (goBackHandler: () => void): ErrorPageConfig => ({
  primaryTitle: '404',
  subTitle: 'Page Not Found',
  description: [
    'The page you are looking for might have been removed,',
    'had it\'s name changed, or is temporarily unavailable.',
  ],
  buttons: [
    { title: 'Go back', theme: 'blue', onClick: goBackHandler, size: 'small' },
  ],
});

export const getBootstrapFailedPageConfig = (tryAgainHandler: () => void): ErrorPageConfig => ({
  secondaryTitle: 'Unable to load app',
  description: [
    'The application could be temporarily unavailable or too busy.',
    'Please, try again in a few moments.',
  ],
  buttons: [
    { title: 'Try again', theme: 'blue', onClick: tryAgainHandler, size: 'small' },
  ],
});
