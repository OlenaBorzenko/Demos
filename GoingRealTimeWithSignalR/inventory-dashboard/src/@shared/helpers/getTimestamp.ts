export const getTimestamp = (value: any): number =>
  value
    ? new Date(value).getTime()
    : 0;
