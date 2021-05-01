declare module '*.jpg';

declare module '*.png';

declare module '*.svg';

declare let CLIENT_ID: string;

declare type UnwrapAsyncFnReturn<T> = T extends (...args: any[]) => Promise<infer U> ? U : T;

declare type AnyFunction = (...args: any[]) => any;
declare type ErrorHandlingFn = (e: Error) => any;
