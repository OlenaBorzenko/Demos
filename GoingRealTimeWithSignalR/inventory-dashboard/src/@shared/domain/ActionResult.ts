import { ApiError } from '@core/Api';
import { HttpStatusCode } from '../enums';

/**
 * Should be used to transmit detailed information about the result of Store action to UI component
 */
export class ActionResult<T = any> {
  public data?: T;
  public errorCode?: HttpStatusCode;
  public success: boolean;

  constructor (result: { success: boolean, error?: ApiError, data?: T } = { success: true }) {
    this.success = result.success;
    this.data = result.data;

    if (result.error) {
      this.errorCode = result.error.status;
    }
  }
}
