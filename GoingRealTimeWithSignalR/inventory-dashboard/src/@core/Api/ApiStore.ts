import { castArray, flow as _flow, isFunction, isString } from 'lodash';
import { Instance, types, getEnv } from 'mobx-state-tree';
import request from 'superagent';

import { GeneralApiErrors } from '@shared/enums';
import { ApiCallParams, ErrorHandlers, ErrorParser } from './ApiCallOptions';
import { ApiError } from './ApiError';
import { ApiResponse } from './ApiResponse';
import { getURL } from './getUrl';
import { IApiStoreEnv } from '@core/storesEnv';
import { Socket } from './Socket';
import { ServerResponse } from './ServerResponse';

export const ApiStore = types.compose(Socket, types
  .model('api', {})
  .actions(self => {
    const { notifier } = getEnv<IApiStoreEnv>(self);
    /**
     * Loops through error handlers, shows toast for each string handler, invokes every function handler with
     * an instance of the error. If no handlers given, default generic toast will be shown
     */
    const processErrorHandlers = (error: ApiError, handlers: ErrorHandlers<any>) => {
      const handlersToExecute = handlers[error.type] || handlers[GeneralApiErrors.DEFAULT];

      const handlersList = castArray(handlersToExecute);

      handlersList.forEach(handler => {
        if (isString(handler)) {
          return notifier.addNotification({ message: handler });
        }

        if (isFunction(handler)) {
          return handler(error);
        }
      });
    };

    const performApiCall = function* (fn: (...args: any[]) => IterableIterator<any>, params: ApiCallParams) {
      try {
        const result = yield* fn();

        const error = params.failIf && params.failIf(result.body);

        if (error) {
          throw (isString(error) ? new Error(error) : new Error());
        }

        return new ApiResponse(result.body);
      } catch (e) {
        const parsers = castArray(params.errorParsers || []);
        const parse = _flow(parsers) as ErrorParser;

        const error = parse(new ApiError(e));

        processErrorHandlers(error, params.errorHandlers);

        return new ApiResponse(null, '', error);
      }
    };

    return ({
      get: flow(function* (url: string, params: ApiCallParams) {
        const call = function* () {
          const requestUrl = params.isAbsoluteUrl ? url : getURL(url);
          const response = yield request(requestUrl).query(params.payload || '');

          return new ServerResponse(response.body);
        };

        return yield* performApiCall(call, params);
      }),
      getRawFile: flow(function* (url: string, params: ApiCallParams) {
        const call = function* () {
          const response = yield request(getURL(url)).responseType('blob');

          return new ServerResponse(response.body, response.headers);
        };

        return yield* performApiCall(call, params);
      }),

      post: flow(function* (url: string, params: ApiCallParams) {
        const call = function* () {
          const response = yield request.post(getURL(url))
            .send(params.payload);

          return new ServerResponse(response.body);
        };

        return yield* performApiCall(call, params);
      }),

      delete: flow(function* (url: string, params: ApiCallParams) {
        const call = function* () {
          const response = yield request.delete(getURL(url))
            .send(params.payload);

          return new ServerResponse(response.body);
        };

        return yield* performApiCall(call, params);
      }),

      put: flow(function* (url: string, params: ApiCallParams) {
        const call = function* () {
          const response = yield request.put(getURL(url))
            .send(params.payload);

          return new ServerResponse(response.body);
        };

        return yield* performApiCall(call, params);
      }),
    });
  }));

export type IApiStore = Instance<typeof ApiStore>;
