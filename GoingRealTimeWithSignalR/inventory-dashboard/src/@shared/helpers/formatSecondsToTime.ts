import { floor } from 'lodash';

const zeroPad = (str: string) => str.padStart(2, '0');

/**
 * Returns a human readable time string in the following format: `hh.mm.ss`
 * @param secondsDiff number
 */
export const formatSecondsToTime = (secondsDiff: number) => {
  const hours   = floor(secondsDiff / 3600);
  const minutes = floor((secondsDiff - (hours * 3600)) / 60);
  const seconds = secondsDiff - (hours * 3600) - (minutes * 60);

  const hoursFormated = zeroPad(`${hours}`);
  const minutesFormated = zeroPad(`${minutes}`);
  const secondsFormated = zeroPad(`${seconds}`);
  const formattedTime = `${hoursFormated}:${minutesFormated}:${secondsFormated}`;

  return formattedTime;
};
