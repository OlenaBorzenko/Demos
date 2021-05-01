import { GeneralApiErrors } from '@shared/enums';
import { ApiError } from './ApiError';

export type ErrorHandler = ErrorHandlingFn | string;
export type ErrorParser = (e: ApiError) => ApiError;

export type ApiErrors<T = any> = GeneralApiErrors | T;

export type ErrorHandlers<E extends string = GeneralApiErrors> = {
  [key in ApiErrors<E>]: ErrorHandler | ErrorHandler[];
};

type Payload = object | number | string | null;

export interface EndpointCallParams<P extends Payload, E extends string = GeneralApiErrors> {
  payload: P;
  errorHandlers: ErrorHandlers<E>;
}

export interface ApiCallParams<
  P extends Payload = any,
  E extends string = GeneralApiErrors,
> extends EndpointCallParams<P, E> {
  errorParsers?: ErrorParser | ErrorParser[];
  isAbsoluteUrl?: boolean; // TODO: Find a better way to handle absolute urls
  failIf?: (response: any) => string | boolean;
}
