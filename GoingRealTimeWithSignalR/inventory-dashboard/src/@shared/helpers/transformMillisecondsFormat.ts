import { replace } from 'lodash';

export const transformMillisecondsFormat = (date: string) => {
  const milliseconds = date.substring(date.indexOf('.'), date.length);
  const transformed = `${milliseconds.slice(0, 4)}Z`;

  return replace(date, milliseconds, transformed);
};
