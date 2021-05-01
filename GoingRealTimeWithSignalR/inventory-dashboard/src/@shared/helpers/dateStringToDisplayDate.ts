const zeroPad = (str: string) => str.padStart(2, '0');

/**
 * Returns a human readable date string in the following format: `dd.mm.yyyy`
 * @param dateStr string
 * @param withTime If this propperty is `true` the output fomat is `dd.mm.yyyy hh:mm`. Default is `false`
 */
export const dateStringToDisplayDate = (dateStr: string, withTime: boolean = false) => {
  const date = new Date(dateStr);

  if (isNaN(date.valueOf())) {
    return dateStr;
  }

  const year = date.getFullYear();
  const month = zeroPad(`${date.getMonth() + 1}`);
  const dayOfMonth = zeroPad(`${date.getDate()}`);
  const formattedDate = [dayOfMonth, month, year].join('.');

  if (!withTime) {
    return formattedDate;
  }
  const hours = zeroPad(`${date.getHours()}`);
  const minutes = zeroPad(`${date.getMinutes()}`);
  const seconds = zeroPad(`${date.getSeconds()}`);
  const formattedTime = `${hours}:${minutes}:${seconds}`;

  return `${formattedDate}, ${formattedTime}`;
};
