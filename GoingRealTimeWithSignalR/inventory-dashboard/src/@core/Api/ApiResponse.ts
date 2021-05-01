import { ApiError } from './ApiError';

export class ApiResponse<T = any> {
  constructor (
    public data: T,
    public fileName: string = '',
    public error?: ApiError,
  ) {}

  public get success () {
    return !this.error;
  }
}
